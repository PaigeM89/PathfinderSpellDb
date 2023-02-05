<script lang="ts">
  import type { PageData } from './$types';
  import { capitalizeFirstLetter } from "../../../Shared";
  import type { Spell } from "./+page";

  export let data: PageData;
  let spell = data.spell;

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