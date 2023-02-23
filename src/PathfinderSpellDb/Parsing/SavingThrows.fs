namespace PathfinderSpellDb.Parsing

open System
open PathfinderSpellDb.Parsing
open PathfinderSpellDb.Parsing.Types

module SavingThrows =

(*
"Reflex negates and Fortitude negates (see text)": 1
"Fortitude negates, Will partial, see text": 1
"Will disbelief, then Fortitude negates": 1
"none and Will partial (see text)": 1
"Fortitude negates (harmless) or Will disbelieves (if interacted with)": 1
"Fortitude half; see text": 1
"Fortitude negates or Reflex half (see text)": 1
"Fortitude partial or negates (see text)": 1
"Will negates (harmless) or Will half, see text": 1
"Reflex negates (object, see text)": 1
"Reflex negates or partial (see text)": 1
"Fortitude half (object)": 1
"Reflex for half; see text": 1
"Will negates or none; see text": 1
"Reflex half or negates; see text": 1
"Will negates (harmless) or none": 1
"Fortitude partial or Will negates; see text": 1
"none (see curse text)": 1
"Fortitude negates (object), then Will negates (see text)": 1
"Reflex half, see text": 1
"none (object) or Will disbelief (if interacted with)": 1
"None (harmless, object)": 1
"Fortitude partial (object)": 1
"Will negates (harmless, object); see text": 1
"Will disbelief, then Will partial (see text)": 1
"Fortitude negates (fatigue only)": 1
"Reflex half and Will partial; see text": 1
"Fortitude partial; see text for enervation": 1
"Fortitude negates (see below)": 1
"Reflex partial or Reflex negates (see text)": 1
"none and Will partial (see below)": 1
"Will negates or Will disbelief (see text)": 1
"Fortitude negates, then Reflex half (see text)": 1
"none or Reflex half; see text": 1
"Will negates (harmless), but see below": 1
"Will negates and Reflex negates; see text": 1
"Fortitude negates (object, harmless)": 1
"Reflex negates (object) and Fortitude negates": 1
"Fortitude negates (harmless, object); see text": 1
"none or Will disbelief (see text)": 1
"Reflex partial (see below)": 1
"none (see description)": 1
"Fortitude negate (harmless)": 1
"Will negates (blinding only)": 1
"Fortitude negates (object) or Reflex negates; see text": 1
"none and Will disbelief (see text)": 1
"Reflex negates; Reflex half; see text": 1
"Reflex half and see below": 1
"Will disbelieve (on hit; see below)": 1
"Will negates (and see below)": 1
"Will disbelief, then Fortitude (see text)": 1
"Will negates (harmless, object), see text": 1
"Will negates (harmless) or Will negates (harmless, object)": 1
"special (see text)": 1
"Fortitude (object)": 1
"Will negates (special, see below)": 1
"no, see below": 1
"Reflex negates (object; see text)": 1
"Will none; see text": 1
"Will half (harmless, see text)": 1
"Will negates, then Reflex partial; see text": 1
"Will partial, see below": 1
"none or Reflex half, see text": 1
"Fort negates (object)": 1
"Will disbelief (see below)": 1
"Fortitude negates and Reflex half, see text": 1
"none and Fortitude negates (see text)": 1
"Will negates, Fortitude negates, see text": 1
"Will disbelief, then Fortitude or Will negates (see text)": 1
"Will disbelief then Fortitude partial; see text": 1
"Will partial (harmless), then Fortitude negates (see text)": 1
"Will negates, then Will partial (see text)": 1
"Fortitude negates; see below": 1
"Fortitude negates (and Will special, see text)": 1
"Will negates or none": 1
"Will negates or Fortitude negates; see text": 1
"Will negates (harmless) and Will disbelief; see text": 1
"Will negates (object, harmless)": 1
"none (object; see text)": 1
"Will negates (object) and Will negates; see text": 1
"Will negates or Will disbelief (if interacted with)": 1
"Will disbelief (if interacted with), see text": 1
"Will negates (object); Will negates (object) or Fortitude half; see text": 1
"Will negates (harmless) and Will disbelief (if interacted with); see text": 1
"none (harmless); see text": 1
"Reflex partial and Fortitude negates (see  text)": 1
"Will negates; see text or none (object)": 1
"none (object), Fortitude negates; see text": 1
"Will negates or none (see text)": 1
"Reflex negates (harmless)": 1
"Will negates (object) or none (see text)": 1
"Fortitude half and Reflex (see description)": 1
"Fort. partial (see below)": 1
"Fortitude": 1
"Reflex halves (see text)": 1
"Reflex negates and Reflex half; see text": 1
"Will negates (harmless) and see text": 1
"Will half, see text": 1
"Will half (harmless) (see text)": 1
"Will partial (harmless)": 1
"Will negates (object) or none; see text": 1
"Will negates (object), see text": 1
"none and Reflex partial (see text)": 1
"none or Will negates (see text)": 1
"yes": 1
"Reflex negates, see text": 1
"none, and Will negates; see text": 1
"Reflex negates, Fortitude negates (see text)": 1
"none and Will negates (harmless)": 1
"none or Will negates (harmless, object)": 1
"Will negate (harmless)": 1
"Reflex half (special, see below)": 1
"Will disbelief or Will negates (see text)": 2
"Will half; see text": 2
"Reflex half and Will negates (see text)": 2
"yes (harmless)": 2
"Reflex half or Reflex negates; see text": 2
"Reflex half (see below)": 2
"Will negates (harmless) or Will negates (object)": 2
"Reflex negates (object)": 2
"special; see text": 2
"none (harmless, see text)": 2
"Will negates (harmless); see text": 2
"Reflex negates or none (see text)": 2
"Fortitude partial (see below)": 2
"special, see below": 2
"Will negates (harmless), see text": 2
"yes (object)": 2
"none or Will disbelief (if interacted with); see text": 2
"Will disbelief, then Fortitude partial (see text)": 2
"Will negates (see below)": 2
"Will disbelief (harmless)": 2
"Fortitude negates (object); see text": 2
"Will negates or Will negates (harmless); see text": 2
"none or Will negates (object)": 2
"Reflex halves": 2
"Will (harmless, object)": 2
"Fortitude partial or Reflex negates (object); see text": 2
"Will partial, see text": 3
"Will negates (object); see text": 3
"none (see below)": 3
"Fort negates": 3
"none or Will negates (harmless)": 3
"Fortitude partial, see text": 3
"none and Will negates (see text)": 3
"Will (harmless)": 4
"Reflex partial, see text": 4
"Fortitude half (see text)": 4
"Will disbelief; see text": 4
"Will half (harmless); see text": 4
"Will half (harmless) or Will half; see text": 4
"none, see text": 4
"Will negates, see text": 4
"Will disbelief, then Fortitude partial; see text": 4
"Will half (harmless)": 5
"Fort negates (harmless)": 5
"Will partial and Fortitude partial; see text": 5
"Reflex partial (see text)": 6
"none or Will negates; see text": 6
"Reflex half (see text)": 6
"Fortitude partial; see text": 6
"Fortitude negates (harmless, object)": 6
"none and Will negates (object)": 6
"Reflex partial; see text": 6
"None": 6
"Will disbelief (if interacted with); varies; see text": 6
"see below": 7
"Reflex negates (see text)": 8
"none (object)": 8
"Fortitude half": 8
"Reflex half; see text": 9
"Reflex negates; see text": 10
"Fortitude negates; see text": 10
"Fortitude negates (see text)": 11
"none (harmless)": 11
"Reflex partial": 12
"no": 12
"Will partial; see text": 12
"none (see text)": 13
"Will disbelief": 14
"Will partial (see text)": 18
"none; see text": 19
"Fortitude partial (see text)": 19
"Reflex negates": 20
"Will half": 20
"Will disbelief (if interacted with)": 22
"Fortitude negates (object)": 26
"Reflex half": 32
"Fortitude partial": 32
"Will negates (see text)": 37
"Will partial": 39
"Will negates; see text": 42
"Will negates (object)": 53
"Will negates (harmless, object)": 57
"see text": 61
"Fortitude negates (harmless)": 81
"Fortitude negates": 114
"Will negates (harmless)": 314
"Will negates": 368
"": 422
"none": 674
*)

(*
  how correct do i need to be RIGHT NOW? Can I just reduce this to fort/reflex/will/none?
  then come back and do this more correct later?
*)

  let parseSavingThrows (str : string) =
    let str = str.ToLowerInvariant()
    if str = "" || str = "none" then
      [SavingThrow.None []]
    else [
      if str.Contains("will") then yield SavingThrow.Will []
      if str.Contains("reflex") then yield SavingThrow.Reflex []
      if str.Contains("fort") then yield SavingThrow.Fortitude []
    ]
