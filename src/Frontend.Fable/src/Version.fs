namespace Frontend

module Version =

  open Fable.Core

  [<Emit("VERSION")>]
  let __VERSION__ : string = jsNative

  