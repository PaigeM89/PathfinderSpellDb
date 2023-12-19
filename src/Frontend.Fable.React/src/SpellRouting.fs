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

  let loadData = async {
    let url = ApiRoot.apiRoute + "/spells"
    let! response = Fetch.fetch url [] |> Async.AwaitPromise
    let! spells = response.json<Shared.Dtos.SpellRow seq>() |> Async.AwaitPromise
    return spells
  }

  [<ReactComponent>]
  let SpellsRouter(spells : SpellRow seq) =
    let (currentUrl, updateUrl) = React.useState(Router.currentUrl())
    React.router [
      router.onUrlChanged updateUrl
      router.children [
        match currentUrl with
        | [] -> SpellList.SpellList(spells)
        | ["spells"; Route.Int spellId ] -> SpellLoader spellId
        | _ -> SpellList.SpellList(spells)
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
    | Deferred.Resolved content ->
      SpellsRouter(content)

  [<ReactComponent>]
  let SpellListLoader2() =
    let isLoading, setLoading = React.useState false
    let content, setContent = React.useState Seq.empty

    let loadData() = async {
      setLoading true
      let! spellRows = loadData
      setLoading false
      setContent spellRows
    }

    React.useEffectOnce(loadData >> Async.StartImmediate)

    if isLoading then Html.p "Loading..."
    else SpellsRouter content

