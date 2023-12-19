module Root

open Pfsdb
open Feliz
open Browser.Dom

let root = document.getElementById("app") |> ReactDOM.createRoot

SpellRouting.SpellListLoader() |> root.render
