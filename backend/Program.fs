namespace backend
#nowarn "20"
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Backend.Endpoints.PostEndpoint
open Backend.Abstractions.FileSystem
open Backend.Services.PostService
open Microsoft.Extensions.Logging

[<AutoOpen>]

module Program =
    let setupSeqLogging (builder: WebApplicationBuilder): WebApplicationBuilder =
      
      let seqUrl = builder.Configuration["SeqUrl"]

      builder.Services.AddLogging(fun loggingBuilder -> 
        loggingBuilder.AddSeq seqUrl |> ignore
      )      

      builder

    let setupEndpoints (app: WebApplication): WebApplication =
      app |> usePostEndpoint 

      app

    let exitCode = 0

    [<EntryPoint>]
    let main args =

        let builder = WebApplication.CreateBuilder(args)

        builder.Services.AddControllers()
        
        builder.Services.AddScoped<IFileSystem, FileSystem>()
        builder.Services.AddScoped<PostService>()

        builder.Services.AddCors(fun options -> 
          let frontendOrigin = builder.Configuration["AllowedOrigins:Frontend"]

          options.AddPolicy("AllowFrontend", fun builder -> 
            builder.WithOrigins(
              frontendOrigin
              )
              .AllowAnyHeader() |> ignore
          )
        ) |> ignore
        
        setupSeqLogging builder 

        let app = builder.Build()

        app.UseHttpsRedirection()

        app.UseAuthorization()
        app.MapControllers()

        app.UseCors("AllowFrontend")

        setupEndpoints app

        app.Run()

        exitCode
