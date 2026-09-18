---
subsystem: plumbing
tags: [design, plumbing, cooling, quenching-systems, automation]
---

# Fluid Systems

**Quench delivery (decided):** Water quench via ball-screw immersion — the sample, still clamped in its [[Design/Mechanisms/Ceramic Mount|ceramic mount]], is lowered directly into a pre-filled water bath. There is no spray/release mechanism for the quench itself. See [[Design/Sample Quenching/Quenching Methods|Quenching Methods]] for the full decision and rationale.

**Decision Pending:** Whether the water bath stays in the vacuum chamber at all times, or is added after (via a 24V mini diaphragm pump) — this is a bath-fill-timing question, separate from the quench-delivery method above.

**Concern:** Water boiling off prevents reaching desired vacuum for argon backfill.

**Opportunity:** The same 24V diaphragm pump's inlet could be modified into a custom nozzle for an alternative spray-quench approach (see "Spray Quenching" in [[Design/Sample Quenching/Quenching Methods|Quenching Methods]]) — helping overcome the Leidenfrost effect and potentially reducing quenching time. This is an opportunistic alternative, not the current decided method, since it would require sample release rather than the current always-clamped ball-screw immersion.

## Automatic Water Control

**Planned Automation** (in development):
- See [[Design/Wiring/NI-DAQ Control Architecture|NI-DAQ Control Architecture]] for integration — pump/valve actuation is a USB-6009 digital SSR line (current cDAQ-9174 architecture; the original NI-9263 analog Channel 3 plan was superseded once pump control moved to purely digital SSR triggering)
- Water filling controlled via solenoid valve or diaphragm pump on/off, via SSR
- Timing-based or pressure-based triggers (TBD)