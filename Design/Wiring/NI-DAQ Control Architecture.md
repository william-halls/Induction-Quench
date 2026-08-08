---
tags: [wiring, control-system, NI-DAQ, automation, PID, stepper-motor]
---

# NI-DAQ Control Architecture

Automated control system using National Instruments data acquisition hardware (NI-9219 + NI-9263) on a legacy laptop for PID-based induction coil power management and auxiliary system automation.

## System Overview

**Control Platform**: Legacy laptop running LabVIEW or Python DAQ software
**Data Acquisition**: NI-9219 (analog input module)
**Signal Control**: NI-9263 (analog output module)
**Motor Control**: NEMA 23 2-phase stepper motor + external controller (TBD identification)

### Control Goals

1. **Induction Coil Power** — PID loop to maintain target temperature
2. **Ball Screw** — Automatic linear motion (stepper motor driven)
3. **Water Filling System** — Automatic control (solenoid valve or pump)
4. **Future: Air Purging** — Automatic oxygen removal + argon backfill

---

## Hardware Configuration

### Data Acquisition Modules

| Module | Function | Channels | Resolution | Notes |
|--------|----------|----------|------------|-------|
| **NI-9219** | Analog Input (ADC) | 4 | ±20mV–60V ranges | Thermocouple, pressure, feedback signals |
| **NI-9263** | Analog Output (DAC) | 4 | 0–10V or ±10V | Coil power control, valve commands, pump speed |

### Connected Sensors & Actuators

**NI-9219 Inputs (Read)**
- Thermocouple (sample temperature) → Channel 1
- Pressure transducer (chamber vacuum) → Channel 2
- Optional: Coil temperature feedback → Channel 3
- Optional: Water flow sensor → Channel 4

**NI-9263 Outputs (Write)**
- Induction power supply voltage command (0-10V) → Channel 1
- Ball screw motor speed/direction signal → Channel 2 (pending stepper controller specs)
- Water pump/solenoid valve control (0-10V) → Channel 3
- Air purge solenoid (future) → Channel 4

### External Equipment

- **Laptop**: Runs control software (LabVIEW, Python, C#)
- **Induction Power Supply**: Takes 0-10V analog control signal
- **Stepper Motor Controller**: NEMA 23 2-phase driver (model TBD)
  - Current status: Unknown input type (pulse/direction vs. analog vs. serial)
  - Power supply: Existing (size/voltage TBD)
- **Solenoid Valve(s)**: 24V for quench trigger + water filling
- **Optional Pump**: Diaphragm pump for water circulation (24V option)

---

## Control Loop Architecture

### PID Loop: Induction Coil Power

```
Thermocouple (sample)
        ↓
   NI-9219 ADC
        ↓
   Laptop (PID controller)
   - Target temp: 1000°C
   - Sensor feedback: Real-time
   - Loop rate: TBD (depends on coil thermal response)
   - Output: 0-10V command voltage
        ↓
   NI-9263 DAC (Channel 1)
        ↓
   Induction Power Supply
   (voltage ramping control)
        ↓
   Induction Coil
        ↓
   Sample heats to setpoint
```

**Parameters to Define During Testing:**
- PID gains (Kp, Ki, Kd)
- Loop update rate (likely 10-100 Hz)
- Coil response time constant
- Power supply input impedance & control linearity

### Ball Screw Control

**Current Status**: Stepper motor + external controller (model unknown)

**Options Pending Controller Identification:**

**Option A: Pulse/Direction Input** (Most Common)
- Stepper controller expects digital step + direction pulses
- Solution: Use NI-9425 (digital output module) instead of NI-9263
- NI-9425 can generate up to 100+ kHz pulse rates
- Direction control via separate digital line

**Option B: Analog 0-10V Speed Control**
- Stepper controller accepts 0-10V analog input for speed
- Solution: NI-9263 Channel 2 directly drives controller
- Direction control: Separate digital signal (or reversed voltage)
- Simpler integration, no additional modules needed

**Option C: Serial/USB Control**
- Stepper controller has USB or RS-232 interface
- Solution: Laptop controls directly via serial/USB
- No NI module needed for this function
- NI hardware reserved for coil + sensors

**ACTION REQUIRED**: Identify stepper controller model to determine integration approach.

### Water Filling System

**Method 1: Direct Solenoid Valve** (Simplest)
- 24V solenoid valve on water supply line
- NI-9263 Channel 3 → 0-10V control signal (threshold comparator → 24V relay)
- OR: 24V relay controlled by NI-9263 via low-power transistor
- Timing: Laptop triggers filling based on pressure/timing

**Method 2: Pump Control**
- Diaphragm pump with variable speed (0-10V analog input)
- NI-9263 Channel 3 → pump voltage directly
- Flow rate proportional to output voltage

**Decision Pending**: Which mechanism is installed?

### Future: Air Purging & Argon Backfill

**Planned Control** (not yet implemented):
- Solenoid valve to shut off air pump → evacuate chamber
- Solenoid valve to open argon bottle supply
- Pressure transducer (NI-9219) monitors argon pressure rise
- Automatic shutdown sequence when target pressure reached

**NI-9263 Channel 4** reserved for this function.

---

## Safety Interlocks

**Critical Conditions (Monitored by NI-9219 + Laptop)**
- ✓ Thermocouple failure → Abort heating immediately
- ✓ Vacuum loss (pressure > threshold) → Stop heating, vent chamber
- ✓ Over-temperature (> 1050°C setpoint) → Cut power to supply
- ✓ Water flow loss (TBD sensor) → Abort operation

**Hard-wired Emergency Stop**
- E-stop button on power supply → Cuts all coil power (independent of laptop)
- Manual vacuum vent valve → Opens chamber safely if electronics fail

---

## Cable & Connectivity Strategy

### NI-9219 Input Signals (Shielded Twisted Pair)
- Thermocouple leads → Twisted pair, twisted pair shield to ground at DAQ end only
- Pressure transducer (if 4-20mA) → Current loop, separate twisted pair, ferrite choke
- Keep signal cables away from high-frequency coil leads

### NI-9263 Output Signals (Stranded, Lower EMI Concern)
- Coil power supply command (0-10V) → 22 AWG min, shielded if runs near coil leads
- Solenoid/pump control (0-10V) → 22 AWG, low-voltage shielding
- Stepper motor control TBD once controller identified

### Coil Leads (High-Frequency, ~1 MHz)
- Existing bronze tube feedthroughs through chamber wall
- Keep physically separated from low-voltage control signals
- No shared conduit or cable tray
- Ferrite filters on coil leads if EMI issues observed

### Power Distribution
- Laptop power: Standard AC outlet (isolated from RF noise if possible)
- NI-DAQ module power: Via USB (from laptop) or external 24V supply (TBD)
- 24V control voltage: Separate isolated supply for solenoids/relays
- Ground star point: Consolidate all grounds at DAQ chassis to minimize loops

---

## Software Architecture (Outline)

**Control Software Runs On Laptop**

```
Initialization:
  ├─ Connect to NI-9219 (ADC)
  ├─ Connect to NI-9263 (DAC)
  └─ Load control parameters (setpoint, PID gains, limits)

Main Loop (runs every Δt):
  ├─ Read NI-9219 (temperature, pressure, other feedback)
  ├─ Calculate PID output for coil power
  ├─ Check safety interlocks
  │  ├─ If vacuum lost → Abort, vent, log event
  │  ├─ If thermocouple failure → Abort, alert operator
  │  └─ If over-temp → Cut power, log event
  ├─ Write NI-9263 outputs
  │  ├─ Channel 1 → Coil power supply command (0-10V)
  │  ├─ Channel 2 → Stepper motor speed (TBD)
  │  └─ Channel 3 → Water system (solenoid or pump)
  └─ Log all readings + timestamps

Quench Trigger (User or Auto):
  ├─ Stop PID loop
  ├─ Activate water valve (NI-9263)
  ├─ Record pressure spike + timing
  └─ Wait for cooldown

Shutdown:
  ├─ Zero all NI-9263 outputs
  ├─ Close DAQ connections
  └─ Save data log
```

**Framework Options:**
- LabVIEW (native NI support, graphical)
- Python + PyDAQmx (flexible, open-source)
- C# + NI-DAQmx (robust, performant)

---

## Outstanding Questions & Actions

| Item | Status | Action |
|------|--------|--------|
| Stepper controller model | ❌ Unknown | Locate controller, document input type |
| Stepper controller input type | ❌ TBD | Pulse/direction? Analog? Serial? USB? |
| Power supply control interface | ⏳ Assumed 0-10V | Verify power supply accepts analog input |
| Water system mechanism | ⏳ TBD | Direct valve or pump? Current setup? |
| NI-DAQ chassis connection | ⏳ TBD | USB or Ethernet from laptop? |
| Control software platform | ⏳ TBD | LabVIEW, Python, or C#? |
| Loop update rate | ⏳ TBD | Determine from coil thermal response testing |
| PID tuning parameters | ⏳ TBD | Empirical tuning during commissioning |

---

## Quick Links

📖 **Related Documentation:**
- [[Design/Wiring/INDEX|Wiring Subsystem INDEX]]
- [[Design/Wiring/Electrical System|Original Electrical System Overview]]
- [[Design/Mechanisms/Control System|Mechanisms & Automation (Manual Phase)]]
- [[Design/Plumbing/Fluid Systems|Fluid Systems & Valve Control]]
- [[Design/Coil Geometry/Induction Coil|Induction Coil Specifications]]

---

*Architecture defined per user input (08-07-2026): NI-9219 data logging + NI-9263 signal control on legacy laptop. Awaiting stepper controller identification to finalize ball screw integration strategy.*
