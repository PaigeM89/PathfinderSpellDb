module App

open Frontend
open Elmish
open Elmish.React

Program.mkProgram Spells.init Spells.update Spells.ListView
|> Program.withReactBatched "app"
|> Program.runWith ("http://localhost:5000")