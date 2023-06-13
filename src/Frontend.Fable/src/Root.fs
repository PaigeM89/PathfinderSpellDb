module App

open Frontend
open Frontend.Types
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
  | None -> Spells.init apiRoute SpellList

let route : Parser<(Route -> Route), Route> =
  oneOf [
    map Spell (s "spells" </> i32)
    map Route.SpellList (s "spells")
  ]

let urlUpdate (result : Route option) model =
  match result with
  | Some (Spell id) -> 
    { model with Spells.Route = Spell id }, Cmd.ofMsg (Spells.LoadSpell id)
  | Some SpellList ->
    { model with Route = SpellList; Spell = None }, Cmd.none
  | None -> { model with Route = SpellList; Spell = None }, Navigation.modifyUrl "#"

let hashRouteParser : Navigation.Parser<Route option> = parseHash route

Program.mkProgram init Spells.update Spells.View
// batching can have buggy text input (loses input) if the app is running slow
// so usee synchronous instead
|> Program.withReactSynchronous "app"
|> Program.toNavigable hashRouteParser urlUpdate
|> Program.run