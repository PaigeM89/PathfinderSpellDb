<script lang="ts">
  import { capitalizeFirstLetter } from "../Shared";
  import { createLocalStorageWritableStore } from "../Stores";


  export let checkboxName : string;
  let label = checkboxName + "-cb";
  let checked = createLocalStorageWritableStore(label, false);
  export let isChecked = false;
  $: isChecked = $checked;
</script>

<input id="${label}" type="checkbox" bind:checked={$checked} />
<label for="${label}">{capitalizeFirstLetter(checkboxName)}</label>