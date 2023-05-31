namespace PathfinderSpellDb

open Shared.Dtos
open PathfinderSpellDb.Parsing.Types
open PathfinderSpellDb.Parsing

module DTOs =

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
      SavingThrowStr = if spell.SavingThrowsStr.Trim() = "" then "None" else spell.SavingThrowsStr
      SpellResistance = spell.SpellResistance
      Source = spell.Source
    }

  let mapSpellsToListDto (spells : Types.Spell list) =
    spells
    |> List.map createSpellRowDto
