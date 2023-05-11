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

    Schools : string list
    CasterClasses : string list

    Search : Types.Search
  } with
    static member Init() = {
      NameSearchInput = None
      ExpandedAdvancedSearchOptions = false

      Schools = []
      CasterClasses = []

      Search = Search.Empty()
    }

  type Msg =
  | NameInputUpdated of string option
  | ToggleAdvancedSearch
  | AddAdvancedSearch
  | DeleteAdvancedSearch of id : Guid
  | AdvancedSearchUpdated of advSearch : AdvancedSearch
  // Raised for the parent component to handle, not of use within this element.
  | SearchUpdated of Types.Search

  let update msg (model: Model) =
    match msg with
    | NameInputUpdated nameOpt ->
      let model = { model with NameSearchInput = nameOpt }
      let search = model.Search |> Search.setName nameOpt
      { model with Search = search }, Cmd.ofMsg (SearchUpdated search)
    | ToggleAdvancedSearch ->
      { model with ExpandedAdvancedSearchOptions = model.ExpandedAdvancedSearchOptions |> not }, Cmd.none
    | AddAdvancedSearch ->
      let search = model.Search |> Search.addAdvancedSearch
      { model with Search = search }, Cmd.none
    | DeleteAdvancedSearch id ->  
      let search = model.Search |> Search.removeAdvancedSearch id
      { model with Search = search }, Cmd.ofMsg (SearchUpdated search)
    | AdvancedSearchUpdated advSearch ->
      printfn "Advanced search updated: %A" advSearch
      let search = model.Search |> Search.replaceAdvancedSearch advSearch
      { model with Search = search }, Cmd.ofMsg (SearchUpdated search)
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

    let advancedSearch model advSearch dispatch =
      let dropdownElements = 
        searchTypes
        |> List.map (fun (st, text) ->
          Html.li [ 
            prop.children [
              Html.a [ 
                prop.text text
              ]
            ]
            prop.onClick (fun _ -> 
                { advSearch with SearchType = st; Values = [] } |> AdvancedSearchUpdated |> dispatch)
          ]
        )
      
      Html.div [
        prop.id (string advSearch.Id)
        prop.children [
          Daisy.dropdown [
            Daisy.button.button [
              button.primary
              prop.text (Map.tryFind advSearch.SearchType searchTypeName |> Option.defaultValue "Error")
            ]
            Daisy.dropdownContent [
              prop.className "p-2 shadow menu bg-base-100 rounded-box w-52"
              prop.tabIndex 0
              prop.children dropdownElements
            ]
          ]
          match advSearch.SearchType with
          | School -> //schoolSearch model advSearch dispatch
            Searching.SearchDropdowns.schoolSearch model.Schools advSearch (AdvancedSearchUpdated >> dispatch)
          | CasterClass -> //casterClassSearch model advSearch dispatch
            Searching.SearchDropdowns.casterClassSearch model.CasterClasses advSearch (AdvancedSearchUpdated >> dispatch)
          | _ -> Html.div []
          Daisy.button.button [
            prop.text "Delete"
            prop.onClick (fun _ -> DeleteAdvancedSearch advSearch.Id |> dispatch)
          ]
        ]
      ]

    let expandedAdvancedSearchPanel model dispatch =
      Html.div [
        prop.children [
          for x in model.Search.AdvancedSearches do advancedSearch model x dispatch
          Daisy.button.button [
            prop.text "Add advanced search"
            prop.onClick (fun _ -> AddAdvancedSearch |> dispatch)
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