module Backend.Services.PostService

open Markdig
open Backend.Models.Post
open Backend.Abstractions.FileSystem
open Microsoft.Extensions.Logging
open System.Threading.Tasks
open FSharp.Control

type FileSystem () =
  interface IFileSystem with
    member _.GetFiles path = System.IO.Directory.GetFiles(path)
    member _.DirectoryInfo path = System.IO.DirectoryInfo(path)
    member _.ReadAllTextAsync path = System.IO.File.ReadAllTextAsync(path) 


type PostService (fileSystem: IFileSystem, logger: ILogger<PostService>) =

  let getMarkdwonToHTML (markdown: string) (index: int): string =
    $"""
    <hr>
    {markdown |> Markdown.ToHtml}
    <button class="next-post-btn" hx-get="http://localhost:5021/api/posts/{index}" hx-trigger="click" hx-swap="outerHTML">
      Read next post!
    </button>
    """.Trim()
    


  let parseAllMarkdownFromPostsDirectory (): Post list =  
    async {
      let dirInfo =
        try
          Some (fileSystem.DirectoryInfo "posts/")
        with
          | ex -> 
            logger.LogError("Error while reading directory metadata: {errorMessage}", ex.Message)
            None
    
      let files: Async<System.IO.FileInfo array> = 
        match dirInfo with
        | Some info ->
            try
              Task.Run(fun () ->
                info.GetFiles "*.md"
              ) |> Async.AwaitTask
            with
              | ex -> 
                logger.LogError("Error while reading files from directory: {errorMessage}", ex.Message)
                Async.AwaitTask (Task.FromResult [||])

        | None -> Async.AwaitTask (Task.FromResult [||])

      let! fileArray = files

      let fileArrayMap =
        fileArray
        |> Array.map (fun file -> async { 
          let! content = fileSystem.ReadAllTextAsync (file.FullName) |> Async.AwaitTask
          return { Title = file.Name; MdContent = content; CreatedAt = file.CreationTime }
        })
        |> Async.Parallel

      let! posts = fileArrayMap
      return posts |> Array.toList
    }
    |> Async.RunSynchronously 

  member _.getLatestPostHTMLContent (): string =
    logger.LogInformation("Getting latest post")

    let latestPost =
      let posts = parseAllMarkdownFromPostsDirectory ()
    
      if List.length posts > 0 then
        posts
        |> List.sortByDescending (fun post -> post.CreatedAt)
        |> List.head 
        |> fun post -> post.MdContent 
      else
        logger.LogWarning("No posts found")
        "204: No content"

    getMarkdwonToHTML latestPost 1


  member _.getPreviousPostHTMLContent (indentifer: int): string =
    logger.LogInformation("Getting previous post with indentifer: {indentifer}", indentifer)

    let sortedPosts = 
      parseAllMarkdownFromPostsDirectory () 
      |> List.sortByDescending (fun post -> post.CreatedAt)
  
    if indentifer < List.length sortedPosts && indentifer > 0 then
      let post = 
        sortedPosts 
        |> List.skip indentifer 
        |> List.head 
        |> fun post -> post.MdContent

      getMarkdwonToHTML post (indentifer + 1)
    else
      logger.LogInformation("No post found with indentifer: {indentifer}", indentifer)
      "404: Page not found"


  member _.getAllPostTitlesAsHTMLUl (): string =
    $"""
    <ul>
      { parseAllMarkdownFromPostsDirectory () 
        |> List.map (fun post -> $"<li>{ post.Title.Substring(0, post.Title.Length - 3) }</li>")
        |> String.concat "\n"
      }
    </ul>
    """
