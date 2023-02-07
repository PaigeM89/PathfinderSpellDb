export function capitalizeFirstLetter(str : string) {
  if (str === undefined || str === null) return "";
  if (str === "" || str.length === 1) return str;
  

  const f = str.charAt(0);
  const rest = str.slice(1);

  return f.toUpperCase() + rest;
}

export interface ClassSpellLevel {
  ClassName : string
  Level : number
}

function fixSummonerUnchained(str : string) {
  if (str === "SummonerUnchained") {
    return "Summoner (Unchained)";
  }
  return str;
}

export function classListToString(csls : ClassSpellLevel []) {
  if (csls && csls.length > 1) {
    return csls.map(csl => `${fixSummonerUnchained(csl.ClassName)} ${csl.Level}`).join(", ")
  } else if (csls && csls.length === 1) {
    return csls.map(csl => `${fixSummonerUnchained(csl.ClassName)} ${csl.Level}`)[0];
  }
  return "";
}

export const baseUrl = "http://localhost:5000";

export function getJson(fetch: (arg0: string) => Promise<any>, route: string) {
  return fetch(baseUrl + route)
    .then(response => response.body)
    .then(data => {
      const reader = data.getReader();
      return new ReadableStream({
        start(controller) {
          return pump();
          function pump() {
            return reader.read().then(({ done, value }): any => {
              // When no more data needs to be consumed, close the stream
              if (done) {
                controller.close();
                return;
              }
              // Enqueue the next data chunk into our target stream
              controller.enqueue(value);
              return pump();
            });
          }
        }
      });
    })
    .then(stream => new Response(stream))
    .then(response => response.blob())
    .then(blob => blob.text())
    .then(text => JSON.parse(text))
    .catch(error => {
      console.log(error);
      return {};
    })
}