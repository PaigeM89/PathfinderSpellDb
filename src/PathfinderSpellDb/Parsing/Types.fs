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
      $"{this.Domain} ({this.Level})"

  type BloodlineClassLevel = {
    Bloodline : string
    ClassLevel : string
  }

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
    Duration: Duration

    SavingThrows : SavingThrow list
    SavingThrowsStr : string


    SpellResistance: bool
    Dismissible: bool
    Shapeable: bool

    Domains: DomainSpellLevel list
    Bloodlines: BloodlineClassLevel list

    Effect : string option
    Targets: string option
  }