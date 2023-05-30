namespace PathfinderSpellDb

open FsLibLog
open System.Reflection

module AssemblyInfo =

    let metaDataValue (mda: AssemblyMetadataAttribute) = mda.Value

    let getMetaDataAttribute (assembly: Assembly) key =
        assembly.GetCustomAttributes(typedefof<AssemblyMetadataAttribute>)
        |> Seq.cast<AssemblyMetadataAttribute>
        |> Seq.find (fun x -> x.Key = key)

    let getReleaseDate assembly =
        "ReleaseDate"
        |> getMetaDataAttribute assembly
        |> metaDataValue

    let getGitHash assembly =
        "GitHash"
        |> getMetaDataAttribute assembly
        |> metaDataValue

    let getVersion assembly =
        "AssemblyVersion"
        |> getMetaDataAttribute assembly
        |> metaDataValue

    let assembly = lazy (Assembly.GetEntryAssembly())

    let printVersion () =
        let version = assembly.Force().GetName().Version
        printfn "%A" version

    let printInfo () =
        let assembly = assembly.Force()
        let name = assembly.GetName()
        let version = assembly.GetName().Version
        let releaseDate = getReleaseDate assembly
        let githash = getGitHash assembly
        printfn "%s - %A - %s - %s" name.Name version releaseDate githash

module Main =
    open Configuration
    open Argu
    open FsLibLog.Operators
    open Serilog
    open Serilog.Core
    open Serilog.Events

    type ThreadIdEnricher() =
      interface ILogEventEnricher with
        member this.Enrich(logEvent : LogEvent, propertyFactory: ILogEventPropertyFactory) =
          logEvent.AddPropertyIfAbsent(
            propertyFactory.CreateProperty(
              "ThreadId",
              System.Threading.Thread.CurrentThread.ManagedThreadId
           )
          )

    let log =
        LoggerConfiguration()
            .MinimumLevel.Verbose()
            .Enrich.FromLogContext()
            .Enrich.With(ThreadIdEnricher())
            .WriteTo.File("log.txt",
              outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:W3}] ({ThreadId}) {Message}{NewLine}{Exception}"
            )
            .WriteTo.Console(
              outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:W3}] ({ThreadId}) {Message}{NewLine}{Exception}"
            )
            .CreateLogger()

    LogProvider.setLoggerProvider (Providers.SerilogProvider.create ())
    Serilog.Log.Logger <- log

    let logger = LogProvider.getLoggerByName "PathfinderSpellDb.Main"


    [<EntryPoint>]
    let main (argv: string array) =
        let parser = ArgumentParser.Create<CLIArguments>(programName = "PathfinderSpellDb")
        let results = parser.Parse(argv)

        if results.Contains Version then
            AssemblyInfo.printVersion ()
        elif results.Contains Info then
            AssemblyInfo.printInfo ()
        elif results.Contains Usage then
            let usage = 
              let x = CLIArguments.Usage :> IArgParserTemplate
              x.Usage
            printfn "%s" usage
        else
          let config = 
            let csvPath = Configuration.tryGetCsvPath results
            let cors = Configuration.tryGetCors results
            ApplicationConfig.Create csvPath cors

          !!! "Configuration: {config}" >>!+ ("config", config)
          |> logger.info
          logger.info <| !!! "Starting webhost"
          Webhost.host config

        0
