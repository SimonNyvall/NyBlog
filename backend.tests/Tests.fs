module Tests

open System
open Xunit
open Backend.Services.PostService
open Foq
open Microsoft.Extensions.Logging
open Backend.Models.Post
open Backend.Abstractions.PostRepository

[<Fact>]
let ``getLatestPostHTMLContent should return string when no input`` () =
  // Arrange
  let samplePosts = [ { Title = "Post1"; MdContent = "Content1"; CreatedAt = DateTime(2020, 1, 1) } ]
    
  let mockRepository = 
    Mock<IPostRepository>()
      .Setup(fun repo -> <@ repo.parseAllMarkdownFromPostsDirectory() @>)
      .Returns(samplePosts)

  let mockLogger = Mock<ILogger<PostService>>()

  let service = PostService(mockRepository.Create(), mockLogger.Create())

  // Act
  let result = service.getLatestPostHTMLContent()

  // Assert
  Assert.Contains("Content1", result)
  Assert.Contains("<hr>", result)


[<Fact>]
let ``getPreviousPostHTMLContent returns string`` () =
  // Arrange
  let samplePosts = [ { Title = "Post1"; MdContent = "Content1"; CreatedAt = DateTime(2020, 1, 1) } ]
    
  let mockRepository = 
    Mock<IPostRepository>()
      .Setup(fun repo -> <@ repo.parseAllMarkdownFromPostsDirectory() @>)
      .Returns(samplePosts)

  let mockLogger = Mock<ILogger<PostService>>()

  let service = PostService(mockRepository.Create(), mockLogger.Create())

  // Act
  let result = service.getPreviousPostHTMLContent 1

  // Assert
  Assert.IsType<string>(result)

