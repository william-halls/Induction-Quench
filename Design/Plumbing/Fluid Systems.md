---
subsystem: plumbing
tags: [design, plumbing, cooling, quenching-systems, automation]
---

# Fluid Systems

**Quench delivery (decided):** Water quench via ball-screw immersion — the sample, still clamped in its [[Design/Mechanisms/Ceramic Mount|ceramic mount]], is lowered directly into a pre-filled water bath. There is no spray/release mechanism for the quench itself. See [[Design/Sample Quenching/Quenching Methods|Quenching Methods]] for the full decision and rationale.

**Decision Pending:** Whether the water bath stays in the vacuum chamber at all times, or is added after (via a 24V mini diaphragm pump) — this is a bath-fill-timing question, separate from the quench-delivery method above.

**Concern:** Water boiling off prevents reaching desired vacuum for argon backfill.

**Opportunity:** The same 24V diaphragm pump's inlet could be modified into a custom nozzle for an alternative spray-quench approach (see "Spray Quenching" in [[Design/Sample Quenching/Quenching Methods|Quenching Methods]]) — helping overcome the Leidenfrost effect and potentially reducing quenching time. This is an opportunistic alternative, not the current decided method, since it would require sample release rather than the current always-clamped ball-screw immersion.

## Automatic Water Control

**Automation** (timing-based fill coded as of 2026-10-28):
- See [[Design/Wiring/NI-DAQ Control Architecture|NI-DAQ Control Architecture]] for integration — pump/valve actuation is a USB-6009 digital SSR line (current cDAQ-9174 architecture; the original NI-9263 analog Channel 3 plan was superseded once pump control moved to purely digital SSR triggering)
- Water filling controlled via solenoid valve or diaphragm pump on/off, via SSR
- Trigger: **timing-based** — pump run time = SPI (seconds per inch of bath depth) × target depth. See calibration below.

### Pump Fill Timing Calibration (2026-10-28)

**Calibrated value: SPI ≈ 49.9 sec/in** (49.86, curve fit through all three tuning runs, solved for zero error).

Tuning runs (ratio = actual fill ÷ target fill; error = 1 − ratio):

| Run | SPI (sec/in) | Ratio | Error |
|---|---|---|---|
| 1 (first guess) | 20.0000 | 0.4667 | 0.5333 |
| 2 | 42.8571 | 0.920 | 0.080 |
| 3 | 46.5839 | 0.966 | 0.034 |
| Calibrated | **49.86** | ≈1.0 | ≈0 |

**Method:** fit a quadratic (curve) through all three (SPI, ratio) points and solved for ratio = 1 → SPI = 49.86. For comparison, a straight-line fit through only runs 2 and 3 gives 49.34. The curve fit was chosen because it uses all the data and captures the flattening response.

**Why the fill isn't proportional (fixed delay):** each "new SPI = old SPI ÷ ratio" correction covered less of the remaining error (0.53 → 0.08 → 0.034). If fill were directly proportional to run time, that correction would converge in one step; it would have predicted only 48.2 after run 3. The undershoot points to a fixed delay at the start of each fill — pump spin-up and filling the line before water reaches the bath — so the first few seconds of run time add no depth. Simple proportional scaling doesn't model that delay, so use the curve fit instead.

**Recalibrate if** the tubing length/routing, pump, supply voltage, or water source changes: run 2–3 fills near the expected SPI, record the ratio for each, and fit again for zero error.