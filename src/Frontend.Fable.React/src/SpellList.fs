namespace Pfsdb

open System
open Shared.Dtos
open Feliz
open Feliz.UseDeferred
open Feliz.DaisyUI

module SpellList =
    let thing = 0

    let sampleData = [
        {
            Id = 0
            Name = "Test spell"
            School = "Evocation"
            ShortDescription = "Short description"
            ClassSpellLevels = [|
                ClassSpellLevel.Create "Bard" 1
            |]
            ClassSpellLevelsString = "Bard 1"
            CastingTime = "3.85 hours"
            Components = []
            Range = "82 feet per 1.5 levels"
            Area = "3.6 cubic meters"
            Duration = "Instant"
            SavingThrowStr = "Reflex 1/4"
            SpellResistance = false
            Source = "Test"
        }
    ]

    let loadData = async {
        do! Async.Sleep 1000
        return sampleData
    }

    [<ReactComponent>]
    let SpellList(spells : SpellRow list) =
        Html.div [
            theme.dark
            prop.children [
                Html.h1 [
                    prop.className "text-2xl text-center"
                    prop.text "Pathfinder Spell DB"
                ]
            ]
        ]

    [<ReactComponent>]
    let SpellListLoader() =
        let data = React.useDeferred(loadData, [||])
        match data with
        | Deferred.HasNotStartedYet -> Html.p "Not Started"
        | Deferred.InProgress -> Html.p "In progress"
        | Deferred.Failed e -> Html.p "Errored"
        | Deferred.Resolved content -> 
            SpellList(content)