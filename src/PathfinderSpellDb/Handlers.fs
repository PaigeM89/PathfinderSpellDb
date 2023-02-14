namespace PathfinderSpellDb

open Falco
open PathfinderSpellDb.Parsing

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

  type ComponentDto = {
    Name: string
    Abbr: string
    Cost: string option
  } with
    static member Create name abbr cost = {
      Name = name
      Abbr = abbr
      Cost = cost
    }

    static member FromCastingComponent (comp : Types.CastingComponent) =
      match comp with
      | Types.CastingComponent.Verbal -> ComponentDto.Create "Verbal" "V" None
      | Types.CastingComponent.Somatic -> ComponentDto.Create "Somatic" "S" None
      | Types.CastingComponent.Material mat ->
        let matStr = mat |> Option.map (fun s -> sprintf " (%s)" s) |> Option.defaultValue ""
        ComponentDto.Create (sprintf "Material%s" matStr) "M" None
      | Types.CastingComponent.CostlyMaterial(mat, cost) ->
        let matStr = sprintf " (%s, %i gp)" mat cost
        ComponentDto.Create (sprintf "Material%s" matStr) "M" None
      | Types.CastingComponent.Focus focus ->
        let focusStr = focus |> Option.map (fun s -> sprintf " (%s)" s) |> Option.defaultValue ""
        ComponentDto.Create (sprintf "Focus%s" focusStr) "F" None
      | Types.CastingComponent.DivineFocus -> ComponentDto.Create "Divine Focus" "DF" None

  let rangeToString (range: Types.Range) =
    match range with
    | Types.Range.Personal -> "Personal"
    | Types.Range.Touch -> "Touch"
    | Types.Range.Close -> "25 ft. + 5 ft./2 levels"
    | Types.Range.Medium -> "100 ft. + 10 ft./level"
    | Types.Range.Long -> "400 ft. + 40 ft./level"
    | Types.Range.Unlimited -> "Unlimited"
    | Types.Range.Other s -> s

  let durationToString (duration : Types.Duration) =
    match duration with
    | Types.Duration.Instantaneous -> "Instantaneous"
    | Types.Duration.RoundPerLevel -> "1 round/level"
    | Types.Duration.MinutePerLevel -> "1 minute/level"
    | Types.Duration.HourPerLevel -> "1 hour/level"
    | Types.Duration.DayPerLevel -> "1 day/level"
    | Types.Duration.Permanent -> "Permanent"
    | Types.Duration.Other s -> s
    | Types.Duration.SeeText -> "See Text"

  type SpellRowDto = {
    Id : int
    Name : string
    School : string
    ShortDescription : string
    ClassSpellLevels: ClassSpellLevelDto list
    CastingTime : string
    Components : ComponentDto list
    Range : string
    Duration : string
  } with
    static member Create id name school desc lvls time comps range duration = {
      Id = id
      Name = name
      School = school
      ShortDescription = desc
      ClassSpellLevels = lvls |> List.map ClassSpellLevelDto.FromClassSpellLevel
      CastingTime = time
      Components = comps
      Range = range
      Duration = duration
    }

  let mapSpellsToListDto (spells : Types.Spell list) =
    spells
    |> List.map (fun spell ->
      let time = spell.CastingTime.ToString()
      let componentDtos = spell.Components |> List.map ComponentDto.FromCastingComponent
      let range = rangeToString spell.Range
      let duration = durationToString spell.Duration
      SpellRowDto.Create spell.Id spell.Name spell.School spell.ShortDescription spell.ClassSpellLevels time componentDtos range duration
    )

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

  type ClassDto = {
    Name : string
  }

  let getClasses : HttpHandler =
    SpellParsing.allClasses
    |> List.map (fun className -> {
      Name = className
    })
    |> Response.ofJson