---
tags: [design, wiring, electrical, power-delivery, control-systems]
---

# Wiring & Electrical Systems

Power delivery, control circuits, and instrumentation for induction heating and data acquisition systems.

## Current Design: Power & Instrumentation (TBD)

**Status**: In progress — Equipment and topology under evaluation.

### Power System
- **Induction Supply**: TBD (frequency ~1 MHz based on coil design)
- **Max Power**: Limited by supply capability and coil losses
- **Coil Connections**: Soldered high-frequency leads via pass-throughs
- **Impedance Matching**: To be verified during thermal testing

### Control Systems (24V DC)

#### Ball Screw Motor Control
**See**: [[Design/Mechanisms/Ball Screw Motor Control|Complete Ball Screw Motor Control documentation]]

- **Power Supply**: SolaHD SDN 10-24-100P (240W, 24V @ 10A)
- **Motion Controller**: ST-PMC1 (SN: 170120011) — programmable pulse+direction sequencer
- **Stepper Driver**: TB6600 (SN: 170120011) — coil amplifier (5–10A per phase)
- **Motor**: NEMA 23/34 stepper with integrated ball screw (SN: 161104226) — ±0.025mm positioning
- **Homing**: Limit switch on Input #1 for automatic home finding
- **Capabilities**: Up to 99 programmed motion sequences, 40 kHz max frequency

#### Instrumentation & Monitoring
- **Temperature Measurement**: Spot-welded thermocouples on sample + feedthrough
- **Pressure Monitoring**: Chamber vacuum gauge (via air control assembly)
- **Motor Feedback**: Limit switch input to ST-PMC1 for homing detection
- **Data Logging**: External equipment integration (TBD)
- **Emergency Stop**: E-stop on power supply; manual vacuum vent available

## Key Design Factors

- **Power Supply Specs** — Frequency, impedance matching, transient behavior
- **Coil Connection** — Series vs. parallel, load matching, efficiency
- **Shielding & EMI** — Cable routing, ferrite filters, grounding strategy
- **Thermocouple Circuits** — Amplification, noise filtering, cold-junction compensation
- **Safety Systems** — E-stop wiring, fuses, thermal cutouts, interlock circuits
- **Data Acquisition** — Sampling rate, resolution, isolation from power circuits

## Integration Points

- [[Design/Coil Geometry/Induction Coil|Coil Geometry]] — Coil load impedance and connections
- [[Design/Mechanisms/Control System|Mechanisms & Automation]] — Control signals and feedback loops
- [[Design/Mechanisms/Ball Screw Motor Control|Ball Screw Motor Control]] — 24V stepper power, motion controller, homing signals
- [[Design/Plumbing/Fluid Systems|Plumbing & Fluid Systems]] — Pump/valve control circuits
- [[Design/Wiring/NI-DAQ Control Architecture|NI-DAQ Control Architecture]] — Detailed automated control system implementation
- [[Design/Archive/Design History|Design Archive]] — Previous power supply configurations
