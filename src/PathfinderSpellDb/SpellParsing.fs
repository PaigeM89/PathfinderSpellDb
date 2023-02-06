namespace PathfinderSpellDb

open System
open System.IO
open FSharp.Data

module Types =
  open FSharp.Reflection

  let GetUnionCaseName (x:'a) = 
    match FSharpValue.GetUnionFields(x, typeof<'a>) with
    | case, _ -> case.Name

  [<RequireQualifiedAccess>]
  type ClassSpellLevel = 
  | Adept of int
  | Alchemist of int
  | Antipaladin of int
  | Bard of int
  | Bloodrager of int
  | Cleric of int
  | Druid of int
  | Hunter of int
  | Inquisitor of int
  | Investigator of int
  | Magus of int
  | Medium of int
  | Mesmerist of int
  | Occultist of int
  | Oracle of int
  | Paladin of int
  | Psychic of int
  | Ranger of int
  | Shaman of int
  | Skald of int
  | Spiritualist of int
  | Sorcerer of int
  | Summoner of int
  | SummonerUnchained of int
  | Warpriest of int
  | Witch of int
  | Wizard of int
  with
    member this.ToTuple() =
      match this with
      | Adept x -> GetUnionCaseName this, x
      | Alchemist x -> GetUnionCaseName this, x
      | Antipaladin x -> GetUnionCaseName this, x
      | Bard x -> GetUnionCaseName this, x
      | Bloodrager x -> GetUnionCaseName this, x
      | Cleric x -> GetUnionCaseName this, x
      | Druid x -> GetUnionCaseName this, x
      | Hunter x -> GetUnionCaseName this, x
      | Inquisitor x -> GetUnionCaseName this, x
      | Investigator x -> GetUnionCaseName this, x
      | Magus x -> GetUnionCaseName this, x
      | Medium x -> GetUnionCaseName this, x
      | Mesmerist x -> GetUnionCaseName this, x
      | Occultist x -> GetUnionCaseName this, x
      | Oracle x -> GetUnionCaseName this, x
      | Paladin x -> GetUnionCaseName this, x
      | Psychic x -> GetUnionCaseName this, x
      | Ranger x -> GetUnionCaseName this, x
      | Shaman x -> GetUnionCaseName this, x
      | Skald x -> GetUnionCaseName this, x
      | Spiritualist x -> GetUnionCaseName this, x
      | Sorcerer x -> GetUnionCaseName this, x
      | Summoner x -> GetUnionCaseName this, x
      | SummonerUnchained x -> GetUnionCaseName this, x
      | Warpriest x -> GetUnionCaseName this, x
      | Witch x -> GetUnionCaseName this, x
      | Wizard x -> GetUnionCaseName this, x


  type Spell = {
    Id: int
    Name : string
    School : string
    SubSchool : string option
    Descriptors : string list
    ShortDescription : string
    Description : string
    ClassSpellLevels : ClassSpellLevel list
  }

module SpellParsing = 
  open Types

  let loadFile path = File.ReadAllLines path

  let rawSpells = CsvFile.Load(__SOURCE_DIRECTORY__ + "/spells.csv").Cache()

  let strValueOrNone (s : string) = if s = "" then None else Some s

  let intValueOrNone (s : string) =
    match Int32.TryParse s with
    | true, x -> Some x
    | false, _ -> None

  let split (s : string) = s.Split(",") |> Array.toList

  let buildClassSpellLevels (row : CsvRow) =
    let tryMapColumn (column: string) toClassSpellLevel =
      row.[column] |> intValueOrNone |> Option.map toClassSpellLevel

    let sorcLevel = tryMapColumn "sor" ClassSpellLevel.Sorcerer
    let wizLevel = tryMapColumn "wiz" ClassSpellLevel.Wizard
    let clericLevel = tryMapColumn "cleric" ClassSpellLevel.Cleric
    let druidLevel = tryMapColumn "druid" ClassSpellLevel.Druid
    // todo: the rest

    [
      sorcLevel
      wizLevel
      clericLevel
      druidLevel
    ]
    |> List.choose id

  let getShortDescription(row : CsvRow) =
    let desc = row.["short_description"].Trim()
    if desc = "" then
      row.["description"].Trim().Split(".")[0] + "."
    else
      desc

  let spells =
    rawSpells.Rows
    |> Seq.sortBy (fun rawSpell -> rawSpell.["name"].Trim())
    |> Seq.mapi (fun index row ->
      {
        Id = index
        Name = row.["name"].Trim()
        School = row.["school"].Trim().ToLowerInvariant()
        SubSchool = row.["subschool"] |> strValueOrNone
        Descriptors = row.["descriptor"] |> split
        ShortDescription = getShortDescription(row)
        Description = row.["description_formatted"]
        ClassSpellLevels = buildClassSpellLevels row
      }
    )
    |> Seq.toList

  printfn "Loaded %i spells" (List.length spells)

  let spellNameSearch (str : string) =
    if (str.Length > 1) then
      let str : string = str.ToLowerInvariant()
      spells |> List.filter (fun s -> s.Name.ToLowerInvariant().Contains(str))
    else
      spells

  let findSpellByIndex (index: int) =
    spells |> List.tryItem index

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