namespace PathfinderSpellDb

open Falco

module Handlers =

  type ClassSpellLevelDto = {
    ClassName : string
    Level : int
  } with
    static member FromClassSpellLevel (lvl : Types.ClassSpellLevel) =
      let (className, lvl) = lvl.ToTuple()
      {
        ClassName = className
        Level = lvl
      }

  type SpellListRowDto = {
    Id : int
    Name : string
    School : string
    ShortDescription : string
    ClassSpellLevels: ClassSpellLevelDto list
  } with
    static member Create id name school desc lvls = {
      Id = id
      Name = name
      School = school
      ShortDescription = desc
      ClassSpellLevels = lvls |> List.map ClassSpellLevelDto.FromClassSpellLevel
    }

  let mapSpellsToListDto (spells : Types.Spell list) =
    spells
    |> List.map (fun spell ->
      SpellListRowDto.Create spell.Id spell.Name spell.School spell.ShortDescription spell.ClassSpellLevels
    )

  let allSpells() = SpellParsing.spells |> mapSpellsToListDto

  let getAllSpells() = allSpells() |> Response.ofJson

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

  type SpellDto = {
    Id : int
    Name : string
    School : string
    Subschool : string option
    Descriptors : string list
    Description : string
    ClassSpellLevels : ClassSpellLevelDto list
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
      }

  let getSpell (id : int) =
    SpellParsing.findSpellByIndex id
    |> Option.map SpellDto.FromSpell
    |> Response.ofJson