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
  }

  type Spell = {
    Id : int
    Name : string
    School : string
    Subschool : string option
    Descriptors: string list
    ShortDescription : string
    FullDescription : string
    ClassSpellLevels: ClassSpellLevel list
    Domains : string
    CastingTime : string
    Components : Component list
    Range : string
    Duration : string
    Source : string
  }