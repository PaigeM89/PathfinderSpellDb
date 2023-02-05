// @ts-nocheck
import { getJson } from '../../../Shared';
import type { PageLoad } from './$types';


export interface Spell {
  Name : string
  School : string
  Subschool : string
  Descriptors: string []
  Description : string
}

export const load = ( async ({ fetch, params }) => {
  const id = params.slug;

  const spell : Spell = await getJson(fetch, `/spells/${id}`);

  return {
    id: id, spell : spell
  };
});;null as any as PageLoad;