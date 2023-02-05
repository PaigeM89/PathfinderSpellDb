// @ts-nocheck
import { getJson } from '../Shared';
import type { PageLoad } from './$types';

export interface Spell {
  Id : number
  Name : string
  School : string
  ShortDescription : string
}

export const load =( async ({ fetch, params}) => {
  const spells : Spell[] = await getJson(fetch, "/spells");
  return {spells: spells, fetch: fetch};
});;null as any as PageLoad;