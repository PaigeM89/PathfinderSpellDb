<script lang="ts">
  import { capitalizeFirstLetter } from "../Shared";
  import { createLocalStorageWritableStore } from "../Stores";


  export let checkboxName : string;
  export let checkboxLabel : string;

  let elementId = checkboxName + "-cb";
  let checked = createLocalStorageWritableStore(elementId, false);
  export let isChecked = false;
  $: isChecked = $checked;
</script>

<input id="${elementId}" type="checkbox" bind:checked={$checked} />
<label for="${elementId}">{capitalizeFirstLetter(checkboxLabel)}</label>