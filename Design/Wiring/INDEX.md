---
subsystem: wiring
tags: [index, wiring, electrical-systems, power-delivery, instrumentation]
---

# Wiring & Electrical Systems Subsystem

Power delivery, control circuits, and instrumentation for induction heating and data acquisition systems.

## Files in This Folder

| File | Purpose | Status |
|------|---------|--------|
| **[[Design/Wiring/Electrical System|Electrical System.md]]** | Power delivery & instrumentation overview | 🔵 Hub |
| **[[Design/Wiring/NI-DAQ Control Architecture|NI-DAQ Control Architecture.md]]** | Automated control via NI-9219 + NI-9263 on legacy laptop | 🟢 Active |
| **[[Design/Mechanisms/Ball Screw Motor Control|Ball Screw Motor Control.md]]** | 24V stepper motor control system (power supply, controller, driver, motor) | 🟢 Complete |

---

## Connected Subsystems

### 🔥 Heating Element
- **[[Design/Coil Geometry/Induction Coil\|Coil Geometry]]** — Induction coil impedance & power requirements
  - *Connection*: Coil load impedance must match power supply output; frequency ~1 MHz; high-frequency leads via coil lead pass-throughs; cooling water circuit isolated from power

### 📦 Chamber Penetrations
- **[[Design/Vacuum Chamber/Vacuum Enclosure\|Vacuum Chamber]]** — Power leads through chamber wall
  - *Connection*: Coil lead pass-throughs (high-frequency power); thermocouple feedthrough (signal); pressure gauge (instrumentation)

### 💧 Plumbing Control
- **[[Design/Plumbing/Fluid Systems\|Plumbing & Fluid Systems]]** — Valve & pump control signals
  - *Connection*: Solenoid valve actuation (24V); 24V diaphragm pump power; pressure sensor monitoring

### 🔧 Mechanical Control & Actuation
- **[[Design/Mechanisms/Control System\|Mechanisms & Automation]]** — Control signals, feedback loops, safety interlocks
  - *Connection*: Temperature sensor (thermocouple); pressure monitoring (vacuum safety); emergency stop wiring; quench trigger signal
- **[[Design/Mechanisms/Ball Screw Motor Control\|Ball Screw Motor Control]]** — 24V stepper motor system for vertical sample positioning
  - *Connection*: Power supply (SDN 10-24-100P, 24V @ 10A); Motion controller (ST-PMC1) receives external commands; Motor driver (TB6600) sends coil drive signals; Limit switch input for automatic homing

### ❄️ Quench Triggering
- **[[Design/Sample Quenching/Quenching Methods\|Sample Quenching Routes]]** — Quench initiation timing
  - *Connection*: Timer circuit or manual trigger → solenoid valve opens; feedback from thermocouple used for temperature-triggered quench option

---

## Power System Architecture

### Induction Heating Power (High-Frequency)
- **Frequency**: ~1 MHz (induction heating standard)
- **Max Power**: Limited by supply capability and coil losses
- **Output**: High-frequency AC to coil
- **Status**: Specifications pending equipment selection

### Stepper Motor Control Power (24V DC)
- **Power Supply**: [[Design/Mechanisms/Ball Screw Motor Control#Component Details|SolaHD SDN 10-24-100P]]
- **Output**: 24V DC @ 10A (240W max)
- **Scope**: Powers ST-PMC1 motion controller, TB6600 stepper driver, solenoid valve, diaphragm pump
- **Protection**: Indefinite short-circuit protection, overvoltage/overtemperature shutdown
- **Status**: 🟢 Complete — Components identified and specified

### Coil Connections (Heating Element)
- **High-Frequency Leads**: Via coil lead pass-throughs (Omega SSLK-14-14, 1/4" x 1/4" NPT compression fittings — bronze/flared-fitting design rejected due to excessive resistive loss, see [[Design/Archive/INDEX|Archive]])
- **Connection Type**: Soldered high-frequency joints; lead from coil to power supply is now one continuous piece (must un-solder/re-solder to swap or iterate on the coil)
- **Impedance Matching**: To be verified during thermal testing
- **Cooling**: Coil cooled via separate water circuit (isolated from electrical)

### Control & Low-Voltage (24V DC)
- **Stepper Motor**: Ball screw vertical positioning (see [[Design/Mechanisms/Ball Screw Motor Control|Ball Screw Motor Control]])
  - Motion controller: ST-PMC1 (40 kHz pulse+direction, 99 programmable sequences)
  - Driver: TB6600 (5–10A per coil phase)
  - Motor: NEMA 23/34 with integrated ball screw (~0.025mm positioning)
- **Solenoid Valve**: 24V for quench trigger (via relay output from ST-PMC1)
- **Diaphragm Pump**: 24V option for spray quenching
- **Pressure Relief**: Valve in air control assembly
- **Emergency Stop**: E-stop on main power supply + manual vent valve
- **Homing**: Limit switch input to ST-PMC1 for automatic home finding

---

## Instrumentation & Monitoring

### Temperature Measurement
- **Sensor**: Spot-welded thermocouples on sample surface
- **Feedthrough**: Dedicated thermocouple pass-through in chamber wall
- **Amplification**: External amplifier (TBD)
- **Purpose**: Record thermal profile during 1000°C heating → quench cycle
- **Data Format**: Voltage output to logging system (TBD)

### Pressure Monitoring
- **Vacuum Gauge**: Chamber pressure (air control assembly)
- **Purpose**: Verify adequate vacuum before heating; detect leaks
- **Feedback**: Interlock prevents heating if vacuum insufficient
- **Display**: External gauge (analog or digital TBD)

### Data Acquisition (Prototype Phase)
- **Status**: External equipment integration
- **Parameters**: Temperature, pressure, quench event timing
- **Sampling Rate**: TBD (depends on material transient behavior)
- **Synchronization**: Quench event timestamping critical

---

## Safety Systems

### Critical Interlocks
- **Vacuum Requirement**: System won't heat without adequate chamber vacuum
- **Temperature Limits**: Emergency shutdown if thermocouple exceeds safe threshold
- **Coolant Monitoring**: Won't operate if coil cooling water fails (TBD)
- **Access Control**: Door interlock prevents opening under vacuum
- **Emergency Stop**: E-stop button cuts all power immediately

### Monitoring & Alerts
- **Over-temperature**: Thermocouple feedback → shut off power supply
- **Low Vacuum**: Pressure gauge → halt operation, vent chamber
- **Quench Failure**: If valve doesn't open, log event, abort test
- **Sensor Failure**: Loss of thermocouple signal → alert operator

---

## Current Status

### Defined
- ✓ Power frequency target (~1 MHz)
- ✓ Coil lead routing (via pass-throughs)
- ✓ Control voltages (24V standard for solenoids/pumps)
- ✓ Thermocouple measurement requirement
- ✓ Safety interlock logic

### Pending Equipment Selection
- ⏳ Induction power supply model & specs
- ⏳ Thermocouple amplifier + data logger
- ⏳ Pressure transducer for automated monitoring
- ⏳ Control circuit implementation (PLC, timer relay, or manual)
- ⏳ Cable sizing & EMI shielding strategy

### Design Decisions Needed
- ⏳ Timer-based vs. temperature-triggered quench
- ⏳ Analog vs. digital instrumentation
- ⏳ Data logging integration (local vs. cloud)
- ⏳ Grounding & shielding strategy for high-frequency coil

---

## Next Steps

1. **Finalize Power Supply Selection** — Choose equipment based on coil impedance
2. **Impedance Verification** — Measure coil load during thermal testing
3. **Control Circuit Design** — Specify timer/trigger logic for quench
4. **Instrumentation Integration** — Select thermocouple amplifier & logger
5. **EMI/Shielding Analysis** — Route high-frequency coil leads away from signals
6. **Safety Review** — Validate interlock logic with Dr. Buchely

---

## Quick Links

📖 **Related Reading:**
- [[Design/Coil Geometry/Round Coil\|Coil Design]] — Heating element specifications
- [[Design/Plumbing/Coil Lead Pass-Throughs\|Lead Feedthroughs]] — Power delivery through chamber
- [[Design/Mechanisms/Control System\|Control Strategy]] — Signal timing & automation
- [[Design/Vacuum Chamber/Vacuum Enclosure\|Chamber Feedthroughs]] — All penetrations overview

---

*Electrical system ties heating, control, and instrumentation together. Clean design critical for reliable testing.*
