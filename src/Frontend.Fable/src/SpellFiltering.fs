namespace Frontend

open System
open Types
open Shared.Dtos

module SpellFiltering =

  let distinctValues (advSearches : AdvancedSearch list) =
    advSearches |> List.collect (fun a -> a.Values) |> List.distinct

  let searchesMatchingType (st : SearchType) (advSearches : AdvancedSearch list) =
    advSearches
    |> List.filter (fun advSearch ->
      match advSearch.SearchType with
      | Some searchType -> searchType = st
      | None -> false
    )

  let private filterByName (search : Search) (spells : SpellRow seq) =
    match search.Name with
      | None -> spells
      | Some n ->
        spells |> Seq.filter (fun spell -> spell.Name.ToLowerInvariant().Contains n)

  let private filterBySchool (search : Search) (spells : SpellRow seq) =
    let schoolFilters = search.AdvancedSearches |> searchesMatchingType School
    let schools = schoolFilters |> List.collect (fun a -> a.Values) |> List.distinct
    match schools with
    | [] -> spells
    | schools ->
      printfn "Filtering by schools: %A" schools
      spells |> Seq.filter (fun spell -> List.contains spell.School schools)

  let private filterByCasterClass (search : Search) (spells : SpellRow seq) =
    let casterClassFilters = search.AdvancedSearches |> searchesMatchingType CasterClass
    let casterClasses = casterClassFilters |> List.collect (fun a -> a.Values) |> List.distinct
    match casterClasses with
    | [] -> spells
    | casterClasses ->
      printfn "Filtering by caster classes: %A" casterClasses
      spells
      |> Seq.filter (fun spell -> 
            spell.ClassSpellLevels 
            |> Array.exists (fun csl -> List.contains csl.ClassName casterClasses) 
      )

  let private filterBySpellLevel (search : Search) (spells : SpellRow seq) =
    let spellLevelFilters = search.AdvancedSearches |> searchesMatchingType Level
    let spellLevels =
      spellLevelFilters |> List.collect (fun a -> a.Values) |> List.distinct
      |> List.map (fun i ->
        match Int32.TryParse i with
        | true, x -> x
        | false, _ -> -1
      )
    match spellLevels with
    | [] -> spells
    | _ ->
      spells
      |> Seq.filter (fun spell ->
        spell.ClassSpellLevels
        |> Array.exists (fun csl -> List.contains csl.Level spellLevels)
      )

  let private filterByCastingTime (search : Search) (spells : SpellRow seq) =
    let castingTimeFilters = search.AdvancedSearches |> searchesMatchingType CastingTime
    let castingTimes =
      castingTimeFilters
      |> List.collect (fun a -> a.Values)
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
      |> List.collect (fun a -> a.Values)
      |> List.distinct
    match components with
    | [] -> spells
    | _ ->
      printfn "Filtering spells for components %A" components
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

  let private filterByDuration (search : Search) (spells : SpellRow seq) =
    let filters = search.AdvancedSearches |> searchesMatchingType Duration
    let durations = filters |> distinctValues
    match durations with
    | [] -> spells
    | _ ->
      spells
      |> Seq.filter (fun spell -> List.contains spell.Duration durations)

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
    |> filterByCasterClass search
    |> filterBySpellLevel search
    |> filterByCastingTime search
    |> filterByComponent search
    |> filterByRange search
    |> filterByDuration search
    |> filterBySource search