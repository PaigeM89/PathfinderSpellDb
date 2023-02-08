<script lang="ts">
  import type { Writable } from "svelte/store";
  import SchoolSearch from "../searchComponents/SchoolSearch.svelte";
  import { capitalizeFirstLetter, classListToString } from "../Shared";
  import { createLocalStorageWritableStore, spellRowsStore } from "../Stores";
  import type { SpellRow } from "../Types";
  import type { PageData } from "./$types";

  export let data: PageData;
  spellRowsStore.set(data.spells);

  let name : Writable<string> = createLocalStorageWritableStore("searchName", "");
  let searchBySchools : string[] = []; //createLocalStorageWritableStore<string[]>("searchSchools", []);
  let wasSearch = false;

  const headers = [
    "Name", "School", "Description", "Level"
  ];

  function filterSpells(spells : SpellRow[], name : string, schools : string[]) : SpellRow[] {
    let searchName = false;
    if (name.trim() === "" && wasSearch === true) {
      wasSearch = false;
      console.log('resetting search');
    } else if (name.trim() !== "") {
      searchName = true;
      wasSearch = true;
    }

    let searchSchool = false;
    if (schools && schools.length > 0) {
      searchSchool = true;
    }

    let filteredSpells =
      spells
        .filter(spell => {
          let namePassFilter = true;
          if (searchName) {
            namePassFilter = spell.Name.toLocaleLowerCase().includes(name.toLocaleLowerCase());
          }
          let schoolPassFilter = true
          if (searchSchool) {
            if (schools.find(school => school.toLocaleLowerCase() === spell.School.toLocaleLowerCase())) {
              schoolPassFilter = true;
            } else {
              schoolPassFilter = false;
            }
          }

          return namePassFilter && schoolPassFilter;
        });
    
    return filteredSpells;
  }

  $: filteredSpells = filterSpells($spellRowsStore, $name, searchBySchools);

  // wow typescript is dumb.
  let timer: string | number | NodeJS.Timeout | undefined;
  const debounceName = (e : Event) => {
    clearTimeout(timer);
    timer = setTimeout( () => {
      if (e && e.target && e.target instanceof HTMLInputElement ) {
        name.set(e.target.value);
      }
    }, 250);
  }
</script>

<h1>Pathfinder Spell Database</h1>
<p>A database of all the spells in Pathfinder 1E.</p>
<h2>Search by name: </h2><input value={$name} on:input={debounceName} />

<h2>Search by school(s):</h2>
<div>
  <SchoolSearch bind:searchBySchools={searchBySchools} />
</div>


{#if $spellRowsStore}
  <h1>Spells</h1>
  <table>
    <thead>
      <tr>
        {#each headers as header}
          <td>{header}</td>
        {/each}
      </tr>
    </thead>
    <tbody>
        {#each filteredSpells as spell}
          <tr>
            <td>
              <a href="/spells/{spell.Id}">{spell.Name}</a>
            </td>
            <td>{capitalizeFirstLetter(spell.School)}</td>
            <td>{@html spell.ShortDescription}</td>
            <td>
              {classListToString(spell.ClassSpellLevels)}
            </td>
          </tr>
        {/each}
    </tbody>
  </table>
{/if}

<style>
  p {
    text-align: justify;
  }

  thead {
    font-size: x-large;
  }

  tbody{
    font-size: small;
  }

  table, thead, td {
    border: 1px solid;
    border-collapse: collapse;
    padding: 0.25rem;
  }

  table {
    width: 100%;
  }
</style>