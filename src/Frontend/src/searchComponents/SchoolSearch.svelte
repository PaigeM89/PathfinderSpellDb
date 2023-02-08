<script lang="ts">
  import SchoolSearchComponent from "./SchoolSearchComponent.svelte";

  interface School {
    Name : string,
    Selected : boolean
  }

  const schools : School[] = [
    { Name: "Abjuration", Selected: false },
    { Name: "Conjuration", Selected: false },
    { Name: "Divination", Selected: false },
    { Name: "Enchantment", Selected: false },
    { Name: "Evocation", Selected: false },
    { Name: "Illusion", Selected: false },
    { Name: "Necromancy", Selected: false },
    { Name: "Transmutation", Selected: false },
    { Name: "Universal", Selected: false }
  ]

  export let searchBySchools : string[] = [];
  $: searchBySchools =
    schools.filter(school => school.Selected).map(school => school.Name);
</script>

{#each schools as school}
  <SchoolSearchComponent school={school.Name} bind:isChecked={school.Selected} />
{/each}
