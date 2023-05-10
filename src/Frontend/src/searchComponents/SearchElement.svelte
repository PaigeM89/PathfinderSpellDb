<script lang="ts">
  import { SearchTarget, Schools } from "./Search";
  import MultiSelect from "svelte-multiselect";
  //import { Option, ObjectOption, DispatchEvents, MultiSelectEvents } from "svelte-multiselect";

  let searchTargets = [
    SearchTarget.School,
    SearchTarget.Class,
    SearchTarget.CastingTime,
    SearchTarget.Components,
    SearchTarget.Range,
    SearchTarget.Duration,
    SearchTarget.Level,
    SearchTarget.Source
  ];
  export let id:string;
  export let selected = "School";
  export let searchOptionsSelected: string[] = [];
  export let classes : string[];

  function handleAdd(event: { detail: { option: string; }; }) {
    searchOptionsSelected = [event.detail.option, ...searchOptionsSelected];
  }

  function handleRemove(event: { detail: { option: string; }; }) {
    searchOptionsSelected = searchOptionsSelected.filter(x => x !== event.detail.option);
  }

</script>

<div id={id}>
  <select bind:value={selected} >
    {#each searchTargets as st}
      <option value={st}>{st}</option>
    {/each}
  </select>

  {#if selected === SearchTarget.School}
  <MultiSelect 
    placeholder="Select schools..."
    options={Schools}
    on:add={(event) => handleAdd(event)}
    on:remove={(event) => handleRemove(event)}
    />
  {:else if selected === SearchTarget.Class}
  <MultiSelect 
    placeholder="Select classes..."
    options={classes}
    on:add={(event) => handleAdd(event)}
    on:remove={(event) => handleRemove(event)}
    />
  {/if}
</div>

<style>
  div {
    margin: auto;
  }
</style>