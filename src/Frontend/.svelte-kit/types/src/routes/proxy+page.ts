// @ts-nocheck
import { getJson } from '../Shared';
import type { PageLoad } from './$types';
import type { CharacterClass } from '../Types';
import { fetchSpellRows } from './SpellRows';

export const ssr = false;

export const load =( async ({ fetch, params}) => {
  let spells = await fetchSpellRows(fetch, 0);
  const classes : CharacterClass[] = await getJson(fetch, "/classes");

  return {spells: spells.SpellRows, currentCount: spells.ResultCount, totalSpells: spells.TotalCount, classes: classes, fetch: fetch};
});;null as any as PageLoad;