// @ts-nocheck
import { error } from '@sveltejs/kit';
import type { PageLoad } from './$types';

export const load = (({ params }) => {

  const id = params.slug;

  return {
    id: id
  };
});;null as any as PageLoad;