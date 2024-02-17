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
  | HasMythic
  | Source

  let searchTypeNames =
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
      HasMythic, "Is Mythic"
      Source, "Source"
    ] |> Map.ofList

  let searchTypePrompts =
    [
      School, "Select School(s)"
      CasterClass, "Select Class(es)"
      Level, "Select Spell Level(s)"
      CastingTime, "Select Casting Time(s)"
      Components, "Select Component(s)"
      Range, "Select Range(s)"
      Area, "Select Area(s)"
      Duration, "Select Duration(s)"
      SavingThrow, "Select Saving Throw(s)"
      SpellResistance, "Select Spell Resistance"
      HasMythic, "Is Mythic"
      Source, "Source"
    ]
    |> Map.ofList

  type AdvancedSearch = {
    Id : Guid
    SearchType : SearchType option
    EmptySearchText : string
    ValuesSearch : string option
    SelectedValues : string list
  } with
    static member Empty() = {
      Id = Guid.NewGuid()
      SearchType = None
      EmptySearchText = ""
      ValuesSearch = None
      SelectedValues = []
    }

    member this.SelectedValuesString() = String.Join(", ", this.SelectedValues)

    member this.IsEmpty() =
      this.SearchType.IsNone || (List.isEmpty this.SelectedValues)

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

    static member Create name advSearches = {
      SpellName = name
      AdvancedSearches = advSearches
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
    IsMythic : string list
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
      IsMythic = [ "Yes"; "No" ]
    }
