---
tags: [design, plumbing, cooling, quenching-systems, automation]
---

# Fluid Systems

**Decision Pending:** Whether water stays in the vacuum chamber at all times, or is added after using a 24V mini diaphragm pump.

**Concern:** Water boiling off prevents reaching desired vacuum for argon backfill.

**Opportunity:** The pump inlet could be modified into a custom nozzle to spray samples, helping overcome the Leidenfrost effect, improving quenching control, and potentially reducing quenching time by removing heat more rapidly.

## Automatic Water Control

**Planned Automation** (in development):
- See [[Design/Wiring/NI-DAQ Control Architecture|NI-DAQ Control Architecture]] for integration with NI-9263 signal output
- Water filling controlled via solenoid valve or pump speed command (0-10V analog)
- NI-9263 Channel 3 reserved for pump/valve actuation
- Timing-based or pressure-based triggers (TBD)