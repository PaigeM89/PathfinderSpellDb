namespace Frontend

open System
open Types

module SpellFiltering =

  let private filterByName (search : Search) (spells : SpellRow seq) =
    match search.Name with
      | None -> spells
      | Some n ->
        spells |> Seq.filter (fun spell -> spell.Name.ToLowerInvariant().Contains n)

  let private filterBySchool (search : Search) (spells : SpellRow seq) =
    let schoolFilters = search.AdvancedSearches |> List.filter (fun a -> a.SearchType = School)
    let schools = schoolFilters |> List.collect (fun a -> a.Values) |> List.distinct
    match schools with
    | [] -> spells
    | schools ->
      printfn "Filtering by schools: %A" schools
      spells |> Seq.filter (fun spell -> List.contains spell.School schools)

  let private filterByCasterClass (search : Search) (spells : SpellRow seq) =
    let casterClassFilters = search.AdvancedSearches |> List.filter (fun a -> a.SearchType = CasterClass)
    let casterClasses = casterClassFilters |> List.collect (fun a -> a.Values) |> List.distinct
    match casterClasses with
    | [] -> spells
    | casterClasses ->
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