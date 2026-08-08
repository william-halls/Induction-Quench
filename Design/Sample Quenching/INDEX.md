---
tags: [index, sample-quenching, cooling, heat-treatment, sample-geometry]
---

# Sample Quenching Subsystem

Sample geometries, rapid cooling strategies, and quenching methods for achieving desired microstructure through controlled thermal cycling.

## Files in This Folder

| File | Purpose | Status |
|------|---------|--------|
| **[[Design/Sample Quenching/Quenching Methods|Quenching Methods.md]]** | Cooling strategy overview & candidate methods | 🔵 Hub |
| **[[Design/Sample Quenching/Charpy|Charpy.md]]** | Standard Charpy sample geometry (10×10×55 mm) | 🔵 Reference |
| **[[Design/Sample Quenching/Modified Charpy|Modified Charpy.md]]** | Test variant with button head (61 mm total) | 🔵 Reference |

---

## Connected Subsystems

### 🔥 Heating Element
- **[[Design/Coil Geometry/Induction Coil\|Coil Geometry]]** — Heating rate determines cooling strategy
  - *Connection*: Coil uniformity + heating rate define required quench cooling rate; thermal stress during quench affects coil longevity

### 📦 Chamber & Cooling
- **[[Design/Vacuum Chamber/Vacuum Enclosure\|Vacuum Chamber]]** — Contains quenching medium; pressure affects boil-off
  - *Connection*: Chamber volume determines medium capacity; vacuum level affects water boiling; material survives thermal shock

- **[[Design/Plumbing/Fluid Systems\|Plumbing & Fluid Systems]]** — Quench medium delivery & circulation
  - *Connection*: Valve controls medium flow; pump options for spray delivery; 24V diaphragm pump available

### 🔧 Mechanical Release
- **[[Design/Mechanisms/Control System\|Mechanisms & Automation]]** — Sample positioning & release timing
  - *Connection*: Sample holder (ceramic mount) must survive quench temperature; timing between heating & quenching critical

### ⚡ Control Timing
- **[[Design/Wiring/Electrical System\|Wiring & Electrical]]** — Quench trigger signals, solenoid valve activation
  - *Connection*: Timer-based or manual trigger; feedback from sensors (temp, pressure) may inform quench decision
- **[[Design/Wiring/NI-DAQ Control Architecture\|NI-DAQ Control Architecture]]** — Automated quench triggering via NI-9263
  - *Connection*: Software triggers quench valve; temperature/time-based logic; data logging of quench event and pressure spike

---

## Sample Geometry

### Standard Charpy
- **Dimensions**: 10 × 10 × 55 mm
- **Purpose**: Reference baseline; conventional impact test geometry
- **Use**: Initial prototype testing
- **CAD**: OnShape document (reference geometry model)

### Modified Charpy
- **Dimensions**: 10 × 10 × 55 mm base + 6 mm button head (61 mm total length)
- **Purpose**: Variant for specific testing needs; allows gripping via button
- **Use**: If sample holder requires positive location feature
- **Consideration**: Dr. Buchely interested in heating longer samples; modified charpy bridges baseline to extended geometry

---

## Quenching Strategy Decision Matrix

**Status**: TBD — Material properties and experimental objectives will determine cooling rate targets.

### Candidate Methods

**Oil Quench**
- Cooling rate: Fast (material-dependent)
- Pros: Good control, high cooling power
- Cons: Oxidation risk, post-quench cleanup required
- Candidate: TBD

**Water Quench**
- Cooling rate: Very fast
- Pros: Rapid cooling, readily available
- Cons: Oxidation if not in inert chamber, thermal shock, boiling at vacuum
- Candidate: TBD (water boil-off concern in vacuum)

**Gas Quench (Argon/Nitrogen)**
- Cooling rate: Slower (material-dependent)
- Pros: Clean environment (no oxidation), maintains inert atmosphere
- Cons: Slower cooling than liquid
- Candidate: Possibly integrated with backfill system

**Spray Quench (Advanced)**
- Concept: Custom nozzle from 24V diaphragm pump
- Pros: Could overcome Leidenfrost effect; better uniformity
- Cons: Requires nozzle design & flow optimization
- Status: Opportunistic if other systems permit

---

## Design Considerations

**Cooling Rate vs. Microstructure:**
- Different materials require different cooling rates
- Too slow → unwanted grain growth
- Too fast → thermal shock risk, uneven cooling
- Project objective: Achieve specific microstructure for materials science testing

**Thermal Stress:**
- Charpy sample undergoes rapid temperature change (1000°C → room temp in seconds)
- Chamber walls experience thermal shock
- Coil cooling water must handle temperature transients
- Safety consideration: Pressure spike during rapid quench

**Uniformity:**
- Leidenfrost effect: Film of vapor insulates hot sample from cooling medium
- Risk: Uneven cooling → uneven microstructure
- Solution: Spray quenching could help break Leidenfrost layer

**Environmental Control:**
- Vacuum chamber prevents oxidation during quench
- If using water/oil quench, they must evaporate/drain post-quench
- Thermocouple wires must endure quench environment

---

## Key Project Requirement

**Longer Samples Capability:**
- Dr. Buchely wants ability to heat samples longer than standard charpy
- Proposed solution: Vertical oscillation (scanning) during heating to progress heat along length
- Implication: Quench timing must account for scan pattern; sample may be partially cooled as it exits coil

---

## Quick Links

📖 **Related Reading:**
- [[Design/Coil Geometry/Induction Coil\|Coil Heating]] — Heating uniformity & rate
- [[Design/Plumbing/Fluid Systems\|Fluid Delivery]] — Quench medium circulation
- [[Design/Mechanisms/Ceramic Mount\|Sample Holder]] — Mount survives thermal cycling
- [[Design/Vacuum Chamber/Vacuum Enclosure\|Chamber]] — Pressure effects on medium

🔗 **CAD Resources:**
- Charpy samples: https://cad.onshape.com/...

---

*Quenching is where the experiment happens. Cooling rate + uniformity determine microstructure and data validity.*
