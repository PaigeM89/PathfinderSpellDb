import { postJson } from '../Shared';
import type { SpellSearchResult } from '../Types';

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

export const fetchAllSpellRows = ( async (fetch: (url: string, body: any) => Promise<any>) => {
  console.log('fetchign all spells');
  
  const paging = {
    Name: null,
    School: null,
    Paging: null
  };

  const spells : SpellSearchResult = await postJson(fetch, "/spells", paging);
  return spells;
});