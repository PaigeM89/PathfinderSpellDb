namespace PathfinderSpellDb

open Falco
open System.Text.Json

module Handlers =

  type SpellSearchInputDto = {
    Name : string option
    School : string option
  }

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

  let handleSpellSearch : HttpHandler =
    let handleOk (search : SpellSearchInputDto) : HttpHandler =
      let message = sprintf "You searched on name %A and/or school %A" search.Name search.School
      Response.ofPlainText message
    Request.mapJson handleOk

  let allSpells() =
    SpellParsing.spells
    |> List.map (fun spell ->
      SpellListRowDto.Create spell.Id spell.Name spell.School spell.ShortDescription
    )

  let getAllSpells() =
    allSpells() |> Response.ofJson