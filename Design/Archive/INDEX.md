---
tags: [index, archive, history, rejected-designs, lessons-learned]
---

# Design Archive & History

Historical designs, rejected concepts, and engineering decisions from previous iterations. Serves as reference for understanding current design choices and prevents revisiting rejected ideas without explicit reason.

## Files in This Folder

| File | Purpose | Status |
|------|---------|--------|
| **[[Design/Archive/Design History\|Design History.md]]** | Consolidated archive of rejected designs & lessons | 🔵 Reference |

---

## Rejected Sample Mounting Mechanisms

### Titanium Claw Gripper (Abandoned)
**Why Rejected**: Fundamental thermal flaw—heat sink effect  
**Versions**: 3 iterations (9-part → 7-part → refined 7-part)  
**Lessons**: Material selection outweighs mechanical optimization  

**See Full Details:** Refer to [[Design/Archive/Design History\|Design History.md]] → Titanium Claw section

**Connected Subsystems:**
- [[Design/Mechanisms/Ceramic Mount\|Current Mount]] — Active alternative
- [[Design/Coil Geometry/Induction Coil\|Coil Geometry]] — Heating uniformity affected by mount

---

### Bottom Lift Mechanism (Deferred)
**Why Deferred**: Seal temperature rating too low (210°F); centering complexity  
**Status**: Possible future reconsideration  
**Reconsider If**: Better seals available (1000°C+); cost reduction for non-magnetic materials  

**See Full Details:** Refer to [[Design/Archive/Design History\|Design History.md]] → Bottom Lift section

**Connected Subsystems:**
- [[Design/Mechanisms/Trapdoor\|Trapdoor]] — Another deferred approach
- [[Design/Plumbing/Vertical Sliding Shaft Seal\|Shaft Seal]] — Current seal solution

---

### Trapdoor Release Mechanism (Deferred)
**Why Deferred**: Incompatible with scanning requirement  
**Status**: Deferred until heating requirements change  
**Incompatibility**: Vertical oscillation (scanning) + rotating release = complex timing & fragile operation  
**Current Solution**: Manual extraction after heating (acceptable for prototype)  

**See Full Details:** Refer to [[Design/Archive/Design History\|Design History.md]] → Trapdoor section

**Connected Subsystems:**
- [[Design/Mechanisms/Control System\|Control Strategy]] — Manual vs. automated operation
- [[Design/Sample Quenching/Quenching Methods\|Quenching]] — Timing considerations

---

## Rejected Coil Designs

### Square/Rectangular Coil (Deferred)
**Why Deferred**: Manufacturing complexity; cooling challenges at 90° corners  
**Theoretical Advantage**: Better field alignment with square charpy geometry  
**Manufacturing Issue**: Corner tubing severely reduces coolant flow  
**Status**: Simpler round coil chosen; can revisit if round insufficient  

**See Full Details:** Refer to [[Design/Coil Geometry/Square Coil\|Square Coil.md]]

**Connected Subsystems:**
- [[Design/Coil Geometry/Round Coil\|Round Coil]] — Active alternative
- [[Design/Coil Geometry/Coil Feature Script\|CAD Tool]] — Supports both geometries

---

## Rejected Chamber Concepts

### Quartz Glass Tube Chamber (Abandoned)
**Why Abandoned**: Sealing complexity too high for initial prototype  
**Concept**: Compact sealed-tube design for faster vacuum cycles  
**Manufacturing Challenge**: 5+ simultaneous sealed penetrations in narrow tube  
**Current Solution**: Repurposed stainless steel bucket (proven vessel, faster prototyping)  

**See Full Details:** Refer to [[Design/Vacuum Chamber/Quartz Glass Tube\|Quartz Glass Tube.md]]

**Connected Subsystems:**
- [[Design/Vacuum Chamber/Used Vacuum Chamber\|Current Chamber]] — Active design
- [[Design/Plumbing/Vacuum Lid Systems\|Plumbing]] — Feedthrough interface

---

## Design Decision Summary Table

| Subsystem | Rejected | Current | Rationale |
|-----------|----------|---------|-----------|
| **Sample Mount** | Titanium gripper | Ceramic + steel shaft | Avoid heat sink; simplicity |
| **Positioning** | Bottom lift | Manual insertion | Simpler for prototype |
| **Release** | Trapdoor | Manual extraction | Incompatible with scanning |
| **Coil** | Square profile | Round cylinder | Easier manufacturing |
| **Chamber** | Quartz tube | Repurposed bucket | Proven vessel; faster |

---

## Key Engineering Insights

### Design Philosophy
- **Simulation Catches Flaws** → FEA identified heat sink effect before prototype testing
- **Simplicity Wins** → Manual controls acceptable for prototype; automate later if needed
- **Material Matters** → Thermal properties outweigh mechanical refinement
- **Reuse Over Rebuild** → Repurposed equipment faster than custom fabrication
- **Flexibility Required** → Design must adapt to changing requirements (e.g., longer samples)

### Decision-Making Process
1. **Identify Problem** (e.g., "need to heat samples to 1000°C")
2. **Propose Solutions** (e.g., multiple coil geometries, chamber designs)
3. **Analyze Trade-offs** (e.g., manufacturing complexity vs. performance)
4. **Validate with Simulation** (e.g., FEA thermal modeling)
5. **Select Simplest Path** (e.g., round coil + bucket chamber)
6. **Defer Complex Additions** (e.g., trapdoor, automated control)
7. **Plan Iteration** (e.g., prototype → test → refine)

---

## Connection Map

```
Rejected Designs Archive
│
├─→ Sample Mounting Decisions
│   ├─ Titanium Claw (abandoned) → Ceramic Mount (active)
│   ├─ Bottom Lift (deferred) → Manual Insertion (active)
│   └─ Trapdoor (deferred) → Manual Extraction (active)
│
├─→ Coil Geometry Decisions
│   └─ Square Coil (deferred) → Round Coil (active)
│
├─→ Chamber Decisions
│   └─ Quartz Tube (abandoned) → Bucket (active)
│
└─→ Lessons Feed Into
    ├─ [[Design/Mechanisms/Control System\|Control Strategy]]
    ├─ [[Design/Coil Geometry/Induction Coil\|Coil Overview]]
    ├─ [[Design/Vacuum Chamber/Vacuum Enclosure\|Chamber Overview]]
    └─ [[Design/Plumbing/Fluid Systems\|Plumbing Strategy]]
```

---

## Why This Archive Matters

**Prevents Revisiting Failed Ideas**
- If someone asks "Why not use titanium gripper?", answer is documented
- Prevents wasting time on concepts already evaluated

**Captures Lessons Learned**
- Thermal simulation identified flaw before costly prototype build
- Simple approach better than over-engineered complexity
- Material properties critical for experimental validity

**Supports Design Evolution**
- If requirements change (e.g., high-throughput mode), archive explains why trapdoor was deferred
- Future engineers understand context of decisions

**Documents Trade-offs**
- Every choice has pros/cons; archive explains what was chosen and why
- Future designers can revisit if circumstances change

---

## Quick Links to Active Designs

**Current Sample Mount:**
- [[Design/Mechanisms/Ceramic Mount\|Ceramic Mount (Active)]]

**Current Chamber:**
- [[Design/Vacuum Chamber/Used Vacuum Chamber\|Bucket Chamber (Active)]]

**Current Coil:**
- [[Design/Coil Geometry/Round Coil\|Round Coil (Active)]]

**Current Control:**
- [[Design/Mechanisms/Control System\|Manual Operation (Prototype)]]
- [[Design/Wiring/NI-DAQ Control Architecture\|NI-DAQ Automated Control (In Development)]]

---

*History is written by the present to inform the future. This archive prevents repeating mistakes and celebrates successful simplicity.*
