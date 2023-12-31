namespace PathfinderSpellDb

open Falco
open PathfinderSpellDb.Parsing

module Handlers =
  open DTOs

  let allSpells config = (SpellParsing.loadedSpells config).Value |> mapSpellsToListDto

  let getAllSpells config = allSpells config |> Response.ofJson

  let lazyLoadSpells config = SpellParsing.loadedSpells config

  let getSpell (spells : Types.Spell list) (id : int) =
    spells
    |> List.tryItem id
    |> Option.map DTOs.toSpellDto
    |> Response.ofJson