namespace Pfsdb

open Feliz

module Spell =
    let private detail label (value: string) =
        Html.div [
            prop.className "flex"
            prop.children [
                Html.h2 [
                prop.className "text-xl font-extrabold"
                prop.text (label + ":")
                ]
                Html.h2 [
                prop.className "text-xl pl-1"
                prop.text value
                ]
            ]
        ]