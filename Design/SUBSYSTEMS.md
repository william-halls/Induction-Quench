---
tags: [navigation, subsystems, interconnections, design-map]
---

# Design Subsystems Map

**Complete interconnected overview** of all 7 design subsystems with their relationships and dependencies.

---

## Subsystem Index

Each folder contains an INDEX.md with detailed connections:

| Subsystem | Focus | Index |
|-----------|-------|-------|
| **🔥 Coil Geometry** | Induction heating element | [[Design/Coil Geometry/INDEX\|View Index]] |
| **📦 Vacuum Chamber** | Inert atmosphere enclosure | [[Design/Vacuum Chamber/INDEX\|View Index]] |
| **🔧 Mechanisms** | Sample mount & actuation | [[Design/Mechanisms/INDEX\|View Index]] |
| **💧 Plumbing** | Gas, vacuum, cooling lines | [[Design/Plumbing/INDEX\|View Index]] |
| **❄️ Quenching** | Cooling strategy & samples | [[Design/Sample Quenching/INDEX\|View Index]] |
| **⚡ Wiring** | Power & instrumentation | [[Design/Wiring/INDEX\|View Index]] |
| **📜 Archive** | Rejected designs & lessons | [[Design/Archive/INDEX\|View Index]] |

---

## System Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│  INDUCTION QUENCH RESEARCH INSTRUMENT                           │
│  1000°C Thermal Testing with Controlled Quenching               │
└─────────────────────────────────────────────────────────────────┘

                    ⚡ POWER DELIVERY LAYER
                  [[Design/Wiring/INDEX|Wiring]]
                 (~1 MHz, E-stop, Safety)
                           ↓
    ┌──────────────────────┼──────────────────────┐
    ↓                      ↓                      ↓
  🔥 HEATING          📦 CHAMBER          💧 COOLING
  [[Design/Coil       [[Design/Vacuum     [[Design/Plumbing
   Geometry/INDEX]]    Chamber/INDEX]]     /INDEX]]
  
  (Induction Coil)    (Bucket Vessel)     (Gas/Vacuum)
  (Water-Cooled)      (Stainless Steel)   (Argon Backfill)
  (Uniform Heat)      (4L Capacity)       (Pressure Relief)
                            ↓
                    ┌───────┼───────┐
                    ↓       ↓       ↓
               🔧 SAMPLE   ❄️ QUENCH  ⚙️ CONTROL
               [[Design/    [[Design/   [[Design/
                Mechanisms   Sample Q.   Mechanisms
                /INDEX]]     /INDEX]]    /INDEX]]
                
              (Ceramic      (Oil/Water/  (Manual
               Mount)       Gas)         Trigger)
              (2-part)      (TBD)        (Prototype)
                            (Rapid Cool)
                            
                            ↓
                    ┌───────────────┐
                    ↓               ↓
                📊 DATA          📜 HISTORY
                (Thermocouple)   [[Design/Archive
                (Pressure)        /INDEX]]
                (Timing)          (Lessons Learned)
```

---

## Information Flow During Test

### Pre-Test Setup
1. **Sample Prep** → [[Design/Sample Quenching\|Quenching]] (charpy geometry)
2. **Mount Install** → [[Design/Mechanisms/Ceramic Mount\|Ceramic Mount]]
3. **Chamber Prep** → [[Design/Vacuum Chamber\|Vacuum Chamber]] + [[Design/Plumbing\|Plumbing]]
4. **System Check** → [[Design/Wiring\|Electrical]] interlocks verified

### During Heating
1. **Power On** → [[Design/Wiring/INDEX\|Wiring]] activates supply
2. **Coil Energize** → [[Design/Coil Geometry/Round Coil\|Round Coil]] heats sample
3. **Temperature Rise** → [[Design/Vacuum Chamber\|Chamber]] maintains inert atmosphere
4. **Cooling Flow** → [[Design/Plumbing/Fluid Systems\|Cooling water]] circulates through coil
5. **Monitoring** → [[Design/Wiring\|Thermocouple]] records thermal profile

### During Quench (Critical Phase)
1. **Trigger Event** → [[Design/Mechanisms/Control System\|Manual valve activation]]
2. **Medium Release** → [[Design/Plumbing/Fluid Systems\|Quench system]] delivers cooling medium
3. **Rapid Cool** → [[Design/Sample Quenching/Quenching Methods\|Sample cools]] to room temp
4. **Data Capture** → [[Design/Wiring\|Pressure spike]] recorded; timing logged

### Post-Test
1. **Extraction** → [[Design/Mechanisms/Ceramic Mount\|Manual extraction]] of sample
2. **Chamber Vent** → [[Design/Plumbing/Vacuum Lid Systems\|Emergency vent]] for access
3. **Sample Analysis** → Microstructure evaluation begins

---

## Critical Dependencies

### Thermal Path
**Coil → Sample → Quench Medium → Chamber Walls → External Cooling**
- Dependency: Coil uniformity determines heating success
- Dependency: Chamber thermal mass affects cooling rate
- Critical: Water-cooled coil must handle high-frequency losses

**Subsystems Involved:**
- [[Design/Coil Geometry\|Heating]]
- [[Design/Vacuum Chamber\|Containment]]
- [[Design/Plumbing\|Cooling loops]]

### Vacuum Path
**Pump → Air Control Assembly → Chamber → Vent**
- Dependency: Vacuum level affects water boil-off and cooling efficiency
- Dependency: Pressure relief prevents over-pressurization during quench
- Critical: Leak-tight throughout

**Subsystems Involved:**
- [[Design/Plumbing\|Air systems]]
- [[Design/Vacuum Chamber\|Chamber integrity]]
- [[Design/Wiring\|Pressure monitoring]]

### Control Path
**User Input → Solenoid Valve → Quench Medium → Sample**
- Dependency: Timing between end of heating and start of quench critical
- Dependency: Emergency stop must kill all power immediately
- Critical: Interlock prevents heating without vacuum

**Subsystems Involved:**
- [[Design/Mechanisms/Control System\|Manual/automated control]]
- [[Design/Wiring\|Electrical signals]]
- [[Design/Plumbing\|Valve actuation]]

### Data Path
**Thermocouple → Feedthrough → Amplifier → Logger**
- Dependency: Thermocouple must survive 1000°C + rapid quench
- Dependency: Signal integrity critical for analysis
- Critical: Cold-junction compensation accurate

**Subsystems Involved:**
- [[Design/Sample Quenching\|Sample measurement]]
- [[Design/Plumbing/Thermal Couple Pass-through\|Feedthrough]]
- [[Design/Wiring\|Instrumentation]]

---

## Design Decisions by Subsystem

### [[Design/Coil Geometry\|Coil Geometry]]
- ✅ **Active**: Round coil (simple manufacturing, proven heating)
- ⏸️ **Deferred**: Square coil (complex cooling at corners)
- 🔗 **Connects To**: Coil lead pass-throughs, vacuum chamber volume, water cooling

### [[Design/Vacuum Chamber\|Vacuum Chamber]]
- ✅ **Active**: Repurposed stainless steel bucket (proven, fast)
- ❌ **Abandoned**: Quartz glass tube (sealing too complex)
- 🔗 **Connects To**: All subsystems via lid feedthroughs

### [[Design/Mechanisms\|Mechanisms]]
- ✅ **Active**: Ceramic mount (simple, no heat sink effect)
- ⏸️ **Deferred**: Bottom lift, trapdoor (incompatible with scanning)
- ❌ **Abandoned**: Titanium claw (heat sink effect discovered in FEA)
- 🔗 **Connects To**: Sample geometry, coil positioning, manual control

### [[Design/Plumbing\|Plumbing]]
- ✅ **Active**: Air control assembly, coil lead pass-throughs, shaft seal
- ⏳ **Pending**: Thermocouple feedthrough design, quench medium selection
- 🔗 **Connects To**: Vacuum pump, argon supply, cooling water, quench valve

### [[Design/Sample Quenching\|Sample Quenching]]
- ✅ **Reference**: Charpy geometries (standard + modified)
- ⏳ **Pending**: Quench medium selection (oil/water/gas evaluation)
- 🔗 **Connects To**: Cooling rate targets, thermal stress analysis

### [[Design/Wiring\|Wiring & Electrical]]
- ✅ **Defined**: Power frequency (~1 MHz), safety interlocks, emergency stop
- ⏳ **Pending**: Power supply selection, thermocouple amplifier, data logger
- 🔗 **Connects To**: All subsystems (power, control, instrumentation)

### [[Design/Archive\|Archive & History]]
- 📜 **Reference**: Why each design was chosen/rejected
- 🎓 **Lessons**: Simplicity wins, simulation catches flaws, material matters
- 🔗 **Informs**: Future design iterations and decisions

---

## How to Navigate

### By Question
- **"What heats the sample?"** → [[Design/Coil Geometry/INDEX\|Coil Geometry INDEX]]
- **"How is vacuum maintained?"** → [[Design/Vacuum Chamber/INDEX\|Vacuum Chamber INDEX]] + [[Design/Plumbing/INDEX\|Plumbing INDEX]]
- **"How does sample get positioned?"** → [[Design/Mechanisms/INDEX\|Mechanisms INDEX]]
- **"How is the sample cooled?"** → [[Design/Sample Quenching/INDEX\|Quenching INDEX]] + [[Design/Plumbing/INDEX\|Plumbing INDEX]]
- **"Why wasn't design X chosen?"** → [[Design/Archive/INDEX\|Archive INDEX]]

### By Subsystem
- Click any subsystem index link above
- Each INDEX.md shows all files in folder + connections to other folders

### By Physical Location
- **In the chamber:** Coil geometry + sample mount + thermocouple feedthrough
- **In the lid:** Air control assembly, power lead pass-throughs, pressure gauge port
- **Outside chamber:** Power supply, cooling water, vacuum pump, argon bottle, quench medium

---

## Master Checklist: Before Running Experiment

**Pre-Test Verification** — All subsystems ready?

- [x] **Coil** — Round coil assembled, water cooling connected (see [[Design/Coil Geometry\|Coil]])
- [ ] **Chamber** — Vacuum verified, lid tight, all feedthroughs sealed (see [[Design/Vacuum Chamber\|Chamber]])
- [ ] **Sample Mount** — Ceramic mount clean, sample prepared (see [[Design/Mechanisms\|Mechanisms]])
- [ ] **Plumbing** — Vacuum pump running, argon pressure set, quench medium ready (see [[Design/Plumbing\|Plumbing]])
- [ ] **Power** — Power supply set to target frequency, emergency stop tested (see [[Design/Wiring\|Wiring]])
- [ ] **Thermocouple** — Sensor seated, feedthrough connected, amplifier powered (see [[Design/Wiring\|Wiring]])
- [ ] **Safety** — Interlock verified (no heating without vacuum), E-stop accessible (see [[Design/Archive\|Safety Notes]])

---

*This map shows how seven interdependent subsystems work together to create a 1000°C induction quench testing instrument.*
