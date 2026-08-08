---
tags: [index, coil-geometry, thermal-systems]
---

# Coil Geometry Subsystem

Design and optimization of the induction heating coil for uniform, efficient heating of charpy samples to 1000°C.

## Files in This Folder

| File | Purpose | Status |
|------|---------|--------|
| **[[Design/Coil Geometry/Induction Coil|Induction Coil.md]]** | Overview & integration points | 🔵 Hub |
| **[[Design/Coil Geometry/Round Coil|Round Coil.md]]** | Active cylindrical coil design | 🟢 Active |
| **[[Design/Coil Geometry/Square Coil|Square Coil.md]]** | Rectangular coil (deferred) | 🟡 Deferred |
| **[[Design/Coil Geometry/Coil Feature Script|Coil Feature Script.md]]** | Parametric CAD automation | 🔵 Tool |

---

## Connected Subsystems

### 🔥 Thermal & Power
- **[[Design/Wiring/Electrical System\|Wiring & Electrical]]** — Power delivery, impedance matching, high-frequency leads
  - *Connection*: Coil load impedance must match supply; connections via pass-throughs
- **[[Design/Wiring/NI-DAQ Control Architecture\|NI-DAQ Control Architecture]]** — PID-based power control
  - *Connection*: Thermocouple feedback → NI-9219 → PID loop → NI-9263 (0-10V power command)

### 📦 Containment
- **[[Design/Vacuum Chamber/Vacuum Enclosure\|Vacuum Chamber]]** — Must fit within chamber envelope
  - *Connection*: Coil must not interfere with chamber walls; clearance critical for sample positioning

### 💧 Cooling
- **[[Design/Plumbing/Fluid Systems\|Plumbing & Fluid Systems]]** — Water-cooling circulation
  - *Connection*: Coil requires water cooling for high-frequency operation; copper tubing + inlet/outlet

### 🎯 Sample Positioning
- **[[Design/Mechanisms/Control System\|Mechanisms & Automation]]** — Sample centering within coil
  - *Connection*: Coil geometry determines sample positioning requirements; ceramic mount must fit inside

### ❄️ Quenching
- **[[Design/Sample Quenching/Quenching Methods\|Sample Quenching Routes]]** — Post-heating cooling
  - *Connection*: Coil heating rate determines quench medium requirements

---

## Design Decisions

**Why Round Coil (Active)?**
- Simpler 3D-print mold manufacturing
- Proven uniform heating capability
- Faster iteration cycle
- Water cooling easier to implement

**Alternative Rejected:**
- **Square Coil** — More complex manufacturing; corner cooling challenges; unknown electrical behavior
- See [[Design/Archive/Design History\|Design Archive]] for full decision rationale

---

## Quick Links

🔗 **CAD Resources:**
- Round Coil: https://cad.onshape.com/...
- FeatureScript: https://cad.onshape.com/documents/f8c5aa176fe5d3504340f7c7/

📖 **Related Reading:**
- [[Design/Vacuum Chamber/Used Vacuum Chamber\|Chamber Integration]] — Mounting constraints
- [[Design/Mechanisms/Ceramic Mount\|Sample Mount]] — Positioning interface
- [[Design/Archive/Design History\|Design History]] — Why round was chosen

---

*Coil geometry is foundational to thermal performance. Changes here cascade to electrical, plumbing, and mechanical subsystems.*
