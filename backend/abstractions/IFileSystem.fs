module Backend.Abstractions.FileSystem

open System.IO
open System.Threading.Tasks

type IFileSystem =
  abstract member GetFiles : string -> string array
  abstract member DirectoryInfo : string -> DirectoryInfo
  abstract member ReadAllTextAsync : string -> Task<string>
