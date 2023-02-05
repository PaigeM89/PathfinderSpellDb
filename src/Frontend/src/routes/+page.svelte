<script lang="ts">
  import { ApolloClient, InMemoryCache, gql } from "@apollo/client/core";
  import { setClient, query } from "svelte-apollo";
  import type { ReadableQuery } from "svelte-apollo";

  let name : string = "";

  const headers = [
    "Name", "School", "Description"
  ];

  interface Spell {
    id : number
    name : string
    school : string
    description : string
  }

  const client = new ApolloClient({
    uri: 'http://localhost:5000/',
    cache: new InMemoryCache(),
  });
  setClient(client);

  const SPELLS =
    gql`
      query SearchSpellsByName($name: String!) {
        spells(name: $name) {
          id
          name
          school
          description
        }
      }
    `;

  type SpellsResult = {
    spells: Spell[]
    spell: Spell
  }

  const spells : ReadableQuery<SpellsResult> = query(SPELLS, {
    variables: { name: name }
  });

  $: spells.refetch({ name: name });

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

  function capitalizeFirstLetter(str : string) {
    if (str === "") {
      return str;
    }

    const f = str.charAt(0);
    const rest = str.slice(1);

    return f.toUpperCase() + rest;
  }

</script>

<h1>Welcome to SvelteKit</h1>
<p>Visit <a href="https://kit.svelte.dev">kit.svelte.dev</a> to read the documentation</p>

<h2>Search by name: </h2><input value={name} on:input={debounce} />

{#if $spells.loading}
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