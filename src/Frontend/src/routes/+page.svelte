<script lang="ts">
  import { ApolloClient, InMemoryCache, gql } from "@apollo/client/core";
  import { setClient, query } from "svelte-apollo";
  import type { ReadableQuery } from "svelte-apollo";

  let name : string = "fireball";

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

  const ALL_SPELLS =
    gql`
      query SpellsList {
        spells {
          name
          school
          description
        }
      }
    `;

  const SEARCH_SPELL_BY_NAME =
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

  const spells : ReadableQuery<SpellsResult> = query(SEARCH_SPELL_BY_NAME, {
    variables: { spellName: name }
  });

  $: spells.refetch({ name });
  $: console.log(name);


</script>

<h1>Welcome to SvelteKit</h1>
<p>Visit <a href="https://kit.svelte.dev">kit.svelte.dev</a> to read the documentation</p>

<h2>Search by name: </h2><input bind:value={name} />

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