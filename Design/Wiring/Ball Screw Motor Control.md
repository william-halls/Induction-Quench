---
subsystem: mechanisms
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

## ⚡ Quick Reference: Confirmed Wiring & Controller Navigation

*Everything below is verified against the physical unit (see Commissioning Log at the end of this doc for the full history). Detailed explanations for each item live in the sections further down.*

### Wiring: ST-PMC1 Controller → Stepper Driver

**Power:**

| PSU (SDN 10-24-100P) | ST-PMC1 |
|---|---|
| +24V | +24V |
| GND | GRD |

**Confirmed physical ports on the ST-PMC1 back panel:**
- **Back left:** `+24V, GRD, OUT3, OUT2, OUT1, OPTO, CW, CP`
- **Back right:** `Com+, Com−, IN2, IN1, A, B, Stop, RUN`

**Signal wiring to the driver's PU/DR/MF terminal block:**

```
ST-PMC1 OPTO ──┬── Driver PU+
               └── Driver DR+

ST-PMC1 CP  ────────── Driver PU−
ST-PMC1 CW  ────────── Driver DR−

Driver MF+ / MF− ─── left unconnected (no matching enable output on ST-PMC1)
```

- **OPTO** is the controller's own isolated common/pull-up rail for the pulse+direction outputs — supplies the `+` side of both driver opto-inputs, so no external 5V source is needed.
- **CP** (pulse) sinks low to generate each step edge → feeds **PU−**.
- **CW** (direction) sinks low/high to set rotation direction → feeds **DR−**.
- Keep OPTO separate from GRD — don't tie them together (avoids ground loops).

**Motor coil wiring (driver → motor):** Standard bipolar 4-wire — A+, A−, B+, B− → driver's coil A/B outputs. Direction is reversed correctly by swapping **A+ and A− with each other** (same coil pair) — never cross wires between coil A and coil B.

**Driver current setting:** **4A** — confirmed working. Reliable up to speed 8000 (unloaded); 8200 produced an occasional jam (treat 8000 as the safe working max until retested under load).

### Navigating the ST-PMC1 Controller

**Entering programming mode:**
1. From **Manual mode** (default/idle screen), press **Edit**.
2. Drops into program edit state — instruction lines auto-numbered starting at **00**.
3. Use **∧ / ∨ arrows** to browse between lines and fields (line number, move parameters, SPEED, direction, etc.).
4. Press **Enter** to select a field for editing.
5. Press **Quit** to exit back to Manual mode — **changes save automatically**.

**Operating modes:**
- **Manual** — direct jog control via keypad.
- **Auto** — runs a saved program from line 00 when Run is pressed.
- **External Trigger** — waits on wired inputs (A/B limit switches, Stop, Run) to advance/start.

**Input bank (back-right ports):** IN1, IN2, A, B, Stop, Run — the 6 optically-isolated inputs, with Com+/Com− as the shared reference depending on whether switches are wired sourcing or sinking.

**✅ Confirmed input voltage (from official ST-PMC1 manual):** All 6 inputs (RUN, STOP, A, B, IN1, IN2) share the same input interface circuit, powered from the **COM+/COM− rail at a fixed DC24V** — not a tolerant/flexible range. Behavior is **active-low**: closing the switch/contact pulls the input low (logic "0", panel indicator lights); open = high (logic "1"). This matches the 24V pull-up behavior already confirmed empirically on the limit-switch input. See [[#Triggering RUN via NI-DAQ]] below for how this applies to triggering the program from the NI-9263.

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

#### ST-PMC1 Physical Port Reference (confirmed from unit)

Ports as labeled on the back of the physical controller, top to bottom:

**Back left (power + pulse/direction outputs):**

| Port | Function |
|---|---|
| +24V | Power input to controller |
| GRD | Power ground |
| OUT3, OUT2, OUT1 | The 3 relay outputs (e.g. trigger quench valve solenoid) |
| **OPTO** | Common/return rail for the opto-isolated output stage (CP, CW, likely OUT1–3) — a **separate isolated common, not the same node as GRD** |
| CW | Direction output (switching signal) |
| CP | Pulse output (switching signal) |

**Back right (inputs):**

| Port | Function |
|---|---|
| Com+, Com− | Common terminals for the input bank — pick one depending on whether switches are wired sourcing or sinking |
| IN2, IN1 | General-purpose inputs |
| A, B | Likely limit switches (home / end-of-travel) |
| Stop | E-stop input |
| Run | Start/run button input |

This accounts for all 6 optically-isolated inputs (IN1, IN2, A, B, Stop, Run) and all 3 relay outputs (OUT1–3) called out in the spec section above.

#### Corrected Wiring: ST-PMC1 → Stepper Driver (PU/DR/MF terminals)

The driver's logic-input terminal block uses **PU+/PU−** (pulse), **DR+/DR−** (direction), and **MF+/MF−** (enable/"motor free" — unused here). These are opto-isolated inputs requiring a common supply on the `+` side and a switched signal on the `−` side.

The ST-PMC1's **OPTO** terminal supplies exactly that common rail — no external 5V source needed:

```
ST-PMC1 OPTO ──┬── Driver PU+
               └── Driver DR+

ST-PMC1 CP  ────────── Driver PU−
ST-PMC1 CW  ────────── Driver DR−
```

- **OPTO → PU+ and DR+** (tied together): supplies pull-up voltage for both driver opto-inputs.
- **CP → PU−**, **CW → DR−**: controller sinks these low to generate each pulse/direction edge.
- **Do not tie OPTO to GRD** — keep it isolated; that's the likely reason it's broken out separately from GRD on the terminal block (avoids ground loops between the pulse/dir circuit and main power ground).
- **MF+/MF−**: originally left unused (no matching enable output on the ST-PMC1) — **now planned for DAQ control (2026-09-05)**, see "MF+/− Motor-Free Control" below.

**⚠️ Needs verification:** Confirm with a multimeter before first power-up that OPTO carries a voltage compatible with the driver's opto-input rating (commonly 5V or 12V) relative to GRD — do not assume it's a pass-through of the 24V supply rail.

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

**Type:** ✅ **Confirmed NEMA 23** (verified against physical unit, SN: 161104226 — mounting faceplate size)

**Mechanical Function:** Converts electrical pulses into rotational mechanical energy; coupled directly to ball screw shaft.

**Typical NEMA 23 Specifications:**

| Parameter | NEMA 23 |
|-----------|---------|
| **Step Angle** | 1.8° |
| **Steps/Revolution** | 200 |
| **Holding Torque** | 2–3 N·m |
| **Current Rating** | 3–5A per phase (verify exact value against motor nameplate before setting driver DIP switches) |
| **Coil Impedance** | ~3–5 Ω |
| **Ball Screw Lead** | 5–8mm/rev |
| **Linear Travel** | 0.025–0.04mm per step |

*(NEMA 34 columns removed — no longer applicable now that the motor is confirmed NEMA 23. Note: exact current rating still needs confirming from the motor's nameplate/model number, since "3–5A" is a typical range for NEMA 23 motors generally, not a spec read directly off this unit.)*

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
- **Max Thrust:** 320N (typical for NEMA 23)
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
    │  │  NEMA 23 (confirmed)        │              │
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
                    │ NEMA 23         │   │    │
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

## Homing Strategy: Limit Switch Method

**Recommendation:** Use a mechanical limit switch for reliable, automated home positioning. The ST-PMC1's 6 optical inputs are perfectly suited for this.

### Why Limit Switch Homing?

| Method | Sensorless | Limit Switch | Encoder |
|--------|-----------|--------------|---------|
| **Hardware Required** | None (current) | Switch + bracket | Switch + encoder |
| **ST-PMC1 Compatible** | ❌ No | ✅ Yes | ❌ No (needs upgrade) |
| **Reliability** | ❌ Not possible | ✅ Failsafe | ✅ Best |
| **Cost** | $0 | ~$10 | ~$50 |
| **Implementation Time** | N/A | 2–4 hours | 1–2 days |

**The Constraint:** ST-PMC1 is open-loop (pulse+direction only). It cannot detect motor stall without external feedback. Sensorless homing requires either stall detection (closed-loop driver) or encoder feedback—neither is available with current hardware.

**The Solution:** Add a mechanical limit switch that the ST-PMC1 can sense via one of its 6 optical inputs.

---

### Mechanical Installation

**Limit Switch Placement:**

```
Ball Screw Assembly (Vertical View)
═══════════════════════════════════

    [ST-PMC1 Controller]
           ▲
           │ CP, CW signals
           │
    [TB6600 Driver]
           ▲
           │ Coil current
           │
    [NEMA Stepper Motor]
           │
           ▼ Rotation
    ╔═══════════════╗
    ║ Ball Screw   ║
    ║ Shaft        ║
    ║              ║
    ║  [Coupler]   ║
    ║      │       ║
    ║      ▼       ║ ← Moving up/down
    ║   [Nut]      ║
    ║      │       ║
    ║      ▼       ║
    ║              ║
    ╚═══════════════╝
           │
           ▼ Linear motion
    
    ┌──────────────────┐
    │  Ceramic Mount   │
    │  + Sample        │
    │                  │
    │  [Moving up]     │
    │  [down]          │
    │                  │
    └──────────────────┘
           │
           ▼ Travels ~100mm range
    
    ┌──────────────────┐
    │ HOME POSITION    │
    │ (lowest)         │
    │                  │
    │ ┌──────────────┐ │
    │ │ Limit Switch │ ◄─── Mechanically actuated
    │ │  (N.O.)      │ │     when mount reaches bottom
    │ └──────────────┘ │
    └──────────────────┘
```

**Physical Implementation:**
1. Install a **normally-open (N.O.) limit switch** at the lowest point of travel
2. Mount switch on fixed frame (not on moving part)
3. Add mechanical actuator arm to sample mount
4. When mount descends, arm presses switch → circuit closes → ST-PMC1 detects

**Example Switch:** Omron V-155-1C25 or equivalent (~$8–15)
- 1 N.O. contact
- Optical isolation ready
- 24V DC rated

---

### Wiring: Limit Switch to ST-PMC1

```
╔════════════════════════════════════════════════════════════╗
║  LIMIT SWITCH INPUT WIRING (ST-PMC1 Input #1)             ║
╚════════════════════════════════════════════════════════════╝

Limit Switch (N.O. contact)           ST-PMC1 Controller
┌──────────────────┐                  ┌──────────────────┐
│ Common (COM)  ───┼──────────────────┤ Input #1 GND     │
│                  │                  │                  │
│ Normally Open ───┼──[+24V pulled]   │ Input #1 Signal  │
│ (N.O.) Contact   │    (10kΩ resistor)                 │
└──────────────────┘                  └──────────────────┘
                │                            │
                │                            │
            ┌───┴────────────────────────────┴────┐
            │  +24V Power Rail (SDN supply)       │
            │  (via 10kΩ pull-up resistor)        │
            └────────────────────────────────────┘

Signal Behavior:
─────────────────────────────────────────────────────────
Mount descending, NOT at limit:
  Switch contact OPEN → Input line pulled HIGH (+24V) → ST-PMC1 reads "1"

Mount hits limit:
  Switch contact CLOSES → Input line pulled to GND → ST-PMC1 reads "0"
  
Program detects this state change → triggers "home found" action
```

**Wiring Requirements:**
- Use **shielded twisted pair** (STP) cable for signal line (~10 ft max)
- Ground shield at power supply end only (prevents ground loops)
- Pull-up resistor: 10 kΩ (built into most ST-PMC1 variants, check manual)
- Wire gauge: #22 AWG minimum

---

### ST-PMC1 Homing Program

**Program: "Auto Home & Move to Working Height"**

```
╔═══════════════════════════════════════════════════════════════╗
║  ST-PMC1 PROGRAM: Home & Position                            ║
║  (99 lines max, typical 8-10 lines for homing)               ║
╚═══════════════════════════════════════════════════════════════╝

Line  Instruction          Param1    Param2    Param3     Effect
────  ───────────────────  ────────  ────────  ─────────  ──────────────
  1   MOVE                 500 steps CCW       5 kHz      Descend slowly
                                                           toward limit

  2   CHECK INPUT #1       CLOSED    JUMP→5    JUMP→3     If limit closed,
                                                           skip to line 5
                                                           (home found!)

  3   CHECK STEPS          500       JUMP→1    -          If moved <500
                                                           steps, retry
                                                           (line 1)

  4   HALT & ERROR         "Home     -         -          Failed to find
                           not found"                       home after
                                                           500 steps

  5   MOVE                 0 steps   CW        -          Stop motor
                                                           (hold position)

  6   WAIT                 2 sec     -         -          Mechanical settle
                                                           time

  7   MOVE                 300 steps CW        5 kHz      Raise sample
                                                           to working
                                                           height (~15mm)

  8   HOLD POSITION        -         -         -          Coils locked,
                                                           ready for heating

  9   WAIT FOR TRIGGER     -         -         -          Operator button
                                                           or external cmd

 10   [Next sequence...]   -         -         -          Raise, trigger
                                                           quench, lower

Program Flow Diagram:
═════════════════════════════════════════════════════════

                          ┌─────────────────────┐
                          │  START: Home Routine│
                          └──────────┬──────────┘
                                     │
                                     ▼
                          ┌─────────────────────┐
                          │ MOVE 500 steps down │
                          │ at 5 kHz (slow)     │
                          └──────────┬──────────┘
                                     │
                                     ▼
                          ┌─────────────────────┐
                          │ Limit Switch Closed?│
                          └──┬────────────────┬─┘
                           Yes               No
                             │                 │
                             ▼                 │
                    ┌──────────────────┐      │
                    │ HOME FOUND ✓     │      │
                    │ Jump to Line 5   │      │
                    └──────────────────┘      │
                                              ▼
                                    ┌──────────────────┐
                                    │ Moved 500 steps? │
                                    └──┬────────────┬──┘
                                     No             Yes
                                      │              │
                                      ▼              ▼
                                  [Retry]    [Error: Not Found]
                                   Line 1         HALT

[Continue from home...]
                             │
                             ▼
                    ┌──────────────────┐
                    │ Stop motor       │
                    │ (hold in place)  │
                    └──────────────────┘
                             │
                             ▼
                    ┌──────────────────┐
                    │ Wait 2 seconds   │
                    │ (settle time)    │
                    └──────────────────┘
                             │
                             ▼
                    ┌──────────────────┐
                    │ Raise 300 steps  │
                    │ to work height   │
                    │ (~15mm up)       │
                    └──────────────────┘
                             │
                             ▼
                    ┌──────────────────┐
                    │ READY FOR TEST   │
                    │ Wait for trigger │
                    └──────────────────┘
```

**Key Program Features:**
- ✅ **Automatic retry** (line 3) if limit not found on first descent
- ✅ **Timeout protection** (line 4) prevents endless loop
- ✅ **Settling time** (line 6) allows mechanical vibration to dampen
- ✅ **Controlled ascent** (line 7) at same slow speed for smooth motion
- ✅ **Position hold** (line 8) with coils energized (failsafe)

---

### Commissioning Procedure

#### **Step 1: Mechanical Setup** (Before powering on)
- [ ] Mount limit switch at lowest point of travel
- [ ] Mount actuator arm on sample mount to press switch
- [ ] Verify switch actuates when mount reaches bottom
- [ ] Test switch manually: press arm → verify switch closes → arm release → switch opens

#### **Step 2: Electrical Wiring**
- [ ] Connect limit switch common (COM) to ST-PMC1 Input #1 GND
- [ ] Connect limit switch N.O. contact to ST-PMC1 Input #1 signal (+24V pull-up)
- [ ] Use shielded twisted pair cable, ground shield at power supply only
- [ ] Verify continuity: multimeter between GND and Input #1 signal
  - Switch open: should read ~24V
  - Switch closed: should read 0V

#### **Step 3: Power On & Test Limit Switch Input**
- [ ] Power on ST-PMC1 (verify LCD lights up)
- [ ] Manually press switch actuator arm
- [ ] Check ST-PMC1 input indicator (LCD shows Input #1 status)
- [ ] Verify LED toggles between "open" and "closed" states

#### **Step 4: Program Homing Sequence**
- [ ] Enter programming mode on ST-PMC1 (see manual)
- [ ] Input the 10-line homing program above
- [ ] Verify each line programmed correctly (LCD display confirmation)
- [ ] Save program to memory

#### **Step 5: Dry Run (No Load)**
- [ ] Remove sample mount or secure it to prevent falling
- [ ] Power on system
- [ ] Press START button to run homing program
- [ ] Observe motor descent: should slow down, stop when limit switch closes
- [ ] Verify motor rises to working height (line 7)
- [ ] Check motor holds position after halt (coils energized)

#### **Step 6: Loaded Run (With Sample)**
- [ ] Install sample mount with sample
- [ ] Run homing program again
- [ ] Verify descent speed is adequate (motor not stalling)
- [ ] Verify rise to working height is smooth
- [ ] Measure actual height from home position (should be ~15mm per 300 steps)
- [ ] Calculate ball screw lead if needed: (height in mm) ÷ (steps/200) × 200

#### **Step 7: Integration Check**
- [ ] Homing program completes successfully ✅
- [ ] Sample positioned correctly in coil ✅
- [ ] Ready to proceed with heating/quenching integration ✅

---

### Troubleshooting Homing

| Symptom | Likely Cause | Fix |
|---------|--------------|-----|
| **Motor descends continuously; doesn't stop** | Limit switch not closing OR not wired to Input #1 | Check switch continuity; verify wiring to correct input |
| **Motor descends then immediately rises** | Limit switch always closed (stuck or inverted) | Test switch manually; check for mechanical binding |
| **Motor descends partway, then error** | Retry limit (line 3) triggered; switch not detected in 500 steps | Lower descent speed (reduce frequency in line 1); verify switch placement |
| **Motor moves but LCD shows no input status** | Pull-up resistor missing or failed | Check ST-PMC1 manual for pull-up; may need external 10kΩ resistor |
| **Inconsistent homing position** | Mechanical backlash in ball screw | Add mechanical stop (rigid pin) at home position; reduces reliance on switch precision |

---

### USB-6009 Pinout Reference

**Update (08-29-2026):** Actual DAQ hardware in hand is an **NI USB-6009** (not the NI-9263 originally assumed — see superseded plan below). Two 16-terminal screw-terminal connector blocks, 32 terminals total. Pulled from NI's official USB-6008/6009 user guide; **verify against the labels silkscreened directly on the physical unit before wiring** — this is safety-relevant wiring.

**Connector 0 — Analog (terminals 1–16):**

| Terminal | Signal | Terminal | Signal |
|---|---|---|---|
| 1 | GND | 9 | AI 6 |
| 2 | AI 0 | 10 | GND |
| 3 | AI 4 | 11 | AI 3 |
| 4 | GND | 12 | AI 7 |
| 5 | AI 1 | 13 | GND |
| 6 | AI 5 | 14 | AO 0 |
| 7 | GND | 15 | AO 1 |
| 8 | AI 2 | 16 | GND |

**Connector 1 — Digital I/O & power (terminals 17–32):**

| Terminal | Signal | Terminal | Signal |
|---|---|---|---|
| 17 | P0.0 | 25 | P1.0 |
| 18 | P0.1 | 26 | P1.1 |
| 19 | P0.2 | 27 | P1.2 |
| 20 | P0.3 | 28 | P1.3 |
| 21 | P0.4 | 29 | PFI 0 |
| 22 | P0.5 | 30 | +2.5V |
| 23 | P0.6 | 31 | +5V |
| 24 | P0.7 | 32 | GND |

**Channel assignments for this project:**

| Terminal | Signal | Used for |
|---|---|---|
| 17 | P0.0 | ST-PMC1 RUN trigger — currently pivoting from relay module to LCD4075DD3 SSR control input, see below |
| 18 | P0.1 | SOLA DC-OK contact → SSR input (AC power release, gates AC into SDN 10-24-100P for the stepper controller+driver) |
| 19 | P0.2 | SOLA DC-OK contact → digital input (software readback: is the 24V rail healthy?) |
| — | P0.x (TBD) | ST-PMC1 IN1 trigger — new SSR, see below |
| — | P0.x (TBD) | ST-PMC1 IN2 trigger — new SSR, see below |
| — | P0.x (TBD) | Diaphragm pump on/off — new **DC-rated** SSR, see below |
| — | P0.x (TBD) | TB6600 driver MF+/− (motor-free/disable) — new SSR (spare LCD4075DD3-type), see "MF+/− Motor-Free Control" below |
| **14 (AO0)** | AO 0 | Reserved for HOTSHOT power command (0-5V → voltage-to-current isolator → HOTSHOT CTB1:1-2, 4-20mA). See [[Design/Wiring/NI-DAQ Control Architecture|NI-DAQ Control Architecture]] for the isolator wiring (ASIN B09KTBJGZB). |
| 32 | GND | Common return |

**Digital line budget (updated 2026-09-05):** 12 digital lines total on the 6009; 3 committed (P0.0, P0.1, P0.2) + 3 more planned (IN1, IN2, pump) + 1 more planned (MF) = 7 of 12. STOP no longer needs a line (permanently jumpered, not DAQ-controlled — see below), freeing up budget versus the earlier plan that held a slot for it. Room remains for the HOTSHOT START trigger plus future expansion.

### On-Demand Position Control: GOTO Instruction + Finalized I/O Mapping (2026-09-01)

**Confirmed: the ST-PMC1 supports absolute positioning**, not just the relative/incremental `MOVE` used in the homing routine above. A separate **`GOTO ±xxxxxxx`** instruction moves directly to an absolute position (in pulses, referenced to the zero point established during homing) regardless of current location — range **-7,999,999 to +7,999,999 pulses**, far more than needed for the ~100mm travel range. This means Home/Center/Top/Bottom can each be a single `GOTO <fixed pulse count>` instruction, callable on-demand from wherever the sample currently sits — no need to track/calculate relative deltas.

**Finalized I/O mapping for on-demand actions** (Home, Center-of-coil, Top-of-coil, Bottom-of-coil, Quench — 5 actions needed):

| Input | Assignment | Behavior |
|---|---|---|
| **A** | Home limit switch (moved from IN1) | Hardware interrupt — instant reaction regardless of program state |
| **B** | Quench trigger | Hardware interrupt — same "whenever, right now" behavior, appropriate for the most time-critical action |
| **IN1 + IN2** | 4 combinations (00/01/10/11) → Home / Center / Top / Bottom | Polled — program must be structured as a continuous poll loop (`CHECK INPUT` → jump → execute `GOTO` → loop back to poll) for these to feel responsive "whenever wanted"; not instantaneous like A/B, but timing is controllable by how tightly the poll loop is written |

This exactly uses all 5 available slots (2 interrupts + 4 polled combinations) — no spare capacity for a 6th action without adding hardware or bypassing to full NI software control (see [[Design/Wiring/NI-DAQ Control Architecture|NI-DAQ Control Architecture]] discussion on continuous scanning).

**Rework required:** the homing routine documented above was built around polling IN1 (`CHECK INPUT #1, JUMP→line5`) — moving the limit switch to A means rewriting it to use A's automatic interrupt-jump entry point instead of a poll, per the A/B mechanism described below.

**Source:** [ST-PMC1 manual excerpt, ManualsLib](https://www.manualslib.com/manual/1269811/St-St-Pmc1.html) — GOTO instruction confirmed via page-specific fetch, 2026-09-01.

### A/B vs. IN1/IN2 — what each ST-PMC1 input actually does (2026-09-01)

Pulled directly from the ST-PMC1 manufacturer manual (not just the port-reference guess further up this doc):

- **A / B — hardware interrupt inputs.** Triggering either one *while a program is running* immediately decelerates the motor to a stop, interrupts the program, and jumps execution to a dedicated subroutine entry point ("A operation" / "B operation") defined separately in the program — the controller remembers the interrupted position. Manual's own framing: for situations where "motor displacement cannot be pre-calculated" — the textbook use case is a limit switch or other real-time event that needs an immediate reaction, not a scheduled check. **Not** a program-select mechanism.
- **IN1 / IN2 — plain "switching signal input terminals."** Read passively, only when the program explicitly checks them (e.g. the `CHECK INPUT #1` step in the homing sequence above). No automatic interrupt behavior.

**Decision (2026-09-01):** Since there's no current use case requiring real-time interrupt/abort behavior, **A and B are left unused for now** — no SSRs purchased for them. **IN1 and IN2 will get DC SSRs** (same LCD4075DD3-type part as RUN) so the DAQ can drive general-purpose polled conditions into the ST-PMC1 program. Exact IN1/IN2 use case still TBD.

**Source:** [ST-PMC1 manual, ManualsLib](https://www.manualslib.com/manual/1269811/St-St-Pmc1.html); [ST-PMC1 manual PDF, cnccat.com](https://cnccat.com/cnccat_photos/files/ST-PMC1%20Single%20Axis%20Programmable%20Controller.pdf)

### Diaphragm Pump — confirmed DC, needs its own DC-rated SSR (2026-09-01)

The diaphragm pump is **24V DC** (consistent with [[Design/Plumbing/Fluid Systems.md]] and [[Design/Wiring/INDEX.md]]), **not** AC. This matters: the AC-rated SSR already in use for the stepper controller/driver power-release circuit (LCDS4048ZD3, triac/thyristor-based) relies on the AC waveform crossing zero to turn off — feeding it a DC load risks it latching on and never releasing. **Do not reuse the AC-output SSR for the pump.** Instead, use a small **DC-rated SSR** (same LCD4075DD3-type family as the RUN/IN1/IN2 signal-level SSRs, just current-rated for the pump's actual draw) switching the 24V DC line to the pump, triggered off its own spare 6009 digital line.

**Full SSR/relay inventory as of 2026-09-01** (see [[Design/Wiring/NI-DAQ Control Architecture|NI-DAQ Control Architecture]] for the HOTSHOT-side additions):

| # | Function | Type | Status |
|---|---|---|---|
| 1 | ST-PMC1 RUN trigger | DC SSR (LCD4075DD3) | In progress, not yet bench-tested |
| 2 | ST-PMC1 IN1 trigger | DC SSR (LCD4075DD3-type) | Planned |
| 3 | ST-PMC1 IN2 trigger | DC SSR (LCD4075DD3-type) | Planned |
| 4 | Diaphragm pump on/off | DC SSR (LCD4075DD3-type, current-rated for pump) | Planned |
| 5 | AC power release (stepper controller+driver) | AC SSR (LCDS4048ZD3) | Existing/documented below |
| 6 | TB6600 driver MF+/− (motor-free/disable) | DC SSR (LCD4075DD3-type, spare unit) | Planned (2026-09-05) — see "MF+/− Motor-Free Control" below |
| — | ST-PMC1 STOP | — | **Not SSR-controlled, by design (2026-09-05)** — permanently jumpered closed, not treated as safety-critical for this mechanism; see below |
| — | ST-PMC1 A, B | — | **Unused** — no SSR needed unless a real-time interrupt use case is defined |

---

### Triggering RUN via USB-6009 — Current Plan: LCD4075DD3 DC-DC SSR

**Update (08-29-2026, later):** Pivoted away from the TS0011 relay module (see "Attempted: TS0011 Relay Module" below for the troubleshooting history) — suspected faulty after extensive testing showed no response to `P0.0` toggling. Switching to the **LCD4075DD3** (DC-DC solid-state relay: 3-32VDC control input, 3-75VDC switched output) already on hand, which was originally acquired for a different purpose but fits this job well — no coil, no separate coil-power rail (`JD-VCC`), no mechanical failure point.

**Wiring plan:**

```
USB-6009 P0.0 (terminal 17) ──► LCD4075DD3 control input (+)
USB-6009 GND  (terminal 32) ──► LCD4075DD3 control input (−)

LCD4075DD3 output (+) ──► ST-PMC1 RUN
LCD4075DD3 output (−) ──► ST-PMC1 COM− (24V rail return)
```

The SSR's output acts as a switch between its two output terminals when the control input is driven — functionally a drop-in replacement for the relay's `COM`/`NO` pair. RUN/COM− is just a signal-level contact closure (not a real load), so the SSR's 3-75V/multi-amp output rating is far oversized for this; that's fine, it'll switch cleanly regardless.

**Why this is expected to succeed where the relay module didn't:**
- No separate coil supply to forget wiring (root cause of the earlier `JD-VCC` problem) — the SSR's control input draws directly off the same signal current the DAQ line already provides.
- Control-side current draw is expected to be low-mA (inferred from the sibling part LCDS4048ZD3, same "LCD" SSR family, confirmed elsewhere in this doc as low-mA at 5V) — likely within the 6009's ~8.5mA budget, unlike the TS0011's spec'd 15–20mA.
- 3-32VDC control range comfortably covers the 6009's 5V logic level, assuming (typical for this SSR family) a ~3V minimum turn-on threshold.

**⚠️ Still needs verification:**
- LCD4075DD3's exact "must operate current" spec has not been independently confirmed from a datasheet — inferred from the sibling LCDS4048ZD3 part only. If turn-on proves unreliable, fall back to a small NPN transistor buffer between P0.0 and the SSR control input, same pattern as discussed for the relay module.
- STOP's normal-state polarity (open vs. closed = "run allowed") — still unconfirmed; STOP stays hard-wired outside DAQ control regardless of RUN's trigger method (see below).
- Test in the USB-6009 NI MAX test panel the same way the relay module was tested: toggle P0.0, confirm RUN response at the ST-PMC1.

**STOP left out of DAQ control:** Per the earlier design discussion, STOP should stay hard-wired in its safe/ready state (e.g., jumpered or through a physical E-stop N.C. contact) rather than routed through the USB-6009, so an E-stop remains available independent of the laptop/software. Confirmed during troubleshooting (08-29-2026) that neither RUN nor STOP are permanently/statically wired to COM− — both are switched, as intended.

**Update (2026-09-05) — STOP is not being treated as a safety-critical E-stop.** Reassessed: the ball screw stage (light sample + mount, vertical lift, NEMA 23 holding torque) doesn't present the kind of hazard that motivated the hardwired-E-stop philosophy elsewhere in this system (e.g. HOTSHOT RF power, vacuum chamber). Decision: **STOP will simply be permanently jumpered closed** (always "run-allowed") rather than wired to a physical E-stop switch or routed through DAQ/SSR control. Actual "stop everything" duty is handled at the power level instead, via the existing AC power-release SSR + SOLA DC-OK interlock (see "AC Power Release Interlock" below) — cutting power to the stepper controller/driver achieves the same practical effect without needing a dedicated STOP circuit. No SSR is allocated to STOP.

### MF+/− Motor-Free Control (added 2026-09-05)

**Decision:** Wire MF+/− on the TB6600 driver for DAQ-controlled "motor free" (coil de-energize) — using the spare LCD4075DD3-type SSR left over after covering RUN/IN1/IN2/pump (5 needed, bought in packs of 2 = 6, 1 spare from stock already on hand).

**Why MF instead of using the spare SSR for STOP:** STOP was deliberately kept independent of DAQ/software (see above) — routing it through an SSR would make the "stop" path depend on the 6009, software, and 24V rail all being healthy, undermining the point of an independent stop. MF has no such safety role — it's a convenience feature (let the shaft spin freely by hand, e.g. during assembly/alignment) — so DAQ control is a clean, low-consequence use of the spare SSR. Note MF is **not** a safety disable: engaging it removes holding torque, so it should never be triggered with the sample loaded/suspended (see load-holding note above — coils energized is the failsafe assumption throughout this doc).

**Wiring plan (same opto-isolated pattern as PU/DR):**

```
ST-PMC1 OPTO (shared pull-up rail) ──► Driver MF+   (tied together with PU+/DR+)

Spare USB-6009 digital line ──► LCD4075DD3 SSR control input (+)
USB-6009 GND ──► LCD4075DD3 SSR control input (−)
LCD4075DD3 SSR output ──► Driver MF−  (sinks low to activate motor-free mode)
```

**Still needs:** assign a spare 6009 digital line (check budget below), bench-test MF activation with the shaft unloaded before ever using it near a mounted sample.

**Source:** [ST-PMC1 Operating Manual (cnccat.com PDF)](https://cnccat.com/cnccat_photos/files/ST-PMC1%20Single%20Axis%20Programmable%20Controller.pdf) — page 4, back panel signal descriptions.

---

### Attempted: TS0011 Relay Module (suspected faulty, superseded)

**Update (08-29-2026):** Originally attempted with an **NI USB-6009** (digital I/O DAQ) plus a **TS0011** (SunFounder-branded 4-channel 5V relay module, SRD-05VDC-SL-C relays on board, 1 of 4 channels used) as the RUN-trigger intermediary, superseding an earlier NI-9263 analog-output plan. After extended troubleshooting (see Commissioning Log), this path was abandoned in favor of the LCD4075DD3 SSR plan above — leaving this section as a record of what was tried and why.

**Confirmed constraint that originally motivated using a relay/SSR at all:** RUN (like STOP, A, B, IN1, IN2) is a **DC24V, active-low opto-isolated input** — confirmed from the ST-PMC1's official manual. The USB-6009's digital lines are 0–5V logic and can't drive a 24V input directly at all (wrong voltage domain, not just a current-limit problem) — some form of relay/SSR intermediary is required. This constraint still applies to the SSR-based plan above; only the intermediary device changed.

**Original wiring plan (TS0011):**

```
USB-6009 P0.0 (5V digital out) ──► TS0011 IN1 (channel 1 of 4, only channel used)
                                              │
                                    Relay module NO contact
                                              │
        ST-PMC1 RUN ◄──────────────────────────┴──────────────────────────► ST-PMC1 COM− (24V rail return)
```

- `VCC`/`GND` (logic side) → USB-6009 terminal 31 (+5V, 200mA) / terminal 32 (GND)
- `IN1` → USB-6009 terminal 17 (P0.0)
- `JD-VCC`/`JD-GND` (coil side) → jumper installed, bridged from `VCC` (no separate supply available on the bench) — current draw (~71mA coil + a few mA logic, well under the 6009's 200mA +5V budget) confirmed safe for single-supply operation
- Confirmed via datasheet: TS0011 is active-LOW (NO connects to COM when IN1 is pulled low) — matches the fail-safe assumption that a floating/undriven line defaults to relay-off/Stop
- Confirmed physically: relay's `COM` (center terminal) wired to ST-PMC1 `COM−`; `NO` wired to `RUN`; neither RUN nor STOP found to be permanently/statically bridged — wiring topology itself was correct

**Why it's suspected faulty:** With the `JD-VCC` jumper removed, no response (expected — coil rail unpowered, matches theory). With jumper reinstalled (bridging `VCC`→`JD-VCC`), an audible click occurred at power-up, but this was a one-time power-on transient — toggling `P0.0` afterward in the NI MAX test panel produced no click, no LED response, and no effect on the ST-PMC1 RUN state, despite correct wiring topology, confirmed ground path expectations, and correct trigger polarity understanding. Root cause not isolated to a single failure point (ground continuity / IN1 voltage swing / module failure were the three candidate explanations) before the decision was made to switch to the LCD4075DD3 rather than continue debugging this specific board.

**Source:** [TS0011 Datasheet (DigiKey-hosted PDF)](https://mm.digikey.com/Volume0/opasdata/d220001/medias/docus/5773/TS0011%20DATASHEET.pdf)

---

### AC Power Release Interlock: USB-6009 + SOLA DC-OK + SSR (separate circuit)

**Not part of the ball screw system** — documented here for now since it shares the USB-6009 and the SDN 10-24-100P with the ball screw circuit above; may get moved to [[Design/Wiring/Electrical System.md]] once the HOTSHOT interlock plan (see [[Design/Wiring/TODO - HOTSHOT Docs & E-Stop-Wiring.md]]) is finalized.

**Purpose:** Gate release of switched AC power (via the LCDS4048ZD3 solid-state relay, 3-32VDC control / 24-480VAC load) so it only fires when both (a) the USB-6009 explicitly commands it, and (b) the SDN 10-24-100P confirms its 24V rail is healthy.

**Wiring — deliberately no relay in this path** (unlike the RUN trigger above): both ends are already voltage/current-compatible, so an intermediate relay would just be an extra failure point.

```
USB-6009 P0.y (5V digital out) ──► SOLA DC-OK contact (N.O. solid-state, 200mA/60Vdc max) ──► LCDS4048ZD3 SSR input(+)
LCDS4048ZD3 SSR input(−) ──► USB-6009 GND
```

- SDN 10-24-100P DC-OK signal: confirmed from the SolaHD SDN-P datasheet — **N.O. solid-state contact, rated 200 mA / 60 Vdc**, active when Vout is within regulation. Both the 6009's ~5V/~8.5mA output and the SSR's low-mA control input sit well within that contact's rating.
- Series arrangement makes this a hardware AND: the SSR only receives a control signal (and therefore only switches AC to the load) when the DAQ line is driven true **and** the DC-OK contact is closed. If the 24V rail sags/faults, DC-OK opens and the SSR de-energizes regardless of DAQ state.
- This only certifies the 24VDC control rail is healthy — it is **not** a substitute for the hardware E-stop → mains contactor path already planned for the HOTSHOT (see TODO doc); keep that as the authoritative mains cutoff.

**⚠️ Still needs verification:**
- Confirm actual LCDS4048ZD3 minimum turn-on current against the 6009's ~8.5mA source budget; add a small NPN transistor buffer stage if marginal.
- Common ground between USB-6009 and this circuit, consistent with the star-ground point called out in [[Design/Wiring/NI-DAQ Control Architecture|NI-DAQ Control Architecture]].

#### Alternative/additional use: DC-OK as a software-readable status input

Separate from the hardware interlock above (DC-OK gating the SSR), DC-OK can also be wired straight into a spare USB-6009 digital line so software can log/monitor "is the 24V rail actually healthy" independent of the SSR path. Since DC-OK is a dry contact (no voltage of its own), the input pin needs a resistor to define its state when the contact is open — otherwise the pin floats and reads noise instead of a clean logic level.

```
+5V (terminal 31) ──[10kΩ]── digital input pin (e.g. P0.2) ──[DC-OK contact]── GND (terminal 32)
```

- Contact **open** (rail unhealthy) → pulled to **HIGH**
- Contact **closed** (rail healthy) → pulled to **LOW**
- Logic is inverted (HIGH = bad) with this pull-up wiring; flip the wiring (switch to +5V side, resistor to GND) for the opposite polarity, or just invert in software (DAQ Assistant) — either works, pick one and be consistent.
- In LabVIEW, read this as a native Boolean by setting the DAQ Assistant's digital input task to **Acquisition Mode → "1 Sample (On Demand)"** — wires directly to a Boolean LED/indicator with no extra conversion. If the task instead outputs a Digital Waveform (e.g. multi-sample/continuous mode), convert with **Digital Waveform to Boolean Array → Index Array [0]** before feeding a Boolean indicator.
- This is a separate physical contact use from the SSR-gating wiring above — don't try to wire the same DC-OK contact into both circuits simultaneously (it's a single series path, not a tap point shared across two consumers).

#### Extending the same SSR pattern to other ST-PMC1 inputs (IN1, A, B, etc.)

The RUN-trigger SSR approach above generalizes to any of the ST-PMC1's other active-low inputs (IN1, IN2, A, B — everything except STOP, which stays hard-wired outside DAQ control per the safety note above). The DAQ can't source/sink the 24V COM− rail directly from a 5V digital line, so each input that needs DAQ-driven triggering needs its own switching device between COM− and that input terminal:

```
USB-6009 P0.x (5V digital out) ──► SSR control input(+)
USB-6009 GND                   ──► SSR control input(−)
SSR output leg 1               ──► ST-PMC1 input terminal (IN1 / A / B / etc.)
SSR output leg 2               ──► ST-PMC1 COM−
```

- Use the same **solid-state relay** approach (LCD4075DD3 or equivalent), not a mechanical relay module — the TS0011 mechanical relay module was already tried for the RUN trigger and abandoned due to the JD-VCC coil-supply reliability problem (see Commissioning Log); no reason to expect a mechanical relay would fare better on a second input.
- Each additional triggered input needs its **own** SSR/digital-output-line pair — one contact closure per input, can't share a single SSR across multiple ST-PMC1 inputs unless only one is ever active at a time.
- Confirmed input behavior (from the ST-PMC1 manual, see Quick Reference section above): active-low, COM+/COM− fixed at DC24V — closing the contact pulls the input to logic "0".

**AC mains-side wiring (confirmed 08-29-2026) — only L goes through the SSR:**

```
AC Source L   ──► LCDS4048ZD3 switched input/output ──► Load L
AC Source N   ─────────────────────────────────────────► Load N   (straight through, never switched)
AC Source GRD ─────────────────────────────────────────► Load GRD (straight through, always continuous)
```

- **Line (L) only** goes through the SSR's switched path.
- **Neutral (N) must never be switched** — if only N were interrupted, the load would remain live relative to ground through the still-connected L, creating a shock hazard on equipment that appears "off."
- **Ground/PE must never be switched, under any circumstance** — it must remain a continuous bonding path at all times so a line-to-chassis fault has a low-impedance return to trip upstream protection instead of energizing the chassis. This applies to any switching device (SSR, relay, contactor), not just this one.
- This SSR switching decision is independent of, and does not replace, the hardware E-stop → mains contactor path already planned for the HOTSHOT in [[Design/Wiring/TODO - HOTSHOT Docs & E-Stop-Wiring.md]] — that remains the authoritative mains cutoff.

---

### Performance After Homing

Once homed, the ball screw system achieves:
- **Repeatability:** ±0.05mm (limited by mechanical backlash, not controller)
- **Settling time:** ~2 seconds (coil damping + mechanical inertia)
- **Hold force:** Stepper coils energized → shaft cannot drift (failsafe)
- **Next cycle:** Operator can repeat homing or move directly to working height

---



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
| **Holding Torque** | 2–3 N·m (NEMA 23) | Sufficient for vertical load |
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

## Commissioning Log

Chronological record of hardware setup/troubleshooting actions taken on the physical unit.

| Date | Action | Notes |
|---|---|---|
| 2026-08-25 | Confirmed motor is **NEMA 23** (measured mounting faceplate on physical unit) | Corrected from earlier "NEMA 23 or 34" guess throughout this doc and related notes |
| 2026-08-25 | Wired ST-PMC1 → driver: **OPTO → PU+/DR+**, **CP → PU−**, **CW → DR−**, MF unused | Based on confirmed physical port labels on back of ST-PMC1 (see port reference table above); not yet bench-verified against oscilloscope/multimeter |
| 2026-08-25 | Set driver current to **4A** | Motor was very loud/noisy at this setting (likely resonance) |
| 2026-08-25 | Reduced driver current to 3.5A to fix noise | Still within NEMA 23 typical range (3–5A per phase) |
| 2026-08-25 | **Reverted to / confirmed 4A — works fine** | Noise at 4A was reassessed and found acceptable; **4A is the final working driver current setting** |
| 2026-08-25 | Accidentally cross-wired **A+ to B−** (wire from Coil A to a Coil B terminal) while attempting to reverse motor direction | Incorrect — crosses two different coils, breaks the 90° phase relationship between them; not a valid way to reverse direction |
| 2026-08-25 | **Corrected: swapped A+ and A− (same coil pair only)** | This is the correct way to reverse rotation direction in hardware; B coil wiring untouched |
| 2026-08-25 | **Empirically tested max speed at 4A, no load: 8000 reliable; 8200 jammed once in 10 back-and-forth cycles** | Retested at **4A** driver current (not 3.5A), motor unloaded (no ball screw/sample load applied). **8000** (SPEED/frequency field) ran reliably across repeated back-and-forth cycling. **8200** jammed once during a 10-cycle back-and-forth test — treat 8200 as unreliable/borderline, not a safe working max. Both figures are well below the ST-PMC1's 40 kHz electrical ceiling and below the previous 20 kHz used in the lift/quench program. Note: this was tested at 4A, which is louder than the 3.5A setting logged above — current setting may have been changed back to 4A for this test; confirm which current setting is the final working config. |
| 2026-08-27 | **Decision: run driver subdivision/microstepping at full step (200 steps/rev)** | Chosen deliberately to prioritize max sample speed over smoothness — positioning resolution at full step (0.025mm/step) is well under the mechanical repeatability floor (±0.05–0.1mm, backlash-limited) anyway, so finer microstepping buys no real precision. Tradeoff accepted: full step has the most torque ripple/resonance of any subdivision setting, which is the likely mechanism behind the single 8200-speed jam logged above. The 8000-reliable/8200-borderline speed limits should still be treated as valid **only at this full-step setting** — do not assume higher subdivision settings share the same safe ceiling, and watch for jams if pushing past 8000 in search of more speed. |
| 2026-08-27 | **Confirmed RUN/STOP/A/B/IN1/IN2 input voltage is fixed DC24V, active-low** (from official manual, cnccat.com PDF) | Resolves earlier open item: these 6 inputs share one input circuit type, powered from COM+/COM− at a fixed 24V (not a tolerant range) — closing the contact pulls the input low (logic 0). Confirms the 24V approach already used for the limit switch is correct and required (not just convenient) for RUN as well. Added NI-9263 → relay → RUN trigger wiring plan to this doc. |
| 2026-08-27 | **DIP switch 4 identified as PUL/DIR vs CW/CCW input-mode selector, not standby current** | Physical test: with switch 4 down, motor only steps in one direction — direction control was lost entirely. This rules out "standby/idle current reduction," which was the initial (incorrect) guess for switch 4 and would not affect directionality. The one-direction symptom matches a driver input-mode switch that toggles between **PUL/DIR mode** (one pulse-train input + one static direction level) and **CW/CCW mode** (two separate pulse-train inputs, one per direction, no direction line). The existing wiring in this doc (**OPTO → PU+/DR+, CP → PU−, CW → DR−**) assumes PUL/DIR mode — CW only ever carries a static HIGH/LOW, never a pulse train, so in CW/CCW mode the driver would never see step pulses for the direction the CW line represents, producing exactly the observed one-direction-only behavior. **Needs confirmation:** flip switch 4 to the opposite position and retest both-direction motion; whichever position restores CCW motion is the correct PUL/DIR setting and should be logged as final alongside SW1–3 (current) and SW5–8 (subdivision). |
| 2026-08-29 | **Superseded NI-9263 RUN-trigger plan with NI USB-6009 + 4-channel relay module (1 channel used)** | Actual DAQ hardware in hand is a USB-6009, not the NI-9263 assumed in the original plan. Rewrote "Triggering RUN via NI-DAQ" section accordingly: USB-6009 digital output → SRD-05VDC-SL-C relay module channel 1 → NO contact bridges ST-PMC1 RUN↔COM−. Also documented a separate, unrelated interlock circuit sharing the same USB-6009: SOLA SDN 10-24-100P DC-OK contact (confirmed 200mA/60Vdc N.O. solid-state, per SolaHD SDN-P datasheet) wired directly (no relay) in series with a second USB-6009 line into the LCDS4048ZD3 SSR's control input, gating AC power release on both DAQ command and power-supply health. |
| 2026-08-29 | **Added USB-6009 pinout reference and assigned specific terminals to both circuits** | Documented the full 32-terminal screw-terminal pinout (from NI's official USB-6008/6009 user guide) and assigned P0.0 (terminal 17) to the relay module's RUN trigger and P0.1 (terminal 18) to the SSR/DC-OK line, with terminal 32 (GND) as the shared return. Confirmed AO0/AO1 (terminals 14–15) are deliberately not used for either circuit — both are simple on/off triggers, better served by digital lines than analog outputs (digital lines default to a known high-impedance/off-reading state at power-up; AO channels have no equivalent documented safe default). Pinout not yet cross-checked against the physical unit's silkscreened labels — treat as needing verification before wiring. |
| 2026-08-29 | **Identified relay module as TS0011 (SunFounder-branded), confirmed active-LOW trigger from datasheet, confirmed USB-6009 +5V (terminal 31) can power module logic side directly** | TS0011 datasheet confirms "NO connects to COM when IN1 is low" — matches active-LOW assumption. Corrected earlier statement that USB-6009 has no onboard 5V — confirmed +5V/200mA available at terminal 31, sufficient for both module logic side and (with JD-VCC jumper installed) the ~71mA relay coil, single-supply operation. |
| 2026-08-29 | **TS0011 troubleshooting: JD-VCC unpowered (jumper removed, no separate supply) → no relay response** | With jumper out and JD-VCC/JD-GND unconnected, LED responded to P0.0 toggling but relay didn't click — consistent with logic side (opto) working while coil side (transistor/coil) has no power rail. Diagnosis: JD-VCC needs either a separate 5V supply or the jumper reinstalled. |
| 2026-08-29 | **TS0011 troubleshooting: jumper reinstalled, single click at power-on, but P0.0 toggling produces no response at all (no click, no LED)** | Regression from earlier session where LED did respond to P0.0. Ruled out static/permanent wiring of RUN or STOP to COM− (confirmed both are switched, not hardwired) and ruled out relay COM/NO wiring topology (confirmed COM− → relay COM, correct). Root cause not isolated between ground continuity, IN1 signal path, and module failure before troubleshooting was stopped. |
| 2026-08-29 | **Decision: abandon TS0011, suspected faulty; pivot RUN-trigger circuit to LCD4075DD3 DC-DC SSR** | After the above troubleshooting failed to restore function, decided to use the already-on-hand LCD4075DD3 (3-32VDC control / 3-75VDC output SSR) as the RUN-trigger intermediary instead of the relay module — eliminates the coil-power-rail failure mode entirely (no JD-VCC equivalent on a solid-state device). Rewrote "Triggering RUN via USB-6009" section accordingly; TS0011 section retained as troubleshooting history under "Attempted: TS0011 Relay Module." Not yet bench-tested. |
| 2026-08-29 | **Confirmed AC mains-side wiring convention for LCDS4048ZD3: Line only, never Neutral or Ground** | Documented under the AC Power Release Interlock section: SSR switches L only; N and GRD/PE run straight through unswitched at all times. Applies generally to any single-pole switching device in this design, not just this SSR. |
| 2026-08-30 | **Confirmed DC-OK working as a bench-tested digital input on the USB-6009 (switch-to-ground, no pull resistor on the closed side)** | Bench setup wired DC-OK contact directly between GND and a digital input pin with no additional GND-side resistor — user confirmed this works correctly on the physical unit. Documented alongside it, for completeness, the general floating-pin rationale for why a pull resistor (10kΩ) is still needed on whichever side isn't actively driven, so the open-contact state is defined rather than floating. |
| 2026-08-30 | **Documented plan to extend the RUN-trigger SSR pattern to other ST-PMC1 inputs (IN1, A, B)** | Same active-low COM− switching approach as RUN, one SSR per additional input needed; explicitly reuses the SSR-over-mechanical-relay lesson learned from the TS0011 troubleshooting above rather than repeating the mechanical relay attempt. |

**Open items / not yet verified:**
- Confirm DIP switch 4 (PUL/DIR vs CW/CCW mode) is set to the position that restores both-direction motion, and log the final SW4 position alongside SW1–3/SW5–8
- Confirm OPTO output voltage against driver's opto-input rating (recommended before this was wired — flagged in port reference section above, still unconfirmed)
- Confirm whether the A+/A− swap actually reversed direction as expected on a test run
- Read exact model number off motor nameplate to get exact (not "typical range") current rating
- Retest max reliable speed **under actual load** (8000/8200 figures are no-load only — loaded max will likely be lower)
- 8200 jam: determine if this was a one-off or a repeatable failure point — recommend treating **8000 as the safe working max** until retested more thoroughly
- Confirm relay module trigger polarity (active-LOW vs active-HIGH) and STOP's safe-state polarity before wiring the USB-6009 RUN trigger live
- Confirm LCDS4048ZD3 minimum turn-on current against USB-6009's ~8.5mA source budget for the SSR/DC-OK interlock circuit
- Build and bench-test the additional IN1/A/B SSR trigger circuits (currently only planned/documented, not yet wired)

---

**Last Updated:** 2026-08-30  
**Status:** 🟢 Complete specification (commissioned components identified); NEMA size and driver current now confirmed on physical unit. 🟡 RUN-trigger circuit mid-pivot: TS0011 relay module suspected faulty (unresolved after troubleshooting), switching to LCD4075DD3 SSR — not yet bench-tested. 🟢 DC-OK confirmed working as a bench-tested USB-6009 digital input.  
**Next Steps:** Bench-test LCD4075DD3 as RUN trigger (P0.0 → SSR control in, SSR output → RUN/COM−); confirm STOP safe-state polarity; build/bench-test SSR trigger circuits for IN1/A/B; commission automated quench sequences; verify A+/A− swap achieved correct direction
