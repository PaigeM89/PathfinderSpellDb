namespace PathfinderSpellDb

open Falco
open PathfinderSpellDb.Parsing

module Handlers =
  open DTOs
  let allSpells() = SpellParsing.spells |> mapSpellsToListDto

  let getAllSpells() = allSpells() |> Response.ofJson

  type PagingDto = {
    Offset: int
    Limit: int
  }

  type SpellSearchInputDto = {
    Name : string option
    School : string option
    Paging : PagingDto option
  }

  type SpellSearchResultDto = {
    SpellRows : SpellRowDto list
    ResultCount : int
    TotalCount: int
  } with
    static member Create spells count = {
      SpellRows = spells
      ResultCount = List.length spells
      TotalCount = count
    }

  let doSearchAndPaging (search : SpellSearchInputDto) =
    let spells = allSpells()
    let spells = 
      match search.Name with
      | Some name ->
        let name = name.ToLowerInvariant()
        spells |> List.filter (fun s -> s.Name.ToLowerInvariant().Contains(name))
      | None -> spells
    let spells =
      match search.School with
      | Some school ->
        let school = school.ToLowerInvariant()
        spells |> List.filter (fun s -> s.School.ToLowerInvariant() = school)
      | None -> spells
    // get count before paging
    let count = List.length spells
    let spells =
      match search.Paging with
      | Some paging ->
        spells |> List.skip paging.Offset |> List.take paging.Limit
      | None -> spells
    spells, count

  let handleSpellSearch : HttpHandler =
    let handleOk (search : SpellSearchInputDto) : HttpHandler =
      let spells, count = doSearchAndPaging search
      let dto = SpellSearchResultDto.Create spells count
      Response.ofJson dto
    Request.mapJson handleOk

  type SpellDto = {
    Id : int
    Name : string
    School : string
    Subschool : string option
    Descriptors : string list
    Description : string
    ClassSpellLevels : ClassSpellLevelDto list
    Domains : string
  } with
    static member FromSpell (spell : Types.Spell) =
      {
        Id = spell.Id
        Name = spell.Name
        School = spell.School
        Subschool = spell.SubSchool
        Descriptors = spell.Descriptors
        Description = spell.Description
        ClassSpellLevels = spell.ClassSpellLevels |> List.map ClassSpellLevelDto.FromClassSpellLevel
        Domains = spell.Domains |> List.map (fun d -> d.ToString()) |> String.concat ", "
      }

  let getSpell (id : int) =
    SpellParsing.findSpellByIndex id
    |> Option.map SpellDto.FromSpell
    |> Response.ofJson

  type ClassDto = {
    Name : string
  }

  let getClasses : HttpHandler =
    SpellParsing.allClasses
    |> List.map (fun className -> {
      Name = className
    })
    |> Response.ofJson