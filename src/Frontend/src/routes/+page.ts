import { getJson } from '../Shared';
import type { PageLoad } from './$types';

interface Spell {
  Id : number
  Name : string
  School : string
  ShortDescription : string
}

export const load : PageLoad =( async ({ fetch, params}) => {
  const spells : Spell[] = await getJson(fetch, "/spells");
  console.log(spells);
  return {spells: spells};
});