// @ts-nocheck
import { getJson, postJson } from '../Shared';
import type { PageLoad } from './$types';
import type { CharacterClass, SpellRow } from '../Types';

export const ssr = false;

export const load =( async ({ fetch, params}) => {
  // todo: this needs to load the spell rows store & return it
  // instead of making the call every time anyways
  // but i'm going to do that after i do paging

  const paging = {
    Name: null,
    School: null,
    Paging: {
      Offset: 0,
      Limit: 200
    }
  }

  const spells : SpellRow[] = await postJson(fetch, "/spells", paging);
  const classes : CharacterClass[] = await getJson(fetch, "/classes");

  return {spells: spells, classes: classes, fetch: fetch};
});;null as any as PageLoad;