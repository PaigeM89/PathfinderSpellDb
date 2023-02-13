<script lang="ts">
  import type { Writable } from "svelte/store";
  import CheckboxList from "../searchComponents/CheckboxList.svelte";
  import SchoolSearch from "../searchComponents/SchoolSearch.svelte";
  import { capitalizeFirstLetter, classListToString } from "../Shared";
  import { allSpellRowsStore, appendDistinct, createLocalStorageWritableStore } from "../Stores";
  import type { Component, SpellRow } from "../Types";
  import type { PageData } from "./$types";
  import { inview } from "svelte-inview/dist/index";
  import { fetchAllSpellRows, fetchSpellRows } from "./SpellRows";

  export let data: PageData;
  let allSpellsFetched = false;
  let page = 0;
  allSpellRowsStore.set(data.spells);

  let name : Writable<string> = createLocalStorageWritableStore("searchName", "");
  let searchBySchools : string[] = [];
  let searchByClasses : string[] = [];
  let wasSearch = false;

  const headers = [
    "Name", "School", "Description", "Casting Time", "Components", "Level"
  ];

  function filterSpells(spells : SpellRow[], name : string, schools : string[], classes: string[]) : SpellRow[] {    
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

    let searchClasses = false;
    if (searchByClasses && searchByClasses.length > 0) {
      searchClasses = true;
    }

    let filteredSpells =
      spells
        .filter(spell => {
          let namePassFilter = true;
          if (searchName) {
            namePassFilter = spell.Name.toLocaleLowerCase().includes(name.toLocaleLowerCase());
          }

          let schoolPassFilter = true;
          if (searchSchool) {
            if (schools.find(school => school.toLocaleLowerCase() === spell.School.toLocaleLowerCase())) {
              schoolPassFilter = true;
            } else {
              schoolPassFilter = false;
            }
          }

          let classesPassFilter = true;
          if (searchClasses) {
            const spellClasses = spell.ClassSpellLevels.map(csl => csl.ClassName.toLocaleLowerCase());
            if (classes.find(className => spellClasses.find(spellClass => spellClass === className.toLocaleLowerCase()))) {
              classesPassFilter = true;
            } else {
              classesPassFilter = false;
            }
          }

          return namePassFilter && schoolPassFilter && classesPassFilter;
        });
    
    return filteredSpells;
  }

  $: filteredSpells = filterSpells($allSpellRowsStore, $name, searchBySchools, searchByClasses);

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

  const fetchData = async (page: number) => {
    const spellResults = await fetchSpellRows(data.fetch, page);
    appendDistinct(spellResults.SpellRows);
  };

  const loadMore = (e : any) => {
    console.log('page is', page);
    if (e.detail.inView) fetchData(page);
    page += 1;
  }

  const fetchAllSpells = async() => {
    const spellResults = await fetchAllSpellRows(data.fetch);
    allSpellRowsStore.set(spellResults.SpellRows);
    allSpellsFetched = true;
  }

  function componentsToString(components: Component[]) {
    if (components && components.length > 0) {
      if (components.length > 1) {
        return components.map(x => x.Abbr).join(", ");
      } else {
        return components.map(x => x.Abbr)[0];
      }
    }
    return "";
  }
</script>

<h1>Pathfinder Spell Database</h1>
<p>A database of all the spells in Pathfinder 1E.</p>
<h2>Search by name: </h2><input value={$name} on:input={debounceName} />

<h2>Search by school(s):</h2>
<div>
  <SchoolSearch bind:searchBySchools={searchBySchools} />
</div>

<h2>Search by class(es):</h2>
<div>
  <CheckboxList checkboxNames={data.classes.map(cc => cc.Name)} bind:selectedCheckboxNames={searchByClasses} />
</div>

<button on:click={fetchAllSpells}>Pre-load all spells</button>

{#if $allSpellRowsStore}
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
            <td>{spell.CastingTime}</td>
            <td>{componentsToString(spell.Components)}</td>
            <td>
              {classListToString(spell.ClassSpellLevels)}
            </td>
          </tr>
        {/each}
    </tbody>
  </table>

  {#if !allSpellsFetched}
  <div use:inview={{}} on:change={loadMore} />
  {/if}
{/if}

<style>
  p {
    text-align: justify;
  }

  thead {
    font-size: x-large;
    background-color: #dab171;
  }

  tbody{
    font-size: small;
  }

  table, thead, td {
    border: 1px solid;
    border-collapse: collapse;
    padding: 0.25rem;
  }

  tr:nth-child(even) {
    background-color: #f5deb3;
  }

  table {
    width: 100%;
  }
</style>