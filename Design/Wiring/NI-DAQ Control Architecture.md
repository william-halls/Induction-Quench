---
subsystem: wiring
tags: [wiring, control-system, NI-DAQ, automation, PID, stepper-motor]
---

# NI-DAQ Control Architecture

Automated control system for PID-based induction coil power management and auxiliary system automation.

**Hardware update (2026-09-01, superseded multiple times — see below):** Actual DAQ hardware in hand is a **cDAQ-9174 (4-slot chassis)** populated with **NI-9269, 9229, 9211, and 9219**, plus a standalone **USB-6009** for digital I/O. Earlier passes through this doc assumed first the original NI-9219/NI-9263 pairing, then a single-slot-chassis constraint (9211 vs. 9269) requiring an external isolator for the HOTSHOT — neither is current. **Current architecture**: **thermocouple → NI-9229** (confirmed, 2026-09-01 — chosen over the 9211 for its speed, 50kS/s/ch, needed given how fast the sample heats; requires an external analog CJC amplifier, e.g. AD8495, placed at the feedthrough before the 9229, since raw mV-level TC signal fed directly into the 9229 was found to have unacceptable noise — see "Thermocouple Signal Chain" below), NI-9269 (native isolated 0-10V) driving the HOTSHOT directly, USB-6009 for digital SSR control only, **9211 and 9219 spare**. See "HOTSHOT Control — Consolidated onto NI cDAQ-9174" and "Thermocouple Signal Chain" below for the full current picture; sections elsewhere in this doc referencing NI-9219/9263, the 6009-AO0-isolator plan, or NI-9211 as the TC module are kept for historical context only.

## System Overview

**Control Platform**: Legacy laptop running LabVIEW or Python DAQ software
**Data Acquisition**: NI-9219 (analog input module)
**Signal Control**: NI-9263 (analog output module)
**Motor Control**: [[Design/Mechanisms/Ball Screw Motor Control|Ball screw stepper system]] (SN: 161104226 motor, ST-PMC1 controller SN: 170120011)

### Control Goals

1. **Induction Coil Power** — PID loop to maintain target temperature
2. **Ball Screw** — Automatic linear motion (stepper motor driven)
3. **Water Filling System** — Automatic control (solenoid valve or pump) — fills/maintains the bath only; not part of the quench trigger itself
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
- **Induction Power Supply**: Takes 0-10V analog control signal — **confirmed 2026-09-01** via HOTSHOT manual (Doc# 801-9252k.doc): CTB1 pins 1(+)/2(−), default 0-10Vdc, jumper-selectable to 4-20mA. Requires touch-pad setting System Options → Control From → Rear Panel. See [[Design/Wiring/Electrical System|Electrical System]] for full CTB1 pinout. **Scaling/linearity confirmed by bench measurement 2026-09-06 — see "HOTSHOT Setpoint Calibration Curve" below.**
- **Stepper Motor Controller**: NEMA 23 2-phase driver (model TBD)
  - Current status: Unknown input type (pulse/direction vs. analog vs. serial)
  - Power supply: Existing (size/voltage TBD)
- **Solenoid Valve(s)**: 24V for water filling only — NOT used for quench triggering; quenching is done by the ball screw lowering the sample into the (separately filled) water bath, with no valve or release mechanism involved
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
- Power supply input impedance & control linearity (0-10V input confirmed via CTB1:1-2, but exact scaling not yet known — see [[Design/Wiring/Electrical System|Electrical System]])

**Loop-speed bottleneck chain (2026-09-01):**

| Link | Response time | Source |
|---|---|---|
| USB-6009 AO0 write | 150 Hz max update rate (software-timed), 1V/µs slew (not a real constraint) | [NI USB-6009 spec sheet](https://download.ni.com/support/manuals/375296c.pdf) — confirmed |
| GLG isolator (ASIN B09KTBJGZB) | ~10ms class ("millisecond fast response" per mfr datasheet) | [GLG datasheet](https://ae01.alicdn.com/kf/S9acb6ff1937942929761b3ccd8fa99d1M.pdf) — confirmed, not a bottleneck |
| HOTSHOT internal power ramp/slew behavior | **Unknown** | Not in the manual pages photographed; a promising lead (photo album on rhinofablab.com) is currently unreachable due to a broken SSL cert on that site — try opening manually in a browser |

The 6009's 150Hz hardware ceiling and the isolator's ~10ms response are both well-characterized and unlikely to be the limiting factor. The HOTSHOT's own internal ramp/slew behavior on the remote power command remains the open unknown — many RF supplies rate-limit power changes internally for reliability, which could dominate regardless of how fast the command signal updates. **Recommend bench-testing this directly once wired**: step AO0 between two values and time the HOTSHOT's actual power response (front-panel reading or CTB1 current scope) rather than relying on datasheet numbers alone.

### Ball Screw Control

**Status**: 🟢 **IDENTIFIED** — See [[Design/Mechanisms/Ball Screw Motor Control|complete motor control documentation]]

**System Components:**
- **Motion Controller**: ST-PMC1 (SN: 170120011) — Programmable stepper sequencer
  - Power: 24V DC from [[Design/Mechanisms/Ball Screw Motor Control#Component Details|SDN 10-24-100P power supply]]
  - Outputs: CP (pulse) + CW (direction) signals to stepper driver
  - Frequency: 1–40 kHz (1 Hz resolution)
  - Capability: Up to 99 programmed sequences
- **Stepper Driver**: TB6600 (SN: 170120011) — Coil amplifier
  - Inputs: Pulse + direction from ST-PMC1
  - Output: 5–10A per coil phase @ 24V
  - Microstepping: Full, 1/2, 1/4, 1/8, 1/16 step options
- **Motor**: NEMA 23 stepper with integrated ball screw (SN: 161104226)
  - 200 steps/revolution (1.8° per step)
  - Positioning: ~0.025mm per full step

**Integration with NI-DAQ:**

**Option A: ST-PMC1 Standalone Mode** (Recommended for simplicity)
- ST-PMC1 operates independently with programmed sequences
- NI-DAQ monitors system state but does not command motor directly
- Trigger: External trigger signal (NI-9263 relay output) can start pre-programmed sequence
- Advantage: Decouples motor control from laptop; failsafe operation if NI-DAQ fails
- Implementation: Configure ST-PMC1 sequences, wire NI-9263 relay to ST-PMC1 trigger input

**Option B: Real-Time Motor Control via NI-DAQ**
- NI-9425 (digital output module) generates pulse+direction signals directly to TB6600 driver
- Bypasses ST-PMC1, controlled entirely by laptop
- Frequency: Up to 100+ kHz (exceeds stepper controller capability)
- Advantage: Full laptop control, real-time coordination with coil power and quench timing
- Disadvantage: Laptop must manage timing; stepper driver requires lower frequencies (~20 kHz typical)
- Implementation: Wire NI-9425 Channels 1-2 to TB6600 pulse/direction inputs

**Recommended Approach**: **Option A (Standalone ST-PMC1)**
- Simplicity: Hardware already supports this architecture
- Reliability: Motor control independent of laptop/NI-DAQ
- Flexibility: Can still coordinate timing via external triggers
- No additional hardware needed (no NI-9425 module required)

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
- E-stop button on power supply → wired N.C. across HOTSHOT CTB1:15-16 (24V @ 3A min); tripping it halts RF output and equipment operation, reset restarts at Home zone (confirmed 2026-09-01, Doc# 801-9252k.doc — see [[Design/Wiring/Electrical System|Electrical System]]). Independent of laptop.
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

## HOTSHOT Control — Consolidated onto NI cDAQ-9174 (2026-09-01, supersedes isolator plan below)

**Major architecture change:** discovered a **cDAQ-9174 (4-slot USB chassis)** already in hand, populated with **9269, 9229, 9211, and 9219** modules — not the single-slot constraint assumed earlier. This resolves several open questions at once:

- **HOTSHOT power command → NI-9269 directly.** The 9269 is an isolated, hardware-timed, native ±10V analog output (100kS/s/ch) — it can drive the HOTSHOT's CTB1:1-2 in its **default 0-10Vdc mode** with no current-loop conversion, no isolator, no divider, and no 6009 involvement at all. This makes the entire "6009 AO0 + GLG isolator (ASIN B09KTBJGZB)" plan below **obsolete** — kept only as historical record in case the 9269 turns out to be needed elsewhere.
- **Thermocouple → NI-9229** (via external analog CJC amplifier — see "Thermocouple Signal Chain" below; corrected 2026-09-01, previously stated as 9211 in error).
- **9211 and 9219 remain spare** for future use (pressure transducer, second TC channel, or backup slow-but-accurate TC reading if ever needed).
- **USB-6009 role narrows to purely digital**: SSR triggers for the stepper system (RUN/IN1/IN2/A/B per [[Design/Wiring/Ball Screw Motor Control|Ball Screw Motor Control]]), AC power release, and the diaphragm pump — no analog duties left on it.

**Eurotherm 2416 controllers (x2) — set aside from the automated loop.** Investigated using one as a standalone PID heating controller (Module 1 = `H5`, native 0-10V heating output — a strong hardware match), but decided against it: the plan now is **continuous, temperature-informed sample scanning through the coil during heating**, which requires the PID loop and the motion control to run in the same real-time software (see [[Design/Wiring/Ball Screw Motor Control|Ball Screw Motor Control]] "On-Demand Position Control" section for the resulting motion architecture). A standalone Eurotherm can't coordinate tightly enough with motion for this. Both units are kept as spares/manual-backup instruments, not part of the automated system. (Also, if reconsidered later: the pre-configured unit's sensor type (J, not confirmed to match actual TC), temperature range (50-300°F — far too low for ~1000°C), and control action (Direct acting — likely wrong for a heating-only load) would all need correcting before use; the second unit's DC module is unconfigured and would need full setup from scratch.)

### Thermocouple Signal Chain — NI-9229 + Analog CJC Amp (confirmed 2026-09-01)

```
Thermocouple → AD8495 (or similar analog CJC amp, mounted at the chamber feedthrough)
             → isolated NI-9229 channel (50kS/s/ch, fast enough for the PID loop)
```

**Why not the 9211 or the 9229 alone:**
- **9211 alone**: accurate (built-in CJC, no amp needed) but only ~14 S/s/ch — far too slow for the fast heating rate this process needs; the PID loop would be flying blind for tens of ms at a time.
- **9229 alone**: fast (50kS/s/ch) but no CJC and built for wide-range signals (up to ±60V) — feeding it a raw ~40mV TC signal directly was tested and found to have ~1mV of noise, i.e. ~24°C of error, unacceptable for control. Also highly exposed to EMI picking up off the ~1MHz induction coil over any cable run.
- **9229 + analog CJC amp**: the amp does CJC and boosts the signal to volts-scale *before* the long cable run to the DAQ, fixing both the noise/SNR problem and the CJC problem, while keeping the 9229's speed (amplifier bandwidth ~2kHz class, not a slow digital transmitter).

**Still open:** exact amplifier part not yet finalized (AD8495 was the example discussed — confirm TC type match, e.g. K-type variant, before ordering) and 9229 channel scaling/calibration against the amp's mV/°C output.

### START / STOP (CTB1:3-5) — still applies regardless of AO source

- **START (CTB1:3-4)** — N.O., momentary closure initiates heating. If automated start is wanted, needs its own DC SSR (same LCD4075DD3-type as the ST-PMC1 RUN trigger), driven as a brief pulse from a spare 6009 digital line — same pattern as ST-PMC1 RUN.
- **STOP (CTB1:4-5)** — N.C., **active-open**: the loop must stay **closed** for heating to be allowed; **opening** it halts heating. This is the opposite of "power it to stop" — you break the connection to stop, not energize something.

**Decision (confirmed 2026-09-07): route through an SSR.** The [[Design/Wiring/Heat Curve Profile Software|Heat Curve Profile Software]]'s Stop control needs to command the HOTSHOT to stop from software, which requires this line to be DAQ-controlled rather than left hardwired — resolving the prior open decision in favor of the SSR option:
  - SSR driven **ON by default** (holding the loop closed) and **de-energized to stop** — this fails safe: DAQ/SSR power loss opens the loop and halts heating on its own, same as a genuine Stop command.
  - Software's Stop action de-energizes this SSR (opens STOP) **and** commands 0A on the NI-9269 output at the same time — belt-and-suspenders, since either alone should halt heating, but doing both avoids relying on only one mechanism.
  - **Do not** put START and STOP on the same DAQ line/SSR — START is a momentary edge-trigger, STOP is a continuously-held level signal; tying them together either causes an immediate self-halt right after starting, or risks re-triggering START if held high (same failure mode already flagged for the ST-PMC1's 1-button start mode).

### TXDIN70 — Dual-Channel Transmitter/Isolator (spare / optional future use, 2026-09-01)

Professor-provided part: **Omega TXDIN70** (confirmed base model, 100-240VAC — not the -24V variant), a DIN-rail dual-channel temperature transmitter/isolator. Full manual pulled from Omega/Dwyer (M4544).

**Status update (2026-09-01):** Originally planned as Channel 1 = sample thermocouple / Channel 2 = HOTSHOT power command, but **superseded on both counts:**
- **Thermocouple duty → NI-9211** (already owned, no purchase needed either way once that was confirmed). The 9211 wins on simplicity: native digital TC input with built-in CJC, no burden resistor, no separate AC power circuit, no dependency on the TXDIN70-DISPLAY accessory (unconfirmed whether available) to configure it. See [[Design/Wiring/Ball Screw Motor Control|Ball Screw Motor Control]] "USB-6009 vs. chassis" discussion — the original 9211-over-9269 chassis-slot decision stands.
- **HOTSHOT power command → 6009 AO0 + generic 0-5V-to-4-20mA isolator** (ASIN B09KTBJGZB) — direct 0-5V match with no divider needed, DC-powered off the shared 24V rail, no configuration-accessory dependency. See below.

**TXDIN70 is therefore set aside as spare / optional future use** — e.g. a second thermocouple channel (the "optional coil temperature feedback" mentioned elsewhere in this doc) if that becomes needed later. The terminal/configuration reference below is kept in case it gets pressed into service.

**Terminal layout (confirmed from manual, page 15):**

| Terminals | Function |
|---|---|
| 1, 2 | Power supply — **confirmed 100-240VAC** (this is the base **TXDIN70**, not the -24V variant, per the professor-provided unit's label, 2026-09-01). Do **not** wire to the shared 24V DC rail — needs its own AC feed, ideally always-on/independent of the switched stepper AC-release circuit so temperature/power-command function stays available regardless of stepper subsystem state |
| 3, 4 | Also under the power block on the physical layout — likely redundant/pass-through screw pairs for 1/2, not a separate function; verify against the physical unit |
| 5 (+), 6 (−) | **Channel 1 current retransmission output** (OP1) |
| 7 (+), 8 (−) | **Channel 2 current retransmission output** (OP2) |
| 9–12 | **Channel 2 input** (IN2) — manual explicitly calls out 10-12 for the input; exact +/− pin within this block for a linear voltage input (vs. the 3-wire RTD layout these terminals also support) needs to be read off the physical unit's printed diagram, not fully resolved from the manual text extraction |
| 13–16 | **Channel 1 input** (IN1) — manual explicitly calls out 14-16 for the input; same caveat on exact +/− pin as above |

**Isolation:** ≥2300Vdc between power/input/output; ≥200Vdc between the two inputs or two outputs. Confirms this is a strong isolation choice for the HOTSHOT channel, well beyond the generic isolator's unspecified rating.

**Configuration (via TXDIN70-DISPLAY accessory or equivalent — confirm the unit has one before assuming this is doable):**

Each channel is configured with these parameters (x = channel 1 or 2):

| Parameter | Meaning | Setting for our use |
|---|---|---|
| `INPx` | Input type code | Channel 1 (thermocouple): `INP1 = 0` for K-type (confirm actual TC type used — S=1, R=2, T=3, E=4, J=5, B=6, N=7, etc.). Channel 2 (HOTSHOT voltage): `INP2 = 31` for 0-1V range (or `32` for 0.2-1V) |
| `SCLx` / `SCHx` | Scale low/high limit — defines the engineering-unit range the input represents | Channel 1: e.g. `SCL1=0, SCH1=1300` (°C, per K-type range). Channel 2: arbitrary units representing 0-100% power, e.g. `SCL2=0, SCH2=100` |
| `OPn` | Retransmission mode (global, not per-channel) | **Must be `OPn=1`** ("1 input 1 output or 2 inputs 2 outputs" — independent channels). `OPn=2` would duplicate channel 2's input across both outputs and ignore channel 1 — wrong for our use, since we need channel 1 (TC) and channel 2 (HOTSHOT) to be fully independent |
| `OPL2` / `OPH2` | Channel 2 output current low/high limit, units of 0.1mA | `OPL2=40` (4.0mA), `OPH2=200` (20.0mA) — maps SCL2-SCH2 (0-100%) linearly to 4-20mA |
| `OPL` / `OPH` | Same, for Channel 1 | Set to match whatever 4-20mA temperature span is wanted for logging, e.g. `OPL=40, OPH=200` mapped to the SCL1-SCH1 temperature range |
| `Loc` | Parameter lock | Must be `808` to allow editing; anything else locks parameters read-only |

**⚠️ Still open:**
- Confirm the unit has (or came with) the **TXDIN70-DISPLAY** accessory — the manual states configuration is done via that external plug-in display; without it, reconfiguring channel input/output types may not be possible.
- Confirm exact TC type in use (K-type assumed but not verified) for `INP1`.
- Confirm exact +/− terminal assignment within the 9-12 and 13-16 input blocks against the physical unit's printed diagram before wiring live.
- ~~Confirm which power variant was provided~~ — ✅ confirmed base TXDIN70, 100-240VAC (2026-09-01). Need to source an always-on AC feed for it, separate from the switched stepper AC-release circuit.
- 5:1 voltage divider (6009 AO0 0-5V → 0-1V) still needs sizing/verification against the TXDIN70's actual input impedance — see prior discussion.

**Source:** [TXDIN70 datasheet, Omega](https://assets.omega.com/spec/TXDIN70.pdf); [TXDIN70 User's Guide, Omega/Dwyer (M4544)](https://assets.dwyeromega.com/manuals/communication-and-connectivity/signal-conditioners-and-transmitters/signal-conditioners/M4544.pdf)

### HOTSHOT Setpoint Calibration Curve (confirmed 2026-09-06)

Bench-measured with `Control From` set to **Rear Panel** and `Start From` left on **Front Panel** (physical START/STOP buttons, no CTB1:3-5 wiring yet), sweeping a 0-10V command onto CTB1:1-2 and reading the resulting Setpoint/Output current off the front panel display:

| Command (V) | Output (A) |
|---|---|
| 1.0 | 7.7–8.8 |
| 1.2 | 13 |
| 1.3 | 27.5 |
| 1.4 | 40.7 |
| 1.5 | 55 |
| 1.6 | 68.2 |
| 2.0 | 123.2 |
| 3.0 | 260 |
| 4.0 | 397 |
| 5.0 | 534 |
| 6.0 | 550 |
| 10.0 | 550 |

**Not a single 0-10V→0-100% linear ramp.** The data fits a threshold + linear + saturation model:

1. **Turn-on threshold ≈ 1.10V.** Below this, RF output is effectively off (the 1.0V reading of ~8A sits just under the knee, consistent with the fitted line crossing zero at 1.10V rather than a separate "dead zone").
2. **Linear region, ~1.10V–~5.1V**: fits **`Amps ≈ 137 × V − 151`** almost exactly (predicted vs. actual matches to within ~1A across 1.2V–5.0V).
3. **Saturation ceiling at 550A, from ~5.1V up to 10V.** Output stops climbing entirely — 6V and 10V both read 550A. This is most likely the HOTSHOT's **Icap** (capacitor safe-current) limit for the current tap/cap setting (§2.3 of the manual: "you are attempting to adjust Setpoint above safe limit for capacitors used"), not a wiring or DAQ scaling problem. **Still open**: visually confirm an `Icap`/`Limit`/`L*` indicator appears on the display at ≥5.1V commanded, to fully confirm this theory rather than some other cause.

**Control software implication**: usable proportional-control range is **1.10V–5.1V**, mapping to roughly **0–550A**. Inverse mapping for a PID output stage: `V = (Amps + 151) / 137`, clamped to [1.10, 5.1]. Commanding above 5.1V wastes range (pinned at 550A); below 1.10V produces no output.

**PID software architecture decision (2026-09-06):** run the PID loop's output in **Amps**, with `PID Advanced.vi` Output Range set to **Low = 0, High = 550** (anti-windup clamps at the real saturation ceiling). The Amps→Volts conversion (`V = (Amps+151)/137`, clamped [1.10, 5.1]) happens as a separate step *after* the PID block, not inside its tuning.

**"Off" cutoff at 50A**: rather than special-casing an explicit heating-off branch, treat any PID output **≤50A as off** (write ~0V / skip HeatOn) instead of passing it through the voltage-conversion formula. Rationale: physically, anything up to ~50A doesn't do meaningful induction heating work on the part regardless of what the display shows — so the entire electrical turn-on knee (1.0V→~8A, 1.2V→13A, i.e. the non-smooth jump right at RF turn-on) sits inside a range that's already irrelevant to the process. No need to smooth or finely control that region at all; just treat all of it as "off." This means the meaningful controllable range is effectively **off, or ~50A+ up to 550A** — fine for this application since the goal is regulating toward a hot setpoint, not holding a precise low-current simmer.

**To raise the 550A ceiling** (if more current is ever needed): would require changing the transformer tap per manual §3.4 ("start high, work down" rule) — only with no part in the coil, and only if actually needed for the process.

### Status outputs (CTB1:8-13) — read-only, no SSR needed

Ready / HeatOn / Fault are solid-state contact **outputs from** the HOTSHOT (no polarity, 1A limit) — read these into free 6009 digital inputs with pull-up resistors, same pattern already used for the SOLA DC-OK status read. Not yet wired; optional future addition.

---

## Laptop / Host Update (08-17-2026)

- NI devices (NI-9219, NI-9263, etc.) will be plugged into a **new laptop** rather than the legacy laptop originally referenced above.
- New laptop has **NI App** (NI hardware configuration/driver software) installed.
- Action: verify NI-DAQmx driver version compatibility and re-confirm chassis connection type (USB vs. Ethernet) on the new machine once devices are connected.

---

## Outstanding Questions & Actions

| Item | Status | Action |
|------|--------|--------|
| Stepper controller model | ✅ Identified | [[Design/Mechanisms/Ball Screw Motor Control|ST-PMC1 (SN: 170120011) + TB6600 (SN: 170120011)]] |
| Stepper controller input type | ✅ Pulse/Direction | NI-DAQ Option A: Standalone; Option B: NI-9425 for real-time control |
| Stepper motor model | ✅ Identified | [[Design/Mechanisms/Ball Screw Motor Control|NEMA 23 with ball screw (SN: 161104226)]] |
| Power supply control interface | ✅ Confirmed, native direct drive, scaling measured | CTB1:1-2 stays in default 0-10Vdc mode; NI-9269 drives it directly, no isolator/current-loop conversion needed (2026-09-01, supersedes prior 6009+isolator plan); scaling confirmed by bench measurement 2026-09-06 — see "HOTSHOT Setpoint Calibration Curve" above (1.10V threshold, `Amps ≈ 137×V − 151` linear region, 550A ceiling ≥5.1V) |
| E-stop interlock interface | ✅ Confirmed pinout, ⏳ scope TBD | CTB1:15-16, N.C., 24V@3A min (2026-09-01) — see [[Design/Wiring/Electrical System|Electrical System]]; unclear if it also cuts mains/chassis power or only internal 24V rail |
| HOTSHOT START/STOP automation | ✅ START planned, STOP confirmed (SSR, fail-safe) | STOP routed through an SSR (ON by default, de-energize to stop) per Heat Curve Profile Software's Stop requirement (2026-09-07) — see "START / STOP (CTB1:3-5)" section above |
| Water system mechanism | ✅ Diaphragm pump, DC SSR | 24V DC pump confirmed (not AC) — needs dedicated DC-rated SSR, not the AC power-release SSR; see [[Design/Wiring/Ball Screw Motor Control|Ball Screw Motor Control]] SSR inventory (2026-09-01) |
| NI-DAQ chassis connection | ⏳ TBD | USB or Ethernet from laptop? |
| Control software platform | ⏳ TBD | LabVIEW, Python, or C#? |
| Loop update rate | ⏳ TBD | Determine from coil thermal response testing |
| PID tuning parameters | ⏳ TBD | Empirical tuning during commissioning |

---

## Quick Links

📖 **Related Documentation:**
- [[Design/Wiring/Heat Curve Profile Software|Heat Curve Profile Software]] — multi-segment ramp/hold GUI and execution engine built on top of this PID loop
- [[Design/Wiring/INDEX|Wiring Subsystem INDEX]]
- [[Design/Wiring/Electrical System|Electrical System Overview]]
- [[Design/Mechanisms/Ball Screw Motor Control|Ball Screw Motor Control (Complete Specification)]]
- Mechanisms & Automation (Manual Phase)
- [[Design/Plumbing/Fluid Systems|Fluid Systems & Valve Control]]
- [[Design/Coil Geometry/Induction Coil|Induction Coil Specifications]]

---

*Architecture defined per user input (08-07-2026): NI-9219 data logging + NI-9263 signal control on legacy laptop. Awaiting stepper controller identification to finalize ball screw integration strategy.*
