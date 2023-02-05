namespace PathfinderSpellDb

open PathfinderSpellDb.Types
open System
open System.IO
open FSharp.Data

module SpellParsing = 
  let loadFile path = File.ReadAllLines path

  let rawSpells = CsvFile.Load(__SOURCE_DIRECTORY__ + "/spells.csv").Cache()


  let spells =
    rawSpells.Rows
    |> Seq.map (fun row ->
      let name = row.["name"]
      let school = row.["school"]
      let description = row.["description_formatted"]
      {
        Name = name
        School = school
        Description = description
      }
    )
    |> Seq.toList

  printfn "Loaded %i spells" (List.length spells)