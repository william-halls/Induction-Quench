---
subsystem: plumbing
tags: [index, plumbing, fluid-systems, vacuum, gas-delivery]
---

# Plumbing & Fluid Systems Subsystem

Gas delivery, vacuum evacuation, and quenching medium circulation systems for chamber environment control.

## Files in This Folder

| File | Purpose | Status |
|------|---------|--------|
| **[[Design/Plumbing/Fluid Systems|Fluid Systems.md]]** | Overview of gas/vacuum/quench systems | 🔵 Hub |
| **[[Design/Plumbing/Vacuum Lid Systems|Vacuum Lid Systems.md]]** | Plumbing in chamber lid | 🔵 Reference |
| **[[Design/Plumbing/Air System Control Assembly|Air System Control Assembly.md]]** | Vacuum/argon manifold (3-way valve) | 🟢 Active |
| **[[Design/Plumbing/Coil Lead Pass-Throughs|Coil Lead Pass-Throughs.md]]** | Omega SSLK-14-14 compression fittings for coil power | 🟢 Active |
| **[[Design/Plumbing/Vertical Sliding Shaft Seal|Vertical Sliding Shaft Seal.md]]** | 1/2" shaft seal for sample port | 🟢 Active |
| **[[Design/Plumbing/Thermal Couple Pass-through|Thermal Couple Pass-through.md]]** | Thermocouple quick-connect interface | 🟡 Planned |
| **[[Design/Plumbing/Random Holes|Random Holes.md]]** | Existing chamber ports (evaluate/seal) | 🔵 Reference |

---

## Connected Subsystems

### 📦 Chamber Interface
- **[[Design/Vacuum Chamber/Vacuum Enclosure\|Vacuum Chamber]]** — All lines penetrate chamber lid/walls
  - *Connection*: Air control assembly in lid; coil lead pass-throughs; thermocouple feedthrough; pressure gauge port

### 🔥 Heating Element
- **[[Design/Coil Geometry/Induction Coil\|Coil Geometry]]** — Water-cooled copper tubing
  - *Connection*: Coil lead pass-throughs carry high-frequency power; cooling water inlet/outlet

### ❄️ Quenching
- **[[Design/Sample Quenching/Quenching Methods\|Sample Quenching Routes]]** — Quench medium delivery
  - *Connection*: Quench valve controls medium flow; 24V diaphragm pump option for spray nozzle

### 🔧 Control & Actuation
- **[[Design/Mechanisms/Control System\|Mechanisms & Automation]]** — Valve/pump control, pressure monitoring
  - *Connection*: Solenoid valve triggers quench; pressure relief for safety; feedback signals

### ⚡ Power & Instrumentation
- **[[Design/Wiring/Electrical System\|Wiring & Electrical]]** — Pump power, solenoid valve control, sensor signals
  - *Connection*: Pump 24V supply; thermocouple amplifier signals; pressure gauge instrumentation
- **[[Design/Wiring/NI-DAQ Control Architecture\|NI-DAQ Control Architecture]]** — Automated pump/valve control via NI-9263
  - *Connection*: Water filling system automation; solenoid actuation via 0-10V command signals

---

## System Overview

### Vacuum System
- **Purpose**: Evacuate chamber; create low-pressure environment to prevent water boiling
- **Components**: Existing pump (details TBD)
- **Concern**: Water boil-off at vacuum — impact on backfill dynamics
- **Integration**: Via air control manifold (3-way valve)

### Inert Gas System
- **Gas**: Argon (primary) or nitrogen (alternative)
- **Purpose**: Backfill after evacuation; maintain inert atmosphere during 1000°C heating
- **Components**: Gas regulator + air control manifold (3-way valve)
- **Safety**: Pressure relief valve in air control assembly

### Quench System
- **Medium**: TBD (oil/water/gas options under evaluation)
- **Delivery**: 
  - Option 1: Gravity feed (pre-fill chamber)
  - Option 2: 24V diaphragm pump with possible spray nozzle
- **Purpose**: Rapid cooling after 1000°C heating
- **Advantage**: Spray nozzle could overcome Leidenfrost effect for better uniformity

### Thermal Monitoring
- **Thermocouple**: Quick-connect via dedicated feedthrough (copper tube with threads)
- **Gauge**: Pressure monitoring in air control assembly

---

## Active Components

### Air System Control Assembly
**Configuration:**
- Material: Bronze 1/4" threaded pipes
- 3-way valve: Inputs (vacuum pump + argon), Output (chamber)
- Pressure gauge monitoring
- Pressure relief valve
- Solenoid valve for control

**Note:** Repurposed from original chamber; being integrated into new lid

### Coil Lead Pass-Throughs
**Configuration (current, as of 2026-08-17):**
- Fitting: Omega SSLK-14-14, 1/4" x 1/4" NPT stainless compression fitting
- Purpose: Allow high-frequency power to reach coil without breaking vacuum seal, with a low-resistance connection
- Trade-off: Lead from coil to power supply must be one continuous piece — swapping/iterating on the coil now requires un-soldering and re-soldering the lead
- Previous design (bronze tube + flared fitting nuts) rejected: brass resistance forced ~2x the power to heat the sample at the same rate — see [[Design/Archive/INDEX|Archive]]

### Vertical Sliding Shaft Seal
**Configuration:**
- Press-fit spring-loaded lip seal (50 psi rated)
- 1/2" OD polished stainless steel shaft
- Attached to ball screw for future linear actuation
- Minimal horizontal load for optimal seal performance

---

## Design Decisions Pending

⏳ **Water Boil-Off** — Evaluate impact on vacuum performance and backfill strategy  
⏳ **Quench Medium** — Select oil/water/gas based on desired cooling rate  
⏳ **Spray Nozzle** — If pursuing spray quench, design custom nozzle from diaphragm pump inlet  
⏳ **Thermocouple Routing** — Finalize lead management through chamber

---

## Quick Links

📖 **Related Reading:**
- [[Design/Vacuum Chamber/Used Vacuum Chamber\|Chamber Integration]] — Lid feedthrough specifications
- [[Design/Mechanisms/Control System\|Control Strategy]] — Valve triggering logic
- [[Design/Sample Quenching/Quenching Methods\|Quenching Methods]] — Medium selection rationale

🔗 **CAD Resources:**
- Air Control Assembly: https://cad.onshape.com/...
- Coil Lead Pass-Throughs: https://cad.onshape.com/...
- Chamber Lid Assembly: https://cad.onshape.com/...

---

*Plumbing is the hidden circulatory system. Every line must be leak-tight under vacuum and rated for chamber temperatures.*
