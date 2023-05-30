namespace Frontend

open Frontend
open System
open Feliz
open Feliz.DaisyUI

module SpellTable =

  type Model = {
    SpellRows : Shared.Dtos.SpellRow list
  } with
    static member Init spellRows = {
      SpellRows = spellRows
    }

  let private spellRow loadingSpellId (spell : Shared.Dtos.SpellRow) dispatch =
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
            if spell.Id = loadingSpellId then 
              prop.text (spell.Name + " (Loading...)")
            else
              prop.text spell.Name
            prop.onClick (fun _ -> dispatch spell.Id)
            prop.className "cursor-pointer"
          ]
        ]
      ]
      Html.td spell.School
      Html.td (p 0 spell.ClassSpellLevelsString)
      Html.td (p 1 spell.CastingTime)
      Html.td componentsStr
      Html.td (p 2 spell.Range)
      Html.td (p 3 spell.Duration)
      Html.td (p 4 spell.SavingThrowStr)
      Html.td (p 5 spellRes)
      Html.td (p 6 spell.ShortDescription)
      Html.td spell.Source
    ]

  let view limit loadingSpell (spellRows : Shared.Dtos.SpellRow []) loadSpellDispatch =
    let loadingSpellId = Option.defaultValue -1 loadingSpell
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
            Html.th "Duration"
            Html.th "Saving Throw"
            Html.th "Spell Resistance"
            Html.th "Description"
            Html.th "Source"
          ]
        ]
        match limit with
        | Some x ->
          if (Array.length spellRows > x) then
            Html.tbody [
              for spell in (Array.take x spellRows) do yield spellRow loadingSpellId spell loadSpellDispatch
            ]
          else
            Html.tbody [
              for spell in spellRows do yield spellRow loadingSpellId spell loadSpellDispatch
            ]
        | None ->
          Html.tbody [
            for spell in spellRows do yield spellRow loadingSpellId spell loadSpellDispatch
          ]
      ]
    ]