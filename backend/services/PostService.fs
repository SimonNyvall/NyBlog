module Backend.Services.PostService

open System
open System.IO

open Markdig

open Backend.Models.Post

let private parseAllMarkdownfromPostsDirectory (): Post list =
  let dirInfo = DirectoryInfo "posts/"
  let files = dirInfo.GetFiles("*.md")
  
  let posts = 
    files |> Array.map (fun file ->
      let content = File.ReadAllText(file.FullName) |> Markdown.ToHtml 
      { Title = file.Name; Content = content; CreatedAt = file.CreationTime }
  )

  posts |> Array.toList


let getLatestPostContent (): string =
  parseAllMarkdownfromPostsDirectory ()
  |> List.sortByDescending (fun post -> post.CreatedAt)
  |> List.head 
  |> fun post -> post.Content


let getPreviousPostContent (indentifer: int): string =
  let sortedPosts = 
    parseAllMarkdownfromPostsDirectory ()
    |> List.sortByDescending (fun post -> post.CreatedAt)
  
  if indentifer < List.length sortedPosts then
    sortedPosts |> List.head |> fun post -> post.Content
  else
    String.Empty

