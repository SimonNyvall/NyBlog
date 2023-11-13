namespace backend
#nowarn "20"
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Backend.Extensions.Endpoints.PostEndpoint
open Backend.Extensions.Logging.SeqLoggingExtension
open Backend.Extensions.Origin.FrontendOriginExtension
open Backend.Abstractions.FileSystem
open Backend.Services.PostService

[<AutoOpen>]

module Program =
    let exitCode = 0

    [<EntryPoint>]
    let main args =

        let builder = WebApplication.CreateBuilder(args)

        builder.Services.AddControllers()
        
        builder.Services.AddScoped<IFileSystem, FileSystem>()
        builder.Services.AddScoped<PostService>()

        builder |> useFrontendOrigin
                
        setupSeqLogging builder 

        let app = builder.Build()

        app.UseHttpsRedirection()

        app.UseAuthorization()
        app.MapControllers()

        app.UseCors("AllowFrontend")

        app |> usePostEndpoint

        app.Run()

        exitCode
