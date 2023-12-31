namespace Pfsdb

open System
open Types
open Shared.Dtos

module SpellFiltering =

  let distinctValues (advSearches : AdvancedSearch list) =
    advSearches |> List.collect (fun a -> a.SelectedValues) |> List.distinct

  let searchesMatchingType (st : SearchType) (advSearches : AdvancedSearch list) =
    advSearches
    |> List.filter (fun advSearch ->
      match advSearch.SearchType with
      | Some searchType -> searchType = st
      | None -> false
    )

  let private filterByName (search : Search) (spells : SpellRow seq) =
    match search.SpellName with
      | None -> spells
      | Some n ->
        spells |> Seq.filter (fun spell -> spell.Name.ToLowerInvariant().Contains n)

  let private filterBySchool (search : Search) (spells : SpellRow seq) =
    let schoolFilters = search.AdvancedSearches |> searchesMatchingType School
    let schools = schoolFilters |> List.collect (fun a -> a.SelectedValues) |> List.distinct
    match schools with
    | [] -> spells
    | schools ->
      spells |> Seq.filter (fun spell -> List.contains spell.School schools)

  let private filterByClassAndLevel (search : Search) (spells : SpellRow seq) =
    let casterClassFilters = search.AdvancedSearches |> searchesMatchingType CasterClass
    let casterClasses = casterClassFilters |> List.collect (fun a -> a.SelectedValues) |> List.distinct
    let spellLevelFilters = search.AdvancedSearches |> searchesMatchingType Level
    let spellLevels =
      spellLevelFilters |> List.collect (fun a -> a.SelectedValues) |> List.distinct
      |> List.map (fun i ->
        match Int32.TryParse i with
        | true, x -> x
        | false, _ -> -1
      )
    match casterClasses, spellLevels with
    | [], [] -> spells
    | [], levels ->
      spells
      |> Seq.filter (fun spell ->
        spell.ClassSpellLevels
        |> Array.exists (fun csl -> List.contains csl.Level levels)
      )
    | classes, [] ->
      spells
      |> Seq.filter (fun spell -> 
            spell.ClassSpellLevels 
            |> Array.exists (fun csl -> List.contains csl.ClassName classes) 
      )
    | classes, levels ->
      spells
      |> Seq.filter (fun spell ->
        spell.ClassSpellLevels
        |> Array.exists (fun csl -> List.contains csl.Level levels && List.contains csl.ClassName classes)
      )

  let private filterByCastingTime (search : Search) (spells : SpellRow seq) =
    let castingTimeFilters = search.AdvancedSearches |> searchesMatchingType CastingTime
    let castingTimes =
      castingTimeFilters
      |> List.collect (fun a -> a.SelectedValues)
      |> List.distinct
    match castingTimes with
    | [] -> spells
    | _ ->
      spells
      |> Seq.filter (fun spell -> List.contains spell.CastingTime castingTimes)

  let private filterByComponent (search : Search) (spells : SpellRow seq) =
    let componentFilters = search.AdvancedSearches |> searchesMatchingType Components
    let components =
      componentFilters
      |> List.collect (fun a -> a.SelectedValues)
      |> List.distinct
    match components with
    | [] -> spells
    | _ ->
      spells
      |> Seq.filter (fun spell ->
        spell.Components
        |> Seq.exists (fun comp ->
          List.contains comp.Name components
        )
      )

  let private filterByRange (search : Search) (spells : SpellRow seq) =
    let filters = search.AdvancedSearches |> searchesMatchingType Range
    let ranges = filters |> distinctValues
    match ranges with
    | [] -> spells
    | _ ->
      spells
      |> Seq.filter (fun spell -> List.contains spell.Range ranges)

  let private filterByArea (search : Search) (spells : SpellRow seq) =
    let filters = search.AdvancedSearches |> searchesMatchingType Area
    let areas = filters |> distinctValues
    match areas with
    | [] -> spells
    | _ ->
      spells
      |> Seq.filter (fun spell -> List.contains spell.Area areas)

  let private filterByDuration (search : Search) (spells : SpellRow seq) =
    let filters = search.AdvancedSearches |> searchesMatchingType Duration
    let durations = filters |> distinctValues
    match durations with
    | [] -> spells
    | _ ->
      spells
      |> Seq.filter (fun spell -> List.contains spell.Duration durations)

  let private filterBySavingThrow (search : Search) (spells : SpellRow seq) =
    let filters = search.AdvancedSearches |> searchesMatchingType SavingThrow
    let savingThrows = filters |> distinctValues
    match savingThrows with
    | [] -> spells
    | _ ->
      spells
      |> Seq.filter (fun spell -> List.contains spell.SavingThrowStr savingThrows)

  let private filterBySpellResistance (search : Search) (spells : SpellRow seq) =
    let filters = search.AdvancedSearches |> searchesMatchingType SpellResistance
    let spellRes = filters |> distinctValues
    match spellRes with
    | [] -> spells
    | _ -> 
      let spellRes = spellRes |> Seq.map (fun sr -> if sr = "Yes" then true else false) |> Seq.toList
      spells
      |> Seq.filter (fun spell -> List.contains spell.SpellResistance spellRes)

  let private filterByIsMythic (search : Search) (spells : SpellRow seq) =
    let filters = search.AdvancedSearches |> searchesMatchingType HasMythic
    let isMythic = filters |> distinctValues
    match isMythic with
    | [] -> spells
    | _ ->
      let isMythic = isMythic |> Seq.map (fun m -> if m = "Yes" then true else false) |> Seq.toList
      spells
      |> Seq.filter (fun spell -> List.contains spell.HasMythic isMythic)

  let private filterBySource (search : Search) (spells : SpellRow seq) =
    let filters = search.AdvancedSearches |> searchesMatchingType Source
    let sources = filters |> distinctValues
    match sources with
    | [] -> spells
    | _ ->
      spells
      |> Seq.filter (fun spell -> List.contains spell.Source sources)

  let filterSpells (search : Search) (spells : SpellRow seq) =
    spells
    |> filterByName search
    |> filterBySchool search
    |> filterByClassAndLevel search
    |> filterByCastingTime search
    |> filterByComponent search
    |> filterByRange search
    |> filterByArea search
    |> filterByDuration search
    |> filterBySavingThrow search
    |> filterBySpellResistance search
    |> filterByIsMythic search
    |> filterBySource search