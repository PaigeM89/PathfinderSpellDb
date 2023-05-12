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

    /// If the user views a specific spell, this will get populated with that spell's details
    Spell : Shared.Dtos.Spell option

    Search : Types.Search
    SearchRootModel : SearchRoot.Model
  } with
    static member Init serverUrl = {
      RootServerUrl = serverUrl
      SpellRows = []
      Spell = None
      Search = Types.Search.Empty()
      SearchRootModel = SearchRoot.Model.Init()
    }

  type Msg =
  | LoadAllSpells
  | AllSpellsLoaded of Types.SpellRow seq
  | SpellLoadingExn of exn
  | LoadSpell of spellId : int
  | SpellLoaded of Shared.Dtos.Spell
  | SpellLoadExn of exn
  | ReturnToList
  | SearchMsg of SearchRoot.Msg

  let init (serverUrl : string) = Model.Init serverUrl, Cmd.ofMsg LoadAllSpells

  module ApiCalls =
    let loadAllSpells model =
      async {
        let url = model.RootServerUrl + "/spells"
        let! response = Fetch.fetch url [] |> Async.AwaitPromise
        let! spells = response.json<Types.SpellRow seq>() |> Async.AwaitPromise
        return spells
      }

    let loadSpell model id =
      async {
        let url = model.RootServerUrl + "/spells/" + (string id)
        let! response = Fetch.fetch url [] |> Async.AwaitPromise
        let! spell = response.json<Shared.Dtos.Spell>() |> Async.AwaitPromise
        return spell
      }

  let update msg (model: Model) =
    match msg with
    | LoadAllSpells ->
      model, Cmd.OfAsync.perform ApiCalls.loadAllSpells model AllSpellsLoaded
    | AllSpellsLoaded spells ->
      let schools = spells |> Seq.map (fun s -> s.School) |> Seq.distinct
      let casterClasses = spells |> Seq.collect (fun s -> s.ClassSpellLevels |> Seq.map(fun x -> x.ClassName)) |> Seq.distinct
      let castingTimes = spells |> Seq.map (fun s -> s.CastingTime) |> Seq.countBy id
      let components = spells |> Seq.collect (fun s -> s.Components |> Seq.map (fun c -> c.Name)) |> Seq.distinct
      let ranges = spells |> Seq.map (fun s -> s.Range) |> Seq.countBy id |> Seq.sortByDescending snd
      let durations = spells |> Seq.map (fun s -> s.Duration) |> Seq.countBy id |> Seq.sortByDescending snd
      let sources = spells |> Seq.map (fun s -> s.Source) |> Seq.countBy id |> Seq.sortByDescending snd
      let srm =
        { model.SearchRootModel with 
            Schools = Seq.toList schools
            CasterClasses = Seq.toList casterClasses
            CastingTimes = Seq.toList castingTimes
            Components = Seq.toList components
            Ranges = Seq.toList (Seq.map fst ranges)
            Durations = Seq.toList (Seq.map fst durations)
            Sources = Seq.toList (Seq.map fst sources)
        }
      let model = 
        { model with SpellRows = spells; SearchRootModel = srm  }
      model, Cmd.none
    | SpellLoadingExn e ->
      console.error e
      model, Cmd.none
    | LoadSpell id ->
      model, Cmd.OfAsync.perform (ApiCalls.loadSpell model) id SpellLoaded
    | SpellLoaded spell ->
      { model with Spell = Some spell }, Cmd.none
    | SpellLoadExn e ->
      console.error e
      { model with Spell = None }, Cmd.none
    | ReturnToList ->
      { model with Spell = None }, Cmd.none
    | SearchMsg (SearchRoot.Msg.SearchUpdated search) ->
      let model = { model with Search = search }
      model, Cmd.none
    | SearchMsg msg ->
      let searchModel, cmd = SearchRoot.update msg model.SearchRootModel
      { model with SearchRootModel = searchModel }, Cmd.map SearchMsg cmd


  let ListView model dispatch = 
    let filteredSpells = SpellFiltering.filterSpells model.Search model.SpellRows
    Html.div [
      theme.dark
      prop.children [
        match model.Spell with
        | Some spell ->
          Spell.view spell (fun () -> dispatch ReturnToList)
        | None ->
          SearchRoot.view model.SearchRootModel (SearchMsg >> dispatch)

          Daisy.divider (sprintf "Spells (%i)" (Seq.length filteredSpells))

          SpellTable.view filteredSpells (LoadSpell >> dispatch)
      ]
    ]

