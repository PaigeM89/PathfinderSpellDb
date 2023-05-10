<script lang="ts">
  import type { Writable } from "svelte/store";
  import CheckboxList from "../searchComponents/CheckboxList.svelte";
  import SchoolSearch from "../searchComponents/SchoolSearch.svelte";
  import { capitalizeFirstLetter, classListToString, fixSummonerUnchained } from "../Shared";
  import { allSpellRowsStore, createLocalStorageWritableStore } from "../Stores";
  import type { Component, SpellRow } from "../Types";
  import type { PageData } from "./$types";
  import { fetchAllSpellRows } from "./SpellRows";
  import SearchElement from "../searchComponents/SearchElement.svelte";

  export let data: PageData;

  let selected : string = "";
  let searchOptionsSelected : string[] = [];

  let ranges = [ "Personal", "Touch", "Close", "Medium", "Long", "Unlimited", "Other" ];

  let lastLoadedSpellsStore : Writable<number> = createLocalStorageWritableStore("lastLoadedSpells", Date.now());

  function LastLoadedWithinOneHour(lastLoaded: number) {
    const now = Date.now();
    const diff = now - lastLoaded;
    return diff < 3600000;
  }

  if ((!$allSpellRowsStore || $allSpellRowsStore.length === 0)) { // && !LastLoadedWithinOneHour($lastLoadedSpellsStore)) {
    fetchAllSpellRows(data.fetch)
      .then(rows => {
        allSpellRowsStore.set(rows);
        lastLoadedSpellsStore.set(Date.now());
      });
  }

  let name : Writable<string> = createLocalStorageWritableStore("searchName", "");
  let searchBySchools : string[] = [];
  let searchByClasses : string[] = [];
  let searchByRanges: string[] = [];
  let wasSearch = false;

  const headers = [
    "Name", "School", "Description", "Casting Time", "Components", "Range", "Duration", "Level", "Source"
  ];

  function filterSpells(spells : SpellRow[], name : string, schools : string[], classes: string[], ranges: string[]) : SpellRow[] {    
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

    let searchRanges = false;
    if (searchByRanges && searchByRanges.length > 0) {
      searchRanges = true;
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

          let rangePassFilter = true;
          if (searchRanges) {
            rangePassFilter = ranges.map(x => x.toLocaleLowerCase()).includes(spell.Range.toLocaleLowerCase());
          }

          return namePassFilter && schoolPassFilter && classesPassFilter && rangePassFilter;
        });
    
    return filteredSpells;
  }

  $: filteredSpells = filterSpells($allSpellRowsStore, $name, searchBySchools, searchByClasses, searchByRanges);

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

<div class="searchElement">
  <h2>Search by name: </h2>
  <input value={$name} on:input={debounceName} />
</div>



<div class="searchElements">
  <div class="searchElement">
    <SearchElement bind:selected bind:searchOptionsSelected={searchBySchools} classes={data.classes.map(cc => fixSummonerUnchained(cc.Name))} />
    <!-- <h2>Search by schools:</h2>
    <div>
      <SchoolSearch bind:searchBySchools={searchBySchools} />
    </div> -->
  </div>

  <div class="searchElement">
    <h2>Search by classes:</h2>
    <div class="checkboxes">
      <CheckboxList namesAndLabels={data.classes.map(cc => [cc.Name, fixSummonerUnchained(cc.Name)])} bind:selectedCheckboxNames={searchByClasses} />
    </div>
  </div>

  <div class="searchElement">
    <h2>Search by ranges:</h2>
    <div class="checkboxes">
      <CheckboxList namesAndLabels={ranges.map(x => [x, x])} bind:selectedCheckboxNames={searchByRanges} />
    </div>
  </div>
</div>
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
            <td>{spell.Range}</td>
            <td>{spell.Duration}</td>
            <td>
              {classListToString(spell.ClassSpellLevels, searchByClasses)}
            </td>
            <td>{spell.Source}</td>
          </tr>
        {/each}
    </tbody>
  </table>
{/if}

<style>
  p {
    text-align: center;
  }

  h1 {
    text-align: center;
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

  .checkboxes {
    border: 1px;
  }

  .searchElements {
    display: flex;
  }

  .searchElement {
    padding: 1rem;
    margin-left: auto;
    margin-right: auto;
  }

</style>