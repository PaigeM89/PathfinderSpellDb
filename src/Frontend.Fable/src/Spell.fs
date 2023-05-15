namespace Frontend

open Frontend
open System
open Feliz
open Fable.Core
open Fable.React
open Fable.React.ReactBindings
open Feliz.DaisyUI
open Fable.React

module Spell =

  let private h2 (text : string) =
    Html.h2 [
      prop.className "text-xl py-1"
      prop.text text
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
        h2 (sprintf "School: %s" school)

        let csl = spell.ClassSpellLevels |> Seq.toList |> List.map (fun csl -> sprintf "%s %i" csl.ClassName csl.Level) |> String.join
        h2 csl
      ]
    ]

  let private description (spell : Shared.Dtos.Spell) =
    Html.div [
      prop.dangerouslySetInnerHTML spell.Description
    ]

  let private footer dispatch =
    Html.a [
      prop.className "underline cursor-pointer pt-4"
      prop.text "Back to list"
      prop.onClick (fun _ -> dispatch())
    ]

  let view (spell : Shared.Dtos.Spell) dispatch =
    Html.div [
      prop.className "p-4"
      prop.children [
        
        Html.h1 [
          prop.className "text-3xl text-center"
          prop.text spell.Name
        ]
        
        details spell

        description spell

        footer dispatch
      ]
    ]