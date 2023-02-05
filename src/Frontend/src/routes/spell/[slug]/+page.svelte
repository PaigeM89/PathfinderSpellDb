<script lang="ts">
  import { ApolloClient, InMemoryCache, gql } from "@apollo/client/core";
  import { setClient, query } from "svelte-apollo";
  import type { ReadableQuery } from "svelte-apollo";

  import type { PageData } from './$types';

  export let data: PageData;

  console.log('data id', data.id);

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
      query SearchSpellsById($id: Int!) {
        spell(id: $id) {
          name
          school
          subschool
          descriptors
          description
        }
      }
    `;

  type SpellsResult = {
    spell: Spell
  }

  const spell : ReadableQuery<SpellsResult> = query(SPELLS, {
    variables: { id: data.id }
  });
</script>

{#if $spell.loading}
  <h2>Loading Spell...</h2>
{:else if $spell.error}
  <h2>Error loading Spells</h2>
  <p>{$spell.error.message}</p>
{:else}
  {#if $spell.data}
    {#if $spell.data.spell}
      <h1>{$spell.data.spell.name}</h1>
    {/if}
  {/if}
{/if}