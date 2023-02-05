namespace PathfinderSpellDb

open Falco
open System.Text.Json

module Handlers =

  type SpellListRowDto = {
    Id : int
    Name : string
    School : string
    ShortDescription : string
  } with
    static member Create id name school desc = {
      Id = id
      Name = name
      School = school
      ShortDescription = desc
    }

  let mapSpellsToListDto (spells : Types.Spell list) =
    spells
    |> List.map (fun spell ->
      SpellListRowDto.Create spell.Id spell.Name spell.School spell.ShortDescription
    )

  let allSpells() = SpellParsing.spells |> mapSpellsToListDto


  let getAllSpells() =
    allSpells() |> Response.ofJson

  type SpellSearchInputDto = {
    Name : string option
    School : string option
  }

  let handleSpellSearch : HttpHandler =
    let handleOk (search : SpellSearchInputDto) : HttpHandler =
      match search.Name with
      | Some name ->
        SpellParsing.spellNameSearch name
        |> mapSpellsToListDto
        |> Response.ofJson
      | None ->
        getAllSpells()
    Request.mapJson handleOk