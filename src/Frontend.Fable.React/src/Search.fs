namespace Pfsdb

open System
open Pfsdb.Types
open Shared.Dtos
open Fable.Core
open Fable.Core.JsInterop
open Feliz
open Feliz.DaisyUI
open BackingModel

module Searching =

  let debouncer = Debouncer("spellNameSearch", 500)

  [<ReactComponent>]
  let AdvancedSearchInput(advancedSearch : AdvancedSearch, onUpdate, onDelete) =
    let filterTargets = BackingModel.useSelector(fun bm -> bm.FilterTargets)

    let dropdownElements = 
      searchTypeNames
      |> Map.toList
      |> List.map (fun (st, text) ->
        Html.li [ 
          prop.children [
            Html.a [
              prop.text text
            ]
          ]
          prop.onClick (fun _ ->
            let nm = { advancedSearch with SearchType = Some st }
            nm |> BackingModel.UpdateAdvancedSearch |> BackingModel.dispatch
            onUpdate nm
          )
        ]
      )
    
    Html.div [
      prop.id (string advancedSearch.Id)
      prop.key advancedSearch.Id
      prop.children [
        Daisy.dropdown [
          Daisy.button.button [
            button.primary
            match advancedSearch.SearchType with
            | None -> prop.text "Select a field"
            | Some st ->
                prop.text (Map.tryFind st searchTypeNames |> Option.defaultValue "Error")
            prop.className "mx-2 my-2"
          ]
          Daisy.dropdownContent [
            prop.className "p-2 shadow menu bg-base-100 rounded-box w-52"
            prop.tabIndex 0
            prop.children dropdownElements
          ]
        ]

        match advancedSearch.SearchType with
        | Some School ->
          SearchDropdown.schoolSearch filterTargets.Schools advancedSearch onUpdate
        | Some CasterClass ->
          SearchDropdown.casterClassSearch filterTargets.CasterClasses advancedSearch onUpdate
        | Some Level ->
          SearchDropdown.spellLevelSearch advancedSearch onUpdate
        | Some CastingTime ->
          SearchDropdown.castingTimeSearch filterTargets.CastingTimes advancedSearch onUpdate
        | Some Components ->
          SearchDropdown.componentSearch filterTargets.Components advancedSearch onUpdate
        | Some Range ->
          SearchDropdown.rangeSearch filterTargets.Ranges advancedSearch onUpdate
        | Some Area ->
          SearchDropdown.areaSearch filterTargets.Areas advancedSearch onUpdate
        | Some Duration ->
          SearchDropdown.durationSearch filterTargets.Durations advancedSearch onUpdate
        | Some SavingThrow -> 
          SearchDropdown.savingThrowsSearch filterTargets.SavingThrows advancedSearch onUpdate
        | Some SpellResistance ->
          SearchDropdown.spellResistanceSearch filterTargets.SpellResistance advancedSearch onUpdate
        | Some HasMythic ->
          SearchDropdown.hasMythicSearch filterTargets.IsMythic advancedSearch onUpdate
        | Some Source ->
          SearchDropdown.sourcesSearch filterTargets.Sources advancedSearch onUpdate
        | _ -> Html.none

        Daisy.button.button [
          prop.text "Delete"
          prop.onClick (fun _ -> advancedSearch.Id |> onDelete)
          prop.className "mx-2 my-2"
        ]
      ]
    ]

  [<ReactComponent>]
  let AdvancedSearchInputList(initial : AdvancedSearch list) =
    let advancedSearches, setAdvancedSearches = React.useState initial

    let onAddClick() = 
      let newSearch = AdvancedSearch.Empty()
      BackingModel.AddAdvancedSearch newSearch |> BackingModel.dispatch
      setAdvancedSearches (advancedSearches @ [newSearch])
    
    let onUpdate advSearch =
      BackingModel.UpdateAdvancedSearch advSearch |> BackingModel.dispatch
      let x = advancedSearches |> List.map (fun a -> if a.Id = advSearch.Id then advSearch else a)
      setAdvancedSearches x

    let onDelete id = 
      id |> BackingModel.DeleteAdvancedSearch |> BackingModel.dispatch
      let newSearches = advancedSearches |> List.filter (fun a -> a.Id <> id)
      setAdvancedSearches newSearches

    Html.div [
      prop.children [
        for advancedSearchInput in advancedSearches do
          AdvancedSearchInput(advancedSearchInput, onUpdate, onDelete)

        Daisy.button.button [
          prop.text "Add advanced search"
          prop.className "mx-2 my-2"
          prop.onClick (fun _ -> onAddClick())
        ]
      ]
    ]

  [<ReactComponent>]
  let AdvancedSearchPanel() =
    let initialList = BackingModel.useSelector (fun bm -> bm.AdvancedSearches)
    let expandAdvancedSearch, setExpandAdvancedSearch = React.useState false

    Html.div [
      prop.className "mt-8 cursor-pointer"
      prop.children [
        Daisy.divider [
          prop.onClick (fun _ -> 
            setExpandAdvancedSearch (not expandAdvancedSearch)
          )
          if expandAdvancedSearch then
            prop.text "Collapse Advanced Search"
          else 
            prop.text "Expand Advanced Search"
        ]
        if expandAdvancedSearch then AdvancedSearchInputList(initialList)
      ]
    ]

  [<ReactComponent>]
  let SpellNameSearch() =
    let initialState = BackingModel.useSelector(fun m -> m.SpellName)
    let textInput, setTextInput = React.useState(initialState |> Option.defaultValue "")

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
          prop.value textInput
          prop.onInput(fun (e: Browser.Types.Event) ->
            let text = e.target?value
            setTextInput text
            let onSearchUpdate = 
              fun t -> SetSearchName t |> BackingModel.dispatch
            debouncer.Debounce onSearchUpdate text
          )
        ]
      ]
    ]

  [<ReactComponent>]
  let SearchRoot() =
    Html.div [
      prop.className "grid place-content-center"
      prop.children [
        SpellNameSearch()

        AdvancedSearchPanel()
      ]
    ]
