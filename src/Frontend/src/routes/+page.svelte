<script lang="ts">
  import { ApolloClient, InMemoryCache, gql } from "@apollo/client/core";
  import { setClient, query } from "svelte-apollo";
  import type { ReadableQuery } from "svelte-apollo";

  let name : string = "";

  const headers = [
    "Name", "School", "Description"
  ];

  interface Spell {
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
      query SearchSpellsByName($spellName: String!) {
        spells(name: $spellName) {
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
    variables: { spellName: name }
  });

  $: spells.refetch({ spellName: name });
  $: console.log('name', name);

  // wow typescript is dumb.
  let timer: string | number | NodeJS.Timeout | undefined;
  const debounce = (e : Event) => {
    clearTimeout(timer);
    timer = setTimeout( () => {
      console.log(e);
      if (e && e.target && e.target instanceof HTMLInputElement ) {
        name = e.target.value;
      }
    }, 500);
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
    {#if $spells.data.spell}
      <h1>{$spells.data.spell.name}</h1>
    {/if}
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
              <td>{spell.name}</td>
              <td>{spell.school}</td>
              <td>{@html spell.description}</td>
            </tr>
          {/each}
      </tbody>
    </table>
    {/if}
  {/if}
{/if}