export interface ClassSpellLevel {
  ClassName : string
  Level : number
}

export interface DU {
  Case : string
  Values: string | null
}

export interface Component {
  Name : string
  Abbr : string
  Cost : string | null
}

export interface SpellRow {
  Id : number
  Name : string
  School : string
  ShortDescription : string
  ClassSpellLevels : ClassSpellLevel []
  CastingTime : string
  Components : Component []
  Range : string
}

export interface Paging {
  Offset: number
  Limit : number
}

export interface SpellSearch {
  Name : string | null
  School : string | null
  Paging : Paging | null
}

export interface SpellSearchResult {
  SpellRows: SpellRow [],
  ResultCount: number,
  TotalCount: number
}

export interface CharacterClass {
  Name : string
}