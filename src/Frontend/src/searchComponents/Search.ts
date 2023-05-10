import {v4 as uuid} from 'uuid';

export enum SearchTarget {
  School = "School",
  Class = "Class",
  Description  = "Description",
  CastingTime = "Casting Time",
  Components = "Components",
  Range = "Range",
  Duration = "Duration",
  Level = "Spell Level",
  Source = "Source"
};

export const Schools = [
  "Abjuration",
  "Conjuration",
  "Divination",
  "Enchantment",
  "Evocation",
  "Illusion",
  "Necromancy",
  "Transmutation",
  "Universal"
];

export type Search = {
  Id : string,
  SearchTarget : SearchTarget,
  TargetValues : string[]
}

export function DefaultSearch() : Search {
  return {
    Id: uuid(),
    SearchTarget: SearchTarget.School,
    TargetValues: []
  };
}