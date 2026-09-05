---
subsystem: wiring
tags: [wiring, electrical, correspondence, ambrell, hotshot]
related: [[Design/Wiring/Electrical System.md]], [[Design/Wiring/NI-DAQ Control Architecture.md]], [[Design/Wiring/TODO - HOTSHOT Docs & E-Stop-Wiring.md]], [[Design/Wiring/HOTSHOT Manual - Full Transcription.md]]
status: draft
---

# Ambrell Contact — HOTSHOT SN 103927

Draft outreach to Ambrell (successor to Ameritherm) requesting legacy documentation and interface specs for the induction power supply used in this project. Send from whalls420@gmail.com or update as needed before sending.

## Context
**Update (2026-09-05, part 2):** A sharp, straight-on photo of manual p.46 (§4.2 Electrical, Customer Input/Output table) resolved both remaining gaps:
- **0-10V remote input impedance confirmed**: Zin = 21kΩ (4-20mA alternative: Zin = 250Ω). See [[Design/Wiring/Electrical System.md]] for the full table.
- **E-stop confirmed as control-rail interlock only**: CTB1:15-16, N.C., rated 3Adc min, opened contact = STOP. Manual still never states it disconnects mains/chassis power — this is a real limitation of the manual, not a photo-quality issue, so if compliance requires "fully de-energized," a separate mains-side contactor driven off the E-stop loop is still recommended regardless of what the HOTSHOT's own interlock does.

Both items below are now considered closed for practical wiring purposes. No further outreach to Ambrell is planned unless a compliance review specifically requires mains-disconnect confirmation.

**Update (2026-09-05):** Full manual now on hand and saved to the vault — [[media/wiring/Ameritherm HOTSHOT 103927 Manual (Doc 801-9252k).pdf]] (all 49 pages, superseding the earlier partial photo set from 2026-09-01, which — despite being logged in this note at the time — was never actually saved into the vault under a real filename; that reference was dangling and has been corrected). This confirms/repeats the CTB1 pinout already logged in [[Design/Wiring/Electrical System.md]] and adds Section 4 "Technical Information" (mechanical dimensions/weight, environmental operating ranges) — see that note for the extracted specs.

**Update (2026-09-01):** Physical manual found on hand — Doc# 801-9252k.doc (Ameritherm HOTSHOT manual, §3 "Customizing Your HOTSHOT," pages 29–35, covering rear panel CTB1 connections).

No manual is on hand for this unit. Two open items depend on confirmed specs before wiring:
1. Emergency stop — need to know if the remote/interlock connector fully de-energizes the unit or only inhibits RF output.
2. Planned PID power control loop — assumes a 0-10V analog input exists for external power commands (see [[Design/Wiring/NI-DAQ Control Architecture.md]]).

### Answers found in manual (2026-09-01, confirmed again 2026-09-05 in full manual)
1. **E-stop (CTB1:15-16)**: Connect a N.C. E-stop rated 24V @ 3A min. If no E-stop is used, 15-16 **must be jumpered** (factory default) — this jumper routes all internal 24Vdc to the controls/display. Manual §1.1 Safety Circuits states verbatim: "Your optional normally-closed E-stop switch can be installed in series with the system 24Vdc supply (CTB1:15-16). When the switch opens, all output power and operations cease. When the switch is re-closed, the power supply returns to Ready state (non-hazardous state) @ 24Vdc." Tripping the E-stop halts RF power output **and** equipment operation (not RF-only inhibit); reset restarts the HOTSHOT at Home zone. Note: this is still described purely as a control-circuit interlock via the internal 24V rail — even the full manual doesn't explicitly state it disconnects mains power to the chassis, so treat "fully de-energizes" as still open if that distinction matters for E-stop compliance (e.g. lockout/tagout, arc-flash boundary).
2. **Analog power control (CTB1:1-2)**: Confirmed. Pin 1 = +, Pin 2 = −. Default configuration is 0-10Vdc (jumper set upper); jumper-selectable to 4-20mA (with included 250Ω resistor across 1-2, jumper set lower). Touch-pad setting required: System Options → Control From → Rear Panel. The full manual's §4.2 Electrical table (p.46) appears to list input impedance/limits for this port but the photo isn't sharp enough to transcribe reliably — exact voltage scaling/linearity (e.g. does 10V = 100% power, is it linear) still not confirmed.

## Draft Email

**Subject:** Legacy documentation request — Ameritherm HOTSHOT SN 103927

Hello,

I have an Ameritherm HOTSHOT induction heating power supply (model/serial 103927) along with a handheld remote control module (part no. 301-0243D). I'm unable to locate a manual or documentation for either unit and I'm hoping Ambrell can help since Ambrell acquired Ameritherm.

Could you help with the following:

1. **User/service manual** for HOTSHOT SN 103927, and any documentation for the 301-0243D handheld remote control module.

2. **Remote/interlock connector**: Does this unit have a rear-panel remote or interlock connector with dedicated safety-interlock or E-stop contacts? Specifically:
   - Does opening the interlock loop fully de-energize the unit, or does it only inhibit RF drive while leaving the chassis/control electronics powered?
   - Pinout for this connector, if available.

3. **External analog power control**: Does this unit accept an external analog control signal (e.g., 0-10V) to command output power remotely, separate from the front-panel controls and the 301-0243D handheld?
   - If yes: connector/pin location, voltage range and scaling (e.g., does 10V correspond to 100% power, and is this linear?), and expected input impedance/source current so we can confirm compatibility with a National Instruments NI-9263 analog output module.

4. Any other documentation on nameplate ratings (frequency range, max output power) specific to SN 103927, since we understand HOTSHOT nameplate specs can vary by unit within the product line.

Thank you for your help — happy to provide photos of the nameplate/rear panel if useful.

Best,
[Your name]
whalls420@gmail.com

---

## Status
- [x] Located physical manual — Doc# 801-9252k.doc (2026-09-01)
- [x] Review manual for interlock/E-stop and 0-10V analog control answers (2026-09-01) — see "Answers found in manual" above
- [x] Full 49-page manual photographed and saved to vault (2026-09-05) — `[[media/wiring/Ameritherm HOTSHOT 103927 Manual (Doc 801-9252k).pdf]]`
- [x] Confirmed E-stop is a 24V control-rail interlock only (CTB1:15-16, N.C., 3Adc min) — manual never states mains disconnect; treat as a known limitation, not an open question, unless compliance review needs it explicitly from Ambrell
- [x] Confirmed 0-10V remote input impedance (Zin = 21kΩ) from manual §4.2 p.46 (2026-09-05)
- [ ] (Optional) Send follow-up email only if a compliance review specifically requires Ambrell's written confirmation on mains-disconnect behavior of the E-stop loop
- [ ] Log Ambrell's response here (if sent)
- [x] Update [[Design/Wiring/Electrical System.md]] and [[Design/Wiring/NI-DAQ Control Architecture.md]] with confirmed CTB1 pinout (2026-09-01)
