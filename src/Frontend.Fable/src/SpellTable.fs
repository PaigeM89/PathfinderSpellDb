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
    Html.tr [
      Html.td spell.Name
      Html.td spell.School
      Html.td classSpellLevelsStr
      Html.td spell.CastingTime
    ]

  let view spellRows dispatch =
    Daisy.table [
      prop.className "w-full"
      prop.children [
        Html.thead [
          Html.tr [
            Html.th "Name"
            Html.th "School"
            Html.th "Spell Level"
            Html.th "Casting time"
          ]
        ]
        Html.tbody [
          for spell in spellRows do yield spellRow spell
        ]
      ]
    ]