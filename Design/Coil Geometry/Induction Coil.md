---
subsystem: coil_geometry
tags: [design, coil, induction-heating, electromagnetic]
---

# Coil Geometry

Induction heating coil design and optimization for uniform, efficient heating of charpy-shaped samples to 1000°C.

## Current Design: Round Coil

**Status**: Active — Currently manufactured and in testing phase.

### Key Specifications
- **Type**: Cylindrical induction coil
- **Material**: 5/16" OD copper tubing
- **Cooling**: Water-cooled via 1/4" OD supply tubing through interior
- **Connections**: Soldered joints
- **Manufacturing**: 3D-printed molds + wrapping technique

### Performance Notes
- ✓ Achieves uniform heating around sample
- ⚠ Slight alignment/tilt issue — may require remake for better centering
- 📋 Thermal testing pending before deciding on redesign

**See** [[Design/Coil Geometry/Round Coil|Round Coil]] **for detailed design information.**

## Key Design Factors

- **Sample Uniformity** — Ensuring even heat distribution across charpy geometry
- **Coil Durability** — Material selection for thermal cycling to 1000°C
- **Coupling Efficiency** — Optimal frequency and geometry for energy transfer
- **Chamber Integration** — Space constraints and positioning within vacuum chamber
- **Electrical Parameters** — Impedance matching with power supply

## Integration Points

- [[Design/Vacuum Chamber/Vacuum Enclosure|Vacuum Chamber]] — Must fit within chamber envelope
- [[Design/Wiring/Electrical System|Wiring & Electrical]] — Coil connections and power feed
- [[Design/Wiring/NI-DAQ Control Architecture|NI-DAQ Control Architecture]] — PID loop power control (0-10V command to power supply)
- [[Design/Archive/Design History|Design Archive]] — Previous coil designs and lessons learned


## Design Documentation

### Current Implementation
- [[Design/Coil Geometry/Round Coil|Round Coil]] — Cylindrical coil design (active approach)

### Archived Concepts
- [[Design/Coil Geometry/Square Coil|Square Coil]] — Rectangular coil design (deferred due to complexity)

### Tools & Resources
- [[Design/Coil Geometry/Coil Feature Script|Coil Feature Script]] — Parametric OnShape script for coil generation
