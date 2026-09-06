---
subsystem: wiring
tags: [design, wiring, electrical, power-delivery, control-systems]
---

# Wiring & Electrical Systems

Power delivery, control circuits, and instrumentation for induction heating and data acquisition systems.

## Current Design: Power & Instrumentation (TBD)

**Status**: In progress — Equipment and topology under evaluation.

### Power System
- **Induction Supply**: Ameritherm HOTSHOT, model/SN 103927 (Ameritherm was later acquired by Ambrell — check Ambrell for legacy support docs). Ameritherm's HOTSHOT line is a ~1 kW solid-state RF induction heating power supply; confirm actual output frequency/power against the nameplate rather than assuming the general product-line spec.
- **Remote Control**: Hand-held control module, part no. 301-0243D — connects to the HOTSHOT to allow remote start/adjustment away from the unit. No manual located yet for this specific part number; treat button/dial functions as unverified until confirmed against the unit or manufacturer.
- **Chiller**: Unidentified — no documentation on hand. **Action needed**: record nameplate data (make, model, serial, flow rate, cooling capacity, voltage/phase, hose fitting sizes) directly off the unit so it can be matched to a manual or datasheet.
- **Max Power**: Limited by supply capability and coil losses
- **Coil Connections**: Soldered high-frequency leads via pass-throughs
- **Impedance Matching**: To be verified during thermal testing

**Documentation update (2026-09-01)**: Physical manual found for the HOTSHOT 103927 — Doc# 801-9252k.doc (Ameritherm, §3 "Customizing Your HOTSHOT," pages 29–35). *(Note: the photo set from this date was never actually saved into the vault under the filename originally logged here — no file exists at that reference. The full 49-page manual saved 2026-09-05 supersedes it regardless; see below.)*

**Documentation update (2026-09-05)**: Full 49-page manual now saved to the vault: [[media/wiring/Ameritherm HOTSHOT 103927 Manual (Doc 801-9252k).pdf]] (supersedes the partial 2026-09-01 photo set). New sections captured beyond the rear-panel pinout:
- **§4.1 Mechanical**: Power supply — 17x21x4.25 in (432x533x108mm), 10 lb (4.5kg) access weight, aluminum construction, black anodized finish. Remote heat station — 8" (203mm) diameter, ~10 lb (4.5kg).
- **§4.3 Environmental**: Ambient temp 40–95°F; water temp 68–95°F; flow (system-dependent); water conductivity < 65000 (units unclear from photo); pH range noted; dew point margin required to avoid condensation.
- **§2.5 Smartburst Technology**: Heat station capacitors are water-cooled and self-tune continuously; a laboratory current ceiling (~750A example) sets a max continuous-heat time before the unit throttles back (CAP TEMP PROTECT → REDUCE OUTPUT) to protect the capacitors — relevant if we push sustained high-current heating cycles.
- **§3.3 Alarm Limits**: Configurable Lo/Hi output-power alarm band (e.g. Lo <1000W, Hi <1500W examples in manual) with adjustable delay (0–999s) before tripping STOP — could be used as an independent overpower/underpower safety check in addition to the E-stop loop.

Still no manuals for the 301-0243D handheld control or the chiller — if physical manuals for those turn up, photograph/scan them into `media/` and link here.

**HOTSHOT rear panel — CTB1 terminal block pinout (confirmed from manual, 2026-09-01, numeric limits confirmed from §4.2 Electrical table 2026-09-05):**

| Pins | Function | Detail |
|---|---|---|
| 1–2 | Remote analog Input | Default 0-10Vdc, **input impedance Zin = 21kΩ** (1=+, 2=−, jumper set upper); jumper-selectable to 4-20mA, Zin = 250Ω (250Ω resistor included, jumper set lower). Requires touch-pad: System Options → Control From → Rear Panel. **Scaling confirmed by bench measurement 2026-09-06 — not a simple 0V=0%/10V=100% linear ramp**: ~1.10V turn-on threshold, linear `Amps ≈ 137×V − 151` from ~1.10V–5.1V, then saturates at a 550A ceiling (likely the Icap/tap-cap limit) from ~5.1V up to 10V. Full data table in [[Design/Wiring/NI-DAQ Control Architecture|NI-DAQ Control Architecture]] "HOTSHOT Setpoint Calibration Curve" |
| 3–4 | START | Provide isolated contact closure (contacts rated 24Vdc min, required wetting current 3mA) |
| 4–5 | STOP | Normally closed loop; opening = STOP |
| 6–7 | FLS (flow switch) | Internally jumpered by default; remove jumper to wire external N.O. flow switch. Opened contact = Fault. Rated 19Vdc @ 0.1A |
| 8–9 | Ready status output | Isolated solid-state output, non-polarized, 24Vdc @ 1A |
| 10–11 | HeatOn status output | Isolated solid-state output, non-polarized, 24Vdc @ 1A |
| 12–13 | Fault status output | Isolated solid-state output, non-polarized, 24Vdc @ 1A |
| 14–16 | 24V source & E-stop | Pin 14 = ground reference for internal 24Vdc; **E-stop across 15–16, normally closed, rated 3Adc min — opened contact = STOP.** If no E-stop used, 15-16 **must be jumpered** (factory default) — this jumper routes all internal 24Vdc to controls/display. E-stop trip halts RF output **and** equipment operation; reset restarts HOTSHOT at Home zone |
| 17–20 | Aux Input/Output | 24V ±2% @ 1A — **not supported at this time** per manual |
| 21–24 | Serial (RS485) | Rx+/Rx-/Tx+/Tx-; requires optional 305-0174 Ameritherm RS485 kit |

**Equipment power ratings (§4.2, this HOTSHOT is the 7.5kW model per nameplate SN 103927):**

| | 3.5kW | 5kW | 7.5kW | Units |
|---|---|---|---|---|
| AC voltage | 220 | 220 | 220 | Vrms ±10%, 3φ |
| Current, max | 15 | 25 | 30 | Arms |
| Internal breaker | 20 | 30 | 30 | Arms |
| Real power (input) | 4.2 | 6 | 9 | kW max |
| Apparent power | 4.7 | 6.7 | 10 | kVA max |
| Power factor | .95 | .95 | .95 | — |

- **RF output frequency**: 150–400 kHz (regulated RF current)
- **Line-to-coil efficiency**: >90%
- Frequency (50-60Hz) is the mains AC line frequency, not the RF output — RF output frequency is the 150-400kHz row above

**Resolved (2026-09-05)** — both prior open items (previously tracked in a since-removed Ambrell outreach draft, superseded once the full manual was on hand) are now answered by manual §4.2:
- E-stop is confirmed as a control-circuit interlock (24V rail across CTB1:15-16, rated 3Adc min, N.C., opens = STOP) — the manual still never states it disconnects mains/chassis power, so for compliance purposes this remains a control-only interlock unless Ambrell confirms otherwise.
- 0-10V remote input impedance is confirmed: **Zin = 21kΩ** (4-20mA alternative: Zin = 250Ω) — output impedance of the driving NI AO module (now NI-9269, not 9263) is far below 21kΩ, so no loading concern. **Scaling is not linear across the full 0-10V range** — bench-measured 2026-09-06 (see [[Design/Wiring/NI-DAQ Control Architecture|NI-DAQ Control Architecture]]): ~1.10V turn-on threshold, `Amps ≈ 137×V − 151` linear region up to ~5.1V, then a hard 550A ceiling from ~5.1V to 10V (likely the Icap/tap-cap safe limit). Worth a courtesy follow-up to Ambrell only if this ceiling needs explaining/certifying — otherwise treat the measured curve as authoritative.

### Control Systems (24V DC)

#### Ball Screw Motor Control
**See**: [[Design/Mechanisms/Ball Screw Motor Control|Complete Ball Screw Motor Control documentation]]

- **Power Supply**: SolaHD SDN 10-24-100P (240W, 24V @ 10A)
- **Motion Controller**: ST-PMC1 (SN: 170120011) — programmable pulse+direction sequencer
- **Stepper Driver**: TB6600 (SN: 170120011) — coil amplifier (5–10A per phase)
- **Motor**: NEMA 23 stepper with integrated ball screw (SN: 161104226) — ±0.025mm positioning
- **Homing**: Limit switch on Input #1 for automatic home finding
- **Capabilities**: Up to 99 programmed motion sequences, 40 kHz max frequency

#### Instrumentation & Monitoring
- **Temperature Measurement**: Spot-welded thermocouples on sample + feedthrough
- **Pressure Monitoring**: Chamber vacuum gauge (via air control assembly)
- **Motor Feedback**: Limit switch input to ST-PMC1 for homing detection
- **Data Logging**: External equipment integration (TBD)
- **Emergency Stop**: E-stop on power supply, wired N.C. across HOTSHOT CTB1:15-16 (rated 24V @ 3A min per manual — see CTB1 pinout above); manual vacuum vent available

## Key Design Factors

- **Power Supply Specs** — Frequency, impedance matching, transient behavior
- **Coil Connection** — Series vs. parallel, load matching, efficiency
- **Shielding & EMI** — Cable routing, ferrite filters, grounding strategy
- **Thermocouple Circuits** — Amplification, noise filtering, cold-junction compensation
- **Safety Systems** — E-stop wiring, fuses, thermal cutouts, interlock circuits
- **Data Acquisition** — Sampling rate, resolution, isolation from power circuits

## Integration Points

- [[Design/Coil Geometry/Induction Coil|Coil Geometry]] — Coil load impedance and connections
- Mechanisms & Automation — Control signals and feedback loops
- [[Design/Mechanisms/Ball Screw Motor Control|Ball Screw Motor Control]] — 24V stepper power, motion controller, homing signals
- [[Design/Plumbing/Fluid Systems|Plumbing & Fluid Systems]] — Pump/valve control circuits
- [[Design/Wiring/NI-DAQ Control Architecture|NI-DAQ Control Architecture]] — Detailed automated control system implementation
- [[Design/Archive/Design History|Design Archive]] — Previous power supply configurations
