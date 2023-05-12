namespace Frontend

open Frontend
open System
open Fable.Core
open Fable.Core.JS
open Fable.Core.JsInterop
open Elmish
open Feliz
open Feliz.DaisyUI

module SpellTable =

  type Model = {
    SpellRows : Types.SpellRow list
  } with
    static member Init spellRows = {
      SpellRows = spellRows
    }

  let private spellRow (spell : Types.SpellRow) =
    let classSpellLevels =
      spell.ClassSpellLevels
      |> Seq.map (fun csl -> 
        sprintf "%s %i" (Formatting.fixSummonerUnchained csl.ClassName) csl.Level
      )
    let classSpellLevelsStr = String.Join(", ", classSpellLevels)

    let components = spell.Components |> Seq.map (fun c -> c.Abbr)
    let componentsStr = String.Join(", ", components)

    Html.tr [
      Html.td spell.Name
      Html.td spell.School
      Html.td [
        Html.p [
          prop.className "whitespace-normal"
          prop.text classSpellLevelsStr
        ]
      ]
      Html.td  [
        Html.p [
          prop.className "whitespace-normal"
          prop.text spell.CastingTime
        ]
      ]
      Html.td componentsStr
      Html.td spell.Range
      Html.td [
        Html.p [
          prop.text spell.Duration
          prop.className "whitespace-normal"
        ]
      ]
      Html.td [
        Html.p [
          prop.className "whitespace-normal"
          prop.text spell.ShortDescription
        ]
      ]
      Html.td spell.Source
    ]

  let view spellRows dispatch =
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
            Html.th "Description"
            Html.th "Source"
          ]
        ]
        Html.tbody [
          for spell in spellRows do yield spellRow spell
        ]
      ]
    ]