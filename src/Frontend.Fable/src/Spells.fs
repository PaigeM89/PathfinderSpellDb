namespace Frontend

open System
open Fable.Core
open Fable.Core.JS
open Fable.Core.JsInterop
open Elmish
open Feliz
open Feliz.DaisyUI
open Elmish.Navigation
open Frontend.Types

module Spells =

  type Model = {
    Route : Route

    RootServerUrl : string
    SpellRows : Shared.Dtos.SpellRow []
    IsFiltering : bool
    FilteredSpells : Shared.Dtos.SpellRow [] option
    RowsLimit : int option // when none, show all spells

    /// If the user views a specific spell, this will get populated with that spell's details
    Spell : Shared.Dtos.Spell option
    /// If populated, that spell id is currently loading
    LoadingSpell : int option

    Search : Types.Search
    SearchRootModel : SearchRoot.Model
  } with
    static member Init serverUrl initialRoute = {
      Route = initialRoute

      RootServerUrl = serverUrl
      SpellRows = [||]
      IsFiltering = false
      FilteredSpells = None
      RowsLimit = Some 100
      
      Spell = None
      LoadingSpell = None
      Search = Types.Search.Empty()
      SearchRootModel = SearchRoot.Model.Init()
    }

  type Msg =
  | LoadAllSpells
  | AllSpellsLoaded of Shared.Dtos.SpellRow seq
  | SpellLoadingExn of exn
  | LoadSpell of spellId : int
  | SpellLoaded of Shared.Dtos.Spell
  | SpellLoadExn of exn
  | ReturnToList
  | SearchMsg of SearchRoot.Msg
  | FilterSpells
  // if None, then remove the limit
  | IncreaseSpellLimit of newLimit : int option


  let init (serverUrl : string) initialRoute = 
    let model = Model.Init serverUrl initialRoute
    match model.Route with
    | SpellList -> model, Cmd.ofMsg LoadAllSpells
    | Spell spellId ->
      let cmds = Cmd.batch [ Cmd.ofMsg LoadAllSpells; Cmd.ofMsg (LoadSpell spellId) ]
      model, cmds

  module ApiCalls =
    let loadAllSpells model =
      async {
        let url = model.RootServerUrl + "/spells"
        let! response = Fetch.fetch url [] |> Async.AwaitPromise
        let! spells = response.json<Shared.Dtos.SpellRow seq>() |> Async.AwaitPromise
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
      let castingTimes = spells |> Seq.map (fun s -> s.CastingTime) |> Seq.countBy id |> Seq.sortByDescending snd |> Seq.map fst
      let components = spells |> Seq.collect (fun s -> s.Components |> Seq.map (fun c -> c.Name)) |> Seq.distinct
      let ranges = spells |> Seq.map (fun s -> s.Range) |> Seq.countBy id |> Seq.sortByDescending snd
      let durations = spells |> Seq.map (fun s -> s.Duration) |> Seq.countBy id |> Seq.sortByDescending snd
      let savingThrows = spells |> Seq.map (fun s -> s.SavingThrowStr) |> Seq.countBy id |> Seq.sortByDescending snd
      let sources = spells |> Seq.map (fun s -> s.Source) |> Seq.countBy id |> Seq.sortByDescending snd
      let filterTargets =
        { Types.FilterTargets.Empty() with 
            Schools = Seq.toList schools
            CasterClasses = Seq.toList casterClasses
            CastingTimes = Seq.toList castingTimes
            Components = Seq.toList components
            Ranges = Seq.toList (Seq.map fst ranges)
            Durations = Seq.toList (Seq.map fst durations)
            SavingThrows = Seq.toList (Seq.map fst savingThrows)
            Sources = Seq.toList (Seq.map fst sources)
        }
      
      let srm = { model.SearchRootModel with FilterTargets = filterTargets }
      let model = { model with SpellRows = Seq.toArray spells; SearchRootModel = srm  }
      model, Cmd.none
    | SpellLoadingExn e ->
      console.error e
      model, Cmd.none
    | LoadSpell id ->
      // sometimes we end up here with the spell already loaded; if so, don't bother loading it again
      match model.Spell with
      | Some spell when spell.Id = id -> model, Cmd.none
      | _ ->
        { model with LoadingSpell = Some id }, Cmd.OfAsync.perform (ApiCalls.loadSpell model) id SpellLoaded
    | SpellLoaded spell ->
      { model with Spell = Some spell; LoadingSpell = None }, Navigation.newUrl (sprintf "#spells/%i" spell.Id)
    | SpellLoadExn e ->
      console.error e
      { model with Spell = None }, Cmd.none
    | ReturnToList ->
      { model with Spell = None }, Navigation.newUrl "#"
    | SearchMsg (SearchRoot.Msg.SearchUpdated search) ->
      if search.IsEmpty() then
        let model = { model with Search = search; FilteredSpells = None }
        model, Cmd.none
      else
        let model = { model with Search = search; IsFiltering = true }
        model, Cmd.ofMsg FilterSpells
    | FilterSpells ->
      let filteredSpells = SpellFiltering.filterSpells model.Search model.SpellRows
      let model = { model with FilteredSpells = Some (Seq.toArray filteredSpells); IsFiltering = false }
      model, Cmd.none
    | SearchMsg msg ->
      let searchModel, cmd = SearchRoot.update msg model.SearchRootModel
      { model with SearchRootModel = searchModel }, Cmd.map SearchMsg cmd
    | IncreaseSpellLimit limit ->
      { model with RowsLimit = limit }, Cmd.none

  let footer = 
    Daisy.footer [
      prop.className "p-10 bg-neutral text-neutral-content grid"
      prop.children [
        Html.h3 [
          prop.className "ml-auto"
          prop.text ("Version: " + Version.__VERSION__)
        ]
        Html.a [
          prop.className "ml-auto"
          prop.text "Contribute on Github!"
          prop.href Types.GithubLink
        ]
      ]
    ]


  let View model dispatch = 
    Html.div [
      theme.dark
      prop.children [
        Html.h1 [
          prop.className "text-2xl text-center"
          prop.text "Pathfinder Spell DB"
        ]
        Html.h2 [
          prop.className "text-xl text-center"
          prop.text "A searchable database of all the spells in Pathfinder 1E"
        ]

        match model.Spell with
        | Some spell ->
          Spell.view spell (fun () -> dispatch ReturnToList)
        | None ->
          SearchRoot.view model.SearchRootModel (SearchMsg >> dispatch)

          let spellCount = 
            match model.FilteredSpells with
            | None -> Seq.length model.SpellRows
            | Some xs -> Seq.length xs

          Daisy.divider (sprintf "Spells (%i)" spellCount)

          if model.IsFiltering then
            SpellTable.view None None [||] (fun _ -> ())
          else
            let spells =
              match model.FilteredSpells with
              | None -> model.SpellRows
              | Some xs -> xs

            SpellTable.view model.RowsLimit model.LoadingSpell spells (LoadSpell >> dispatch)

            Daisy.divider ""
            match model.RowsLimit with
            | Some limit ->
              if limit < spells.Length then
                Html.div [
                  prop.className "flex place-content-center"
                  prop.children [
                    Daisy.button.button [
                      prop.text "Load next 100 spells"
                      prop.onClick (fun _ -> IncreaseSpellLimit (Some (limit + 100)) |> dispatch)
                      prop.className "mt-2 mb-4 mx-2"
                    ]
                    Daisy.button.button [
                      prop.text "Load all spells"
                      prop.onClick (fun _ -> IncreaseSpellLimit None |> dispatch)
                      prop.className "mt-2 mb-4 mx-2"
                    ]
                  ]
                ]
            | None -> Html.none

            footer
      ]
    ]
