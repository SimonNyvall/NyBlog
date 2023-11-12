module Backend.Abstractions.FileSystem

open System.IO

type IFileSystem =
  abstract member GetFiles : string -> string array
  abstract member DirectoryInfo : string -> DirectoryInfo
  abstract member ReadAllText : string -> string
