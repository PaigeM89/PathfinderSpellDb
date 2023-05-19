namespace Frontend

open System
open Shared.Dtos

module Types =

  type SearchType =
  | School
  | CasterClass
  | Level
  | CastingTime
  | Components
  | Range
  | Duration
  | Source


  let searchTypeName =
    [
      School, "School"
      CasterClass, "Class"
      Level, "Spell Level"
      CastingTime, "Casting Time"
      Components, "Components"
      Range, "Range"
      Duration, "Duration"
      Source, "Source"
    ] |> Map.ofList

  type AdvancedSearch = {
    Id : Guid
    SearchType : SearchType option
    Values : string list
  } with
    static member Empty() = {
      Id = Guid.NewGuid()
      SearchType = None
      Values = []
    }

    member this.ValuesString() = String.Join(", ", this.Values)

    member this.IsEmpty() =
      this.SearchType.IsNone || (List.isEmpty this.Values)

  type Search = {
    Name : string option
    AdvancedSearches : AdvancedSearch list
  } with
    static member Empty() = {
      Name = None
      // start with an advanced search option
      AdvancedSearches = [ AdvancedSearch.Empty() ]
    }

    member this.IsEmpty() = 
      let nameEmpty = this.Name.IsNone
      match this.AdvancedSearches with
      | [] -> nameEmpty
      | xs -> nameEmpty && xs |> List.forall (fun a -> a.IsEmpty())

  module Search =
    let forName n (s : Search) = { s with Name = Some n }
    let clearName (s : Search) = { s with Name = None }
    let setName n (s : Search) = { s with Name = n }

    let addAdvancedSearch (s : Search) =
      { s with AdvancedSearches = s.AdvancedSearches @ [ AdvancedSearch.Empty() ]}
    
    let removeAdvancedSearch id (s : Search) = 
      { s with AdvancedSearches = s.AdvancedSearches |> List.filter (fun a -> a.Id <> id) }

    let replaceAdvancedSearch (advSearch : AdvancedSearch) (s : Search) =
      { s with 
          AdvancedSearches =
            s.AdvancedSearches
            |> List.map (fun a -> if a.Id = advSearch.Id then advSearch else a)
      }

  type FilterTargets = {
    Schools : string list
    CasterClasses : string list
    CastingTimes : string list
    Components : string list
    Ranges : string list
    Durations : string list
    Sources : string list
  } with
    static member Empty() = {
      Schools = []
      CasterClasses = []
      CastingTimes = []
      Components = []
      Ranges = []
      Durations = []
      Sources = []
    }

module String =
  let join (xs : string seq) = String.Join(", ", xs)