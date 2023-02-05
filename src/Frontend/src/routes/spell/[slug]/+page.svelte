<script lang="ts">
  import { ApolloClient, InMemoryCache, gql } from "@apollo/client/core";
  import { setClient, query } from "svelte-apollo";
  import type { ReadableQuery } from "svelte-apollo";

  import type { PageData } from './$types';
    import { capitalizeFirstLetter } from "../../../Shared";

  export let data: PageData;

  console.log('data id', data.id);

  interface Spell {
    name : string
    school : string
    subschool : string
    descriptors: string []
    fullDescription : string
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
          fullDescription
        }
      }
    `;

  type SpellsResult = {
    spell: Spell
  }

  const spell : ReadableQuery<SpellsResult> = query(SPELLS, {
    variables: { id: data.id }
  });

  function schoolStr(spell : Spell) {
    var subschool = "";
    if (spell.subschool) {
      subschool = ` (${spell.subschool}) `;
    }
    var desc = "";
    if (spell.descriptors && spell.descriptors.length > 0 && spell.descriptors[0] !== "") {
      let joined = spell.descriptors.join(", ");
      desc = ` [${joined}]`;
    }
    const school = capitalizeFirstLetter(spell.school);
    return `${school}${subschool}${desc}`;
  }
</script>

{#if $spell.loading}
  <h2>Loading Spell...</h2>
{:else if $spell.error}
  <h2>Error loading Spell</h2>
  <p>{$spell.error.message}</p>
{:else}
  {#if $spell.data && $spell.data.spell}
    <h1>{$spell.data.spell.name}</h1>
    <h2>{schoolStr($spell.data.spell)}</h2>
    <p>{@html $spell.data.spell.fullDescription}</p>
  {/if}
{/if}
<a href="/">Back to spell list</a>