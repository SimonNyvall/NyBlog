namespace backend
#nowarn "20"
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Backend.Endpoints.PostEndpoint

[<AutoOpen>]

module Program =
    let setupEndpoints (app: WebApplication): WebApplication =
      app |> usePostEndpoint 

      app

    let exitCode = 0

    [<EntryPoint>]
    let main args =

        let builder = WebApplication.CreateBuilder(args)

        builder.Services.AddControllers()

        let app = builder.Build()

        app.UseHttpsRedirection()

        app.UseAuthorization()
        app.MapControllers()

        setupEndpoints app

        app.Run()

        exitCode
