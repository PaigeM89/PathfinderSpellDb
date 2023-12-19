namespace Pfsdb

module Version =

  open Fable.Core

  [<Emit("VERSION")>]
  let __VERSION__ : string = jsNative

module ApiRoot =
  let apiRoute = 
    #if DEBUG
      "http://localhost:5000"
    #else
      "https://api.pathfinderspelldb.com"
    #endif
    
module String =
  open System
  
  let join (xs : string seq) = String.Join(", ", xs)