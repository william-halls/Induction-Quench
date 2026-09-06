---
subsystem: mechanisms
tags: [index, mechanisms, actuation, control, sample-handling]
---

# Mechanisms & Automation Subsystem

Mechanical systems, actuation, control logic, and safety interlocks for repeatable operation and data acquisition.

## Files in This Folder

| File | Purpose | Status |
|------|---------|--------|
| **[[Design/Mechanisms/Ceramic Mount|Ceramic Mount.md]]** | Active sample holder (2-part design) | 🟢 Active |
| **[[Design/Mechanisms/Ball Screw|Ball Screw.md]]** | Stepper-driven vertical shaft actuation | 🟢 Active |
| **[[Design/Mechanisms/Bottom Lift|Bottom Lift.md]]** | Linear actuator approach (deferred) | 🟡 Deferred |
| **[[Design/Mechanisms/Titanium Claw|Titanium Claw.md]]** | Gripper mechanism (abandoned - heat sink) | 🔴 Abandoned |
| **[[Design/Mechanisms/Trapdoor|Trapdoor.md]]** | Rotating release mechanism (deferred) | 🟡 Deferred |



---

## Connected Subsystems

### 🎯 Sample Positioning
- **[[Design/Sample Quenching/Quenching Methods\|Sample Quenching Routes]]** — Quench timing & triggering
  - *Connection*: Ball screw lowers the mounted sample directly into the water bath — the sample stays clamped in its mount the entire time, no release mechanism involved

### 📦 Chamber Integration
- **[[Design/Vacuum Chamber/Vacuum Enclosure\|Vacuum Chamber]]** — Access, door interlock, scissor lift
  - *Connection*: Mount must fit inside chamber; door interlock prevents opening under vacuum; lift supports chamber base

### ⚡ Control Signals
- **[[Design/Wiring/Electrical System\|Wiring & Electrical]]** — Ball screw actuation signals, emergency stop
  - *Connection*: E-stop wiring; thermocouple/pressure sensor inputs. No quench valve exists — quenching is triggered by ball screw immersion, not a valve
- **[[Design/Wiring/NI-DAQ Control Architecture\|NI-DAQ Control Architecture]]** — Automated ball screw control
  - *Connection*: [[Design/Mechanisms/Ball Screw|Ball screw]] motor control via NI-9263 drives the quench-immersion trigger directly; safety monitoring via NI-9219. (If a quench valve is ever added, it would be triggered by a separate SSR controlled by the USB-6009 — not yet implemented)

### 💧 Fluid Control
- **[[Design/Plumbing/Fluid Systems\|Plumbing & Fluid Systems]]** — Water bath fill/drain, pressure relief (no quench valve; bath is pre-filled, sample is immersed by the ball screw)
  - *Connection*: Ball screw immersion is the quench trigger, not a valve; pressure monitoring for safety interlocks

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
5. Heating occurs inside chamber; quench is performed by the [[Design/Mechanisms/Ball Screw|ball screw]] lowering the whole assembly (sample still clamped in the mount) down into the water bath — no release mechanism, the sample never disconnects from the mount
6. Manual extraction after cooling

---

## Rejected/Deferred Designs

**Decided:** quenching uses ball-screw immersion (see [[Design/Sample Quenching/Quenching Methods|Quenching Methods]]) — the sample is lowered into the water bath by the ball screw while still clamped in its mount. No separate sample-release mechanism is used, which is why the designs below (all release/drop mechanisms) remain rejected/deferred rather than pursued further.

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
- Manual ball screw actuation (immersion into water bath) or simple timer
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
