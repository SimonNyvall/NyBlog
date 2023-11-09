module Backend.Services.PostService

open System.IO

open Markdig

open Backend.Models.Post

let private parseMarkdwonToHTML (markdown: string): string =
  $"""
  <hr>
  {markdown |> Markdown.ToHtml}
  <button class="next-post-btn" id="next-post-btn" hx-get="" hx-trigger="click" hx-swap="outerHTML">
    Read next post!
  </button>
  """


let private parseAllMarkdownFromPostsDirectory (): Post list =
  let dirInfo = DirectoryInfo "posts/"
  let files = dirInfo.GetFiles("*.md")
  
  let posts = 
    files |> Array.map (fun file ->
      let content = File.ReadAllText(file.FullName) |> parseMarkdwonToHTML
      { Title = file.Name; HTMLContent = content; CreatedAt = file.CreationTime }
  )

  posts |> Array.toList


let getLatestPostHTMLContent (): string =
  parseAllMarkdownFromPostsDirectory ()
  |> List.sortByDescending (fun post -> post.CreatedAt)
  |> List.head 
  |> fun post -> post.HTMLContent


let getPreviousPostHTMLContent (indentifer: int): string =
  let sortedPosts = 
    parseAllMarkdownFromPostsDirectory ()
    |> List.sortByDescending (fun post -> post.CreatedAt)
  
  if indentifer < List.length sortedPosts && indentifer > 0 then
    sortedPosts 
    |> List.skip indentifer 
    |> List.head 
    |> fun post -> post.HTMLContent
  else
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
