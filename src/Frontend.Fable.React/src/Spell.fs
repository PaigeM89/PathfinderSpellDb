namespace Pfsdb

open System
open Feliz
open Feliz.DaisyUI

module Spell =
  let private detail label (value: string) =
    Html.div [
      prop.className "flex"
      prop.children [
        Html.h2 [
          prop.className "text-xl font-extrabold"
          prop.text (label + ":")
        ]
        Html.h2 [
          prop.className "text-xl pl-1"
          prop.text value
        ]
      ]
    ]

  let private details (spell : Shared.Dtos.Spell) =

    let school =
      match spell.Subschool, Seq.toList spell.Descriptors with
      | Some ss, [] -> sprintf "%s [%s]" spell.School ss
      | Some ss, xs -> 
        let x = String.Join(", ", xs)
        if x = "" then sprintf "%s [%s]" spell.School ss
        else sprintf "%s [%s] (%s)" spell.School ss x
      | None, xs ->
        // for some reason, it won't match on []
        let x = String.Join(", ", xs)
        if x = "" then spell.School
        else sprintf "%s (%s)" spell.School x
  
    Html.div [
      prop.className "py-2"
      prop.children [
        detail "School" school

        let csl =
          spell.ClassSpellLevels
          |> Seq.toList
          |> List.map (fun csl -> 
              sprintf "%s %i" (Formatting.fixSummonerUnchained csl.ClassName) csl.Level
          )
          |> String.join
        detail "Level" csl
        if spell.Domains <> "" then detail "Domains" spell.Domains else Html.none
        detail "Source" spell.Source
        
        Daisy.divider "Casting"
        detail "Casting Time" spell.CastingTime
        detail "Components" (Formatting.componentsStr spell.Components)
        
        Daisy.divider "Effect"
        detail "Range" spell.Range
        if spell.Area <> "" then detail "Area" spell.Area
        detail "Duration" spell.Duration
        detail "Saving throw" spell.SavingThrows
        detail "Spell Resistance" (if spell.SpellResistance then "Yes" else "No")
      ]
    ]

  let private description (spell : Shared.Dtos.Spell) =
    Html.div [
      prop.className "text-justify"
      prop.dangerouslySetInnerHTML spell.Description
    ]

  let private footer =
    Html.div [
      prop.className "pt-4"
      prop.children [
        Html.a [
          prop.className "underline cursor-pointer"
          prop.text "Back to list"
          prop.href ""
        ]
      ]
    ]

  [<ReactComponent>]
  let Spell(spell : Shared.Dtos.Spell) =
    Html.div [
      theme.dark
      prop.className "p-4"
      prop.children [
        
        Html.h1 [
          prop.className "text-3xl text-center"
          prop.text spell.Name
        ]
        
        details spell
        
        Daisy.divider "Description"

        description spell

        footer
      ]
    ]