namespace Pfsdb

open Fable.Core
open Shared.Dtos
open Feliz
open Feliz.Router
open Feliz.UseDeferred

module SpellRouting =

  let loadSpell id = async {
    let url = ApiRoot.apiRoute + "/spells/" + (string id)
    let! response = Fetch.fetch url [] |> Async.AwaitPromise
    let! spell = response.json<Shared.Dtos.Spell>() |> Async.AwaitPromise
    return spell
  }

  [<ReactComponent>]
  let SpellLoader(id : int) =
    let spell = React.useDeferred(loadSpell id, [| |])
    match spell with
    | Deferred.HasNotStartedYet -> Html.p "Not Started"
    | Deferred.InProgress -> Html.p "In progress"
    | Deferred.Failed e -> Html.p "Errored"
    | Deferred.Resolved content -> Spell.Spell content

  let createFilterTargets (spells : Shared.Dtos.SpellRow seq) =
    let schools = spells |> Seq.map (fun s -> s.School) |> Seq.distinct
    let casterClasses = spells |> Seq.collect (fun s -> s.ClassSpellLevels |> Seq.map(fun x -> x.ClassName)) |> Seq.distinct
    let castingTimes = spells |> Seq.map (fun s -> s.CastingTime) |> Seq.countBy id |> Seq.sortByDescending snd |> Seq.map fst
    let components = spells |> Seq.collect (fun s -> s.Components |> Seq.map (fun c -> c.Name)) |> Seq.distinct
    let ranges = spells |> Seq.map (fun s -> s.Range) |> Seq.countBy id |> Seq.sortByDescending snd
    let areas = spells |> Seq.map (fun s -> s.Area) |> Seq.countBy id |> Seq.sortByDescending snd
    let durations = spells |> Seq.map (fun s -> s.Duration) |> Seq.countBy id |> Seq.sortByDescending snd
    let savingThrows = spells |> Seq.map (fun s -> s.SavingThrowStr) |> Seq.countBy id |> Seq.sortByDescending snd
    let sources = spells |> Seq.map (fun s -> s.Source) |> Seq.countBy id |> Seq.sortByDescending snd
    { Types.FilterTargets.Empty() with 
        Schools = Seq.toList schools
        CasterClasses = Seq.toList casterClasses
        CastingTimes = Seq.toList castingTimes
        Components = Seq.toList components
        Ranges = Seq.toList (Seq.map fst ranges)
        Areas = Seq.toList (Seq.map fst areas)
        Durations = Seq.toList (Seq.map fst durations)
        SavingThrows = Seq.toList (Seq.map fst savingThrows)
        Sources = Seq.toList (Seq.map fst sources)
    }

  let loadData = async {
    let url = ApiRoot.apiRoute + "/spells"
    let! response = Fetch.fetch url [] |> Async.AwaitPromise
    let! spells = response.json<Shared.Dtos.SpellRow seq>() |> Async.AwaitPromise
    let filterTargets = createFilterTargets spells
    return (spells, filterTargets)
  }

  [<ReactComponent>]
  let SpellsRouter(spells : SpellRow seq, filterTargets) =
    let (currentUrl, updateUrl) = React.useState(Router.currentUrl())
    React.router [
      router.onUrlChanged updateUrl
      router.children [
        match currentUrl with
        | [] -> SpellList.SpellList(spells, filterTargets)
        | ["spells"; Route.Int spellId ] -> SpellLoader spellId
        | _ -> SpellList.SpellList(spells, filterTargets)
      ]
    ]

  [<ReactComponent>]
  let SpellListLoader() =
    let data = React.useDeferred(loadData, [||])
    match data with
    | Deferred.HasNotStartedYet -> Html.p "Not Started"
    | Deferred.InProgress -> Html.p "In progress"
    | Deferred.Failed e -> 
      Html.p "Errored"
    | Deferred.Resolved (spells, filterTargets) ->
      SpellsRouter(spells, filterTargets)

