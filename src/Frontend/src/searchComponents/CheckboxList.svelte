<script lang="ts">
  import CheckboxInput from "./CheckboxInput.svelte";

  interface Checkbox {
    Name : string
    Label : string
    Selected : boolean
  }

  export let namesAndLabels : [string, string][] = [];

  const checkboxes = namesAndLabels.map<Checkbox>( nameAndLabel => {
    return {
      Name: nameAndLabel[0],
      Label: nameAndLabel[1],
      Selected: false
    }
  });

  export let selectedCheckboxNames : string[] = [];
  $: selectedCheckboxNames =
    checkboxes.filter(cb => cb.Selected).map(cb => cb.Name);
</script>

{#each checkboxes as checkbox}
  <div class="checkbox">
    <CheckboxInput checkboxName={checkbox.Name} checkboxLabel={checkbox.Label} bind:isChecked={checkbox.Selected} />
  </div>
{/each}


<style>
  .checkbox {
    border: 1px;
  }
</style>