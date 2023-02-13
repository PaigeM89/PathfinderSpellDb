import { getJson, postJson } from '../Shared';
import type { SpellRow, SpellSearchResult } from '../Types';

const limit = 200;

export const fetchSpellRows = ( async (fetch: (url: string, body: any) => Promise<any>, page: number) => {
  const offset = page * limit;
  console.log('fetchign spells', offset, limit);
  
  const paging = {
    Name: null,
    School: null,
    Paging: {
      Offset: page * limit,
      Limit: limit
    }
  };

  const spells : SpellSearchResult = await postJson(fetch, "/spells", paging);
  return spells;
});

export const fetchAllSpellRows = ( async (fetch: (url: string) => Promise<any>) => {
  const spells : SpellRow[]  = await getJson(fetch, "/spells");
  return spells;
});