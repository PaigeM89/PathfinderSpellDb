import { postJson } from '../Shared';
import type { PageLoad } from './$types';
import type { SpellRow } from '../Types';

export const ssr = false;

export const load : PageLoad =( async ({ fetch, params}) => {
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

  console.log('spells', spells);

  return {spells: spells, fetch: fetch};
});