---
subsystem: plumbing
tags: [design, plumbing, thermocouple, instrumentation, data-collection, automation]
---

# Thermocouple Pass-Through

**Current Plan:**
- Use existing thermocouple pass-throughs with threads (purchased individually, not McMaster)
- Tap the vacuum chamber lid and install
- Add quick-connect fittings (compatible with thermocouple monitor connectors)
- Enables easy wire connection and experimental setup inside chamber

**Status:** Un-CADed (design ready, documentation pending)

## Integration with Data Acquisition

**Thermocouple Measurement System:**
- Sensor: Spot-welded thermocouple on sample surface
- Feedthrough: This pass-through in chamber lid
- Signal path: Thermocouple → NI-9219 ADC (analog input)
- Cold-junction compensation: Required for accurate temperature measurement
- See [[Design/Wiring/Electrical System|Wiring & Electrical System]] for signal conditioning
- See [[Design/Wiring/NI-DAQ Control Architecture|NI-DAQ Control Architecture]] for PID loop feedback