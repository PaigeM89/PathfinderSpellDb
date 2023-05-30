namespace PathfinderSpellDb

module Configuration =

    open System
    open System.Reflection
    open Argu


    let private appName () =
        Assembly.GetExecutingAssembly().GetCustomAttributes<AssemblyProductAttribute>()
        |> Seq.tryHead
        |> Option.map (fun attr -> attr.Product)
        |> Option.defaultValue "unknown"

    let getEnv varname = System.Environment.GetEnvironmentVariable varname
    let getEnvOr defaultValue varname =
        try
            getEnv varname
        with
        | _ -> defaultValue

    let getEnvOrFunc func varname =
        try
            getEnv varname
        with
        | _ -> func()

    type ApplicationConfig = {
      CsvPath : string
      CorsOrigins: string array
    } with
      static member Empty() = {
        CsvPath = ""
        CorsOrigins = [||]
      }

      static member Create csvPath cors = {
        CsvPath = csvPath
        CorsOrigins = cors
      }

    type CLIArguments =
        | Info
        | Version
        | Usage
        | CsvPath of string
        | CorsOrigins of string
        interface IArgParserTemplate with
            member s.Usage =
                match s with
                | Info -> "More detailed information."
                | Version -> "Version of application."
                | Usage -> "Print this usage info."
                | CsvPath _ -> "The path of the CSV to load."
                | CorsOrigins _ -> "The allowed CORS origins."

    let parseArgs args =
        let parser = ArgumentParser.Create<CLIArguments>(programName = "PathfinderSpellDb")
        parser.Parse(args)

    let printUsage() =
        let parser = ArgumentParser.Create<CLIArguments>(programName = "PathfinderSpellDb")
        parser.PrintUsage()

    let tryGetCsvPath (results : ParseResults<CLIArguments>) =
      let defaultPath = __SOURCE_DIRECTORY__ + "/spells.csv"
      let path = getEnvOrFunc (fun () -> results.GetResult(CsvPath, defaultValue = defaultPath)) "CSV_PATH"
      if isNull path then defaultPath else path

    let tryGetCors (results : ParseResults<CLIArguments>) =
        let defaultCors = "http://localhost:8080,http://localhost:5000"
        let envValues = getEnvOrFunc (fun () -> results.GetResult(CorsOrigins, defaultValue = defaultCors)) "CORS_ORIGINS"
        let envValues = if isNull envValues then defaultCors else envValues
        envValues.Split(",")