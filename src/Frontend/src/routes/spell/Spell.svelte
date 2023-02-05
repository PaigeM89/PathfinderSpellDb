<script lang="ts">
  import { schoolStr } from './SpellHelpers';
  import type { Spell } from './SpellHelpers';

  export let spell : Spell;
</script>

{#if spell}
  <h1>{spell.Name}</h1>
  <h2>{schoolStr(spell)}</h2>
  <p>{@html spell.Description}</p>
{/if}

<a href="/">Back to spell list</a>