namespace PathfinderSpellDb.Parsing

open System
open System.IO
open FSharp.Data
open System.Text.Json
open System.Text.Json.Serialization

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
      let name = GetUnionCaseName this
      match this with
      | Adept x -> name, x
      | Alchemist x -> name, x
      | Antipaladin x -> name, x
      | Bard x -> name, x
      | Bloodrager x -> name, x
      | Cleric x -> name, x
      | Druid x -> name, x
      | Hunter x -> name, x
      | Inquisitor x -> name, x
      | Investigator x -> name, x
      | Magus x -> name, x
      | Medium x -> name, x
      | Mesmerist x -> name, x
      | Occultist x -> name, x
      | Oracle x -> name, x
      | Paladin x -> name, x
      | Psychic x -> name, x
      | Ranger x -> name, x
      | Shaman x -> name, x
      | Skald x -> name, x
      | Spiritualist x -> name, x
      | Sorcerer x -> name, x
      | Summoner x -> name, x
      | SummonerUnchained x -> name, x
      | Warpriest x -> name, x
      | Witch x -> name, x
      | Wizard x -> name, x

  [<RequireQualifiedAccess>]
  type CastingTime =
  | StandardAction
  | Other of string
  with
    override this.ToString() =
      match this with
      | StandardAction -> "1 standard action"
      | Other other -> other

  [<JsonFSharpConverter>]
  [<RequireQualifiedAccess>]
  type CastingComponent =
  | Verbal
  | Somatic
  | Material of material : string option
  | CostlyMaterial of material : string * cost : int
  | Focus of focus: string option
  | DivineFocus

  [<RequireQualifiedAccess>]
  type Range =
  | Personal
  | Touch
  | Close
  | Medium
  | Long
  | Unlimited
  | Other of range: string

  [<RequireQualifiedAccess>]
  type Duration =
  | Instantaneous
  | RoundPerLevel
  | MinutePerLevel
  | HourPerLevel
  | DayPerLevel
  | Permanent
  | Other of duration: string
  | SeeText

  type DomainSpellLevel = {
    Domain : string
    Level : int
  } with
    static member Create domain level = {
      Domain = domain
      Level = level
    }

    override this.ToString() =
      $"{this.Domain} {this.Level}"

  type BloodlineClassLevel = {
    Bloodline : string
    Level : int
  } with
    static member Create bloodline level = {
      Bloodline = bloodline
      Level = level
    }

    override this.ToString() = $"{this.Bloodline} ({this.Level})"

  type PatronClassLevel = {
    Patron : string
    Level : int
  } with
    static member Create patron level = {
      Patron = patron
      Level = level
    }

    override this.ToString() = $"{this.Patron} ({this.Level})"

  [<RequireQualifiedAccess>]
  type SavingThrowDescriptor =
  | Half
  | Partial
  | Negates
  | Harmless
  | Disbelieve
  | SeeText
  | Object

  [<RequireQualifiedAccess>]
  type SavingThrow = 
  | Fortitude of descriptors : SavingThrowDescriptor list
  | Reflex of descriptors : SavingThrowDescriptor list
  | Will of descriptors : SavingThrowDescriptor list
  | None of descriptors : SavingThrowDescriptor list

  [<RequireQualifiedAccess>]
  type SpellResistanceDescriptor =
  | Harmless
  | Object

  [<RequireQualifiedAccess>]
  type SpellResistance = 
  | Yes of descriptors: SpellResistanceDescriptor list
  | No  of descriptors: SpellResistanceDescriptor list
  | SeeText of descriptors: SpellResistanceDescriptor list

  type Spell = {
    Id: int
    Name : string
    School : string
    SubSchool : string option
    Descriptors : string list
    ShortDescription : string
    Description : string
    ClassSpellLevels : ClassSpellLevel list
    CastingTime : CastingTime
    Components: CastingComponent list
    Range: Range
    Area: string option
    Duration: Duration

    SavingThrows : SavingThrow list
    SavingThrowsStr : string


    SpellResistance: bool
    Dismissible: bool
    Shapeable: bool

    Domains: DomainSpellLevel list
    Bloodlines: BloodlineClassLevel list
    Patrons: PatronClassLevel list

    Effect : string option
    Targets: string option

    Source : string
  }