---
tags: [archive, rejected-ideas, past-designs, reference, lessons-learned]
---

# Design Archive & Lessons Learned

Historical designs, rejected concepts, and engineering decisions from previous iterations. This archive serves as reference for understanding current design choices and prevents revisiting rejected ideas without explicit reason.

## Rejected Sample Mounting Mechanisms

### Titanium Claw Gripper (Abandoned)
- **Versions**: 3 iterations (9-part → 7-part → refined 7-part)
- **Why Rejected**: **Fundamental flaw — Heat sink effect**
  - Titanium grips conducted heat away from charpy during heating
  - Created uneven temperature gradient across sample
  - Compromised experimental validity (uneven microstructure)
  - Issue discovered via thermal FEA simulation
- **Lessons Learned**: 
  - Material selection matters more than mechanical optimization
  - Simulation is essential (caught flaw before testing)
  - Simplicity > refinement when core concept is flawed
  - Need materials that don't conduct away test heat

**Full Details**: See [[Design/Mechanisms/Titanium Claw|Titanium Claw]]

### Bottom Lift Mechanism (Deferred)
- **Concept**: Linear actuator with offset shaft to raise sample into coil
- **Why Deferred**: 
  - Cantilever stress on ceramic platform
  - Lip seal rated only to 210°F (far below 1000°C requirement)
  - Centering complexity (required tedious per-experiment setup)
  - Non-magnetic/non-inductive material costs prohibitive
- **Reconsider If**: Better seals available; non-magnetic material costs drop; simplified centering developed

**Full Details**: See [[Design/Mechanisms/Bottom Lift|Bottom Lift]]

### Trapdoor Release Mechanism (Deferred)
- **Concept**: Rotating ceramic plate to release sample during quench
- **Why Deferred**: Incompatible with scanning requirement
  - Project needs: Heat longer samples via vertical oscillation (scanning)
  - Problem: Trapdoor + scanning = complex timing, fragile operation
  - Solution: Manual extraction sufficient for initial prototype
- **Reconsider If**: Sample requirements change; automated release needed for throughput testing

**Full Details**: See [[Design/Mechanisms/Trapdoor|Trapdoor]]

---

## Rejected Coil Designs

### Square/Rectangular Coil (Deferred)
- **Concept**: Rectangular coil geometry
- **Why Deferred**: Manufacturing complexity exceeds round coil approach
- **Current**: Round coil uses 3D-printed molds (simpler, faster iteration)
- **Reconsider If**: Round coil alignment issues unresolvable; rectangular geometry needed for specific heating patterns

**Full Details**: See [[Design/Coil Geometry/Square Coil|Square Coil]]

---

## Rejected Chamber Concepts

### Quartz Glass Tube Chamber (Abandoned)
- **Concept**: Compact sealed-tube design for initial prototype
- **Why Abandoned**: Sealing complexity; joined-tube approach difficult to fabricate reliably
- **Current**: Repurposed industrial stainless steel bucket (proven, faster)
- **Advantage**: Existing equipment → faster prototyping with proven vacuum integrity

**Full Details**: See [[Design/Vacuum Chamber/Quartz Glass Tube|Quartz Glass Tube]]

---

## Design Decision Summary

| Subsystem | Rejected | Current | Rationale |
|-----------|----------|---------|-----------|
| **Sample Mount** | Titanium gripper | Ceramic + steel shaft | Avoid heat sink effect; simplicity |
| **Positioning** | Bottom lift | Manual insertion | Simpler for prototype; acceptable for testing |
| **Release** | Trapdoor | Manual extraction | Incompatible with scanning; simpler control |
| **Coil** | Square profile | Round cylinder | Easier 3D-print mold manufacturing; faster iteration |
| **Chamber** | Quartz tube | Steel bucket | Proven vessel; faster setup; reliable sealing |

---

## Key Engineering Insights

✓ **Simulation Catches Flaws** — Thermal FEA identified heat sink effect before prototype testing  
✓ **Simplicity Wins** — Manual controls acceptable for prototype; automate later if needed  
✓ **Material Matters** — Thermal properties outweigh mechanical refinement  
✓ **Reuse Over Rebuild** — Repurposed equipment faster than custom fabrication  
✓ **Flexibility Required** — Design must adapt to changing sample requirements (e.g., scanning)

---

*This archive prevents revisiting rejected ideas without explicit reason and documents engineering trade-offs.*
