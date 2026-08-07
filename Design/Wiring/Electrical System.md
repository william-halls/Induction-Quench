---
tags: [design, wiring, electrical, power-delivery, control-systems]
---

# Wiring & Electrical Systems

Power delivery, control circuits, and instrumentation for induction heating and data acquisition systems.

## Current Design: Power & Instrumentation (TBD)

**Status**: In progress — Equipment and topology under evaluation.

### Power System
- **Induction Supply**: TBD (frequency ~1 MHz based on coil design)
- **Max Power**: Limited by supply capability and coil losses
- **Coil Connections**: Soldered high-frequency leads via pass-throughs
- **Impedance Matching**: To be verified during thermal testing

### Instrumentation & Control
- **Temperature Measurement**: Spot-welded thermocouples on sample + feedthrough
- **Pressure Monitoring**: Chamber vacuum gauge (via air control assembly)
- **Data Logging**: External equipment integration (TBD)
- **Emergency Stop**: E-stop on power supply; manual vacuum vent available

## Key Design Factors

- **Power Supply Specs** — Frequency, impedance matching, transient behavior
- **Coil Connection** — Series vs. parallel, load matching, efficiency
- **Shielding & EMI** — Cable routing, ferrite filters, grounding strategy
- **Thermocouple Circuits** — Amplification, noise filtering, cold-junction compensation
- **Safety Systems** — E-stop wiring, fuses, thermal cutouts, interlock circuits
- **Data Acquisition** — Sampling rate, resolution, isolation from power circuits

## Integration Points

- [[Design/Coil Geometry/Induction Coil|Coil Geometry]] — Coil load impedance and connections
- [[Design/Mechanisms/Control System|Mechanisms & Automation]] — Control signals and feedback loops
- [[Design/Plumbing/Fluid Systems|Plumbing & Fluid Systems]] — Pump/valve control circuits
- [[Design/Archive/Design History|Design Archive]] — Previous power supply configurations
