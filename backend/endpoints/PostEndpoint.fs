module Backend.Endpoints.PostEndpoint

open System
open Microsoft.AspNetCore.Builder
open Backend.Services.PostService
open Microsoft.Extensions.DependencyInjection
open Microsoft.AspNetCore.Http

let usePostEndpoint (app: WebApplication): WebApplication =
  
  app.MapGet("/api/posts", Func<HttpContext, string>(fun httpContext ->
    let postService = httpContext.RequestServices.GetRequiredService<PostService>()
    postService.getLatestPostHTMLContent()
  )) |> ignore

  app.MapGet("/api/posts/{id}", Func<HttpContext, int, string>(fun httpContext id ->
    let postService = httpContext.RequestServices.GetRequiredService<PostService>()
    postService.getPreviousPostHTMLContent(id)
  )) |> ignore

  app.MapGet("/api/posts/titles", Func<HttpContext, string>(fun httpContext ->
    let postService = httpContext.RequestServices.GetRequiredService<PostService>()
    postService.getAllPostTitlesAsHTMLUl()
  )) |> ignore


  app
