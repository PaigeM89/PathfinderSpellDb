namespace Frontend

open System

module Types =
  type ClassSpellLevel = {
    ClassName: string
    Level : int
  }

  type Component = {
    Name : string
    Abbr : string
    Cost : string option
  }

  type SpellRow = {
    Id : int
    Name : string
    School : string
    ShortDescription : string
    ClassSpellLevels : ClassSpellLevel[]
    CastingTime : string
    Components : Component list
    Range : string
    Duration : string
    Source : string
  }

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

  type Search = {
    Name : string option
    AdvancedSearches : AdvancedSearch list
  } with
    static member Empty() = {
      Name = None
      // start with an advanced search option
      AdvancedSearches = [ AdvancedSearch.Empty() ]
    }

  module Search =
    let forName n (s : Search) = { s with Name = Some n }
    let clearName (s : Search) = { s with Name = None }
    let setName n (s : Search) = { s with Name = n }

    let addAdvancedSearch (s : Search) =
      { s with AdvancedSearches = AdvancedSearch.Empty() :: s.AdvancedSearches }
    
    let removeAdvancedSearch id (s : Search) = 
      { s with AdvancedSearches = s.AdvancedSearches |> List.filter (fun a -> a.Id <> id) }

    let replaceAdvancedSearch (advSearch : AdvancedSearch) (s : Search) =
      { s with 
          AdvancedSearches =
            s.AdvancedSearches
            |> List.map (fun a -> if a.Id = advSearch.Id then advSearch else a)
      }