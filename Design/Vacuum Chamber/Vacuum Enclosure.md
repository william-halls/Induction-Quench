---
tags: [design, vacuum, chamber, inert-atmosphere, containment]
---

# Vacuum Chamber

Inert atmosphere enclosure for controlled heating and quenching without oxidation. Maintains stable environment from room temperature to 1000°C.

## Current Design: Repurposed Industrial Chamber

**Status**: Active — Adapted from existing resin-degassing chamber.

### Specifications
- **Material**: Stainless steel bucket (base), custom acrylic lid (to be fabricated)
- **Capacity**: ~4 liters
- **Envelope**: ~11" diameter × 11" height
- **Operating Pressure**: Vacuum to ~atmospheric
- **Temperature Range**: Room temperature to 1000°C
- **Atmosphere**: Vacuum backfilled with inert gas (argon/nitrogen)

### Sealing & Access
- **Main Seal**: Press-fit spring-loaded lip seal (50 psi rated) on 1/2" shaft
- **Sample Port**: Central 1/2" OD shaft for ceramic sample holder
- **Thermocouple**: Dedicated feedthrough port (planned)
- **Gas/Vacuum Lines**: Via air control manifold
- **Existing Ports**: See [[Design/Plumbing/Random Holes|Random Holes]] for evaluation/sealing
- **Support**: Scissor lift base for maintenance access

**See** [[Design/Vacuum Chamber/Used Vacuum Chamber|Used Vacuum Chamber]] **for detailed specifications.**

## Key Design Factors

- **Material Selection** — Thermal stability, outgassing at temperature, mechanical strength
- **Vacuum Integrity** — Leak rates, O-ring compatibility, feedthrough design
- **Thermal Cycling** — Stress management during repeated 0→1000°C cycles
- **Accessibility** — Sample insertion/removal without breaking vacuum
- **Instrumentation** — Pressure sensors, temperature feedthroughs, vent valves

## Design Documentation

### Current Implementation
- [[Design/Vacuum Chamber/Used Vacuum Chamber|Used Vacuum Chamber]] — Repurposed resin-degassing chamber (active design)
- [[Design/Vacuum Chamber/Vacuum Chamber CAD|Vacuum Chamber CAD]] — OnShape parametric model and assembly

### Archived Concepts
- [[Design/Vacuum Chamber/Quartz Glass Tube|Quartz Glass Tube]] — Compact sealed-tube design (abandoned due to sealing complexity)

## Integration Points

- [[Design/Coil Geometry/Induction Coil|Coil Geometry]] — Houses the induction coil
- [[Design/Plumbing/Fluid Systems|Plumbing & Fluid Systems]] — Vacuum pump, inert gas, quench medium lines
- [[Design/Mechanisms/Control System|Mechanisms & Automation]] — Access door, pressure relief, scissor lift
- [[Design/Wiring/NI-DAQ Control Architecture|NI-DAQ Control Architecture]] — Pressure monitoring via NI-9219; thermocouple feedthrough for temperature feedback
- [[Design/Archive/Design History|Design Archive]] — Previous chamber designs and material trials
