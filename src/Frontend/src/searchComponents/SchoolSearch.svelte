<script lang="ts">
  const schools = [
    "Abjuration", "Conjuration", "Divination", "Enchantment", "Evocation", "Illusion", "Necromancy", "Transmutation", "Universal", "Other (See Spell Text)"
  ];

  let abjuration = false;
  let conjuration = false;
  let divination = false;
  let enchantment = false;
  let evocation = false;
  let illusion = false;
  let necromancy = false;
  let transmuation = false;
  let universal = false;
  // exactly one spell has "other" as a school
  // let other = false;


  export let searchBySchools : string[] = [];
  function addSearchSchool(school : string) {
    searchBySchools = [school, ...searchBySchools];
  }
  function removeSearchSchool(school : string) {
    searchBySchools = searchBySchools.filter(s => s !== school);
  }
  function updateSchool(school : string, flag : boolean) {
    console.log(school, flag);
    if (flag) {
      addSearchSchool(school);
    } else {
      removeSearchSchool(school);
    }
  }
  $: updateSchool("Abjuration", abjuration);
  $: updateSchool("Conjuration", conjuration);
  $: updateSchool("Divination", divination);
  $: updateSchool("enchantment", enchantment);
  $: updateSchool("evocation", evocation);
  $: updateSchool("illusion", illusion);
  $: updateSchool("necromancy", necromancy);
  $: updateSchool("transmuation", transmuation);
  $: updateSchool("universal", universal);
  // $: updateSchool("other", other);

</script>

<input id="abjuration-cb" type="checkbox" bind:checked={abjuration} />
<label for="abjuration-cb">Abjuration</label>

<input id="conjuration-cb" type="checkbox" bind:checked={conjuration} />
<label for="conjuration-cb">Conjuration</label>

<input id="divination-cb" type="checkbox" bind:checked={divination} />
<label for="divination-cb">Divination</label>

<input id="enchantment-cb" type="checkbox" bind:checked={enchantment} />
<label for="enchantment-cb">Enchantment</label>

<input id="evocation-cb" type="checkbox" bind:checked={evocation} />
<label for="evocation-cb">Evocation</label>

<input id="illusion-cb" type="checkbox" bind:checked={illusion} />
<label for="illusion-cb">Illusion</label>

<input id="necromancy-cb" type="checkbox" bind:checked={necromancy} />
<label for="necromancy-cb">Necromancy</label>

<input id="transmuation-cb" type="checkbox" bind:checked={transmuation} />
<label for="transmuation-cb">Transmuation</label>

<input id="universal-cb" type="checkbox" bind:checked={universal} />
<label for="universal-cb">Universal</label>

<!-- <input id="other-cb" type="checkbox" bind:checked={other} />
<label for="other-cb">Other (See text)</label> -->
