---
tags: [index, mechanisms, actuation, control, sample-handling]
---

# Mechanisms & Automation Subsystem

Mechanical systems, actuation, control logic, and safety interlocks for repeatable operation and data acquisition.

## Files in This Folder

| File | Purpose | Status |
|------|---------|--------|
| **[[Design/Mechanisms/Control System|Control System.md]]** | Overview & integration points | 🔵 Hub |
| **[[Design/Mechanisms/Ceramic Mount|Ceramic Mount.md]]** | Active sample holder (2-part design) | 🟢 Active |
| **[[Design/Mechanisms/Ball Screw|Ball Screw.md]]** | Stepper-driven vertical shaft actuation | 🟢 Active |
| **[[Design/Mechanisms/Ball Screw Motor Control|Ball Screw Motor Control.md]]** | Complete control system (power supply, controller, driver, motor) | 🟢 Complete |
| **[[Design/Mechanisms/Bottom Lift|Bottom Lift.md]]** | Linear actuator approach (deferred) | 🟡 Deferred |
| **[[Design/Mechanisms/Titanium Claw|Titanium Claw.md]]** | Gripper mechanism (abandoned - heat sink) | 🔴 Abandoned |
| **[[Design/Mechanisms/Trapdoor|Trapdoor.md]]** | Rotating release mechanism (deferred) | 🟡 Deferred |

---

## Connected Subsystems

### 🎯 Sample Positioning
- **[[Design/Sample Quenching/Quenching Methods\|Sample Quenching Routes]]** — Quench timing & triggering
  - *Connection*: Mount geometry determines how sample is released or positioned for quenching

### 📦 Chamber Integration
- **[[Design/Vacuum Chamber/Vacuum Enclosure\|Vacuum Chamber]]** — Access, door interlock, scissor lift
  - *Connection*: Mount must fit inside chamber; door interlock prevents opening under vacuum; lift supports chamber base

### ⚡ Control Signals
- **[[Design/Wiring/Electrical System\|Wiring & Electrical]]** — Valve/pump actuation signals, emergency stop
  - *Connection*: E-stop wiring; solenoid valve control; thermocouple/pressure sensor inputs
- **[[Design/Wiring/NI-DAQ Control Architecture\|NI-DAQ Control Architecture]]** — Automated ball screw & valve control
  - *Connection*: [[Design/Mechanisms/Ball Screw|Ball screw]] motor control via NI-9263; quench valve automation; safety monitoring via NI-9219

### 💧 Fluid Control
- **[[Design/Plumbing/Fluid Systems\|Plumbing & Fluid Systems]]** — Quench valve, pump relay, pressure relief
  - *Connection*: Mechanism triggers quench valve opening; pressure monitoring for safety interlocks

### 🔥 Thermal System
- **[[Design/Coil Geometry/Induction Coil\|Coil Geometry]]** — Sample centering within coil
  - *Connection*: Mount must position sample uniformly within coil for even heating

---

## Sample Mounting System

### Current Design: Ceramic Mount (Active)

**Why Chosen:**
- ✓ Simplicity (2 parts only)
- ✓ No heat sink effect (boron nitride is poor conductor)
- ✓ Natural centering via quartz tube walls
- ✓ Uses existing materials (boron nitride scrap)
- ✓ Easy replacement if ceramic cracks

**Operation:**
1. Modify charpy sample to 6mm length
2. Insert into ceramic cylinder groove
3. Screw steel shaft to lock
4. Slide complete assembly into quartz tube
5. Heating + quenching occur inside chamber
6. Manual extraction after cooling

---

## Rejected/Deferred Designs

### Abandoned: Titanium Claw
**Issue:** Fundamental flaw — thermal performance
- Titanium grips conducted heat away from charpy
- Created uneven temperature gradient → uneven microstructure
- Discovered via FEA (not prototype testing)

### Deferred: Bottom Lift
**Issues:** Seal temperature rating too low; centering complexity
- Reconsider if: Better seals available (rated 1000°C+)

### Deferred: Trapdoor
**Issue:** Incompatible with scanning requirement
- Project needs: Heat longer samples via vertical oscillation
- Problem: Trapdoor + scanning = complex timing
- Reconsider if: Requirements change; high-throughput mode needed

**See** [[Design/Archive/Design History\|Design Archive]] **for full engineering analysis.**

---

## Control Strategy (Prototype Phase)

**Manual Operation:**
- Temperature ramp via power supply controls
- Manual quench valve actuation or simple timer
- Thermocouple monitoring (external equipment)
- Safety: Vacuum pressure monitoring required before heating

**Future Automation:**
- Programmed temperature ramp profiles
- Timer-based or temperature-triggered quench
- Integrated data logging system
- Advanced safety interlocks

---

## Quick Links

📖 **Related Reading:**
- [[Design/Vacuum Chamber/Vacuum Enclosure\|Chamber Integration]] — Mounting interface
- [[Design/Sample Quenching/Quenching Methods\|Quenching Methods]] — Cooling strategies
- [[Design/Archive/Design History\|Design History]] — Why Ceramic Mount chosen; lessons from other designs

🔗 **CAD Resources:**
- Ceramic Mount: https://cad.onshape.com/...
- Control System Assembly: https://cad.onshape.com/...

---

*Sample mount is the critical interface between heating and quenching. Simplicity and thermal isolation are key.*
