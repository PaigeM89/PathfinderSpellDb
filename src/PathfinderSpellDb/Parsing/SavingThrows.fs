namespace PathfinderSpellDb.Parsing

open System
open PathfinderSpellDb.Parsing
open PathfinderSpellDb.Parsing.Types

module SavingThrows =

(*
  ["Will negates"; "Will negates (harmless)"; ""; "none"; "see text";
 "Fortitude negates (object)"; "Fortitude negates (harmless)";
 "Fortitude negates (see text)"; "Fortitude negates"; "Reflex negates; see text";
 "Reflex partial (see text)"; "Reflex negates (see text)"; "Reflex partial";
 "Reflex negates"; "Reflex negates and Fortitude negates (see text)"; "no";
 "Will negates (harmless, object)"; "Will negates (object)";
 "Will negates (see text)"; "none or Will negates; see text"; "none (see text)";
 "none; see text"; "Will partial"; "Fortitude partial (see text)";
 "Will disbelief or Will negates (see text)"; "Will partial, see text";
 "Will partial (see text)"; "Will disbelief"; "none (object)";
 "Will half; see text"; "Fortitude negates, Will partial, see text";
 "Will disbelief, then Fortitude negates"; "none and Will partial (see text)";
 "Reflex half and Will negates (see text)"; "Will negates (object); see text";
 "Will partial; see text"; "Will (harmless)"; "yes (harmless)";
 "Will negates; see text"; "Reflex half or Reflex negates; see text";
 "Reflex half (see below)";
 "Fortitude negates (harmless) or Will disbelieves (if interacted with)";
 "none (harmless)"; "Fortitude half; see text"; "Reflex partial, see text";
 "Fortitude negates or Reflex half (see text)";
 "Fortitude partial or negates (see text)";
 "Will negates (harmless) or Will half, see text";
 "Will negates (harmless) or Will negates (object)"; "Reflex half";
 "Reflex negates (object, see text)"; "Reflex negates or partial (see text)";
 "Fortitude half (object)"; "Fortitude half (see text)"; "Fortitude partial";
 "Reflex for half; see text"; "Will negates or none; see text"; "Fortitude half";
 "Reflex half or negates; see text"; "Reflex half (see text)";
 "Will negates (harmless) or none";
 "Fortitude partial or Will negates; see text"; "Fortitude negates; see text";
 "Fortitude partial; see text"; "none (see below)"; "Will disbelief; see text";
 "Will half (harmless)"; "Will half (harmless); see text";
 "Will half (harmless) or Will half; see text"; "none (see curse text)";
 "Fortitude negates (object), then Will negates (see text)"; "none, see text";
 "Will half"; "Reflex half, see text"; "Fortitude negates (harmless, object)";
 "none (object) or Will disbelief (if interacted with)";
 "None (harmless, object)"; "none and Will negates (object)";
 "Fortitude partial (object)"; "Will negates (harmless, object); see text";
 "Will disbelief, then Will partial (see text)"; "Reflex negates (object)";
 "Fortitude negates (fatigue only)"; "Will disbelief (if interacted with)";
 "Reflex half and Will partial; see text"; "Reflex partial; see text";
 "Reflex half; see text"; "None"; "Fort negates";
 "Fortitude partial; see text for enervation"; "Fortitude negates (see below)";
 "Will negates, see text"; "Reflex partial or Reflex negates (see text)";
 "none and Will partial (see below)"; "special; see text";
 "Will negates or Will disbelief (see text)";
 "Fortitude negates, then Reflex half (see text)";
 "none or Will negates (harmless)"; "none or Reflex half; see text";
 "Will negates (harmless), but see below"; ...]
*)

(*
  how correct do i need to be RIGHT NOW? Can I just reduce this to fort/reflex/will/none?
  then come back and do this more correct later?
*)

  let parseSavingThrow (str : string) =
    if str = "" || str = "none" then
      SavingThrow.None []
    else SavingThrow.None []
