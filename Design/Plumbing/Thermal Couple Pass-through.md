---
subsystem: plumbing
tags: [design, plumbing, thermocouple, instrumentation, data-collection, automation]
---

# Thermocouple Pass-Through

**Current Plan:**
- Use existing thermocouple pass-throughs with threads (purchased individually, not McMaster)
- Part: **PFT2NPT-1K**
- Tap the vacuum chamber lid and install
- Add quick-connect fittings (compatible with thermocouple monitor connectors)
- Enables easy wire connection and experimental setup inside chamber

**Status:** Un-CADed (design ready, documentation pending)

## Integration with Data Acquisition

**Thermocouple Measurement System:**
- Sensor: Spot-welded thermocouple on sample surface
- Feedthrough: This pass-through in chamber lid
- Signal path: Thermocouple → external analog CJC amplifier (e.g. AD8495, mounted at the feedthrough) → NI-9229 (analog input, chosen for speed — 50kS/s/ch)
- Cold-junction compensation: Handled by the external CJC amp ahead of the 9229 (the 9229 itself has no built-in CJC)
- See [[Design/Wiring/Electrical System|Wiring & Electrical System]] for signal conditioning
- See [[Design/Wiring/NI-DAQ Control Architecture|NI-DAQ Control Architecture]] for full "Thermocouple Signal Chain" rationale and PID loop feedback