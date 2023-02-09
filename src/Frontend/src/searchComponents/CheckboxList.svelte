<script lang="ts">
    import CheckboxInput from "./CheckboxInput.svelte";
  import SchoolSearchCheckbox from "./SchoolSearchCheckbox.svelte";

  interface Checkbox {
    Name : string
    Selected : boolean
  }

  export let checkboxNames : string[] = [];

  const checkboxes : Checkbox[] = checkboxNames.map<Checkbox>(name => {
      return {
        Name: name,
        Selected: false
      }
    });

  export let selectedCheckboxNames : string[] = [];
  $: selectedCheckboxNames =
    checkboxes.filter(cb => cb.Selected).map(cb => cb.Name);
</script>

{#each checkboxes as checkbox}
  <CheckboxInput checkboxName={checkbox.Name} bind:isChecked={checkbox.Selected} />
{/each}
