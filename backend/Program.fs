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

        builder.Services.AddCors(fun options -> 
          let frontendOrigin = builder.Configuration["AllowedOrigins:Frontend"]

          options.AddPolicy("AllowFrontend", fun builder -> 
            builder.WithOrigins(
              frontendOrigin
              )
              .AllowAnyHeader() |> ignore
          )
        ) |> ignore

        let app = builder.Build()

        app.UseHttpsRedirection()

        app.UseAuthorization()
        app.MapControllers()

        app.UseCors("AllowFrontend")

        setupEndpoints app

        app.Run()

        exitCode
