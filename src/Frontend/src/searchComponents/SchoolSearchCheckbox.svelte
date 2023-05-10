<script lang="ts">
  import { capitalizeFirstLetter } from "../Shared";
  import { createLocalStorageWritableStore } from "../Stores";


  export let school : string;
  let label = school + "-cb";
  let checked = createLocalStorageWritableStore(label, false);
  export let isChecked = false;
  $: isChecked = $checked;
</script>

<input id="${label}" type="checkbox" bind:checked={$checked} />
<label for="${label}">{capitalizeFirstLetter(school)}</label>
