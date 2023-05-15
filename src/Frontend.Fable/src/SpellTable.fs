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

  let private spellRow (spell : Shared.Dtos.SpellRow) dispatch =
    let classSpellLevels =
      spell.ClassSpellLevels
      |> Seq.map (fun csl -> 
        sprintf "%s %i" (Formatting.fixSummonerUnchained csl.ClassName) csl.Level
      )
    let classSpellLevelsStr = String.Join(", ", classSpellLevels)

    let components = spell.Components |> Seq.map (fun c -> c.Abbr)
    let componentsStr = String.Join(", ", components)

    let p (text : string) =
      Html.p [
        // this key is needed to suppress a warning but doesn't have a lot of value
        prop.key (sprintf "%i-%s" spell.Id text)
        prop.className "whitespace-normal"
        prop.text text
      ]

    Html.tr [
      Html.td [
        prop.id (string spell.Id)
        prop.className "underline"
        prop.children [
          Html.a [
            prop.text spell.Name
            prop.onClick (fun _ -> dispatch spell.Id)
            prop.className "cursor-pointer"
          ]
        ]
      ]
      Html.td spell.School
      Html.td (p classSpellLevelsStr)
      Html.td (p spell.CastingTime)
      Html.td componentsStr
      Html.td (p spell.Range)
      Html.td (p spell.Duration)
      Html.td (p spell.ShortDescription)
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
          for spell in spellRows do yield spellRow spell dispatch
        ]
      ]
    ]