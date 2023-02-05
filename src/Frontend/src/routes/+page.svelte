<script lang="ts">
  import { ApolloClient, InMemoryCache, gql } from "@apollo/client/core";
  import { setClient, query } from "svelte-apollo";
  import type { ReadableQuery } from "svelte-apollo";
    import { each } from "svelte/internal";

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

  const SPELLS_QUERY =
    gql`
      query SpellsList {
        spells {
          name
          school
          description
        }
      }
    `;

  type SpellsResult = {
    spells: Spell[]
  }

  const spells : ReadableQuery<SpellsResult> = query(SPELLS_QUERY);
</script>

<h1>Welcome to SvelteKit</h1>
<p>Visit <a href="https://kit.svelte.dev">kit.svelte.dev</a> to read the documentation</p>

{#if $spells.loading}
  <h2>Loading Spells...</h2>
{:else if $spells.error}
  <h2>Error loading Spells</h2>
  <p>{$spells.error.message}</p>
{:else}
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
      {#if $spells.data}
        {#each $spells.data.spells as spell}
          <tr>
            <td>{spell.name}</td>
            <td>{spell.school}</td>
            <td>{@html spell.description}</td>
          </tr>
        {/each}
      {/if}
    </tbody>
  </table>
{/if}