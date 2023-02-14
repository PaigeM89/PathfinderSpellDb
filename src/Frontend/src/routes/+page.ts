import { getJson } from '../Shared';
import type { PageLoad } from './$types';
import type { CharacterClass } from '../Types';
import { fetchAllSpellRows } from './SpellRows';

export const ssr = false;

export const load : PageLoad =( async ({ fetch, params}) => {
  //let spells = await fetchAllSpellRows(fetch);
  const classes : CharacterClass[] = await getJson(fetch, "/classes");

  return {classes: classes, fetch: fetch};
});