module Tests

open System.IO
open Xunit
open Backend.Services.PostService
open Backend.Abstractions.FileSystem
open Foq


[<Fact>]
let ``getLatestPostHTMLContent returns string`` () =
  // Arrange
  let mockFileSystem = Mock<IFileSystem>()

  mockFileSystem.Setup(fun x -> <@ x.GetFiles "*.md" @>).Returns([|"file1.md"; "file2.md"|]) |> ignore
  let mockDirectoryInfo = new DirectoryInfo("posts/")
  mockFileSystem.Setup(fun x -> <@ x.DirectoryInfo "posts/" @>).Returns(mockDirectoryInfo) |> ignore
  mockFileSystem.Setup(fun x -> <@ x.ReadAllText "file1.md" @>).Returns("mock file content") |> ignore

  let service = PostService (mockFileSystem.Create())

  // Act
  let result = service.getLatestPostHTMLContent()

  // Assert
  Assert.IsType<string>(result)

[<Fact>]
let ``getPreviousPostHTMLContent returns string`` () =
  // Arrange
  let mockFileSystem = Mock<IFileSystem>()

  mockFileSystem.Setup(fun x -> <@ x.GetFiles "*.md" @>).Returns([|"file1.md"; "file2.md"|]) |> ignore
  let mockDirectoryInfo = new DirectoryInfo("posts/")
  mockFileSystem.Setup(fun x -> <@ x.DirectoryInfo "posts/" @>).Returns(mockDirectoryInfo) |> ignore
  mockFileSystem.Setup(fun x -> <@ x.ReadAllText "file1.md" @>).Returns("mock file content") |> ignore

  let service = PostService (mockFileSystem.Create())

  // Act
  let result = service.getPreviousPostHTMLContent 1

  // Assert
  Assert.IsType<string>(result)

