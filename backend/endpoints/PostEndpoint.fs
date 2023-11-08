module Backend.Endpoints.PostEndpoint

open System
open Microsoft.AspNetCore.Builder
open Backend.Services.PostService

let usePostEndpoint (app: WebApplication): WebApplication =
  
  app.MapGet("/api/posts", Func<string>(fun () -> getLatestPostContent() )) |> ignore

  app.MapGet("/api/posts/{id}", Func<int, string>(fun id -> getPreviousPostContent(id) )) |> ignore

  app
