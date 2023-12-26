namespace Pfsdb

module Version =
  open Fable.Core

  [<Emit("ENV.VERSION")>]
  let __VERSION__ : string = jsNative

module Api =
  open System

  let apiRoute = 
    #if DEBUG
      "http://localhost:5000"
    #else
      "https://api.pathfinderspelldb.com"
    #endif

  let concat l r = l + "/" + r

  let (/) = concat

  module Spells = 
    let root = apiRoute / "spells"
    let spell id = root / (sprintf "%i" id)

module String =
  open System
  
  let join (xs : string seq) = String.Join(", ", xs)