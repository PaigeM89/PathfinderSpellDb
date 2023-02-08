import { writable, type Writable, get } from "svelte/store";
import type { SpellRow } from "./Types";


export const createLocalStorageWritableStore = <T>(key: string, startingValue: T): Writable<T> => {
  const store = writable(startingValue);

  const storedValueStr = localStorage.getItem(key);
  if (storedValueStr != null) {
    store.set(JSON.parse(storedValueStr));
  }

  store.subscribe( (val) => localStorage.setItem(key, JSON.stringify(val)) );

  window.addEventListener('storage', () => {
    const storedValueStr = localStorage.getItem(key);
    if (storedValueStr == null) return;

    const localValue: T = JSON.parse(storedValueStr);
    if (localValue !== get(store)) store.set(localValue);
  });

  return store;
}

export const spellRowsStore = createLocalStorageWritableStore<SpellRow[]>('spellRows', []);

