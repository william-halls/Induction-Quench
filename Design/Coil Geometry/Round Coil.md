---
tags: [coil-design, round, manufactured, current, induction-heating, power-delivery]
---

# Round Coil Design

## Concept

Cylindrical coil geometry wound concentrically around the sample. This approach is **easier to manufacture** compared to square/rectangular profiles, making it the primary candidate for initial testing.

## Power Control Integration

See [[Design/Wiring/NI-DAQ Control Architecture|NI-DAQ Control Architecture]] for automated power delivery:
- Induction power supply receives 0-10V command signal from NI-9263 (DAC)
- PID loop maintains target sample temperature (1000°C) via thermocouple feedback from NI-9219 (ADC)
- Coil power is ramped and controlled automatically during heating phase

## Manufacturing Approach

1. **CAD Model** — Used FeatureScript to parameterize coil geometry
2. **Mold Creation** — Generated 3D-printed negative mold around coil outline
3. **Wrapping** — Wrapped final coil inside 3D-printed guide to achieve desired geometry

## Current Implementation

### Materials
- **Coil Tubing** — 5/16" OD copper tube (heating element)
- **Water Supply Tubing** — 1/4" OD tube with ~5/16" ID (cooling flow)
- **Connections** — Soldered joints

### Cooling System
Water-cooled through the copper tubing to allow high-frequency operation without overheating the coil. Connected to external circulation system.

## Performance & Issues

### Results
✓ Geometry achieves uniform heating around sample  
✗ Slight tilt/lean when assembled — coil not perfectly centered  
⚠ May require remake for better alignment

### Next Steps
- Reassess mold design for better centering
- Consider alternative manufacturing method
- Test thermal performance as-is before remake decision

## FeatureScript Settings

Based on research of charpy geometry (unmodified):

![[round-coil-geometry.png]]
