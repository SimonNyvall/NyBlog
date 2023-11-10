module Backend.Services.PostService

open System.IO
open Markdig
open Backend.Models.Post
open Serilog

let private getMarkdwonToHTML (markdown: string) (index: int): string =
  $"""
  <hr>
  {markdown |> Markdown.ToHtml}
  <button class="next-post-btn" hx-get="http://localhost:5021/api/posts/{index}" hx-trigger="click" hx-swap="outerHTML">
    Read next post!
  </button>
  """


let private parseAllMarkdownFromPostsDirectory (): Post list =
  let dirInfo = DirectoryInfo "posts/"
  let files = dirInfo.GetFiles("*.md")
  
  let posts = 
    files |> Array.map (fun file ->
      let content = File.ReadAllText(file.FullName)
      { Title = file.Name; HTMLContent = content; CreatedAt = file.CreationTime }
  )

  posts |> Array.toList


let getLatestPostHTMLContent (): string =
  Log.Information("Getting latest post")

  let latestPost = 
    parseAllMarkdownFromPostsDirectory ()
    |> List.sortByDescending (fun post -> post.CreatedAt)
    |> List.head 
    |> fun post -> post.HTMLContent
  getMarkdwonToHTML latestPost 1


let getPreviousPostHTMLContent (indentifer: int): string =
  Log.Information("Getting previous post with indentifer: {0}", indentifer)

  let sortedPosts = 
    parseAllMarkdownFromPostsDirectory ()
    |> List.sortByDescending (fun post -> post.CreatedAt)
  
  if indentifer < List.length sortedPosts && indentifer > 0 then
    let post = 
      sortedPosts 
      |> List.skip indentifer 
      |> List.head 
      |> fun post -> post.HTMLContent

    getMarkdwonToHTML post (indentifer + 1)
  else
    Log.Error("No post found with indentifer: {0}", indentifer)
    "404: Page not found"


let getAllPostTitlesAsHTMLUl (): string =
 $"""
  <ul>
    { parseAllMarkdownFromPostsDirectory ()
      |> List.map (fun post -> $"<li>{ post.Title.Substring(0, post.Title.Length - 3) }</li>")
      |> String.concat "\n"
    }
  </ul>
  """
