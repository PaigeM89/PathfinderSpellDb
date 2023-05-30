module App

open Frontend
open Elmish
open Elmish.React
open Elmish.UrlParser
open Elmish.Navigation

let apiRoute = 
#if DEBUG
  "http://localhost:5000"
#else
  "https://api.pathfinderspelldb.com"
#endif

let init initialRoute = 
  match initialRoute with
  | Some route -> Spells.init apiRoute route
  | None -> Spells.init apiRoute Spells.SpellList

let route : Parser<(Spells.Route -> Spells.Route), Spells.Route> =
  oneOf [
    map Spells.Spell (s "spells" </> i32)
    map Spells.Route.SpellList (s "spells")
  ]

let urlUpdate (result : Spells.Route option) model =
  match result with
  | Some (Spells.Spell id) -> 
    { model with Spells.Route = Spells.Spell id }, Cmd.ofMsg (Spells.LoadSpell id)
  | Some Spells.SpellList ->
    { model with Route = Spells.SpellList; Spell = None }, Cmd.none
  | None -> { model with Route = Spells.SpellList; Spell = None }, Navigation.modifyUrl "#"

let hashRouteParser : Navigation.Parser<Spells.Route option> = parseHash route

Program.mkProgram init Spells.update Spells.View
// batching can have buggy text input (loses input) if the app is running slow
//|> Program.withReactBatched "app"
|> Program.withReactSynchronous "app"
|> Program.toNavigable hashRouteParser urlUpdate
|> Program.run
//|> Program.runWith apiRoute