<script lang="ts">
  import type { PageData } from './$types';
  import Spell from '../Spell.svelte';

  export let data: PageData;
  let spell = data.spell;
</script>

<Spell spell={spell} />
