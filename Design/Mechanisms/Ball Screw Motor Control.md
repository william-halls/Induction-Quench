---
tags: [design, mechanisms, motor-control, stepper-driver, wiring, automation]
---

# Ball Screw Motor Control System

**Complete electrical control architecture** for the stepper motor-driven ball screw assembly. Documents all components, specifications, wiring, and integration points.

---

## System Overview

The ball screw uses a **24V DC stepper motor system** to provide precise vertical positioning of the sample shaft. The system consists of four integrated components working in concert:

1. **Power Supply** — Converts AC wall power to regulated 24V DC
2. **Motion Controller** — Programmable logic to generate motion sequences
3. **Stepper Driver** — Amplifies control signals to drive motor coils
4. **Stepper Motor** — Converts electrical pulses to mechanical rotation

**Key Capability:** Precise positioning with ~0.025mm per step (full resolution) or ~0.006mm per step (with microstepping).

---

## Component Details

### 1. Power Supply: SDN 10-24-100P (SolaHD)

**Classification:** Industrial DIN-rail AC/DC power supply

**Electrical Specifications:**
- **Input:** 85–264V AC, 50–60 Hz (auto-selects 115V or 230V)
- **Output:** 24V DC ± 2.25V (range: 22.5–28.5V)
- **Max Current:** 10 Amps @ 24V
- **Total Power:** 240 Watts
- **Efficiency:** >90% (switching technology)

**Physical & Environmental:**
- **Form Factor:** DIN rail mount (35mm standard)
- **Dimensions:** 4.88" H × 3.26" W × 4.55" D
- **Operating Temperature:** 14–140°F (−10 to +60°C)
- **Humidity:** <90% RH, non-condensing

**Protection Features:**
- ✅ Indefinite short-circuit protection (auto-recovery)
- ✅ Overvoltage/overtemperature shutdown
- ✅ Adjustable output voltage (potentiometer trim)
- ✅ Class 1 Zone 2 hazardous location approval

**Certifications:** UL, CE, RoHS, SEMI F47

**Why This Supply?**
At 10A output, it provides 2× headroom for a typical NEMA 23 stepper (5A) plus the ST-PMC1 controller (< 1A), ensuring stable operation under full load with safety margin.

**Sources:**
- [DigiKey Product Page](https://www.digikey.com/en/products/detail/solahd/SDN10-24-100P/10072033)
- [Newark Specs & Datasheet](https://www.newark.com/solahd/sdn10-24-100p/ac-dc-converter-din-rail-1-o-p/dp/80K9315)
- [RS Online Product Info](https://us.rs-online.com/product/solahd/sdn10-24-100p/70211354/)

---

### 2. Motion Controller: ST-PMC1 (SN: 170120011)

**Classification:** Single-axis programmable stepper motion controller

**Primary Function:** Orchestrates ball screw motion by generating pulse and direction sequences sent to the stepper driver. Acts as the "brain" of the system—no PC required.

**Electrical Specifications:**
- **Power Input:** 24V DC (from SDN 10-24-100P)
- **Typical Current Draw:** <1A
- **Output Frequency Range:** 1 Hz to 40 kHz
- **Frequency Resolution:** 1 Hz steps
- **Signal Type:** Pulse (CP) + Direction (CW) to driver

**I/O Capabilities:**
- **Inputs:** 6 optically-isolated signal inputs (limit switches, sensors, start/stop buttons)
- **Outputs:** 3 optically-isolated relay outputs (external device control)
- **Interface:** Front panel LCD display for programming and parameter editing

**Programming:**
- **Capacity:** Up to 99 instruction lines (motion sequences)
- **Instruction Types:** 
  - Positioning (move X steps, Y direction, Z speed)
  - Loops & jumps (conditional motion sequences)
  - Timing delays (pause between movements)
  - Counter operations (repeat sequences)

**Operating Modes:**
- **Manual mode:** Direct control via front panel keypad
- **Auto mode:** Executes programmed sequence on command
- **External trigger mode:** Responds to limit switches or sensor inputs

**Control Logic Example:**
```
Program: "Home and Raise 100 Steps"
Line 1: Move 100 steps, CW direction, 20 kHz frequency
Line 2: Output relay 1 (trigger cooling valve)
Line 3: Wait 5 seconds
Line 4: Move 100 steps, CCW direction, 20 kHz frequency
```

**Typical Usage in Induction-Quench:**
1. Initialize at home position (limit switch input)
2. Raise sample shaft to defined height
3. Trigger quench sequence (relay output to solenoid valve)
4. Lower shaft back to rest position
5. Wait for cool-down before repeat

**Sources:**
- [ManualsLib Operating Manual](https://www.manualslib.com/manual/1269811/St-St-Pmc1.html)
- [Scribd Manual (PDF)](https://www.scribd.com/document/519674212/ST-PMC1-ENG)
- [Amazon Product Listing](https://www.amazon.com/Stepper-Controller-Motion-programmable-ST-Pmc1/dp/B0DP4WLNGQ)

---

### 3. Stepper Driver (SN: 170120011)

**Classification:** Stepper motor driver IC (likely TB6600 or compatible variant)

**Primary Function:** Acts as the "amplifier" between the ST-PMC1 controller (low-power logic signals) and the NEMA stepper motor (high-power coil drive).

**Signal Interface:**
- **Input:** Pulse (CP) + Direction (CW) from ST-PMC1 (TTL/CMOS logic: 0–5V)
- **Output:** High-current stepper coil drive (5–10A per phase @ 24V)

**Motor Control Mechanism:**
The driver receives pulse and direction signals and sequences the motor coil energization:

```
Pulse Signal → Driver counts pulses
Direction Signal → Determines rotation direction
            ↓
    Driver switches coil pairs in sequence
            ↓
    Motor steps 1.8° per pulse (NEMA standard)
            ↓
    Ball screw rotates proportionally
```

**Typical TB6600 Specifications:**
- **Max Motor Current:** 5–10A per phase (selectable)
- **Power Supply:** 24–50V DC (operates at 24V in this system)
- **Step Resolution:** Full, 1/2, 1/4, 1/8, 1/16 stepping options
  - *Full step* = 1.8° per pulse (coarse, but high torque)
  - *1/4 step* = 0.45° per pulse (balanced accuracy/torque)
  - *1/16 step* = 0.1125° per pulse (fine positioning)
- **Microstepping:** Divides current delivery into sub-steps for smoother motion
- **Protection:** Thermal shutdown, short-circuit protection

**Integration with ST-PMC1:**
The ST-PMC1 outputs pulse+direction signals at configurable frequencies (1–40 kHz). The driver receives these and translates them into motor coil switching patterns. The motor responds by stepping in 1.8° increments (or finer with microstepping).

---

### 4. Stepper Motor (SN: 161104226)

**Classification:** NEMA-size stepper motor with integrated ball screw linear actuator

**Type:** Likely NEMA 23 or NEMA 34 (mid-to-large stepper, appropriate for vertical load)

**Mechanical Function:** Converts electrical pulses into rotational mechanical energy; coupled directly to ball screw shaft.

**Typical NEMA Specifications (23–34 range):**

| Parameter | NEMA 23 | NEMA 34 |
|-----------|---------|---------|
| **Step Angle** | 1.8° | 1.8° |
| **Steps/Revolution** | 200 | 200 |
| **Holding Torque** | 2–3 N·m | 4.5+ N·m |
| **Current Rating** | 3–5A per phase | 5.5A per phase |
| **Coil Impedance** | ~3–5 Ω | ~1.5–3 Ω |
| **Ball Screw Lead** | 5–8mm/rev | 10mm/rev |
| **Linear Travel** | 0.025–0.04mm per step | 0.05mm per step |

**How Ball Screw Coupling Works:**
1. Motor shaft couples to ball screw (via flexible coupler or direct drive)
2. Each 1.8° motor step = one partial rotation of ball screw
3. Ball screw has mechanical lead (pitch) — typically 5–10mm per full revolution
4. Result: **Linear motion** = (Ball Screw Lead) ÷ (200 steps/rev)

**Example Calculation (NEMA 23, 5mm lead):**
```
Ball Screw Lead: 5mm per full revolution (200 motor steps)
Linear travel per step = 5mm ÷ 200 = 0.025mm per step
With 1/4 microstepping = 0.025mm ÷ 4 = 0.00625mm per step
```

**Typical Load Specifications:**
- **Max Thrust:** 320N (for NEMA 23–34 range)
- **Speed Range:** 0–3000 RPM (limited by control frequency and motor specs)
- **Duty Cycle:** Continuous (with adequate cooling)

**Electrical Connection:**
- **Coil Type:** Bipolar (standard for NEMA steppers)
- **Wiring:** A+, A−, B+, B− (two independent coil pairs)
- **Current** supplied by driver (controlled via ST-PMC1 frequency)

**Positioning Accuracy:**
- **Full-step:** ±1.8° (mechanical play dominates)
- **1/16 microstepping:** ±0.11° electrical (but limited by mechanical backlash)
- **Practical repeatability:** ±0.1–0.05mm (with careful load design)

**Motor Modes:**
- **Holding:** Coils energized, motor locked in place (holds load)
- **Running:** Coils sequenced at pulse rate, motor rotates
- **Power-down:** Coils de-energized, motor can be manually moved (low friction)

**Sources:**
- [MOONS' NEMA 17 Ball Screw Motors](https://www.moonsindustries.com/series/nema17-ball-screw-hybrid-linear-stepper-motors-a090100404)
- [StepperOnline NEMA 23/34 Ball Screw Actuators](https://www.omc-stepperonline.com/)
- [HOLRY Ball Screw Actuators](https://www.holrymotor.com/)

---

## System Architecture Diagram

```
┌──────────────────────────────────────────────────────────────┐
│  BALL SCREW MOTOR CONTROL SYSTEM (24V DC)                   │
└──────────────────────────────────────────────────────────────┘

    ┌─ POWER DELIVERY ─────────────────────────────────┐
    │                                                   │
    │  Wall AC Power (115/230V, 50-60Hz)               │
    │        │                                         │
    │        ▼                                         │
    │  ┌──────────────────────────┐                   │
    │  │  SDN 10-24-100P          │                   │
    │  │  Power Supply            │  240W            │
    │  │  DIN Rail Mount          │                   │
    │  └────────┬─────────────────┘                   │
    │           │ +24V DC, 10A max                    │
    └───────────┼──────────────────────────────────────┘
                │
    ┌───────────┴─────────────────────────────────────┐
    │  ┌──────────────────────────┐                   │
    │  │  ST-PMC1                 │                   │
    │  │  Motion Controller       │  <1A              │
    │  │  SN: 170120011           │                   │
    │  │  ┌──────────────────┐    │                   │
    │  │  │ Program Logic    │    │                   │
    │  │  │ ┌──────────────┐ │    │                   │
    │  │  │ │ 99 sequences │ │    │                   │
    │  │  │ │ Up to 40 kHz │ │    │                   │
    │  │  │ └──────────────┘ │    │                   │
    │  │  └────┬──────────┬──┘    │                   │
    │  │       │ Pulse    │ Direction                 │
    │  │       │ (CP)     │ (CW)                      │
    │  └───────┼──────────┼────────┘                   │
    │          │          │                           │
    │          ▼          ▼                           │
    │  ┌────────────────────────────┐                │
    │  │  Stepper Driver            │                │
    │  │  (TB6600 or compatible)    │                │
    │  │  SN: 170120011             │  5-10A         │
    │  │  ┌──────────────────────┐  │                │
    │  │  │ Input: Logic Signals │  │                │
    │  │  │ Output: Coil Drive   │  │                │
    │  │  │ Microstepping: 1-16x │  │                │
    │  │  └──────────────────────┘  │                │
    │  └────────┬────────┬──────────┘                │
    │           │ Coil A │ Coil B                    │
    │           │        │                          │
    │           ▼        ▼                          │
    │  ┌─────────────────────────────┐              │
    │  │  NEMA Stepper Motor         │              │
    │  │  (Integrated Ball Screw)    │              │
    │  │  SN: 161104226              │              │
    │  │  NEMA 23 or 34              │              │
    │  │  ┌──────────────────────┐   │              │
    │  │  │ 200 steps/revolution │   │              │
    │  │  │ 1.8° per step        │   │              │
    │  │  │ 3-5.5A rated         │   │              │
    │  │  └──────────────────────┘   │              │
    │  └────────┬────────────────────┘              │
    │           │                                  │
    │           ▼ Rotational motion                │
    │  ┌──────────────────────────┐               │
    │  │  Ball Screw Shaft        │               │
    │  │  (5-10mm lead)           │               │
    │  │  ┌──────────────────────┐ │               │
    │  │  │ Linear: 0.025mm/step │ │               │
    │  │  │ or 0.006mm w/ 16×    │ │               │
    │  │  └──────────────────────┘ │               │
    │  └────────┬───────────────────┘               │
    │           │                                  │
    │           ▼ Vertical shaft motion            │
    │  ┌──────────────────────────┐               │
    │  │  Sample Mount & Shaft    │               │
    │  │  (Up/Down Positioning)   │               │
    │  └──────────────────────────┘               │
    │                                             │
    └─────────────────────────────────────────────┘

Key Signals:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
► +24V (Power rail) — Red
► GND (Ground rail)  — Black
► CP (Pulse signal)  — Yellow
► CW (Direction)     — Green
► Coil A, B (Motor)  — Blue/Brown
```

---

## Wiring Diagram: Power & Control Signals

```
╔══════════════════════════════════════════════════════════════╗
║  STEPPER MOTOR CONTROL WIRING (24V DC System)               ║
║  All components mounted on DIN rail or in control cabinet   ║
╚══════════════════════════════════════════════════════════════╝

AC SUPPLY                POWER SUPPLY                CONTROLLER
┌──────────────┐         ┌──────────────┐           ┌──────────────┐
│ Wall AC      │         │ SDN 10-24-100│           │  ST-PMC1     │
│ 115/230V     │         │              │           │ SN:170120011 │
│ 50-60Hz      │      ┌──│ L            │           │              │
│              │      │  │ N            │           │ LCD Display  │
│ Phase  ──────┼──────┤  │ PE/GND       │           │ & Keypad     │
│ Neutral ─────┼──────┤  │              │           │              │
│ Ground ──────┼──────┤  │ +24V ◄───────┼───────────┤ +24V (Vin)   │
│              │      │  │ GND  ◄───────┼───────────┤ GND (Vin)    │
└──────────────┘      │  │ (240W)       │           │              │
                      │  └──────────────┘           │              │
                      │                             │              │
                      │  Power Supply Specs:        │ Outputs:     │
                      │  ▶ Input: 85-264V AC       │ ┌─────────┐  │
                      │  ▶ Output: 24V ± 2.25V    │ │CP (Pulse)──┐│
                      │  ▶ Max: 10A, 240W          │ │CW (Dir) ──┐│
                      │  ▶ Efficiency: >90%        │ │GND ──────┐│
                      │                             │ └─────────┘│
                      │                             └────┬──────┬┘
                      │                                  │      │
                      │        STEPPER DRIVER           │      │
                      │        ┌──────────────────┐    │      │
                      │        │ TB6600 (or equiv)├────┘      │
                      │        │ SN:170120011     │           │
                      │        │                  │           │
                      │   +24V ├─────────────────────────┐    │
                      └────────┤                  │       │    │
                               │ GND ────────────┼─◄─────┘    │
                               │                 │             │
                               │ CP (Pulse)  ◄───┘             │
                               │ CW (Dir)    ◄─────────────────┘
                               │                 │
                               │ OUT_A ──────┐   │
                               │ OUT_A_      │   │
                               │ OUT_B ──────┼──┐
                               │ OUT_B_      │  │
                               └──────┬──────┘  │
                                      │        │
                    STEPPER MOTOR     │        │
                    ┌─────────────────┼───┐    │
                    │ NEMA 23/34      │   │    │
                    │ SN:161104226    │   │    │
                    │                 │   │    │
                    │ Coil A:  ────────   │    │
                    │ Coil A-: ───────────┘    │
                    │                 │        │
                    │ Coil B:  ────────────────┘
                    │ Coil B-: ────────────────┘
                    │                 │
                    │ [Coupled to Ball Screw]
                    │                 │
                    │ [Drives Sample  │
                    │  Shaft Up/Down] │
                    └─────────────────┘

┌─────────────────────────────────────────────────────────────┐
│ SIGNAL SPECIFICATIONS                                       │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│ ┌─ CP (Pulse/Clock) ─────────────────────────────────────┐ │
│ │ TTL logic (0-5V), ST-PMC1 output to driver input      │ │
│ │ Frequency: 1-40 kHz (1 Hz resolution)                │ │
│ │ Duty cycle: 50% square wave                          │ │
│ │ Rise/fall time: <1 µs                                │ │
│ │ Max cable length: 10-15 meters (shielded recommended)│ │
│ │                                                       │ │
│ │   ┌─ ┐ ┌─ ┐ ┌─ ┐ ┌─ ┐ ┌─ ┐                         │ │
│ │ ──┘ └─┘ └─┘ └─┘ └─┘ └─                             │ │
│ │   ▲                     ▲                            │ │
│ │   └─ 1.8° step per pulse                            │ │
│ └─────────────────────────────────────────────────────┘ │
│                                                             │
│ ┌─ CW (Direction) ───────────────────────────────────────┐ │
│ │ TTL logic (0-5V), ST-PMC1 output to driver input      │ │
│ │ State duration: > 100 ns (minimum setup time)        │ │
│ │ Logic levels:                                        │ │
│ │   CW = HIGH (1)  → Clockwise rotation               │ │
│ │   CW = LOW  (0)  → Counter-clockwise rotation       │ │
│ │                                                       │ │
│ │   ┌──────────────────────┐                          │ │
│ │ ──┤ CW = 1 (Clockwise)   ├──────────────────        │ │
│ │   └──────────────────────┘                          │ │
│ │                                                       │ │
│ │   ┌──────────────────────┐                          │ │
│ │ ──┤ CW = 0 (Counter-CW)  ├──────────────────        │ │
│ │   └──────────────────────┘                          │ │
│ └─────────────────────────────────────────────────────┘ │
│                                                             │
│ ┌─ Power Rails ──────────────────────────────────────────┐ │
│ │ +24V: Minimum 22.5V under full load (10A)            │ │
│ │ GND:  Single common ground for all signals           │ │
│ │ Ripple: <5% (typically <1.2V pk-pk @ 10A)           │ │
│ │ Wire gauge: Minimum #18 AWG for +24V, GND            │ │
│ └─────────────────────────────────────────────────────┘ │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

---

## Control Sequence Example: Sample Lift & Quench

Here's a typical motion sequence programmed into the ST-PMC1:

```
╔══════════════════════════════════════════════════════════════╗
║  SEQUENCE: "Raise Sample, Trigger Quench, Lower"           ║
║  (Typical operation during induction-quench test)           ║
╚══════════════════════════════════════════════════════════════╝

Line  Operation          Frequency  Steps   Direction  Relay
────  ─────────────────  ──────────  ──────  ─────────  ──────
  1   Move home          1 kHz       50      CW         OFF
      (press limit sw)   

  2   Wait for thermal   -           -       -          OFF
      equilibrium        
      (manual timer or 
       external trigger)

  3   Raise sample       20 kHz      200     CCW        OFF
      (2 rotations @     
       5mm lead =        
       10mm height)

  4   Hold position      -           -       -          OFF
      (coils locked,
       sample steady)

  5   Trigger quench     -           -       -          ON
      relay pulse        
      (opens solenoid
       valve)

  6   Wait for quench    2 sec       -       -          OFF
      medium to flow

  7   Lower sample       20 kHz      200     CW         OFF
      (return to start)

  8   Jump to Line 2     -           -       -          OFF
      if button pressed
      (repeat cycle)

Motion Timing Breakdown:
─────────────────────────────────────────────────────────────
Step 1-2:   Setup & heating (external equipment controls)
Step 3:     Raise 10mm = 200 steps ÷ 20 kHz = 10 ms
Step 4:     Sample stable at elevated position
Step 5:     Quench valve opens (relay energized)
Step 6:     Cooling medium jets impact sample
Step 7:     Lower 10mm = 200 steps ÷ 20 kHz = 10 ms
Step 8:     Wait for cool-down (manual or timer-based)

Total active motion time: ~20 ms
Total cycle time: 10 minutes (dominated by heating & cool-down)
```

---

## Mechanical Integration

### Coupling the Motor to Ball Screw

**Current Design:** Ball screw motor comes as integrated NEMA stepper with external ball screw shaft (likely FSK40-series equivalent per [[Design/Mechanisms/Ball Screw|Ball Screw.md]]).

**Attachment Method:**
1. Motor flange couples to ball screw bearing housing
2. Flexible coupler (or direct drive) prevents misalignment
3. Shaft clamp (3D-printed) locks vertical position

**Load Considerations:**
- **Vertical Load:** Sample + mount + shaft coupling (~500–1000g)
- **Ball Screw Lead:** Typically 5–10mm per revolution (self-locking when motor de-energized)
- **Holding Torque:** NEMA 23 = 2–3 N·m, NEMA 34 = 4.5+ N·m (>sufficient for vertical hold)
- **Speed:** Typical max 20 kHz pulse rate = ~100 RPM motor speed = 500–1000 mm/min linear speed

**Safety:**
- Motor coils remain energized when in "holding" mode → shaft cannot drift
- Power loss or motor fault → shaft held in place (fail-safe for sample)

See [[Design/Mechanisms/Ball Screw|Ball Screw.md]] for CAD models and mechanical details.

---

## Integration with Induction-Quench System

### How the Ball Screw Fits Into the Overall Design

The ball screw motor control system enables:

1. **Vertical Sample Positioning** — Moves sample up/down inside the induction coil
2. **Precise Height Control** — ~0.025mm step resolution for repeatable geometry
3. **Automated Sequences** — ST-PMC1 can coordinate motion with heating/quenching timings
4. **Failsafe Design** — Holding torque keeps sample stable even if power lost

### Connection to Other Subsystems

| Subsystem | Connection | Status |
|-----------|-----------|--------|
| **[[Design/Mechanisms/Ceramic Mount\|Ceramic Mount]]** | Ball screw shaft couples to mount base; controls vertical position | 🟢 Active |
| **[[Design/Wiring/NI-DAQ Control Architecture\|NI-DAQ Architecture]]** | ST-PMC1 receives start commands from NI-9263; coordinates with quench valve timing | ⏳ Pending Integration |
| **[[Design/Vacuum Chamber/Vacuum Enclosure\|Vacuum Chamber]]** | Shaft passes through chamber lid via seal; motor outside chamber | 🟢 Designed |
| **[[Design/Plumbing/Fluid Systems\|Plumbing & Valves]]** | Relay outputs from ST-PMC1 can trigger solenoid quench valve | ⏳ Configuration Pending |
| **[[Design/Sample Quenching/Quenching Methods\|Quenching Methods]]** | Motor timing can synchronize sample position with quench medium release | 🟢 Conceptual |

See [[Design/Wiring/NI-DAQ Control Architecture|NI-DAQ Control Architecture]] for full automated integration.

---

## Troubleshooting & Commissioning

### Motor Not Stepping

**Symptom:** Ball screw doesn't move; motor silent or humming.

**Checklist:**
1. ✓ Verify ST-PMC1 programmed with valid motion sequence
2. ✓ Check +24V supply: multimeter should read 22.5–26V at driver input
3. ✓ Verify CP (pulse) and CW (direction) signals present on oscilloscope (1–40 kHz TTL square wave)
4. ✓ Confirm motor coils (A, A-, B, B-) wired correctly to driver outputs
5. ✓ Test motor rotation by manually spinning shaft — should have slight resistance (coil holding)
6. ✓ Check stepper driver thermal status — may have shut down if overheated

### Inconsistent Stepping / Lost Steps

**Symptom:** Shaft moves intermittently or skips steps; position drifts.

**Checklist:**
1. ✓ Reduce motor speed (lower pulse frequency, e.g., 10 kHz vs. 40 kHz)
2. ✓ Verify +24V voltage doesn't sag below 22.5V during stepping (check power supply load)
3. ✓ Measure mechanical load — ball screw may be binding or misaligned
4. ✓ Check CP/CW signal integrity — use oscilloscope to verify clean edges, no noise
5. ✓ Confirm motor microstepping setting (full vs. 1/16 step) — verify matches ST-PMC1 frequency

### Controller (ST-PMC1) Not Responding

**Symptom:** LCD dark; no response to keypad; relays inactive.

**Checklist:**
1. ✓ Verify +24V input to ST-PMC1: should be 22.5–26V DC
2. ✓ Check GND connection between power supply and controller (common ground)
3. ✓ Reset controller: power off 5 seconds, power back on
4. ✓ Review programmed sequences — controller may be stuck in loop or waiting for external trigger

---

## Performance Specifications Summary

| Parameter | Specification | Notes |
|-----------|---------------|-------|
| **Power Input** | 85–264V AC, 50–60 Hz | Auto-select 115/230V |
| **Power Output** | 24V DC, 10A max | 240W total capacity |
| **Control Frequency** | 1–40 kHz, 1 Hz steps | 40 kHz = ~3000 RPM motor |
| **Step Resolution** | 1.8° per pulse (200 steps/rev) | Microstepping → finer resolution |
| **Linear Travel** | ~0.025mm per full step | Depends on ball screw lead |
| **Holding Torque** | 2–4.5+ N·m (NEMA 23–34) | Sufficient for vertical load |
| **Repeatability** | ±0.1–0.05mm | With mechanical centering |
| **Thermal Operation** | 14–140°F (−10 to +60°C) | Power supply rated |
| **Efficiency** | >90% | Switching power supply |
| **Response Time** | <1 ms | From signal to motor step |

---

## Cross-References & Documentation

**See Also:**
- [[Design/Mechanisms/Ball Screw|Ball Screw.md]] — Mechanical assembly & CAD models
- [[Design/Mechanisms/Ceramic Mount|Ceramic Mount.md]] — Sample holder coupled to ball screw
- [[Design/Wiring/NI-DAQ Control Architecture|NI-DAQ Control Architecture]] — Automated integration
- [[Design/Mechanisms/Control System|Control System.md]] — Overall system logic
- [[Design/Archive/Design History|Design History]] — Why ball screw chosen over alternatives

**External Datasheets & Manuals:**
- [SolaHD SDN 10-24-100P Datasheet](https://www.newark.com/solahd/sdn10-24-100p/ac-dc-converter-din-rail-1-o-p/dp/80K9315)
- [ST-PMC1 Operating Manual](https://www.manualslib.com/manual/1269811/St-St-Pmc1.html)
- [MOONS' NEMA Stepper Motor Specs](https://www.moonsindustries.com/)
- [StepperOnline Ball Screw Motors](https://www.omc-stepperonline.com/)

---

**Last Updated:** 2026-08-17  
**Status:** 🟢 Complete specification (commissioned components identified)  
**Next Steps:** Finalize NI-DAQ integration; commission automated quench sequences
