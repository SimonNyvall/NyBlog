module Backend.Repositories.PostRepository

open Backend.Abstractions.FileSystem
open Backend.Models.Post
open System.Threading.Tasks
open FSharp.Control
open Microsoft.Extensions.Logging

type FileSystem () =
  interface IFileSystem with
    member _.GetFiles path = System.IO.Directory.GetFiles(path)
    member _.DirectoryInfo path = System.IO.DirectoryInfo(path)
    member _.ReadAllTextAsync path = System.IO.File.ReadAllTextAsync(path) 


type PostRepository (fileSystem: IFileSystem, logger: ILogger<PostRepository>) =

  member _.parseAllMarkdownFromPostsDirectory (): Post list =  
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

