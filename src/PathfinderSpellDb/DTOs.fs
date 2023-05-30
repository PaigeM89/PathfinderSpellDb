namespace PathfinderSpellDb

open PathfinderSpellDb.Parsing.Types
open PathfinderSpellDb.Parsing

module DTOs =

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

    member this.ToTuple() = (this.ClassName, this.Level)

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

  // type SpellRowDto = {
  //   Id : int
  //   Name : string
  //   School : string
  //   ShortDescription : string
  //   ClassSpellLevels: ClassSpellLevelDto list
  //   CastingTime : string
  //   Components : ComponentDto list
  //   Range : string
  //   Duration : string
  //   Source : string
  // } with
  //   static member Create id name school desc lvls time comps range duration source = {
  //     Id = id
  //     Name = name
  //     School = school
  //     ShortDescription = desc
  //     ClassSpellLevels = lvls |> List.map ClassSpellLevelDto.FromClassSpellLevel
  //     CastingTime = time
  //     Components = comps
  //     Range = range
  //     Duration = duration
  //     Source = source
  //   }

  let castingComponentToDto (cc : Types.CastingComponent) =
    match cc with
    | CastingComponent.Verbal -> Shared.Dtos.Component.Create "Verbal" "V" None
    | CastingComponent.Somatic -> Shared.Dtos.Component.Create "Somatic" "S" None
    | CastingComponent.Material material ->
      match material with
      | Some material ->
        Shared.Dtos.Component.Create (sprintf "Material (%s)" material) "M" None
      | None ->
        Shared.Dtos.Component.Create "Material" "M" None
    | CastingComponent.CostlyMaterial (material, cost) ->
      let material = sprintf "Material (%s, %igp)" material cost
      let abbr = sprintf "M (%igp)" cost
      Shared.Dtos.Component.Create material abbr (Some (string cost))
    | CastingComponent.Focus focus ->
      match focus with
      | Some focus ->
        let name = sprintf "Focus (%s)" focus
        Shared.Dtos.Component.Create name "F" None
      | None ->
        Shared.Dtos.Component.Create "Focus" "F" None
    | CastingComponent.DivineFocus ->
      Shared.Dtos.Component.Create "Divine Focus" "DF" None

  let createSpellRowDto (spell : Types.Spell) : Shared.Dtos.SpellRow =
    let time = spell.CastingTime.ToString()
    let componentDtos = spell.Components |> List.map castingComponentToDto
      //|> List.map ComponentDto.FromCastingComponent
    let range = rangeToString spell.Range
    let duration = durationToString spell.Duration
    let classSpellLevels =
      spell.ClassSpellLevels
      |> List.map (fun csl ->
        let t = csl.ToTuple()
        fst t, snd t
      )
    {
      Id = spell.Id
      Name = spell.Name
      School = spell.School
      ShortDescription = spell.ShortDescription
      ClassSpellLevels = classSpellLevels |> List.map (fun (c, l) -> Shared.Dtos.ClassSpellLevel.Create c l) |> List.toArray
      ClassSpellLevelsString = classSpellLevels |> List.map (fun (c, l) -> sprintf "%s %i" c l) |> String.join
      CastingTime = time
      Components = componentDtos
      Range = range
      Duration = duration
      Source = spell.Source
    }

  let mapSpellsToListDto (spells : Types.Spell list) =
    spells
    |> List.map (fun spell ->
      // let time = spell.CastingTime.ToString()
      // let componentDtos = spell.Components |> List.map ComponentDto.FromCastingComponent
      // let range = rangeToString spell.Range
      // let duration = durationToString spell.Duration
      //SpellRowDto.Create spell.Id spell.Name spell.School spell.ShortDescription spell.ClassSpellLevels time componentDtos range duration spell.Source
      createSpellRowDto spell
    )

  // type SpellDto = {
  //   Id : int
  //   Name : string
  //   School : string
  //   Subschool : string option
  //   Descriptors: string list
  //   ShortDescription : string
  //   FullDescription : string
  //   ClassSpellLevels: ClassSpellLevelDto list
  //   Domains : string
  //   CastingTime : string
  //   Components : ComponentDto list
  //   Range : string
  //   Duration : string
  //   Source : string
  // }

module GraphQL =
  open Types
  open FSharp.Data.GraphQL.Types
  open DTOs

  let thing = 0
  // let findSpellByName (name : string) = 
  //   SpellParsing.spells |> List.tryFind (fun s -> s.Name.ToLowerInvariant() = name)

  // let findSpellByIndex (index: int) =
  //   SpellParsing.spells |> List.tryItem index

  // let spellNameSearch (str : string) =
  //   if (str.Length > 1) then
  //     let str : string = str.ToLowerInvariant()
  //     SpellParsing.spells |> List.filter (fun s -> s.Name.ToLowerInvariant().Contains(str))
  //   else
  //     SpellParsing.spells

  /// GraphQL type
  // let SpellType : ObjectDef<SpellDto> =
  //   Define.Object<SpellDto>(
  //     name = "Spell",
  //     description = "A magical effect created by a character when cast.",
  //     fields = [
  //       Define.Field("id", Int, "The id of the spell. Used for querying. Generated on data load", fun _ (s : SpellDto) -> s.Id)
  //       Define.Field("name", String, "The name of the spell", fun _ (s : SpellDto) -> s.Name)
  //       Define.Field("school", String, "The school of the spell", fun _ (s : SpellDto) -> s.School)
  //       Define.Field("subschool", Nullable String, "The subschool of the spell, if any", fun _ (s : SpellDto) -> s.Subschool)
  //       Define.Field("descriptors", ListOf String, "The descriptors of the spell, if any", fun _ (s: SpellDto) -> s.Descriptors)
  //       Define.Field("shortDescription", String, "The short description of the spell", fun _ (s : SpellDto) -> s.ShortDescription)
  //       Define.Field("fullDescription", String, "The full HTML description of the spell", fun _ (s: SpellDto) -> s.FullDescription)
  //       //Define.Field("classSpellLevels", ListOf ClassSpellLevelDto, "The class & spell levels for this spell", fun _ (s: SpellDto) -> s.ClassSpellLevels |> List.map ClassSpellLevelDto.FromClassSpellLevel)
  //     ]
  //   )