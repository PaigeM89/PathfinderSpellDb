namespace Pfsdb

open System
open Pfsdb.Types
open Shared.Dtos
open Fable.Core
open Fable.Core.JsInterop
open Feliz
open Feliz.DaisyUI

module Searching =

  let doSearch (spells : SpellRow seq) (search : Search) =
      match search.SpellName with
      | Some spellName ->
          spells
          |> Seq.filter (fun spell ->
              spell.Name.ToLowerInvariant().Contains (spellName.Trim().ToLowerInvariant())
          )
      | None -> spells

  let debouncer = Debouncer("spellNameSearch", 500)
  let advSearchDebouncer = Debouncer("advancedSearchDebouncer", 500)

  [<ReactComponent>]
  let AdvancedSearchInput(advancedSearch : AdvancedSearch, filterTargets: FilterTargets, onUpdate, onDeleteClick) =
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
            { advancedSearch with SearchType = Some st } |> onUpdate
          )
        ]
      )
    
    let dispatch x = onUpdate x

    Html.div [
      prop.id (string advancedSearch.Id)
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
          SearchDropdown.schoolSearch filterTargets.Schools advancedSearch dispatch
        | Some CasterClass ->
          SearchDropdown.casterClassSearch filterTargets.CasterClasses advancedSearch dispatch
        | Some Level ->
          SearchDropdown.spellLevelSearch advancedSearch dispatch
        | Some CastingTime ->
          SearchDropdown.castingTimeSearch filterTargets.CastingTimes advancedSearch dispatch
        | Some Components ->
          SearchDropdown.componentSearch filterTargets.Components advancedSearch dispatch
        | Some Range ->
          SearchDropdown.rangeSearch filterTargets.Ranges advancedSearch dispatch
        | Some Area ->
          SearchDropdown.areaSearch filterTargets.Areas advancedSearch dispatch
        | Some Duration ->
          SearchDropdown.durationSearch filterTargets.Durations advancedSearch dispatch
        | Some SavingThrow -> 
          SearchDropdown.savingThrowsSearch filterTargets.SavingThrows advancedSearch dispatch
        | Some SpellResistance ->
          SearchDropdown.spellResistanceSearch filterTargets.SpellResistance advancedSearch dispatch
        | Some Source ->
          SearchDropdown.sourcesSearch filterTargets.Sources advancedSearch dispatch
        | _ -> Html.none

        Daisy.button.button [
          prop.text "Delete"
          prop.onClick (fun _ -> onDeleteClick advancedSearch.Id)
          prop.className "mx-2 my-2"
        ]
      ]
    ]

  [<ReactComponent>]
  let AdvancedSearchInputList(advancedSearches : AdvancedSearch list, filterTargets, onAddClick, onUpdate, onDeleteClick) =
    Html.div [
      prop.children [
        for advancedSearchInput in advancedSearches do
          AdvancedSearchInput(advancedSearchInput, filterTargets, onUpdate, onDeleteClick)

        Daisy.button.button [
          prop.text "Add advanced search"
          prop.className "mx-2 my-2"
          prop.onClick (fun _ -> onAddClick())
        ]
      ]
    ]

  [<ReactComponent>]
  let AdvancedSearchPanel(initialList : AdvancedSearch list, filterTargets, onAdvancedSearchUpdate) =
    let expandAdvancedSearch, setExpandAdvancedSearch = React.useState false
    let (advancedSearchInputs : AdvancedSearch list, setAdvancedSearchInputs) = React.useState initialList

    let doUpdate (advancedSearches : AdvancedSearch list) =
      setAdvancedSearchInputs advancedSearches
      onAdvancedSearchUpdate advancedSearches

    let onAddClick() =
      advancedSearchInputs @ [ AdvancedSearch.Empty() ]
      |> doUpdate

    let onDeleteClick id =
      advancedSearchInputs
      |> List.filter (fun a -> a.Id <> id)
      |> doUpdate

    let onUpdate (newModel : AdvancedSearch) =
      advancedSearchInputs
      |> List.map (fun a ->
        if a.Id = newModel.Id then 
          newModel 
        else a
      )
      |> doUpdate

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
        if expandAdvancedSearch then AdvancedSearchInputList(advancedSearchInputs, filterTargets, onAddClick, onUpdate, onDeleteClick)
      ]
    ]

  [<ReactComponent>]
  let SearchRoot(filterTargets, onSearchUpdate) =
      let searchModel, setSearchModel = React.useState(Search.Empty())

      let onAdvancedSearchUpdate advSearches = 
        let updatedModel = 
          { searchModel with AdvancedSearches = advSearches }
        setSearchModel updatedModel
        onSearchUpdate updatedModel
        //advSearchDebouncer.Debounce onSearchUpdate searchModel


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
            match searchModel.SpellName with
            | Some s -> prop.value s
            | None -> prop.value ""
            prop.onInput(fun (e: Browser.Types.Event) ->
              let text = e.target?value
              let searchModel = Search.maybeSetSpellName text searchModel
              setSearchModel searchModel
              debouncer.Debounce onSearchUpdate searchModel
            )
          ]

          AdvancedSearchPanel (searchModel.AdvancedSearches, filterTargets, onAdvancedSearchUpdate)

        ]
      ]