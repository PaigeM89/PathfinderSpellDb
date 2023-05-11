namespace Frontend

open System
open Types

module SpellFiltering =

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

  let filterSpells (search : Search) (spells : SpellRow seq) =
    spells
    |> filterByName search
    |> filterBySchool search
    |> filterByCasterClass search