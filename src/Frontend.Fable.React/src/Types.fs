namespace Pfsdb

open System

module Types =

  let GithubLink = "https://github.com/PaigeM89/PathfinderSpellDb"

  type SearchType =
  | School
  | CasterClass
  | Level
  | CastingTime
  | Components
  | Range
  | Area
  | Duration
  | SavingThrow
  | SpellResistance
  | Source

  let searchTypeName =
    [
      School, "School"
      CasterClass, "Class"
      Level, "Spell Level"
      CastingTime, "Casting Time"
      Components, "Components"
      Range, "Range"
      Area, "Area"
      Duration, "Duration"
      SavingThrow, "Saving Throw"
      SpellResistance, "Spell Resistance"
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
    SpellName : string option
    AdvancedSearches : AdvancedSearch list
  } with
    static member Empty() = {
      SpellName = None
      // start with an advanced search option, but empty,
      // so the user has something to interact with immediately
      AdvancedSearches = [ AdvancedSearch.Empty() ]
    }

    member this.IsEmpty() = 
      let nameEmpty = this.SpellName.IsNone
      match this.AdvancedSearches with
      | [] -> nameEmpty
      | xs -> nameEmpty && xs |> List.forall (fun a -> a.IsEmpty())

  module Search =
    let forSpellName n (s : Search) = { s with SpellName = Some n }
    let clearSpellName (s : Search) = { s with SpellName = None }
    let setSpellName n (s : Search) = { s with SpellName = n }

    let maybeSetSpellName name (s : Search) = 
      if String.IsNullOrWhiteSpace name then clearSpellName s else setSpellName (Some name) s

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
    Areas : string list
    Durations : string list
    SavingThrows : string list
    SpellResistance : string list
    Sources : string list
  } with
    static member Empty() = {
      Schools = []
      CasterClasses = []
      CastingTimes = []
      Components = []
      Ranges = []
      Areas = []
      Durations = []
      SavingThrows = []
      SpellResistance = [ "Yes"; "No" ]
      Sources = []
    }