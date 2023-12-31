namespace Pfsdb

open System
open Pfsdb.Types
open Feliz
open Feliz.DaisyUI

module SearchDropdown =

  let private listItems advSearch dispatch (dropdownValues: (string * string) list)  =
    dropdownValues
    |> List.filter (fun ddv ->
      match advSearch.ValuesSearch with
      | None -> true
      | Some x -> 
        let s = snd ddv
        s.ToLowerInvariant().Contains(x.ToLowerInvariant())
    )
    |> List.map (fun (ddValue, ddText) ->
      Html.li [
        prop.children [
          Daisy.label [
            Daisy.checkbox [
              prop.isChecked (advSearch.SelectedValues |> List.contains ddValue)
              // we handle the change on the onClick below
              // `defaultChecked` has some buggy behavior regarding checked state,
              // whereas this is always accurate, and `isChecked` requires an `onChange` handler
              prop.onChange (fun (_ : bool) -> ())
            ]
            Daisy.labelText ddText
          ]
        ]
        prop.className "text-right"
        prop.onClick (fun ev ->
          ev.preventDefault()
          if List.contains ddValue advSearch.SelectedValues then
            let advSearch = { advSearch with SelectedValues = List.filter (fun v -> v <> ddValue) advSearch.SelectedValues }
            advSearch |> dispatch
          else
            let advSearch = { advSearch with SelectedValues = ddValue :: advSearch.SelectedValues |> List.sort }
            advSearch |> dispatch
        )
      ]
    )

  let debouncer = Debouncer("debouncer", 1000)

  let private dropdownContent advSearch onAdvSearchUpdate (elements: Fable.React.ReactElement list) =
    Daisy.dropdownContent [
      prop.className "p-2 shadow menu bg-base-100 rounded-box w-52"
      prop.tabIndex 0
      prop.children [
        Daisy.input [
          input.bordered
          prop.className "w-52"
          prop.placeholder ""
          match advSearch.ValuesSearch with
          | Some s -> prop.value s
          | None -> prop.value ""
          prop.onChange (fun (text : string) ->
            let newModel = 
              if String.IsNullOrWhiteSpace text then { advSearch with ValuesSearch = None }
              else { advSearch with ValuesSearch = Some text }
            
            onAdvSearchUpdate newModel
          )
        ]
        yield! elements
      ]
    ]

  let dropdown advSearch (emptyValuesText : string)  onAdvSearchUpdate dropdownElements =
    Daisy.dropdown [
      Daisy.button.button [
        button.primary
        match advSearch.SelectedValues with
        | [] -> prop.text emptyValuesText
        | _ -> prop.text (advSearch.SelectedValuesString())
        prop.className "mx-4 my-2"
      ]
      dropdownContent  advSearch onAdvSearchUpdate dropdownElements
    ]

  let schoolSearch schools advSearch dispatch =
    let dropdownElements =
      schools
      |> List.sort
      |> List.map (fun school ->
        school, school
      )
      |> listItems advSearch dispatch

    dropdown advSearch "Select School(s)"  dispatch dropdownElements

  let casterClassSearch casterClasses advSearch dispatch  =
    let dropdownElements =
      casterClasses
      |> List.sort
      |> List.map (fun cc -> cc, Formatting.fixSummonerUnchained cc)
      |> listItems advSearch dispatch

    dropdown advSearch "Select Class(es)" dispatch dropdownElements

  let spellLevelSearch advSearch dispatch =
    let dropdownElements =
      [0..9]
      |> List.map (fun i -> string i, string i)
      |> listItems advSearch dispatch
    dropdown advSearch "Select Spell Level(s)" dispatch dropdownElements

  let castingTimeSearch castingTimes advSearch dispatch =
    let dropdownElements =
      castingTimes
      |> List.map (fun castingTime -> castingTime, castingTime)
      |> listItems advSearch dispatch
    dropdown advSearch "Select casting time(s)" dispatch dropdownElements

  let componentSearch components advSearch dispatch =
    components
    |> List.sort
    |> List.map (fun c -> c, c)
    |> listItems advSearch dispatch
    |> dropdown advSearch  "Select Component(s)" dispatch

  let rangeSearch ranges advSearch dispatch =
    ranges
    |> List.map (fun r -> r, r)
    |> listItems advSearch dispatch
    |> dropdown advSearch "Select Range(s)" dispatch

  let areaSearch areas advSearch dispatch =
    areas
    |> List.map (fun a -> a, a)
    |> listItems advSearch dispatch
    |> dropdown advSearch "Select Area(s)" dispatch

  let durationSearch durations advSearch dispatch =
    durations
    |> List.map (fun r -> r, r)
    |> listItems advSearch dispatch
    |> dropdown advSearch "Select Duration(s)" dispatch

  let savingThrowsSearch savingThrows advSearch dispatch =
    savingThrows
    |> List.map (fun r -> r, r)
    |> listItems advSearch dispatch
    |> dropdown advSearch "Select Saving Throw(s)" dispatch

  let spellResistanceSearch spellRes advSearch dispatch =
    spellRes
    |> List.map (fun r -> r, r)
    |> listItems advSearch dispatch
    |> dropdown advSearch "Select Spell Resistance(s)" dispatch

  let sourcesSearch sources advSearch dispatch =
    sources
    |> List.map (fun r -> r, r)
    |> listItems advSearch dispatch
    |> dropdown advSearch "Select Source(s)" dispatch

