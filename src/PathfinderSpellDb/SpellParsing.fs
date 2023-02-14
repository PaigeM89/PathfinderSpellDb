namespace PathfinderSpellDb

open System
open System.IO
open FSharp.Data
open PathfinderSpellDb.Parsing

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

    [
      tryMapColumn "sor" ClassSpellLevel.Sorcerer
      tryMapColumn "wiz" ClassSpellLevel.Wizard
      tryMapColumn "cleric" ClassSpellLevel.Cleric
      tryMapColumn "druid" ClassSpellLevel.Druid
      tryMapColumn "ranger" ClassSpellLevel.Ranger
      tryMapColumn "bard" ClassSpellLevel.Bard
      tryMapColumn "paladin" ClassSpellLevel.Paladin
      tryMapColumn "alchemist" ClassSpellLevel.Alchemist
      tryMapColumn "summoner" ClassSpellLevel.Summoner
      tryMapColumn "witch" ClassSpellLevel.Witch
      tryMapColumn "inquisitor" ClassSpellLevel.Inquisitor
      tryMapColumn "oracle" ClassSpellLevel.Oracle
      tryMapColumn "antipaladin" ClassSpellLevel.Antipaladin
      tryMapColumn "magus" ClassSpellLevel.Magus
      tryMapColumn "adept" ClassSpellLevel.Adept
      tryMapColumn "bloodrager" ClassSpellLevel.Bloodrager
      tryMapColumn "shaman" ClassSpellLevel.Shaman
      tryMapColumn "psychic" ClassSpellLevel.Psychic
      tryMapColumn "medium" ClassSpellLevel.Medium
      tryMapColumn "mesmerist" ClassSpellLevel.Mesmerist
      tryMapColumn "occultist" ClassSpellLevel.Occultist
      tryMapColumn "spiritualist" ClassSpellLevel.Spiritualist
      tryMapColumn "skald" ClassSpellLevel.Skald
      tryMapColumn "investigator" ClassSpellLevel.Investigator
      tryMapColumn "hunter" ClassSpellLevel.Hunter
      tryMapColumn "summoner_unchained" ClassSpellLevel.SummonerUnchained
    ]
    |> List.choose id

  let getShortDescription(row : CsvRow) =
    let desc = row.["short_description"].Trim()
    if desc = "" then
      row.["description"].Trim().Split(".")[0] + "."
    else
      desc

  let buildCastingTime (row: CsvRow) =
    let casting_time = row.["casting_time"].Trim()
    if casting_time = "" then
      CastingTime.Other "Unknown"
    else if casting_time.ToLowerInvariant().Contains("standard action") then
      CastingTime.StandardAction
    else
      CastingTime.Other casting_time


  let buildCastingComponents (row : CsvRow) =
    [
      if row.["verbal"].Trim() = "1" then yield CastingComponent.Verbal
      if row.["somatic"].Trim() = "1" then yield CastingComponent.Somatic
      if row.["material"].Trim() = "1" then 
        if row.["costly_components"].Trim() = "1" then
          yield CastingComponent.CostlyMaterial ("", 0) //TODO: get costs. Handle variable costs
        else
          yield CastingComponent.Material None //todo: get material
      if row.["focus"].Trim() = "1" then yield CastingComponent.Focus None //todo: get materials
      if row.["divine_focus"].Trim() = "1" then yield CastingComponent.DivineFocus
    ]

  let buildRange (row : CsvRow) =
    let range = row.["range"].Trim().ToLowerInvariant()
    if range = "personal" then Range.Personal
    else if range = "touch" then Range.Touch
    else if range.Contains "close" then Range.Close
    else if range.Contains "medium" then Range.Medium
    else if range.Contains "long" then Range.Long
    else if range.Contains "unlimited" then Range.Unlimited
    else Range.Other range

  let buildDuration (row : CsvRow) =
    let duration = row.["duration"].Trim().ToLowerInvariant()
    if duration.StartsWith "1 round/level" then Duration.RoundPerLevel
    elif duration.StartsWith "1 min./level" then Duration.MinutePerLevel
    elif duration.StartsWith "1 minute/level" then Duration.MinutePerLevel
    elif duration.StartsWith "1 hour/level" then Duration.HourPerLevel
    elif duration.StartsWith "1 day/level" then Duration.DayPerLevel
    elif duration.StartsWith "permanent" then Duration.Permanent
    elif duration.StartsWith "see text" then Duration.SeeText
    elif duration = "instantaneous" then Duration.Instantaneous
    else Duration.Other duration

  let parseDomains (row : CsvRow) =
    let domainStr = row.["domain"].Trim()
    if domainStr = "" then
      []
    else
      []

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
        CastingTime = buildCastingTime row
        Components = buildCastingComponents row
        Range = buildRange row
        Duration = buildDuration row

        SavingThrow = row.["saving_throw"].Trim()

        Dismissible = row.["dismissible"].Trim() = "1"
        Shapeable = row.["shapeable"].Trim() = "1"

        Domains = []
        Bloodlines = []

        Effect = row.["effect"] |> strValueOrNone
        Targets = row.["targets"] |> strValueOrNone
      }
    )
    |> Seq.toList

  printfn "Loaded %i spells" (List.length spells)

  // let distinctSavingThrows = spells |> List.map (fun s -> s.SavingThrow) |> List.distinct

  // printfn "%i Distinct saving throws:\n%A" (List.length distinctSavingThrows) distinctSavingThrows

  let spellNameSearch (str : string) =
    if (str.Length > 1) then
      let str : string = str.ToLowerInvariant()
      spells |> List.filter (fun s -> s.Name.ToLowerInvariant().Contains(str))
    else
      spells

  let findSpellByIndex (index: int) =
    spells |> List.tryItem index

  let allClasses =
    spells
    |> List.collect (fun spell -> spell.ClassSpellLevels |> List.map (fun csl -> csl.ToTuple() |> fst))
    |> List.distinct

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