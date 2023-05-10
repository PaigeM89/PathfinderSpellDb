namespace Frontend


module Formatting =
  let fixSummonerUnchained (s : string) =
    if s = "SummonerUnchained" then "Summoner Unchained" else s
