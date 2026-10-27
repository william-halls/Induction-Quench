---
subsystem: coil_geometry
tags: [findings, power-supply, resonance, hotshot, 3kw-supply, coil-geometry]
---

# Power Supply Matching — Findings (as of 2026-10-23)

**Facts only.** Everything here was either measured/read by the user, printed in a manual, or seen on a label. Inductance/frequency/temperature *predictions* made in chat are deliberately **not** recorded here (they are estimates, not facts) — see "Open items" at the bottom.

## Decision (2026-10-23)

The Ameritherm HOTSHOT 5 with the 8-loop coil was operating below its rated 150 kHz lower limit and delivering only ~20 A against a 550–600 A setpoint (section 1). **Reasoning for switching to the 3 kW supply for now.** Changing the coil geometry (fewer turns) remains an option; the 3 kW supply is the primary path because its series inductor and capacitor bank can be adjusted (section 4).

---

## 1. HOTSHOT 5 (SN 103927) + 8-loop coil — observations

All of these were observed with the 8-loop coil (section 3).

| Tap setting | Observation |
|---|---|
| 22 (setting when testing began) | While trying to heat: **143–170 kHz**; while heating: **18.7–20.9 A at ~146 kHz**; W reading **0–5 W** with the setpoint at **550–600 A**; frequency shown with the unit idle and the coil connected: **142 kHz** |
| 26 | Highest tap that did **not** trigger Cap Over-Voltage; frequency **142–162 kHz**; still under-heating at the maximum setpoint |
| 38 | Cap Over-Voltage |

- Messages the unit gave: **"Increase Tap"** and **"lower cap value"**.
- Raising the tap from 22 to 26 barely changed the frequency range (143–170 → 142–162 kHz).
- Setpoint note: the vault's 2026-09-06 bench calibration ([[Design/Wiring/NI-DAQ Control Architecture|NI-DAQ Control Architecture]]) shows the remote setpoint saturating at 550 A.
- **Heating result at a 600 A setpoint, ~147 kHz:** heating rate about **1.5 °C/s**, plateau at about **350 °C**. Per the user, treat this as the supply running outside its operating range. Checked against the documents: the frequency (142–170 kHz) is below the HOTSHOT's **150–400 kHz** specification (manual §4.2), and the delivered current (~20 A) and power reading (0–5 W) were far below the setpoint. "Out of range" is therefore a **frequency-range** statement, not a power-capacity one — the supply has far more power capacity than it delivered. The cause (the tank not reaching resonance) is a hypothesis, not a measurement.
- **Not reconciled:** the 0–5 W reading and ~20 A do not obviously agree with a 1.5 °C/s heating rate of a steel bar. Sample material/size was not recorded for this run.

## 2. HOTSHOT 5 — configuration and documentation

- Startup screen: **"HOTSHOT 5, 5 kW RF power"**. (The vault previously said 7.5 kW; corrected 2026-10-23 in [[Design/Wiring/Electrical System|Electrical System]].)
- Manual (Doc 801-9252k, [[media/wiring/Ameritherm HOTSHOT 103927 Manual (Doc 801-9252k).pdf]]): RF output **150–400 kHz** (§4.2). Heat Station menu changes to Tap/Cap only *inform* the unit — "this does not change capacitance" (p. 23).
- Heat Station → Modify Cap shows: model **Custom**, **400 kVAR**, adjustable in **steps of 25**. Other models listed: 150, 300P, 300S, 400, 800P, 800S, 1000, 1000P, 1000S. The manual's example screen shows C1 = 0.10 µF, tap 18 (a sample, not this unit's value).
- Capacitor rating read as **1 µF** (user-reported).
- A large capacitor label read: "type UL30, 260.0 ±10%, 600 V DC, 0545" (what it is within the unit was not determined).
- Manual §3.4 tap-change steps (p. 45): STOP, AC power off at the rear (AC LED off), remove the work-head cover, read the present tap from the table, move the wires per the table for the new tap, tighten, replace the cover, then Menu → Heat Station → Modify Tap to enter the new tap and return to Home. The tap-to-wire table was too blurry to read in our copy; the example given is TAP 43.

## 3. Coil and leads

**8-loop coil (Onshape "Induction Coil" feature):** ![[coil-8loop-cad-parameters.png]]

| Parameter | Value |
|---|---|
| Number of Turns | 7 (7.5 revolutions → 8 visible loops) |
| Coil Inner Diameter | 25.4 mm |
| Coil Length | 65 mm — measured from the very top of the coil to the very bottom (highest to lowest point vertically) |
| Tube OD | 4.76 mm |
| Lead Length | 53 mm |
| Bend Radius | 5 mm |
| Outer Sleeve | on, 1.5 mm thick (fibreglass), sleeve lead length 45 mm |

![[coil-8loop-cad-side-view.png]]

**Test coil (separate):** ID 1.5 in, 3 turns, 0.75 in long.

**Real supply leads:** 3/8" OD copper tube, **1.013 m for both leads together**, plus about **5 in** more in the real build. One leg of the long top run is **bowed** (see sketch below, red = straight leg, green = bowed leg). The leads were cleaned and the loop-plus-interface resistance measured **under 0.2–0.3 Ω** with a handheld meter. The setup measured that way had **three 90° bends** (at the coil, out of the lid, and to the meter). The **3 kW setup will have two 90° bends** (at the coil and out of the lid).

![[coil-lead-top-run-bow-sketch.png]] ![[coil-8loop-lead-run-cad.png]]

## 4. 3 kW supply — Model 3-135/400-3 (Rev E, 12/1/04)

Manual: [[media/wiring/3kW Induction Supply 3-135-400-3 Manual (Rev E).pdf]]

| Item | Value |
|---|---|
| Output power | 3 kW (6.6 kVA at 230 V input, power factor 0.50), 100% duty |
| Frequency | **135–400 kHz** |
| Max output voltage | 350 V rms (limited by the resonant capacitors' rating) |
| Inverter current limit | 70 A peak (short-circuit protected). The load coil current can be much higher |
| Input | 208–240 V ±10%, 50–60 Hz, single phase, 14.5 A at 230 V, 3.6 kVA; 20 A breaker |
| Size / weight | 20.5 × 11.7 × 12.7 in, 43.5 lb; air cooled (three fans), heatsink temperature switch |
| Heat station | **Two 660 nF, 350 V capacitors**, adjustable for load matching; **series inductor** with turn selector bar |
| Load | Operates with any load Q provided output frequency and voltage stay in spec |
| Water (coil) | Max 100 psi, min 30 psi differential, min 0.5 GPM, max inlet 105 °F |
| Feedback | Default input-power feedback (jumper on control PCB); output voltage, auxiliary and inverter current selectable |
| Interfaces | Remote power reference 0–10 V (Terminal Block 1, INT/EXT switch SW301); RS-485 port; Terminal Block 2: start/stop, E-stop, aux interlock, reset, and Heat On / Fault / Ready relays (250 V, 3 A) |

**Limit lights (yellow):** POWER (limited at 3 kW), VOLT (capacitor voltage limited at 350 V rms), FREQ (limited at the tank's resonant frequency), CURRENT (inverter current limited at 70 A peak). **Trip lights (red):** FREQ, CURRENT (both from a short or poor tuning), DOOR, TEMP (heatsink over 85 °C), FLOW (not used), AUX.

**Start frequency:** the supply starts above 400 kHz and lowers the frequency toward resonance. If the resonance is below 200 kHz it can take over a second to reach power. Starting the supply below the tank's operating frequency causes a frequency trip and may damage the unit.

**Tuning (§6.1, breaker off before changing anything):** aim for full 3 kW with no limit lights. If it sits near ~135 kHz without full power and no limit lit, the tank frequency is outside the unit's range. If the CURRENT limit/trip lights, add series-inductor turns; if the FREQ limit/trip lights, remove turns. Raising the series inductance until the current reads about 50% at full power optimizes efficiency.

**Series inductor (§6.2.1):** turn selector bar — near position = 0 turns, far position = 11 turns, tightened with mounting screws; a variable shorting bar (high = 1, low = 3) gives finer steps. (Figure 4 of the manual is a vector drawing that could not be extracted.)

**First run on the 3 kW supply (2026-10-26):** the CURRENT limit light came on; after adjustment, the inverter current at **max power read 66 A** (the limit is 70 A peak; the manual recommends tuning for about 50% of the scale at full power). Capacitor configuration and series-inductor position for that run were not recorded.

**Remote power reference (user-measured):** 0.5 V → 4 %, 1.0 V → 10 %, 1.5 V → 15 %, 2.0 V → 20 % — power % ≈ 10 × V. Details and the PID/LabVIEW implication are in [[Design/Wiring/NI-DAQ Control Architecture|NI-DAQ Control Architecture]] ("3 kW supply setpoint calibration").

**Appendix B tuning examples (as printed):**

| Load | Caps (µF) | Taps | Coil (turns × ID) | Freq (kHz) | Power (kW) | Voltage (Vrms) | Inverter current (A) | Notes |
|---|---|---|---|---|---|---|---|---|
| 1" steel pipe | 1.320 | 8 | 4 × 2.5" | 165 | 3 | 180 | 74 | |
| 1" steel pipe | 1.320 | 7 | 2 × 2.5" | 223 | 3 | 160 | 54 | |
| 1" steel pipe | 1.320 | 5 | 1 × 2.5" | 349 | 2.9 | 183 | 70 | |
| Open | 1.320 | 8 | 5 × 3/4" | 308 | 3 | 230 | 44 | |
| 1" steel pipe | 1.320 | 3 | 3 × 1.5" | 324 | 3 | 90 | 70 | |
| 1" steel pipe | 0.660 | 5 | 3 × 1.5" | 392 | 1.2 | 70 | 24 | Frequency limit |
| 1" steel pipe | 0.210 | 10 | 8 × 4" | 256 | 2.3 | 500 | 65 | Capacitor limit |
| 1/2" stud | 1.320 | 5 | 3 × 3/4" | 300 | 3 | 230 | 70 | |

(The "Taps" column presumably refers to the series-inductor turn selector — an inference; the manual does not define the column.)

**Observed on the actual unit:** two capacitors screwed in parallel on a copper rail, with an open spot for a third. Each reads **"CHG6066E, 1020 — High Energy Corp"**. A web search found no datasheet for CHG6066E. Other High Energy Corp families use "…6066" for 0.66 µF parts (CHL6066, CHM6066M, CHN6066M — rated about 600 V, 225–300 A rms), and its CHG family is listed at 0.1–1.32 µF up to 700 Vrms. These match the manual's 660 nF but are **other series**, so the CHG6066E's exact rating is unconfirmed.

---

## Open items (not yet verified)

- HOTSHOT nameplate vs the "5 kW" startup screen (nameplate photo still outstanding — see [[Design/Wiring/TODO - HOTSHOT Docs & E-Stop-Wiring|TODO]]).
- Actual HOTSHOT tank capacitance (1 µF reported; Custom 400 kVAR setting).
- CHG6066E capacitance and current rating.
- Which coil the 600 A / 1.5 °C/s / 350 °C test used, and the sample material and size for it.
- Sample material/size and target heating rate for the instrument.
- How to physically change the capacitor bank on the 3 kW unit (§6.2 covers only the series inductor), and whether a 230 V single-phase 20 A outlet is available.
- Whether a lower-inductance coil (fewer turns, tighter leads) would bring the HOTSHOT into range — this is an estimate from chat, not measured.
