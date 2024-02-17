namespace Pfsdb

open System
open Elmish
open ElmishStore
open Feliz
open Types

module BackingModel =
  
  type Model = {
    SpellName : string option
    AdvancedSearches : AdvancedSearch list
    FilterTargets : FilterTargets
    FullSpellList : Shared.Dtos.SpellRow list
    FilteredSpells : Shared.Dtos.SpellRow list option
  } with
    static member Empty() = {
      SpellName = None
      AdvancedSearches = [ AdvancedSearch.Empty() ]
      FilterTargets = FilterTargets.Empty()
      FullSpellList = []
      FilteredSpells = None
    }

    member this.SpellCount() = 
      match this.FilteredSpells with
      | None -> Seq.length this.FullSpellList
      | Some f -> Seq.length f

    member this.Spells() =
      match this.FilteredSpells with
      | Some f -> f
      | None -> this.FullSpellList

  let init() = Model.Empty(), Cmd.none
  
  type Msg =
  | ClearSearchName
  | SetSearchName of name : string
  | AddAdvancedSearch of newSearch : AdvancedSearch
  | UpdateAdvancedSearch of advSearch : AdvancedSearch
  | DeleteAdvancedSearch of id : Guid
  | SetFullSpellList of spells : Shared.Dtos.SpellRow seq
  | SetFilterTargets of ft : FilterTargets
  | DoFiltering

  let doFilteringCmd = Cmd.ofMsg DoFiltering

  let update msg (model : Model) =
    match msg with
    | ClearSearchName ->
      { model with SpellName = None }, doFilteringCmd
    | SetSearchName name ->
      if String.IsNullOrWhiteSpace name 
      then { model with SpellName = None }, doFilteringCmd
      else { model with SpellName = Some name }, doFilteringCmd
    | AddAdvancedSearch newSearch ->
      // we don't need to do filtering when a search is added because it will be new & empty
      { model with AdvancedSearches = model.AdvancedSearches @ [ newSearch ] }, Cmd.none
    | UpdateAdvancedSearch advSearch ->
      let updated = 
        model.AdvancedSearches
        |> List.map (fun a ->
          if a.Id = advSearch.Id then advSearch else a
        )
      { model with AdvancedSearches = updated }, doFilteringCmd
    | DeleteAdvancedSearch id ->
      let updated = model.AdvancedSearches |> List.filter (fun x -> x.Id <> id)
      { model with AdvancedSearches = updated }, doFilteringCmd
    | SetFullSpellList spells ->
      { model with FullSpellList = spells |> Seq.toList }, Cmd.none
    | SetFilterTargets ft ->
      { model with FilterTargets = ft }, Cmd.none
    | DoFiltering ->
      let search = Search.Create model.SpellName model.AdvancedSearches
      
      let filteredSpells = 
        if search.IsEmpty()
        then None
        else 
          SpellFiltering.filterSpells search model.FullSpellList
          |> Seq.toList
          |> Some
      { model with FilteredSpells = filteredSpells }, Cmd.none


  let store =
    Program.mkProgram init update (fun _ _ -> ())
    |> ElmishStore.createStore "backingModel"

  [<Hook>]
  let useSelector (selector : Model -> 'a) = React.useElmishStore(store, selector)

  [<Hook>]
  let useSelectorMemoized (memoizedSelector: Model -> 'a) = React.useElmishStoreMemoized (store, memoizedSelector)

  let dispatch = store.Dispatch
