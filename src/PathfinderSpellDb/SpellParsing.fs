namespace PathfinderSpellDb

open System
open System.IO
open FSharp.Data

module Types =
  

  type Spell = {
    Id: int
    Name : string
    School : string
    SubSchool : string option
    Descriptors : string list
    ShortDescription : string
    Description : string
  }

module SpellParsing = 
  open Types

  let loadFile path = File.ReadAllLines path

  let rawSpells = CsvFile.Load(__SOURCE_DIRECTORY__ + "/spells.csv").Cache()

  let strValueOrNone (s : string) = if s = "" then None else Some s

  let split (s : string) = s.Split(",") |> Array.toList

  let spells =
    rawSpells.Rows
    |> Seq.mapi (fun index row ->
      {
        Id = index
        Name = row.["name"]
        School = row.["school"]
        SubSchool = row.["subschool"] |> strValueOrNone
        Descriptors = row.["descriptor"] |> split
        ShortDescription = row.["short_description"].Trim()
        Description = row.["description_formatted"]
      }
    )
    |> Seq.toList

  printfn "Loaded %i spells" (List.length spells)

module GraphQL =
  open Types
  open FSharp.Data.GraphQL.Types

  let findSpellByName (name : string) = 
    SpellParsing.spells |> List.tryFind (fun s -> s.Name.ToLowerInvariant() = name)

  let findSpellByIndex (index: int) =
    SpellParsing.spells |> List.tryItem index

  let spellNameSearch (str : string) =
    if (str.Length > 1) then
      let str : string = str.ToLowerInvariant()
      SpellParsing.spells |> List.filter (fun s -> s.Name.ToLowerInvariant().Contains(str))
    else
      SpellParsing.spells

  /// GraphQL type
  let SpellType : ObjectDef<Spell> =
    Define.Object<Spell>(
      name = "Spell",
      description = "A magical effect created by a character when cast.",
      fields = [
        Define.Field("id", Int, "The id of the spell. Used for querying. Generated on data load", fun _ (s : Spell) -> s.Id)
        Define.Field("name", String, "The name of the spell", fun _ (s : Spell) -> s.Name)
        Define.Field("school", String, "The school of the spell", fun _ (s : Spell) -> s.School)
        Define.Field("subschool", Nullable String, "The subschool of the spell, if any", fun _ (s : Spell) -> s.SubSchool)
        Define.Field("descriptors", ListOf String, "The descriptors of the spell, if any", fun _ (s: Spell) -> s.Descriptors)
        Define.Field("description", String, "The short description of the spell", fun _ (s : Spell) -> s.ShortDescription)
        Define.Field("fullDescription", String, "The full HTML description of the spell", fun _ (s: Spell) -> s.Description)
      ]
    )