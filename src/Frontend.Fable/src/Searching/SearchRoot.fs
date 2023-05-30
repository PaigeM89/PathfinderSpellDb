namespace Frontend

open System
open Frontend.Searching
open Fable.Core
open Fable.Core.JsInterop
open Frontend.Types
open Elmish
open Feliz
open Feliz.DaisyUI

module SearchRoot =

  type Model = {
    NameSearchInput : string option
    ExpandedAdvancedSearchOptions : bool

    FilterTargets : FilterTargets
    AdvancedSearches : AdvancedSearch.Model list
  } with
    static member Init() = {
      NameSearchInput = None
      ExpandedAdvancedSearchOptions = false

      FilterTargets = FilterTargets.Empty()

      AdvancedSearches = [ AdvancedSearch.Model.Init() ]
    }

    member this.ToSearch() = {
      Name = this.NameSearchInput
      AdvancedSearches = this.AdvancedSearches |> List.map (fun a -> a.ToAdvancedSearch())
    }

  type Msg =
  | NameInputUpdated of string option
  | NameInputCompleted of string option
  | ToggleAdvancedSearch
  | AddAdvancedSearch

  | AdvancedSearchMsg of modelId : Guid * AdvancedSearch.Msg

  | AdvancedSearchUpdated of advSearch : AdvancedSearch.Model
  | DoFiltering
  // Raised for the parent component to handle, not of use within this element.
  | SearchUpdated of Types.Search

  let private updateAdvancedSearchOptions (model : Model) (advSearch : AdvancedSearch.Model) =
    match advSearch.SearchType with
    | Some School ->
      { advSearch with Options = model.FilterTargets.Schools }
    | Some CasterClass ->
      { advSearch with Options = model.FilterTargets.CasterClasses }
    | Some  Level ->
      { advSearch with Options = [0..9] |> List.map string }
    | Some CastingTime ->
      { advSearch with Options = model.FilterTargets.CastingTimes }
    | Some Components ->
      { advSearch with Options = model.FilterTargets.Components }
    | Some Range ->
      { advSearch with Options = model.FilterTargets.Ranges }
    | Some Duration ->  
      { advSearch with Options = model.FilterTargets.Durations }
    | Some SavingThrow ->
      { advSearch with Options = model.FilterTargets.SavingThrows }
    | Some SpellResistance ->
      { advSearch with Options = model.FilterTargets.SpellResistance }
    | Some Source ->
      { advSearch with Options = model.FilterTargets.Sources }
    | None -> advSearch

  let update msg (model: Model) =
    match msg with
    | NameInputUpdated nameOpt ->
      { model with
          NameSearchInput = nameOpt
      }, Cmd.none
    | NameInputCompleted nameOpt ->
      let model = { model with NameSearchInput = nameOpt }
      let search = model.ToSearch()
      model, Cmd.ofMsg (SearchUpdated search)
    | ToggleAdvancedSearch ->
      { model with ExpandedAdvancedSearchOptions = model.ExpandedAdvancedSearchOptions |> not }, Cmd.none
    | AddAdvancedSearch ->
      let model = { model with AdvancedSearches = model.AdvancedSearches @ [ Searching.AdvancedSearch.Model.Init() ] }
      model, Cmd.none
    
    | AdvancedSearchMsg (_, AdvancedSearch.DeleteAdvancedSearch id) ->
      let advSearches = model.AdvancedSearches |> List.filter (fun adv -> adv.Id <> id)
      { model with AdvancedSearches = advSearches }, Cmd.ofMsg (SearchUpdated <| model.ToSearch())
    
    | AdvancedSearchMsg (_, AdvancedSearch.AdvancedSearchUpdate advSearch) ->
      let advSearches =
        model.AdvancedSearches
        |> List.map (fun a -> if a.Id = advSearch.Id then advSearch else a)
      { model with AdvancedSearches = advSearches }, Cmd.ofMsg (SearchUpdated <| model.ToSearch())

    | AdvancedSearchMsg (_, AdvancedSearch.AdvancedSearchTypeUpdate advSearch) ->
      let advSearch = updateAdvancedSearchOptions model advSearch
      let advSearches =
        model.AdvancedSearches
        |> List.map (fun a -> if a.Id = advSearch.Id then advSearch else a)
      { model with AdvancedSearches = advSearches }, Cmd.ofMsg (SearchUpdated <| model.ToSearch())

    | AdvancedSearchMsg (modelId, msg) ->
      let x = 
        model.AdvancedSearches
        |> Seq.tryFind (fun a -> a.Id = modelId)
        |> Option.map (fun a -> AdvancedSearch.update msg a)
      
      match x with
      | Some (advSearchModel, cmd) ->
        let advancedSearches = model.AdvancedSearches |> List.map (fun a -> if a.Id = advSearchModel.Id then advSearchModel else a)
        let model = { model with AdvancedSearches = advancedSearches }
        let cmd = Cmd.map (fun cmd -> AdvancedSearchMsg(advSearchModel.Id, cmd)) cmd
        model, cmd
      | _ -> model, Cmd.none

    | AdvancedSearchUpdated advSearch ->
      let advSearches =
        model.AdvancedSearches
        |> List.map(fun a -> if a.Id = advSearch.Id then advSearch else a)
      
      { model with AdvancedSearches = advSearches }, Cmd.none

    // we need to make an Advanced Search component because otherwise this has to 
    // find the right Advanced Search element
    | DoFiltering -> 
      model, Cmd.none
    // This is raised from this element to notify parent elements
    // and doesn't need to be handled here.
    | SearchUpdated _ -> model, Cmd.none

  let debouncer = Debouncer("nameSearch", 500)

  module private Views =

    let nameSearch model dispatch =
      Html.div [
        prop.className "grid place-content-center"
        prop.children [
          Daisy.label [
            Daisy.labelText "Search by name"
          ]
          Daisy.input [
            input.bordered
            input.lg
            prop.placeholder "Spell name"
            match model.NameSearchInput with
            | Some s -> prop.value s
            | None -> prop.value ""
            prop.onInput(fun (e: Browser.Types.Event) ->
              let text = e.target?value
              NameInputUpdated (Some text) |> dispatch
              debouncer.Debounce dispatch (NameInputCompleted (Some text))
            )
          ]
        ]
      ]

    let expandedAdvancedSearchPanel model dispatch =
      Html.div [
        prop.children [
          for x in model.AdvancedSearches do AdvancedSearch.view x (fun m -> AdvancedSearchMsg (x.Id, m) |> dispatch)
          //advancedSearch model x dispatch
          Daisy.button.button [
            prop.text "Add advanced search"
            prop.onClick (fun _ -> AddAdvancedSearch |> dispatch)
            prop.className "mx-2 my-2"
          ]
        ]
      ]

    let advancedSearchPanel model dispatch =
      Html.div [
        prop.className "mt-8 cursor-pointer"
        prop.children [
          Daisy.divider [
            prop.onClick (fun _ -> ToggleAdvancedSearch |> dispatch)
            if model.ExpandedAdvancedSearchOptions then
              prop.text "Collapse Advanced Search"
            else 
              prop.text "Expand Advanced Search"
          ]
          if model.ExpandedAdvancedSearchOptions then expandedAdvancedSearchPanel model dispatch
        ]
      ]

  let view model dispatch =
    Html.div [
      Views.nameSearch model dispatch
      Views.advancedSearchPanel model dispatch
    ]