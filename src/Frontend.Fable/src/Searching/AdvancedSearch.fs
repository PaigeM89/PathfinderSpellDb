namespace Frontend.Searching

open System
open Fable.Core
open Frontend.Types
open Elmish
open Feliz
open Feliz.DaisyUI

module AdvancedSearch =

  type Model = {
    Id : Guid
    SearchType : SearchType option
    Options : string list
    FilteredOptions : string list option
    SelectedValues : string list
    UserFilterInput : string option
  } with
    static member Init() = {
      Id = Guid.NewGuid()
      SearchType = None
      Options = []
      FilteredOptions = None
      SelectedValues = []
      UserFilterInput = None
    }

    member this.SelectedValuesString() = String.Join(", ", this.SelectedValues)

    member this.ToAdvancedSearch() = {
      Id = this.Id
      SearchType = this.SearchType
      Values = this.SelectedValues
    }

  type Msg =
  | ModelUpdate of model : Model
  | UserInputUpdated of modelId : Guid * input : string option
  | UserInputCompleted of modelId : Guid

  // Handled at higher component
  | DeleteAdvancedSearch of id : Guid
  // Handled at higher component
  | AdvancedSearchUpdate of Model
  | AdvancedSearchTypeUpdate of Model

  let update msg model =
    match msg with
    | ModelUpdate model ->
      model, Cmd.ofMsg (AdvancedSearchUpdate model)
    | UserInputUpdated (modelId, input) ->
      if modelId = model.Id then
        { model with UserFilterInput = input }, Cmd.none
      else model, Cmd.none
    | UserInputCompleted modelId ->
      if modelId = model.Id then
        let model = 
          match model.UserFilterInput with
          | Some str when str.Length > 0 ->
            let filteredOptions = model.Options |> List.filter (fun o -> o.ToLowerInvariant().Contains(str.ToLowerInvariant()))
            { model with FilteredOptions = Some filteredOptions}
          | _ ->
            { model with FilteredOptions = None }
        model, Cmd.none
      else model, Cmd.none
    // these are handled at the higher component
    | DeleteAdvancedSearch _
    | AdvancedSearchTypeUpdate _
    | AdvancedSearchUpdate _ -> model, Cmd.none


  let private listItems (model : Model) dispatch (dropdownValues: (string * string) list)  =
    dropdownValues
    |> List.map (fun (ddValue, ddText) ->
      let isSelected = model.SelectedValues |> List.contains ddValue
      Html.li [
        prop.children [
          Daisy.label [
            Daisy.checkbox [ 
              prop.isChecked isSelected
              // we handle the change on the onClick below
              // `defaultChecked` has some buggy behavior regarding checked state,
              // whereas this is always accurate, and `isChecked` requires an `onChange` handler
              prop.onChange (fun (_ : bool) -> ())
            ]
            Daisy.labelText ddText
          ]
        ]
        prop.className "text-right w-52"
        prop.onClick (fun e ->
          e.preventDefault()
          if isSelected then
            let model = { model with SelectedValues = List.filter (fun v -> v <> ddValue) model.SelectedValues }
            model |> ModelUpdate |>  dispatch
          else
            let model = { model with SelectedValues = ddValue :: model.SelectedValues |> List.sort }
            model |> ModelUpdate  |> dispatch
        )
      ]
    )

  let debouncer = Frontend.Debouncer("advSearchDebouncer", 500)

  let private dropdownContent dispatch model (elements: Fable.React.ReactElement list) =
    Daisy.dropdownContent [
      prop.className "p-2 shadow menu bg-base-100 rounded-box w-auto overflow-x-clip overflow-y-auto h-96 flex-row"
      prop.tabIndex 0
      prop.children [
        Daisy.input [
          input.bordered
          prop.className "w-56"
          prop.placeholder ""
          match model.UserFilterInput with
          | None -> prop.value ""
          | Some x -> prop.value x
          prop.onChange (fun (text : string) ->
            UserInputUpdated(model.Id, Some text) |> dispatch
            debouncer.Debounce dispatch (UserInputCompleted (model.Id))
          )
        ]
        yield! elements
      ]
    ]

  let dropdown dispatch (model: Model) (emptyValuesText : string) dropdownElements =
    Daisy.dropdown [
      Daisy.button.button [
        button.primary
        match model.SelectedValues with
        | [] -> prop.text emptyValuesText
        | _ -> prop.text (model.SelectedValuesString())
        prop.className "mx-4 my-2"
      ]
      dropdownContent dispatch model dropdownElements
    ]

  let schoolSearch schools model dispatch =
    let dropdownElements =
      schools
      |> List.sort
      |> List.map (fun school ->
        school, school
      )
      |> listItems model dispatch

    dropdown dispatch model "Select School(s)" dropdownElements

  let casterClassSearch casterClasses model dispatch  =
    let dropdownElements =
      casterClasses
      |> List.sort
      |> List.map (fun cc -> cc, Frontend.Formatting.fixSummonerUnchained cc)
      |> listItems model dispatch

    dropdown dispatch model "Select Class(es)" dropdownElements

  let spellLevelSearch model dispatch =
    let dropdownElements =
      [0..9]
      |> List.map (fun i -> string i, string i)
      |> listItems model dispatch
    dropdown dispatch model "Select Spell Level(s)" dropdownElements

  let castingTimeSearch castingTimes model dispatch =
    let dropdownElements =
      castingTimes
      |> List.map (fun castingTime -> castingTime, castingTime)
      |> listItems model dispatch
    dropdown dispatch model "Select casting time(s)" dropdownElements

  let componentSearch components model dispatch =
    components
    |> List.sort
    |> List.map (fun c -> c, c)
    |> listItems model dispatch
    |> dropdown dispatch model "Select Component(s)"

  let rangeSearch ranges model dispatch =
    ranges
    |> List.map (fun r -> r, r)
    |> listItems model dispatch
    |> dropdown dispatch model "Select Range(s)"

  let durationSearch durations model dispatch =
    durations
    |> List.map (fun r -> r, r)
    |> listItems model dispatch
    |> dropdown dispatch model "Select Duration(s)"

  let savingThrowsSearch savingThrows advSearch dispatch =
    savingThrows
    |> List.map (fun r -> r, r)
    |> listItems advSearch dispatch
    |> dropdown dispatch advSearch "Select Saving Throw(s)"

  let spellResistanceSearch spellRes advSearch dispatch =
    spellRes
    |> List.map (fun r -> r, r)
    |> listItems advSearch dispatch
    |> dropdown dispatch advSearch "Select Spell Resistance(s)"

  let sourcesSearch sources model dispatch =
    sources
    |> List.map (fun r -> r, r)
    |> listItems model dispatch
    |> dropdown dispatch model "Select Source(s)"

  let view (model: Model) dispatch =
    let dropdownElements = 
      searchTypeName
      |> Map.toList
      |> List.map (fun (st, text) ->
        Html.li [ 
          prop.children [
            Html.a [
              prop.text text
            ]
          ]
          prop.onClick (fun _ ->
              { model with SearchType = Some st; SelectedValues = [] } |> AdvancedSearchTypeUpdate |> dispatch)
        ]
      )
    
    Html.div [
      prop.id (string model.Id)
      prop.children [
        Daisy.dropdown [
          Daisy.button.button [
            button.primary
            match model.SearchType with
            | None -> prop.text "Select a field"
            | Some searchType ->
              prop.text (Map.tryFind searchType searchTypeName |> Option.defaultValue "Error")
            prop.className "mx-2 my-2"
          ]
          Daisy.dropdownContent [
            prop.className "p-2 shadow menu bg-base-100 rounded-box w-52"
            prop.tabIndex 0
            prop.children dropdownElements
          ]
        ]

        let values = 
          match model.FilteredOptions with
          | None -> model.Options
          | Some x -> x

        match model.SearchType with
        | Some School ->
          schoolSearch values model dispatch
        | Some CasterClass ->
          casterClassSearch values model dispatch
        | Some Level ->
          spellLevelSearch model dispatch
        | Some CastingTime ->
          castingTimeSearch values model dispatch
        | Some Components ->
          componentSearch values model dispatch
        | Some Range ->
          rangeSearch values model dispatch
        | Some Duration ->
          durationSearch values model dispatch
        | Some SavingThrow -> savingThrowsSearch values model dispatch
        | Some SpellResistance -> spellResistanceSearch values model dispatch
        | Some Source ->
          sourcesSearch values model dispatch
        | _ -> Html.none
        Daisy.button.button [
          prop.text "Delete"
          prop.onClick (fun _ -> DeleteAdvancedSearch model.Id |> dispatch)
          prop.className "mx-2 my-2"
        ]
      ]
    ]