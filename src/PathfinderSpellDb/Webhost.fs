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
      .AddSimpleConsole(fun opts -> opts.IncludeScopes <- true)
    |> ignore
  )

  let configureLogging(log: ILoggingBuilder) = 
    FsLibLog.Providers.MicrosoftExtensionsLoggingProvider.setMicrosoftLoggerFactory microsoftLoggerFactory
    log.ClearProviders().AddConsole()

  let configureCors (builder : CorsPolicyBuilder) =
    builder.WithOrigins("http://localhost:5173")
      .AllowAnyMethod()
      .AllowAnyHeader()
      .AllowAnyOrigin()
      .Build()

  let addCors (svcs: IServiceCollection) =
    svcs
      .AddCors(fun options ->
        options.AddDefaultPolicy(CorsPolicyBuilder() |> configureCors)
      )
      .Configure<KestrelServerOptions>(fun (options: KestrelServerOptions) ->
        options.AllowSynchronousIO <- true
      )

  let useCors (x: IApplicationBuilder) =
    x.UseCors()

  let host() =
    webHost [||] {
      add_service addCors
      use_middleware useCors
      logging configureLogging
      endpoints [
        //post "/" (Endpoints.GraphQLHandler.handle)
        get "/spells/ranges" Handlers.getRanges
        get "/spells" (Handlers.getAllSpells())
        get "/spells/{id:int}" (fun ctx ->
          let route = Request.getRoute ctx
          let spellId = route.GetInt "id"
          Handlers.getSpell spellId ctx
        )
        // this was extremely slow for some reason
        //post "/spells" Handlers.handleSpellSearch
        get "/classes" Handlers.getClasses
      ]
    }