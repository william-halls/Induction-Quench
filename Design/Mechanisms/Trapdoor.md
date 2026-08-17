---
subsystem: mechanisms
tags: [mechanisms, sample-release, archived, deferred]
---

# Trapdoor Release Mechanism

Rotating ceramic plate mechanism for releasing charpy sample during quenching without manual intervention.

**Status**: Deferred — Incompatible with current testing requirements.

## Design Concept

### Operating Principle

1. **Loading Phase**: Sample rests on ceramic plate while coil heats it from above
2. **Quench Trigger**: Stepper motor/actuator rotates plate ~90°
3. **Release**: As plate rotates, sample slides off onto quench medium below
4. **Quenching**: Sample undergoes thermal shock in quench medium

### Advantages

✓ **Automated Release** — No manual sample extraction needed  
✓ **Reproducible Timing** — Electronic trigger ensures consistent timing  
✓ **Thermal Isolation** — Ceramic plate avoids induction/heat sink effects  
✓ **Simple Mechanism** — Rotating platform is mechanically straightforward  

## Why Deferred

### Incompatibility with Longer Samples

**Dr. Buchely Requirement**: Instrument must handle samples longer than standard charpy

**Current Constraint**: Power supply coil has limited power avalible; limited heating length

**Proposed Solution**: Oscillate (scan) sample vertically during heating to heat full length progressively

**Problem**: Trapdoor concept requires stationary sample for rotation release
- Combining scanning + rotation adds complexity
- Timing becomes difficult (when to rotate?)
- May release sample mid-scan

### Result

The requirement to support longer samples with scanning operation makes the trapdoor concept:
- Over-complicated
- Timing-dependent and fragile
- Better addressed with simpler mechanical release

**Better Solution**: Ceramic mount with manual extraction after heating completes

---

## Design Details (As Conceived)

### Plate Design
- **Material**: Ceramic (non-magnetic, low thermal conductivity)
- **Motion**: Hinged or pivoting mount
- **Rotation Angle**: ~90° minimum to release sample
- **Timing**: Pneumatic or solenoid actuation

### Actuation Options Considered
1. **Rotary Solenoid** — Direct 90° rotation
2. **Linear Actuator** — Cam-driven plate rotation
3. **Pneumatic Cylinder** — Spring-return for reliability

### Quench Medium Interface
- Sample falls into quench medium container below
- Requires coordinated positioning
- Release must be reliable (no sample sticking)

---

## Key Insights

**Design for Flexibility**: Requirements evolve; architecture must adapt.  
**Avoid Coupling**: Automated systems create interdependencies (scanning ↔ release timing).  
**Simplicity Wins**: Manual release simpler than multi-axis automation for prototyping.  
**Validate Requirements Early**: Sample length needs should drive mechanism selection.

## Revisit If

- Sample length requirements change back to standard charpy only
- Manual operation becomes impractical (high-throughput testing phase)
- Automated release timing can be decoupled from heating scan via software control

---

## Related Mechanisms

- [[Design/Mechanisms/Ceramic Mount|Ceramic Mount]] — Current approach (manual release)
- [[Design/Mechanisms/Bottom Lift|Bottom Lift]] — Vertical positioning mechanism
- [[Design/Mechanisms/Titanium Claw|Titanium Claw]] — Gripping mechanism (rejected for other reasons)
- [[Design/Vacuum Chamber/Quartz Glass Tube|Quartz Glass Tube]] — Original chamber design context
