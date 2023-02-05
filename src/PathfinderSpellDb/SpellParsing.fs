namespace PathfinderSpellDb

open System
open System.IO
open FSharp.Data

module Types =
  open FSharp.Data.GraphQL.Types

  type Spell = {
    Id: int
    Name : string
    School : string
    Description: string
  }

  /// GraphQL type
  let SpellType : ObjectDef<Spell> =
    Define.Object<Spell>(
      name = "Spell",
      description = "A magical effect created by a character when cast.",
      fields = [
        Define.Field("name", String, "The name of the spell", fun _ (s : Spell) -> s.Name)
        Define.Field("school", String, "The school of the spell", fun _ (s : Spell) -> s.School)
        Define.Field("description", String, "The description of the spell", fun _ (s: Spell) -> s.Description)
      ]
    )

module SpellParsing = 
  open Types

  let loadFile path = File.ReadAllLines path

  let rawSpells = CsvFile.Load(__SOURCE_DIRECTORY__ + "/spells.csv").Cache()

  let spells =
    rawSpells.Rows
    |> Seq.mapi (fun index row ->
      let name = row.["name"]
      let school = row.["school"]
      let description = row.["description_formatted"]
      {
        Id = index
        Name = name
        School = school
        Description = description
      }
    )
    |> Seq.toList

  printfn "Loaded %i spells" (List.length spells)
