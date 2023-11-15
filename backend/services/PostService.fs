module Backend.Services.PostService

open Markdig
open Backend.Models.Post
open Microsoft.Extensions.Logging
open Backend.Abstractions.PostRepository

type PostService (repository: IPostRepository, logger: ILogger<PostService>) =

  let getMarkdwonToHTML (markdown: string) (index: int): string =
    $"""
    <hr>
    {markdown |> Markdown.ToHtml}
    <button class="next-post-btn" hx-get="http://localhost:5021/api/posts/{index}" hx-trigger="click" hx-swap="outerHTML">
      Read next post!
    </button>
    """.Trim()

  member _.getLatestPostHTMLContent (): string =
    logger.LogInformation("Getting latest post")

    let latestPost =
      let posts = repository.parseAllMarkdownFromPostsDirectory ()
    
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
      repository.parseAllMarkdownFromPostsDirectory () 
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
      { repository.parseAllMarkdownFromPostsDirectory () 
        |> List.map (fun post -> $"<li>{ post.Title.Substring(0, post.Title.Length - 3) }</li>")
        |> String.concat "\n"
      }
    </ul>
    """
