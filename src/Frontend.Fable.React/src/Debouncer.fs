namespace Pfsdb

open System.Collections.Generic

type Debouncer(key, timeout: int) =
  let mutable timeoutDict = Dictionary<string, int>(HashIdentity.Structural)
  
  member this.Debounce f value =
    match timeoutDict.TryGetValue(key) with
    | true, timeoutId -> Fable.Core.JS.clearTimeout timeoutId
    | _ -> ()

    let func() =  
      timeoutDict.Remove key |> ignore
      f value

    let timeoutId =
      Fable.Core.JS.setTimeout func timeout
    
    timeoutDict.Remove key |> ignore
    timeoutDict.Add(key, timeoutId) |> ignore
