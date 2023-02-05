<script lang="ts">
  import SchoolSearch from "../searchComponents/SchoolSearch.svelte";
  import { baseUrl, capitalizeFirstLetter, getJson } from "../Shared";
  import type { PageData } from "./$types";
  import type { Spell } from "./+page";

  export let data: PageData;
  let spells = data.spells;

  let name : string = "";
  let searchBySchools : string[] = [];
  let wasSearch = false;

  const headers = [
    "Name", "School", "Description", "Level"
  ];

  async function search(name : string) {
    if (name.trim() === "" && wasSearch === true) {
      const json = await getJson(data.fetch, "/spells");
      spells = json;
      wasSearch = false;
      console.log('resetting search');
      return;
    } else if (name.trim() !== "") {
      console.log('search', name);

      const payload = {
        Name : name,
        School : ""
      }

      const res = await fetch(`${baseUrl}/spells`, {
        method: 'POST',
        body: JSON.stringify(payload)
      });

      const json = await res.json();
      spells = json;
      wasSearch = true;
    }
  }

  $: filteredSpells =
    spells.filter(spell => {
      if (searchBySchools && searchBySchools.length > 0) {
        if (searchBySchools.find(school => school.toLocaleLowerCase() === spell.School.toLocaleLowerCase())) {
          return true;
        }
        return false;
      }
      return true;
    });

  $: search(name);

  // wow typescript is dumb.
  let timer: string | number | NodeJS.Timeout | undefined;
  const debounce = (e : Event) => {
    clearTimeout(timer);
    timer = setTimeout( () => {
      if (e && e.target && e.target instanceof HTMLInputElement ) {
        name = e.target.value;
      }
    }, 500);
  }

  function classListToString(spell : Spell) {
    if (spell.ClassSpellLevels && spell.ClassSpellLevels.length > 1) {
      return spell.ClassSpellLevels.map(csl => `${csl.ClassName} ${csl.Level}`).join(", ")
    } else if (spell.ClassSpellLevels && spell.ClassSpellLevels.length === 1) {
      return spell.ClassSpellLevels.map(csl => `${csl.ClassName} ${csl.Level}`)[0];
    }
    return "";
  }

</script>

<h1>Pathfinder Spell Database</h1>
<p>A database of all the spells in Pathfinder 1E.</p>
<h2>Search by name: </h2><input value={name} on:input={debounce} />

<h2>Search by school(s):</h2>
<div>
  <SchoolSearch bind:searchBySchools={searchBySchools} />
</div>


{#if spells}
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
              <a href="/spell/{spell.Id}">{spell.Name}</a>
            </td>
            <td>{capitalizeFirstLetter(spell.School)}</td>
            <td>{@html spell.ShortDescription}</td>
            <td>
              {classListToString(spell)}
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