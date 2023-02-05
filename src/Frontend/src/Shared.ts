export function capitalizeFirstLetter(str : string) {
  if (str === "") {
    return str;
  }

  const f = str.charAt(0);
  const rest = str.slice(1);

  return f.toUpperCase() + rest;
}