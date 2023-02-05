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

  let configureLogging(log: ILoggingBuilder) = log.ClearProviders().AddConsole()

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
        post "/" (Endpoints.GraphQLHandler.handle)
      ]
    }