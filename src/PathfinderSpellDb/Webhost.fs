namespace PathfinderSpellDb


module Webhost =
  open Falco
  open Falco.Routing
  open Falco.HostBuilder
  open Microsoft.Extensions.Logging
  open Microsoft.AspNetCore.Builder
  open Microsoft.Extensions.DependencyInjection
  open Microsoft.AspNetCore.Cors.Infrastructure
  open Microsoft.AspNetCore.Server.Kestrel.Core

  let microsoftLoggerFactory = LoggerFactory.Create(fun builder ->
    builder
      .SetMinimumLevel(LogLevel.Debug)
      .AddSimpleConsole(fun opts -> 
        opts.TimestampFormat <- "HH:mm:ss.ffff"
        opts.IncludeScopes <- true
      )
    |> ignore
  )

  let configureLogging(log: ILoggingBuilder) = 
    FsLibLog.Providers.MicrosoftExtensionsLoggingProvider.setMicrosoftLoggerFactory microsoftLoggerFactory
    log.ClearProviders().AddConsole()

  let configureCors cors (builder : CorsPolicyBuilder) =
    builder.WithOrigins(cors)
      .AllowAnyMethod()
      .AllowAnyHeader()
      .AllowAnyOrigin()
      .Build()

  let addCors (cors : string[]) (svcs: IServiceCollection) =
    svcs
      .AddCors(fun options ->
        options.AddDefaultPolicy(CorsPolicyBuilder() |> (configureCors cors))
      )
      .Configure<KestrelServerOptions>(fun (options: KestrelServerOptions) ->
        options.AllowSynchronousIO <- true
      )

  let useCors (x: IApplicationBuilder) =
    x.UseCors()

  let host (config : Configuration.ApplicationConfig) =
    let lazySpells = Handlers.lazyLoadSpells config
    webHost [||] {
      add_service (addCors config.CorsOrigins)
      use_middleware useCors
      logging configureLogging
      endpoints [
        get "/spells/{id:int}" (fun ctx ->
          let route = Request.getRoute ctx
          let spellId = route.GetInt "id"
          (Handlers.getSpell lazySpells.Value spellId) ctx
        )
        get "/spells" (Handlers.getAllSpells config)
      ]
    }