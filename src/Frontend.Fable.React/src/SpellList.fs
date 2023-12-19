namespace Pfsdb

open System
open Shared.Dtos
open Fable.Core
open Fable.Core.JsInterop
open Feliz
open Feliz.DaisyUI

module SpellList =

    let private spellRow (spell : Shared.Dtos.SpellRow) =
        let components = spell.Components |> Seq.map (fun c -> c.Abbr)
        let componentsStr = String.Join(", ", components)

        // ints are faster for large amounts of spell rows being rendered
        let key (i : int) = spell.Id + i

        let p (k : int) (text : string) =
            Html.p [
                // this key is needed to suppress a warning but doesn't have a lot of value
                prop.key (key k)
                prop.className "whitespace-normal"
                prop.text text
            ]

        let spellRes = if spell.SpellResistance then "Yes" else "No"

        Html.tr [
            Html.td [
                prop.id (string spell.Id)
                prop.className "underline"
                prop.children [
                    Html.a [
                        prop.text spell.Name
                        prop.href (sprintf "#spells/%i" spell.Id)
                        prop.className "cursor-pointer"
                    ]
                ]
            ]
            Html.td spell.School
            Html.td (p 0 spell.ClassSpellLevelsString)
            Html.td (p 1 spell.CastingTime)
            Html.td componentsStr
            Html.td (p 2 spell.Range)
            Html.td (p 2 spell.Area)
            Html.td (p 3 spell.Duration)
            Html.td (p 4 spell.SavingThrowStr)
            Html.td (p 5 spellRes)
            Html.td (p 6 spell.ShortDescription)
            Html.td spell.Source
        ]

    [<ReactComponent>]
    let SpellTable(spells : SpellRow seq, limit: int option) =
        Daisy.table [
            table.compact
            prop.className "w-full table-zebra"
            prop.children [
                Html.thead [
                    Html.tr [
                        Html.th "Name"
                        Html.th "School"
                        Html.th "Spell Level"
                        Html.th "Casting time"
                        Html.th "Components"
                        Html.th "Range"
                        Html.th "Area"
                        Html.th "Duration"
                        Html.th "Saving Throw"
                        Html.th "Spell Resistance"
                        Html.th "Description"
                        Html.th "Source"
                    ]
                ]
                match limit with
                | Some x ->
                    if (Seq.length spells > x) then
                        Html.tbody [
                            for spell in (Seq.take x spells) do yield spellRow spell
                        ]
                    else
                        Html.tbody [
                            for spell in spells do yield spellRow spell
                        ]
                | None ->
                    Html.tbody [
                        for spell in spells do yield spellRow spell
                    ]
            ]
        ]

    type Search = {
        SpellName : string option
    } with
        static member Empty() = {
            SpellName = None
        }

    module Search =
        let setSpellName name search = 
            if String.IsNullOrWhiteSpace name then { search with SpellName = None }
            else { search with SpellName = Some name }

        let doSearch (spells : SpellRow seq) search =
            match search.SpellName with
            | Some spellName ->
                spells
                |> Seq.filter (fun spell ->
                    spell.Name.ToLowerInvariant().Contains (spellName.Trim().ToLowerInvariant())
                )
            | None -> spells

    let debouncer = Debouncer("spellNameSearch", 500)

    [<ReactComponent>]
    let SearchRoot(onSearchUpdate) =
        let searchModel, setSearchModel = React.useState(Search.Empty())
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
                let searchModel = Search.setSpellName text searchModel
                setSearchModel searchModel
                debouncer.Debounce onSearchUpdate searchModel
              )
            ]
          ]
        ]

    [<ReactComponent>]
    let SpellList(spells : SpellRow seq) =
        let filteredSpells, setFilteredSpells = React.useState(spells)
        let spellCount = filteredSpells |> Seq.length
        
        let onSearchUpdate (searchModel : Search) =
            Search.doSearch spells searchModel
            |> setFilteredSpells

        Html.div [
            theme.dark
            prop.children [
                Html.h1 [
                    prop.className "text-2xl text-center"
                    prop.text "Pathfinder Spell DB"
                ]
                Html.h2 [
                    prop.className "text-xl text-center"
                    prop.text "A searchable database of all the spells in Pathfinder 1E"
                ]

                SearchRoot(onSearchUpdate)

                Daisy.divider (sprintf "Spells (%i)" spellCount)

                SpellTable(filteredSpells, Some 100)
            ]
        ]
