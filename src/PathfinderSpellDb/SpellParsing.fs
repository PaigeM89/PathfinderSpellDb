namespace PathfinderSpellDb

open System
open System.IO
open System.Text
open System.Text.RegularExpressions
open FSharp.Data
open FsLibLog
open FsLibLog.Operators
open PathfinderSpellDb.Parsing

module SpellParsing = 
  open Types


  let logger = LogProvider.getLoggerByName "PathfinderSpellDb.SpellParsing"


  let loadFile path = File.ReadAllLines path

  let rawSpells (config : Configuration.ApplicationConfig) =
    !!! "Loading raw spells off CSV Path {path}"
    >>!+ ("path", config.CsvPath)
    |> logger.info

    CsvFile.Load(config.CsvPath).Cache()

  let strValueOrNone (s : string) = if s = "" then None else Some s

  let intValueOrNone (s : string) =
    match Int32.TryParse s with
    | true, x -> Some x
    | false, _ -> None

  let capitalizeFirstLetter (s : string) =
    if s.Length <= 1 then s.ToUpperInvariant()
    else
      let firstLetter = s.Substring(0, 1)
      let rest = s.Substring(1)
      firstLetter.ToUpperInvariant() + rest
    

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
    |> List.sortBy (fun x -> x.ToTuple() |> fst)

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

  let buildArea (row : CsvRow) =
    let area = row.["area"].Trim()
    if area = "" then None else Some area

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
      // remove the () for easier regex matching
      let domainsAndLevels = domainStr.Replace("(", ""). Replace(")", "")
      let regexStr = """(\w+) (\d)"""
      let regex = Regex(regexStr, RegexOptions.IgnoreCase)
      let regexMatches = regex.Matches(domainsAndLevels)

      [
        for regexMatch in regexMatches do
          let groups = regexMatch.Groups
          let domain =
            let capture = groups.Item 1
            capture.Value
          let level = 
            let capture = groups.Item 2
            capture.Value |> intValueOrNone
          match level with
          | Some level ->
            DomainSpellLevel.Create domain level |> Some
          | None -> None
      ]
      |> List.choose id

  let parseBloodlines (row : CsvRow) =
    let bloodlinesStr = row.["bloodline"].Trim()
    if bloodlinesStr = "" then
      []
    else
      let str = bloodlinesStr.Replace("(", "").Replace(")", "")
      let regexStr = """(\w+) (\d)"""
      let regex = Regex(regexStr, RegexOptions.IgnoreCase)
      let regexMatches = regex.Matches(str)

      [
        for regexMatch in regexMatches do
          let groups = regexMatch.Groups
          let bloodline =
            let capture = groups.Item 1
            capture.Value
          let level =
            let capture = groups.Item 2
            capture.Value |> intValueOrNone
          match level with
          | Some level ->
            BloodlineClassLevel.Create bloodline level |> Some
          | None -> None
      ]
      |> List.choose id

  let parsePatrons (row : CsvRow) =
    let patronsStr = row.["patron"].Trim()
    if patronsStr = "" then
      []
    else
      let str = patronsStr.Replace("(", "").Replace(")", "")
      let regexStr = """(\w+) (\d)"""
      let regex = Regex(regexStr, RegexOptions.IgnoreCase)
      let regexMatches = regex.Matches str

      [
        for regexMatch in regexMatches do
          let groups = regexMatch.Groups
          let patron = 
            let capture = groups.Item 1
            capture.Value
          let level =
            let capture = groups.Item 2
            capture.Value |> intValueOrNone
          match level with
          | Some level -> 
            PatronClassLevel.Create patron level |> Some
          | None -> None
      ]
      |> List.choose id

  let spells (config : Configuration.ApplicationConfig) =
    let spells = rawSpells config
    
    spells.Rows
    |> Seq.map (fun row ->
      {
        Id = 0
        Name = row.["name"].Trim()
        School = row.["school"].Trim().ToLowerInvariant() |> capitalizeFirstLetter
        SubSchool = row.["subschool"] |> strValueOrNone
        Descriptors = row.["descriptor"] |> split
        ShortDescription = getShortDescription(row)
        Description = row.["description_formatted"]
        ClassSpellLevels = buildClassSpellLevels row
        CastingTime = buildCastingTime row
        Components = buildCastingComponents row
        Range = buildRange row
        Area = buildArea row
        Duration = buildDuration row

        SavingThrows = row.["saving_throw"].Trim() |> SavingThrows.parseSavingThrows
        SavingThrowsStr = 
          if row.["saving_throw"].Trim() = "" || row.["saving_throw"].Trim() = "none" then
            "None"
          else
            row.["saving_throw"].Trim()

        SpellResistance = row.["spell_resistance"].Trim().ToLowerInvariant().Contains("yes")
        Dismissible = row.["dismissible"].Trim() = "1"
        Shapeable = row.["shapeable"].Trim() = "1"

        Domains = parseDomains row
        Bloodlines = parseBloodlines row
        Patrons = parsePatrons row

        Effect = row.["effect"] |> strValueOrNone
        Targets = row.["targets"] |> strValueOrNone

        Source = row.["source"].Trim()
      }
    )
    |> Seq.sortBy (fun s -> s.Name)
    |> Seq.mapi (fun index spell -> { spell with Id = index } )
    |> Seq.toList
  
  let loadedSpells config = lazy (spells config)

  // let distinctSavingThrows = 
  //   spells
  //   |> List.groupBy (fun s -> s.SavingThrowsStr)
  //   |> List.map (fun (saving, spells) -> saving, List.length spells)
  //   |> List.sortBy snd
  //   |> List.iter (fun (saving, count) -> printfn "%A: %i" saving count)

  // printfn "%i Distinct saving throws:\n%A" (List.length distinctSavingThrows) distinctSavingThrows

  // let spellNameSearch (str : string) =
  //   if (str.Length > 1) then
  //     let str : string = str.ToLowerInvariant()
  //     spells |> List.filter (fun s -> s.Name.ToLowerInvariant().Contains(str))
  //   else
  //     spells

  let findSpellByIndex config (index: int) =
    (loadedSpells config).Value |> List.tryItem index

  let allClasses config =
    (loadedSpells config).Value
    |> List.collect (fun spell -> spell.ClassSpellLevels |> List.map (fun csl -> csl.ToTuple() |> fst))
    |> List.distinct

  let distinctRanges config =
    (loadedSpells config).Value
    |> List.map (fun spell -> spell.Range.ToString())
    |> List.distinct

  let distinctSchools config =
    (loadedSpells config).Value
    |> List.map (fun spell -> spell.School)
    |> List.distinct

