namespace Shared

module Dtos =
  type ClassSpellLevel = {
    ClassName: string
    Level : int
  } with
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
    CastingTime : string
    Components : Component list
    Range : string
    Duration : string
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
    Domains : string
    CastingTime : string
    Components : Component seq
    Range : string
    Duration : string
    Source : string
  }