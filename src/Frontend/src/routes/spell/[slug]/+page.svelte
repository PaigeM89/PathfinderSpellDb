<script lang="ts">
  import { ApolloClient, InMemoryCache, gql } from "@apollo/client/core";
  import { setClient, query } from "svelte-apollo";
  import type { ReadableQuery } from "svelte-apollo";

  import type { PageData } from './$types';
  import { capitalizeFirstLetter } from "../../../Shared";
  import type { Spell } from "./+page";

  export let data: PageData;
  let spell = data.spell;
  // interface Spell {
  //   name : string
  //   school : string
  //   subschool : string
  //   descriptors: string []
  //   fullDescription : string
  // }

  // const client = new ApolloClient({
  //   uri: 'http://localhost:5000/',
  //   cache: new InMemoryCache(),
  // });
  // setClient(client);

  // const SPELLS =
  //   gql`
  //     query SearchSpellsById($id: Int!) {
  //       spell(id: $id) {
  //         name
  //         school
  //         subschool
  //         descriptors
  //         fullDescription
  //       }
  //     }
  //   `;

  // type SpellsResult = {
  //   spell: Spell
  // }

  // const spell : ReadableQuery<SpellsResult> = query(SPELLS, {
  //   variables: { id: data.id }
  // });

  function schoolStr(spell : Spell) {
    var subschool = "";
    if (spell.Subschool) {
      subschool = ` (${spell.Subschool}) `;
    }
    var desc = "";
    if (spell.Descriptors && spell.Descriptors.length > 0 && spell.Descriptors[0] !== "") {
      let joined = spell.Descriptors.join(", ");
      desc = ` [${joined}]`;
    }
    const school = capitalizeFirstLetter(spell.School);
    return `${school}${subschool}${desc}`;
  }
</script>

{#if spell}
  <h1>{spell.Name}</h1>
  <h2>{schoolStr(spell)}</h2>
  <p>{@html spell.Description}</p>
{/if}

<a href="/">Back to spell list</a>