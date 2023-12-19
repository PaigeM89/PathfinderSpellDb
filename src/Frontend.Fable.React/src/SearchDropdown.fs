namespace Pfsdb

open System
open System.Collections.Generic
open Pfsdb.Types
open Feliz
open Feliz.DaisyUI

module SearchDropdown =

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