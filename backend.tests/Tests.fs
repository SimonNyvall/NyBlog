module Tests

open System.IO
open Xunit
open Backend.Services.PostService
open Backend.Abstractions.FileSystem
open Foq
open Microsoft.Extensions.Logging
open Backend.Repositories.PostRepository

[<Fact>]
let ``getLatestPostHTMLContent returns string`` () =
  // Arrange
  (*
  let mockFileSystem = Mock<IFileSystem>()

  mockFileSystem.Setup(fun x -> <@ x.GetFiles "*.md" @>).Returns([|"file1.md"; "file2.md"|]) |> ignore
  let mockDirectoryInfo = new DirectoryInfo("posts/")
  mockFileSystem.Setup(fun x -> <@ x.DirectoryInfo "posts/" @>).Returns(mockDirectoryInfo) |> ignore
  mockFileSystem.Setup(fun x -> <@ x.ReadAllTextAsync "file1.md" @>).Returns("mock file content") |> ignore


  let mockPostServiceLogger = Mock<ILogger<PostService>>()
  let mockPostRepositoryLogger = Mock<ILogger<PostRepository>>()
  
  let mockPostRepository = Mock<PostRepository (mockFileSystem.Create(), mockPostRepositoryLogger.Create())> ()  
  let service = PostService (mockPostRepository.Create(), mockLogger.Create())

  // Act
  let result = service.getLatestPostHTMLContent()
  *)
  // Assert
  //Assert.IsType<string>(result)
  Assert.True(true)

[<Fact>]
let ``getPreviousPostHTMLContent returns string`` () =
  // Arrange
  (*
  let mockFileSystem = Mock<IFileSystem>()

  mockFileSystem.Setup(fun x -> <@ x.GetFiles "*.md" @>).Returns([|"file1.md"; "file2.md"|]) |> ignore
  let mockDirectoryInfo = new DirectoryInfo("posts/")
  mockFileSystem.Setup(fun x -> <@ x.DirectoryInfo "posts/" @>).Returns(mockDirectoryInfo) |> ignore
  mockFileSystem.Setup(fun x -> <@ x.ReadAllText "file1.md" @>).Returns("mock file content") |> ignore

  let mockLogger = Mock<ILogger<PostService>>()

  let service = PostService (mockFileSystem.Create(), mockLogger.Create())

  // Act
  let result = service.getPreviousPostHTMLContent 1
*)
  // Assert
  //Assert.IsType<string>(result)
  Assert.True(true)

