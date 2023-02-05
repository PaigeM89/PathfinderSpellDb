import { capitalizeFirstLetter } from "../../Shared"

export interface Spell {
  Name : string
  School : string
  Subschool : string
  Descriptors: string []
  Description : string
}

export function schoolStr(spell : Spell) {
  var subschool = "";
  if (spell.Subschool) {
    subschool = ` (${spell.Subschool}) `;
  }
  var desc = "";
  if (spell.Descriptors && spell.Descriptors.length > 0 && spell.Descriptors[0] !== "") {
    let joined = spell.Descriptors.join(", ");
    desc = ` [${joined}]`;
  }
  const school = capitalizeFirstLetter(spell.School);
  return `${school}${subschool}${desc}`;
}