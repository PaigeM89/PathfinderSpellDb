<script lang="ts">
  import { baseUrl, capitalizeFirstLetter, getJson } from "../Shared";
  import type { PageData } from "./$types";

  export let data: PageData;
  let spells = data.spells;

  let name : string = "";
  let wasSearch = false;

  const headers = [
    "Name", "School", "Description"
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
</script>

<h1>Pathfinder Spell Database</h1>
<p>A database of all the spells in Pathfinder 1E.</p>
<h2>Search by name: </h2><input value={name} on:input={debounce} />

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
        {#each spells as spell}
          <tr>
            <td>
              <a href="/spell/{spell.Id}">{spell.Name}</a>
            </td>
            <td>{capitalizeFirstLetter(spell.School)}</td>
            <td>{@html spell.ShortDescription}</td>
          </tr>
        {/each}
    </tbody>
  </table>
{/if}

<style>
  p {
    text-align: justify;
  }

  table, thead, td {
    border: 1px solid;
    border-collapse: collapse;
    padding: 15px;
  }

  table {
    width: 100%;
  }
</style>