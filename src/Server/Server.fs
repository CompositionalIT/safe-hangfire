module Server

open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Hangfire
open Hangfire.SqlServer
open System
open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open Saturn

open Shared

module Storage =
    let todos = ResizeArray()

    let addTodo (todo: Todo) =
        if Todo.isValid todo.Description then
            todos.Add todo
            Ok()
        else
            Error "Invalid todo"

    do
        addTodo (Todo.create "Create new SAFE project")
        |> ignore

        addTodo (Todo.create "Write your app") |> ignore
        addTodo (Todo.create "Ship it !!!") |> ignore

let todosApi =
    { getTodos = fun () -> async { return Storage.todos |> List.ofSeq }
      addTodo =
        fun todo ->
            async {
                return
                    match Storage.addTodo todo with
                    | Ok () -> todo
                    | Error e -> failwith e
            }
      queueJob =
          fun () ->
              async {
                  BackgroundJob.Enqueue(fun () -> Console.WriteLine $"Job queued at {DateTime.Now}") |> ignore
              } }

let webApp =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.fromValue todosApi
    |> Remoting.buildHttpHandler

let configureServices (services: IServiceCollection) =
    let config = services.BuildServiceProvider().GetService<IConfiguration>()

    services.AddHangfire(fun (configuration: IGlobalConfiguration) ->
        configuration
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(
                 config.GetConnectionString("HangfireConnection"),
                 SqlServerStorageOptions
                    (
                        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                        QueuePollInterval = TimeSpan.Zero,
                        UseRecommendedIsolationLevel = true,
                        DisableGlobalLocks = true
                    ))
            |> ignore)
    |> ignore

    services.AddHangfireServer() |> ignore

    services

let configure (app: IApplicationBuilder) =
    app.UseHangfireDashboard()

let app =
    application {
        url "http://*:8085"
        use_router webApp
        memory_cache
        use_static "public"
        use_gzip
        service_config configureServices
        app_config configure
    }

[<EntryPoint>]
let main _ =
    run app
    0