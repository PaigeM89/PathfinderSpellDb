namespace Frontend.Searching

open System
open System.Collections.Generic
open Frontend
open Frontend.Types
open Elmish
open Feliz
open Feliz.DaisyUI

module SearchDropdowns =

  let private listItems advSearch dispatch (dropdownValues: (string * string) list)  =
    dropdownValues
    |> List.map (fun (ddValue, ddText) ->
      Html.li [
        prop.children [
          Daisy.label [
            Daisy.checkbox [ 
              prop.isChecked (advSearch.Values |> List.contains ddValue)
              // we handle the change on the onClick below
              // `defaultChecked` has some buggy behavior regarding checked state,
              // whereas this is always accurate, and `isChecked` requires an `onChange` handler
              prop.onChange (fun (_ : bool) -> ())
            ]
            Daisy.labelText ddText
          ]
        ]
        prop.className "text-right"
        prop.onClick (fun _ ->
          if List.contains ddValue advSearch.Values then
            let advSearch = { advSearch with Values = List.filter (fun v -> v <> ddValue) advSearch.Values }
            advSearch |>  dispatch
          else
            let advSearch = { advSearch with Values = ddValue :: advSearch.Values |> List.sort }
            advSearch |> dispatch
        )
      ]
    )

  let debouncer = Debouncer("debouncer", 1000)

  let private dropdownContent (elements: Fable.React.ReactElement list) =
    Daisy.dropdownContent [
      prop.className "p-2 shadow menu bg-base-100 rounded-box w-52"
      prop.tabIndex 0
      prop.children [
        Daisy.input [
          input.bordered
          prop.className "w-52"
          prop.placeholder ""
        ]
        yield! elements
      ]
    ]

  let dropdown advSearch (emptyValuesText : string) dropdownElements =
    Daisy.dropdown [
      Daisy.button.button [
        button.primary
        match advSearch.Values with
        | [] -> prop.text emptyValuesText
        | _ -> prop.text (advSearch.ValuesString())
        prop.className "mx-4 my-2"
      ]
      dropdownContent dropdownElements
    ]

  let schoolSearch schools advSearch dispatch =
    let dropdownElements =
      schools
      |> List.sort
      |> List.map (fun school ->
        school, school
      )
      |> listItems advSearch dispatch

    dropdown advSearch "Select School(s)" dropdownElements

  let casterClassSearch casterClasses advSearch modelUpdateDispatch filterDispatch  =
    let dropdownElements =
      casterClasses
      |> List.sort
      |> List.map (fun cc -> cc, Formatting.fixSummonerUnchained cc)
      |> listItems advSearch modelUpdateDispatch

    dropdown advSearch "Select Class(es)" dropdownElements

  let spellLevelSearch advSearch dispatch =
    let dropdownElements =
      [0..9]
      |> List.map (fun i -> string i, string i)
      |> listItems advSearch dispatch
    dropdown advSearch "Select Spell Level(s)" dropdownElements

  let castingTimeSearch castingTimes advSearch dispatch =
    let dropdownElements =
      castingTimes
      |> List.map (fun castingTime -> castingTime, castingTime)
      |> listItems advSearch dispatch
    dropdown advSearch "Select casting time(s)" dropdownElements

  let componentSearch components advSearch dispatch =
    components
    |> List.sort
    |> List.map (fun c -> c, c)
    |> listItems advSearch dispatch
    |> dropdown advSearch "Select Component(s)"

  let rangeSearch ranges advSearch dispatch =
    ranges
    |> List.map (fun r -> r, r)
    |> listItems advSearch dispatch
    |> dropdown advSearch "Select Range(s)"

  let durationSearch durations advSearch dispatch =
    durations
    |> List.map (fun r -> r, r)
    |> listItems advSearch dispatch
    |> dropdown advSearch "Select Duration(s)"

  let savingThrowsSearch savingThrows advSearch dispatch =
    savingThrows
    |> List.map (fun r -> r, r)
    |> listItems advSearch dispatch
    |> dropdown advSearch "Select Saving Throw(s)"

  let spellResistanceSearch spellRes advSearch dispatch =
    spellRes
    |> List.map (fun r -> r, r)
    |> listItems advSearch dispatch
    |> dropdown advSearch "Select Spell Resistance(s)"

  let sourcesSearch sources advSearch dispatch =
    sources
    |> List.map (fun r -> r, r)
    |> listItems advSearch dispatch
    |> dropdown advSearch "Select Source(s)"

