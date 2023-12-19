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

  [<ReactComponent>]
  let AdvancedSearchInput() =
    Html.div [
      prop.children [
        Daisy.button.button [
          prop.text "Add advanced search"
          prop.className "mx-2 my-2"
        ]
      ]
    ]

  [<ReactComponent>]
  let AdvancedSearchPanel() =
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
        if expandAdvancedSearch then AdvancedSearchInput()
      ]
    ]

  [<ReactComponent>]
  let SearchRoot(onSearchUpdate) =
      let searchModel, setSearchModel = React.useState(Search.Empty())
      let expandAdvancedSearch, setExpandAdvancedSearch = React.useState false

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

          AdvancedSearchPanel()

        ]
      ]