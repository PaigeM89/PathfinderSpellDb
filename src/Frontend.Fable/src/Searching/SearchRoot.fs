namespace Frontend

open System
open Fable.Core
open Frontend.Types
open Elmish
open Feliz
open Feliz.DaisyUI

module SearchRoot =

  type Model = {
    NameSearchInput : string option
    ExpandedAdvancedSearchOptions : bool

    Search : Types.Search
  } with
    static member Init() = {
      NameSearchInput = None
      ExpandedAdvancedSearchOptions = false

      Search = Search.Empty()
    }

  type Msg =
  | NameInputUpdated of string option
  | SearchUpdated of Types.Search
  | ToggleAdvancedSearch

  let update msg (model: Model) =
    match msg with
    | NameInputUpdated nameOpt ->
      let model = { model with NameSearchInput = nameOpt }
      let search = model.Search |> Search.setName nameOpt
      { model with Search = search }, Cmd.ofMsg (SearchUpdated search)
    | ToggleAdvancedSearch ->
      { model with ExpandedAdvancedSearchOptions = model.ExpandedAdvancedSearchOptions |> not }, Cmd.none
    // This is raised from this element to notify parent elements
    // and doesn't need to be handled here.
    | SearchUpdated _ -> model, Cmd.none

  module private Views =
    let private removeSpaces (s : string) = s.Replace(" ", "")

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
            prop.onChange (fun (s : string) ->
              if removeSpaces s = "" then 
                NameInputUpdated None |> dispatch
              else
                NameInputUpdated (Some s) |> dispatch
            )
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
          if model.ExpandedAdvancedSearchOptions then 
            Html.div [ prop.text "This is additional content" ]
        ]
      ]

  let view model dispatch =
    Html.div [
      Views.nameSearch model dispatch
      Views.advancedSearchPanel model dispatch
    ]