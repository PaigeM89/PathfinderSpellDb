namespace Shared

module Dtos =
  type ClassSpellLevel = {
    ClassName: string
    Level : int
  } with
      static member Create className level = {
        ClassName = className
        Level = level
      }
      
      override this.ToString() = sprintf "%s %i" this.ClassName this.Level

  type Component = {
    Name : string
    Abbr : string
    Cost : string option
  } with
    static member Create name abbr cost = {
      Name = name
      Abbr = abbr
      Cost = cost
    }

  type SpellRow = {
    Id : int
    Name : string
    School : string
    ShortDescription : string
    ClassSpellLevels : ClassSpellLevel[]
    ClassSpellLevelsString : string
    CastingTime : string
    Components : Component list
    Range : string
    Area : string
    Duration : string
    SavingThrowStr : string
    SpellResistance : bool
    HasMythic : bool
    Source : string
  }

  type Spell = {
    Id : int
    Name : string
    School : string
    Subschool : string option
    Descriptors: string seq
    Description : string
    ClassSpellLevels: ClassSpellLevel seq
    ClassSpellLevelsString : string
    Domains : string
    CastingTime : string
    Components : Component seq
    Range : string
    Area : string
    Duration : string
    SavingThrows : string
    SpellResistance: bool
    MythicText : string option
    Source : string
  }