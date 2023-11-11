module Backend.Endpoints.PostEndpoint

open System
open Microsoft.AspNetCore.Builder
open Backend.Services.PostService

let usePostEndpoint (app: WebApplication): WebApplication =
  
  app.MapGet("/api/posts", Func<string>(fun () -> getLatestPostHTMLContent () )) |> ignore

  app.MapGet("/api/posts/{id}", Func<int, string>(fun (id: int) -> getPreviousPostHTMLContent id )) |> ignore

  app.MapGet("/api/posts/titles", Func<string>(fun () -> getAllPostTitlesAsHTMLUl() )) |> ignore

  app
