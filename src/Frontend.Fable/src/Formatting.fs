namespace Frontend

open Frontend

module Formatting =
  let fixSummonerUnchained (s : string) =
    if s = "SummonerUnchained" then "Summoner Unchained" else s

  let componentsStr (components : Shared.Dtos.Component seq) =
    components |> Seq.map (fun c -> c.Name) |> Seq.distinct |> Frontend.String.join