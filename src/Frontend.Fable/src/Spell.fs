namespace Frontend

open Frontend
open System
open Feliz
open Feliz.DaisyUI

module Spell =

  let private h2 (text : string) =
    Html.h2 [
      prop.className "text-xl"
      prop.text text
    ]

  let view (spell : Shared.Dtos.Spell) dispatch =
    Html.div [
      prop.children [
        Html.h1 [
          prop.className "text-2xl"
          prop.text spell.Name
        ]
        
        let school =
          match spell.Subschool, spell.Descriptors with
          | None, [] -> spell.School
          | Some ss, [] -> sprintf "%s [%s]" spell.School ss
          | Some ss, xs -> 
            let x = String.Join(", ", xs)
            sprintf "%s [%s] (%s)" spell.School ss x
          | None, xs -> 
            let x = String.Join(", ", xs)
            sprintf "%s (%s)" spell.School x

        h2 (sprintf "School: %s" school)

        let csl = spell.ClassSpellLevels |> List.map (fun csl -> csl.ToString()) |> String.join
        h2 csl

        Html.a [
          prop.className "underline cursor-pointer"
          prop.text "Back to list"
          prop.onClick (fun _ -> dispatch())
        ]
      ]
    ]