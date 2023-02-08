<script lang="ts">
  import { capitalizeFirstLetter } from "../Shared";


  export let school : string;
  export let isChecked = false;

  let label = school + "-cb";
</script>

<input id="${label}" type="checkbox" bind:checked={isChecked} />
<label for="${label}">{capitalizeFirstLetter(school)}</label>