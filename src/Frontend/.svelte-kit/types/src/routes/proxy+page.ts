// @ts-nocheck
import { getJson } from '../Shared';
import type { PageLoad } from './$types';
import type { CharacterClass } from '../Types';
import { fetchAllSpellRows } from './SpellRows';

export const ssr = false;

export const load =( async ({ fetch, params}) => {
  let spells = await fetchAllSpellRows(fetch);
  const classes : CharacterClass[] = await getJson(fetch, "/classes");

  const count = spells.length;

  return {spells: spells, currentCount: count, totalSpells: count, classes: classes, fetch: fetch};
});;null as any as PageLoad;