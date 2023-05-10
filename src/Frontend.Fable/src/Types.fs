namespace Frontend

module Types =
  type ClassSpellLevel = {
    ClassName: string
    Level : int
  }

  type Component = {
    Name : string
    Abbr : string
    Cost : string option
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

  type Search = {
    Name : string option
  } with
    static member Empty() = {
      Name = None
    }

  module Search =
    let forName n (s : Search) = { s with Name = Some n }
    let clearName (s : Search) = { s with Name = None }
    let setName n (s : Search) = { s with Name = n }