<script lang="ts">
  import type { PageData } from './$types';
  import { schoolStr } from '../SpellHelpers';
    import Spell from '../Spell.svelte';

  export let data: PageData;
  let spell = data.spell;
</script>

<Spell spell={spell} />

<!-- {#if spell}
  <h1>{spell.Name}</h1>
  <h2>{schoolStr(spell)}</h2>
  <p>{@html spell.Description}</p>
{/if}

<a href="/">Back to spell list</a> -->