module Backend.Abstractions.PostRepository

open Backend.Models.Post

type IPostRepository =
  abstract member parseAllMarkdownFromPostsDirectory : unit -> Post list
