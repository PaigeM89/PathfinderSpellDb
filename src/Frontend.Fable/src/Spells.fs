namespace Frontend

open System
open Fable.Core
open Fable.Core.JS
open Fable.Core.JsInterop
open Elmish
open Feliz
open Feliz.DaisyUI

module Spells =

  type Model = {
    RootServerUrl : string
    SpellRows : Types.SpellRow seq
    Search : Types.Search
    SearchRootModel : SearchRoot.Model
  } with
    static member Init serverUrl = {
      RootServerUrl = serverUrl
      SpellRows = []
      Search = Types.Search.Empty()
      SearchRootModel = SearchRoot.Model.Init()
    }

  type Msg =
  | LoadAllSpells
  | AllSpellsLoaded of Types.SpellRow seq
  | SpellLoadingExn of exn
  | SearchMsg of SearchRoot.Msg

  let init (serverUrl : string) = Model.Init serverUrl, Cmd.ofMsg LoadAllSpells

  let filterSpells (model : Model) =
    let filteredByName =
      match model.Search.Name with
      | None -> model.SpellRows
      | Some n ->
        let l = Seq.length model.SpellRows
        console.log("Searching %i spells for %s", l, n)

        model.SpellRows |> Seq.filter (fun spell -> spell.Name.ToLowerInvariant().Contains n)
    filteredByName

  module ApiCalls =
    let loadAllSpells model =
      async {
        let url = model.RootServerUrl + "/spells"
        let! response = Fetch.fetch url [] |> Async.AwaitPromise
        let! spells = response.json<Types.SpellRow seq>() |> Async.AwaitPromise
        return spells
      }

  let update msg (model: Model) =
    match msg with
    | LoadAllSpells ->
      model, Cmd.OfAsync.perform ApiCalls.loadAllSpells model AllSpellsLoaded
    | AllSpellsLoaded spells ->
      console.log("Loaded %i spells", (Seq.length spells))
      console.log("spells", spells)
      { model with SpellRows = spells }, Cmd.none
    | SpellLoadingExn e ->
      console.error e
      model, Cmd.none
    | SearchMsg (SearchRoot.Msg.SearchUpdated search) ->
      let model = { model with Search = search }
      model, Cmd.none
    | SearchMsg msg ->
      let searchModel, cmd = SearchRoot.update msg model.SearchRootModel
      { model with SearchRootModel = searchModel }, Cmd.map SearchMsg cmd


  let ListView model dispatch = 
    console.log("in view, model has %i spells", (Seq.length model.SpellRows))
    Html.div [
      theme.dark
      prop.children [
        SearchRoot.view model.SearchRootModel (SearchMsg >> dispatch)

        Daisy.divider "Spells"

        SpellTable.view (filterSpells model) (fun _ -> ())
      ]
    ]

