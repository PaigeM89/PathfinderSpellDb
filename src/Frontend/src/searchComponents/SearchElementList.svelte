<script lang="ts">
  import { DefaultSearch, type Search } from "./Search";
  import SearchElement from "./SearchElement.svelte";

  let selectedSearchType : string[] = [];
  let selectedSearchOptions : string[][] = [];

  export let Searches : Search[] = [];

  function addSearchField() {
    Searches = [ DefaultSearch(), ...Searches];
  }

</script>

<h2>Searching on...</h2>

{#each Searches as search}
  <SearchElement id={search.Id} classes={[]} />
{/each}

<button on:click={() => addSearchField()}>Add Search Field</button>