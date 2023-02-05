<script lang="ts">
  import { ApolloClient, InMemoryCache, gql } from "@apollo/client/core";
  import { setClient, query } from "svelte-apollo";
  import type { ReadableQuery } from "svelte-apollo";
  import { capitalizeFirstLetter } from "../Shared";
  import type { PageData } from "./$types";

  export let data: PageData;

  let name : string = "";

  const headers = [
    "Name", "School", "Description"
  ];

  // const client = new ApolloClient({
  //   uri: 'http://localhost:5000/',
  //   cache: new InMemoryCache(),
  // });
  // setClient(client);

  // const SPELLS =
  //   gql`
  //     query SearchSpellsByName($name: String!) {
  //       spells(name: $name) {
  //         id
  //         name
  //         school
  //         description
  //       }
  //     }
  //   `;

  // type SpellsResult = {
  //   spells: Spell[]
  //   spell: Spell
  // }

  // const spells : ReadableQuery<SpellsResult> = query(SPELLS, {
  //   variables: { name: name }
  // });

  // $: spells.refetch({ name: name });

  // // wow typescript is dumb.
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

{#if data.spells}
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
        {#each data.spells as spell}
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

<!-- {#if $spells.loading}
  <h2>Loading Spells...</h2>
{:else if $spells.error}
  <h2>Error loading Spells</h2>
  <p>{$spells.error.message}</p>
{:else}
  {#if $spells.data}
    {#if $spells.data.spells}
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
          {#each $spells.data.spells as spell}
            <tr>
              <td>
                <a href="/spell/{spell.id}">{spell.name}</a>
              </td>
              <td>{capitalizeFirstLetter(spell.school)}</td>
              <td>{@html spell.description}</td>
            </tr>
          {/each}
      </tbody>
    </table>
    {/if}
  {/if}
{/if} -->

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