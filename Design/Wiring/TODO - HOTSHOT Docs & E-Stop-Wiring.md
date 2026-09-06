---
subsystem: wiring
tags: [wiring, electrical, safety, todo, ambrell, hotshot]
related: [[Design/Wiring/Electrical System.md]], [[Design/Wiring/NI-DAQ Control Architecture.md]], [[Design/Wiring/Ball Screw Motor Control.md]]
status: open
---

# TODO — HOTSHOT Documentation, End Stops, and Wiring Layout

Action list to close the open items from the 08-28-2026 e-stop / 0-10V control discussion.

## 1. Get nameplate photos → send to Ambrell support

**Resolved (2026-09-05)** — the full HOTSHOT manual was located and transcribed (see [[Design/Wiring/HOTSHOT Manual - Full Transcription.md]]), answering the interlock and 0-10V questions this outreach was meant to resolve. The Ambrell outreach draft is no longer needed and has been removed; nameplate photos are no longer required unless a future compliance review needs mains-disconnect confirmation directly from Ambrell.

- [ ] Photograph HOTSHOT main nameplate (SN 103927) — model, voltage/phase, frequency range, max output power, any date codes
- [ ] Photograph HOTSHOT rear panel in full — capture **all** connectors, not just mains input (remote/interlock connector, any DB9/terminal blocks, labels/silkscreen text)
- [ ] Close-up photo of any labeled pins/pinout printed near the remote connector, if present
- [ ] Photograph the 301-0243D handheld remote — nameplate/label + cable connector end
- [ ] Save photos into `media/` in this vault (descriptive filenames, e.g. `hotshot-103927-nameplate.jpg`, `hotshot-rear-panel.jpg`)
- [ ] Embed photos in [[Design/Wiring/Electrical System.md]] for permanent reference

## 2. Figure out if end stops (limit switches) are needed

Context: [[Design/Wiring/Ball Screw Motor Control.md]] currently documents **one** limit switch, used for homing at the lowest point of travel (ST-PMC1 Input #1). It does not yet document a switch at the **top/opposite end** of travel.

- [ ] Decide: is the homing switch alone sufficient, or is a second limit switch needed at the top of travel to prevent over-travel/crashing the ball screw or sample into the coil/chamber top?
- [ ] Check ST-PMC1 input count (6 optically-isolated inputs available) — confirm at least one more is free for a second (end-of-travel) limit switch
- [ ] If needed: select/mount a second N.O. limit switch at the upper travel limit, wire to a free ST-PMC1 input, and mirror the wiring pattern already used for Input #1 (see [[Design/Wiring/Ball Screw Motor Control.md]] "Wiring: Limit Switch to ST-PMC1" section)
- [ ] Confirm software/motion-sequence behavior on end-stop trip (hard stop vs. controlled deceleration) — same question as the existing homing switch, applied to the new one
- [ ] Update [[Design/Wiring/Ball Screw Motor Control.md]] with the finalized limit-switch count/placement once decided

## 3. Wiring layout

- [x] Interlock connector pinout confirmed from manual (2026-09-05) — CTB1:15-16 for E-stop (N.C., 3Adc min), CTB1:1-2 for 0-10V analog input (Zin = 21kΩ). See [[Design/Wiring/Electrical System.md]] for the full pinout table.
- [ ] Decide interlock strategy: e-stop wired into (a) HOTSHOT CTB1:15-16, (b) mains contactor in the 240V feed, or both (recommended, since CTB1:15-16 is confirmed control-rail-only, not a mains disconnect — see [[Design/Wiring/Electrical System.md]])
- [x] NI-9263 → HOTSHOT wiring can now be finalized: CTB1:1 (+) / CTB1:2 (−), 0-10Vdc, Zin = 21kΩ — well above NI-9263 output impedance, so no loading concerns. Shielding requirements still per [[Design/Wiring/NI-DAQ Control Architecture.md]] cable notes.
- [ ] Draw/document a single consolidated wiring diagram covering: 240V mains → contactor → HOTSHOT; e-stop loop (button → CTB1:15-16 and/or contactor coil); NI-9263 0-10V control line → CTB1:1-2; ball screw limit switch(es) → ST-PMC1 inputs
- [ ] Add the consolidated diagram to [[Design/Wiring/Electrical System.md]] or [[Design/Wiring/INDEX.md]] once drafted

## Status
Nameplate photos (item 1) and end-stop decision (item 2) still open. Item 3's Ambrell-dependent blockers are resolved — interlock pinout and 0-10V interface are confirmed from the full manual (2026-09-05); remaining item-3 work is just drawing the consolidated diagram and deciding the interlock strategy (CTB1 only vs. CTB1 + mains contactor).
