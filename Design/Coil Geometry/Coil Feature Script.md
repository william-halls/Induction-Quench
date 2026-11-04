---
subsystem: coil_geometry
tags: [coil-design, cad, parametric, feature-script, automation, inductance, efficiency]
---

# Coil Feature Script

Parametric OnShape FeatureScript for rapid generation of induction coil geometries. Supports round and rectangular tube profiles, an optional outer sleeve, an optional dummy sample or crucible, and a set of estimated electrical and cooling values.

> **Status (2026-10-31):** the source below is the latest version (FeatureScript 3044). It **builds in OnShape**: the ASCII-label dialog loads, including the new "Uniformity" option, and the coil, dummy sample and Efficiency message all generate (screenshot below). The **Uniformity output itself hasn't been looked at in OnShape yet**. Its math was checked offline in C#: the elliptic integrals match reference values and the on-axis loop formula.
>
> ![[coil-featurescript-onshape-build.png]]
> *5-turn coil, 53.34 mm ID, 7.938 mm (5/16") round tube, 2 mm gap, cylinder dummy sample. Message: "Efficiency 93.8% @ 293.7 kHz · 5 kW in → 4.69 kW to work, 0.31 kW copper loss (water +2.2 °C) · Q 1.8".*

## Quick Access

🔗 **OnShape Document**: https://cad.onshape.com/documents/f8c5aa176fe5d3504340f7c7/v/e52646392fe2bef23f6bfba3/e/91393e0f3187f0dfdba36f16

## Features

✓ **Tube profile** — Round (OD) or rectangular (axial L x radial W) toggle  
✓ **Spacing mode** — Coil Length (default) or Gap Between Winds (checkbox)  
✓ **Orientation** — Automatic centering, reverse winding, rotation angle  
✓ **Smart leads** — Bezier-smooth inlet/outlet bends  
✓ **Leads together** — Optional: leads run out until clear of the coil, then an S-bend brings them into a closely spaced parallel pair (side by side at mid-height, or stacked); lead inductance and copper loss follow the modeled path  
✓ **Outer sleeve** — Optional, with independent sleeve lead length  
✓ **Named / colored bodies** — Coil `#B87333`, Sleeve `#E6E6E6`, Sample/Charge `#A8A9AD`, Crucible graphite `#3A3A3A`  
✓ **Optional tube bore** — Hollow the coil (round ID or rectangular wall thickness)  
✓ **Dummy sample** — Cylinder, prism, existing part, or crucible with charge  
✓ **Estimated values** — Inductance, leads, tank frequency, efficiency, cooling  
✓ **Heating uniformity** — Axial heating profile along the sample's outer (side) surface, not the top/bottom end faces: uniform zone, target-zone min/max, end drop-off, turn ripple  
✓ **Thermal estimate** — Max reachable temperature, heat-up time, and soak time for the core to be within a set % of a target temperature  

## Dialog Layout

| Group | Contents |
| ----- | -------- |
| Placement | Center target (point/line), reverse winding, rotation angle |
| Coil | Coil ID, number of turns, "Specify gap between winds" checkbox → Coil length (default) or Gap |
| Tube Profile | Round / Rectangular toggle, OD or L and W |
| Leads | Lead length, bend radius; "Bring leads together" checkbox → arrangement (Side by side / Stacked / Own heights), "Continue around coil, then straight out", lead spacing (center to center, default 13 mm; error if less than tube + sleeves), optional "Set join distance from coil OD" (gap from the coil OD to the outside of the paired leads; the leads always hug the coil with no straight sections; unset = tightest join that fits, set = the leads curve together at this distance; the join is pushed out automatically if the leads need more room to climb to the pair height) |
| Outer sleeve | Checkbox → sleeve thickness, sleeve lead length |
| Estimated values | Checkbox → "Show in message" (Inductance / Tank / Efficiency / Cooling / Uniformity / Thermal / All) |
| Dummy sample | Checkbox → type, dimensions, material (see below; Custom also asks for thermal conductivity, density, specific heat) |
| Heating uniformity (side surface) | Collapsed group (shown with a dummy sample): heating band (+/- %, default 10), target zone length (default 10 mm, centered on the coil) |
| Thermal estimate | Checkbox (shown with a dummy sample) → group: target temperature, start / ambient temperature, core within (+/- % of target C), surface emissivity, side insulation thickness and conductivity, insulate end faces too, extra heat draw at target |
| Cooling water | Tube ID or wall thickness (round, checkbox "Specify wall thickness instead of ID", default 0.8128 mm = 0.032 in) or wall thickness (rectangular), "Model tube bore", flow rate |
| Tank circuit & power supply | Capacitance (uF); "Extra leads to capacitor" checkbox (on by default) → extra lead length and spacing; supply power (kW); supply min / max frequency (kHz, default 150 / 400 = HOTSHOT); supply max coil current (A, default 550, 0 = no check) |

## Key Parameters

| Parameter | Range | Purpose |
| --------- | ----- | ------- |
| Coil Inner Diameter | 0.1-2000 mm | Center hole diameter |
| Number of Turns | 0.01-500 | Winding count (helix builds turns + 0.5) |
| Coil Length | 0.1-2000 mm | Overall axial extent (default mode) |
| Gap Between Winds | 0-500 mm | Spacing, used when the gap checkbox is on |
| Tube OD / L / W | 0.1-200 mm | Tube size |
| Lead Length | 0-2000 mm | Supply line length |
| Bend Radius | 0.1-500 mm | Smooth entry/exit curves |
| Sleeve | Optional | Water-jacket visualization |

### Spacing modes
- **Length mode (default):** `pitch = (Coil Length - tube axial size) / (turns + 0.5)`. Errors with the minimum length if the turns would overlap.
- **Gap mode:** `pitch = tube axial size + gap`.

## Dummy Sample and Materials

- **Sample types:** Cylinder, Prism, Part (pick an existing body), Crucible.
- **Material presets:** Steel cold (magnetic, ur ~100), Steel hot (above Curie), Stainless 304 (room temperature, 72 uOhm-cm), Stainless 304 - hot (~1000 C, 117 uOhm-cm), Copper, Aluminum, Brass, Titanium, Graphite, Custom (resistivity in uOhm-cm and relative permeability).
- **Materials come from the script's own table** (the "Sample material" / "Charge material" dropdown), for every sample type including Part. OnShape part materials can't be used: materials set in the Properties dialog are applied after the Part Studio's features run, so a feature can't see them, and even when readable only the name and density come back (researched 2026-11-01: [forum](https://forum.onshape.com/discussion/2348/read-and-write-part-properties-or-document-properties), [forum](https://forum.onshape.com/discussion/31486/featurescript-access-to-complete-material-properties-and-the-onshape-material-library)). The Efficiency and Thermal messages name the material used, e.g. "material Stainless 304 - hot". For a stainless sample at 1000 °C pick **Stainless 304 - hot**.
- **Crucible:** outer diameter, wall, height, bottom thickness; crucible material Graphite / Ceramic (non-conductive) / Custom; separate charge material (default steel) and fill height. Models a hollow body named "Crucible" and a body named "Charge".
- Prism and Part samples are treated as an equivalent round cylinder for the electrical model.

## Estimated Values (what the script computes)

| Value | Method |
| ----- | ------ |
| Coil inductance (empty) | Single-layer coil formula (Nagaoka via Lundin approximation) plus Rosa's conductor-shape correction. **HF** (surface current, the right number for induction) is fitted to a numerical perfect-conductor solution of the current on the round turns: sheet radius ID/2 + 0.19·d, conductor GMD 0.38·d (d = tube OD; equal-perimeter round size for rectangular tube). Within ~4 % for ID/d 3.5–13, pitch/d 1.05–3, 2–14 turns; 0.5 % on the 65.5 and 78.2 mm coils. **LF** uses the tube centerline radius and the solid-conductor GMD |
| Lead inductance | Finite-length (partial-inductance) model along the modeled lead paths: L = L_self(A) + L_self(B) − 2·M_AB, with each path segment treated as a filament parallel to the lead direction (exact finite-filament mutual inductance) and Rosa's HF self-inductance for each lead's path length. The spacing is corrected for current crowding between close tubes, so long pairs match the two-wire formula; paths start at the helix ends. Extra leads to the capacitor use the same model as a straight pair. In wrap mode the curved part around the coil uses the two-wire formula at its mean spacing |
| Tank frequency | Parallel tank: `f = (1/2pi) * sqrt(1/(L*C) - (R/L)^2)` with HF coil inductance plus leads and the series resistance R (copper + load); iterated with the load's effect on L and R to give a loaded frequency. Reports "no resonance" when Q < ~1. Checked against the supply frequency window |
| Coil current, tank voltage | `I = sqrt(P / (R_load + R_copper))` at the supply power; `V = I * sqrt(R² + (w(L_coil + L_leads))²)` (across the capacitor). Coil current checked against the supply max current |
| Sample / crucible loading | Solid cylinder in an AC axial field (Kelvin/Bessel solution, series expansion, thick-body limit above r/delta = 30); crucible wall modeled as a conductive slab layer in front of the charge, giving the charge vs crucible power split |
| Load coupling | How strongly each load segment loads the coil: `G = mu0·pi·Ro²·∫H(z)² dz` over the whole segment (also the parts outside the coil), with H the coil's axial field per amp at the load surface. Field from one loop per revolution at the effective current radius ID/2 + 0.35·d (within ~2 % of the numerical current solution). Load resistance = −w·G·Im(F), inductance change = G·(Re(F) − 1) |
| Efficiency | Load resistance / (load resistance + copper resistance). Electrical only: inverter, capacitor and radiated losses are not included |
| Power split, water rise | From the supply power entered; water temperature rise from copper loss only |
| Cooling pressure drop | Darcy-Weisbach, water at 20 C: straight sections laminar 64/Re or Haaland (drawn copper); helix via curved-pipe correlations (Mishra and Gupta, Schmidt critical Re). Also reports velocity, Reynolds number, regime, and total tube length. Flags water speed above 3 m/s |
| Heating uniformity | Each helix revolution is treated as a circular loop at its mean height (last half turn = half-weight loop) at the effective current radius (same field model as the load coupling), split into 3 filaments across the tube's inner face. The axial field at the sample's outer cylindrical surface (radius = sample radius, i.e. the side of the rod, not the top face) comes from the off-axis loop formula (elliptic integrals K, E by AGM). Heating per unit length ∝ H², sampled at about 8 points per pitch along the sample (charge zone for a crucible). See the table below for the reported numbers |

### Heating uniformity outputs

| Output | Meaning |
| ------ | ------- |
| Uniform zone (`#heatUniformZone_mm`) | Length around the coil center where the pitch-averaged heating stays within ± band % of the center value. Says "whole sample" if the band covers the full length |
| Target zone min/max (`#heatZoneMinMax_pct`) | Weakest ÷ strongest pitch-averaged heating inside the centered target zone (e.g. the Charpy notch region). 100% = perfectly even |
| Ends (`#heatEndRatio_pct`) | Heating at the colder end of the sample ÷ heating at the center. Low values mean cold ends, e.g. when the sample sticks out past the coil |
| Turn ripple (`#heatRipple_pct`) | (max - min) ÷ max of the local (unaveraged) heating within one pitch of the center: banding under each turn vs between turns |

Offline check of the algorithm, using a 10×10×55 mm Charpy sample (equivalent radius 5.64 mm) in a 25.4 mm ID coil made from 4.76 mm round tube, with a ±10% band:

| Coil | Uniform zone | 10 mm zone min/max | Ends vs center | Turn ripple |
| ---- | ------------ | ------------------ | -------------- | ----------- |
| 7 turns, 65 mm long (8-loop coil) | 29 mm | 99% | 55% | 2.4% |
| 4 turns, 40 mm long | 13.5 mm | 96% | 5% | 3.4% |
| 10 turns, 100 mm long | whole 55 mm | 92% (full 55 mm zone) | 92% | 4.3% |

With the current 8-loop coil, only about the middle 29 mm of a 55 mm Charpy sample gets even heating. The ends get about half the center's heating. Most of that difference is smoothed out by heat conduction along the bar, but expect the ends to lag.

All results are also saved as part studio variables: `#coilInductanceLF_uH`, `#coilInductanceHF_uH`, `#coilInductanceLoaded_uH`, `#leadInductance_uH`, `#tankFreqEmpty_kHz`, `#tankFreqLoaded_kHz`, `#efficiency_pct`, `#efficiencyCharge_pct`, `#workPower_kW`, `#copperLoss_kW`, `#waterTempRise_C`, `#coilQ`, `#coilCurrent_A`, `#tankVoltage_V`, `#coolantDeltaP_kPa`, `#coolantVelocity_mps`, `#tubeLength_mm`, `#heatUniformZone_mm`, `#heatZoneMinMax_pct`, `#heatEndRatio_pct`, `#heatRipple_pct`, `#thermMaxTemp_C`, `#thermHeatup_s`, `#thermSoak_s`, `#thermSoakFromCold_s`, `#thermLossAtTarget_W`, `#thermFullPowerLag_C`, `#thermHoldSupply_kW` (the heat variables are 0 without a dummy sample).


### Thermal estimate (added 2026-11-01, builds in OnShape)

Tick **Thermal estimate** (below the Dummy sample group), then pick **Thermal** in "Show in message". Inputs: target temperature (C), start/ambient temperature (C), uniformity tolerance (% of the target in °C, e.g. 2 % of 1000 °C = ±20 °C), surface emissivity, side insulation thickness (0 = bare) and conductivity, "Insulate end faces too", and an extra heat draw at the target (W), e.g. the Cu probe. Custom materials also get thermal conductivity, density and specific heat inputs.

| Output | Method |
| ------ | ------ |
| **Max temperature** (`#thermMaxTemp_C`) | Steady state where losses = power into the load (`#workPower_kW`), found by bisection. Losses: bare sides radiate (εσA(T⁴−Ta⁴)); insulated sides conduct through a cylindrical shell 2πkL(T−Ta)/ln((R+t)/R) with the outer face at ambient; both end faces radiate (or conduct through the insulation if ticked); plus the extra draw scaled with (T−Ta) |
| **Heat-up time** (`#thermHeatup_s`) | ∫ m·cp dT / (P − loss(T)) from start to target, whole body at one temperature (lumped). −1 if the target can't be reached |
| **Loss at target** (`#thermLossAtTarget_W`) | Same loss model at the target temperature |
| **Soak time** (`#thermSoak_s`) | **After a full-power heat-up** (how the rig is run): when the surface reaches the target the core trails by the net-power lag, (P − loss at target) ÷ side area × R / 2k, in a roughly parabolic profile. Its centre decays as 1.108 · lag · e^(−λ₁²Fo), so t = τ · ln(1.108 · lag ÷ tolerance), τ = R² / (α λ₁²), λ₁ = 2.4048. 0 if the lag is already inside the tolerance |
| **Soak from cold** (`#thermSoakFromCold_s`) | Worst case: uniform cold body with its surface suddenly held at the target. t = τ · ln(1.602 ÷ f), f = tolerance ÷ (target − start) |
| **Core lag at full power** (`#thermFullPowerLag_C`) | Quasi-steady centre-to-surface difference while heating at full power: q″·R / (2k), q″ = power ÷ side area |
| **Hold power** (`#thermHoldSupply_kW`) | Supply power needed to hold the target: loss at target ÷ electrical efficiency. Also shown as % of the supply power. A starting point for the PID output |
| **Melting warning** | Shown when the max temperature reaches the material's approximate solidus (304: 1400 °C, steel 1420, Cu 1085, Al 660, brass 900, Ti 1668, graphite 3600; none for Custom) |

Thermal properties are fixed averages over roughly 20–1000 °C (k W/m·K, ρ kg/m³, cp J/kg·K): Steel cold 45/7850/490, Steel hot 27/7650/650, Stainless 304 22/7900/560, Copper 380/8900/400, Aluminum 220/2700/950, Brass 120/8500/390, Titanium 18/4500/580, Graphite 80/1800/1300. A crucible is treated as one body with the charge's properties. A Part sample uses the equivalent cylinder (same volume and length).

**Limits:** power into the load comes from the electrical model at the chosen material's resistivity (use "Stainless 304 - hot" for a 1000 °C stainless sample); properties don't vary with temperature; soak is radial only (no axial gradient, no end-face losses during the soak) and uses the one-term series, so it is least accurate for very loose tolerances (> ~50 % of the rise); the insulation's outer face is assumed at ambient, which overstates its loss slightly (max temperature is conservative).

> **Stale (2026-11-02):** the table below used the old load coupling (59.9 % efficiency). With the corrected coupling the 78.2 mm coil comes out around **44 %**, so about 2.2 kW reaches the rod instead of 3.0 kW: heat-up gets longer, max temperatures drop and hold power rises. Re-run in OnShape before relying on these numbers.

Offline check (C-equivalent math, 2026-11-01) for this rig: 304 rod 50.8 mm × 76.2 mm, 3.0 kW into the rod (5 kW supply), ε 0.8, target 1000 °C, ±2 %:

| Case | Max temp | Loss at 1000 °C | Heat-up | Soak from cold (core within ±20 °C; after-heat-up soak is ~40–45 s) | Core lag at full power | Hold (at 59.9 % eff., % of 5 kW supply) |
|---|---|---|---|---|---|---|
| 12.7 mm Kaowool (k 0.15) on sides, ends bare | ~1690 °C | 0.66 kW | ~4 min | ~98 s | ~140 °C | ~1.1 kW (22 %) |
| Same + 500 W probe draw | ~1530 °C | 1.16 kW | ~4.5 min | ~98 s | ~140 °C | ~1.9 kW (39 %) |
| Bare | ~1150 °C | 1.93 kW | ~4.8 min | ~98 s | ~140 °C | ~3.2 kW (64 %) |

Max temperatures above ~1400 °C mean the supply can melt 304: the PID and an over-temperature cutoff have to limit it.
### Assumptions and limits
- **HF is the number to trust** for induction heating; LF is mostly an upper bound.
- **Cold steel ur = 100** is a rough effective value and falls toward saturation; use "Steel - hot" above the Curie point (~770 C) or Custom.
- Resistivities are **room-temperature** values except "Steel - hot". Hot metal has higher resistivity, which usually raises efficiency.
- Coil copper loss assumes current across the full tube width on the inner face, which is **optimistic for round tube**; real efficiency is somewhat lower.
- The crucible wall uses a flat-wall approximation, best when the wall is thin compared with the crucible radius.
- Cooling pressure drop **excludes hoses, fittings, and manifolds**; add margin on top for real supply pressure.
- Rectangular GMD (0.2235 x (W+L)) is exact for a square and a few percent off for very flat tube; effect on total is well under 1%.
- Leads and workpiece effects are only included when the matching options are enabled.
- **Load coupling uses the empty-coil field** at each slice of the load. The load's own reaction (it pushes field out toward its ends and changes the current distribution in the turns) is not modeled, so real coupling is a little lower again, mostly for loads that nearly fill the coil.
- The HF inductance and field-radius fits were made for round tube; rectangular tube uses an equal-perimeter round size (approximate).
- **Uniformity is heat generated, not temperature.** Heat conduction along the sample and radiation from its ends are not modeled. Temperature is always more even than the heating profile, but the hot zone shape still follows it.
- **Uniformity is measured on the outside of the rod (the side surface), not the top surface.** Every number in the uniformity outputs is a position along the rod's length on that outer surface. The top and bottom end faces aren't evaluated at all.
- Uniformity uses the **empty-coil field**. Field crowding at the sample's ends and the extra heating at its end faces and corners are not included, and neither are the leads, the helix's uneven start and finish, or variation around the coil.
- Material properties are assumed constant along the sample. In reality, sections that pass the Curie point heat differently from colder sections.
- **Turn ripple is overstated when the sample nearly touches the coil** (gap smaller than about the tube size), because the current is modeled as thin filaments. Example: a 24 mm sample in a 25.4 mm ID coil reads about 89% ripple.

## Usage

1. Open the OnShape document
2. Set desired parameters in the FeatureScript feature panel
3. Geometry auto-generates with smooth Bezier transition curves
4. Tick "Estimated values" and (optionally) "Dummy sample" to see inductance, tank frequency, efficiency and cooling estimates in the feature message and the variable table
5. Export STL for 3D printing molds or STEP for manufacturing

## Implementation Highlights

- **Centered Helix** — Auto-calculates total height and centers coil vertically
- **Bezier Transitions** — Smooth inlet/outlet curves with configurable bend radius
- **Dual-Lead Support** — Separate lead lengths for main coil and optional sleeve
- **Cleanup Routine** — Removes construction geometry for clean final model
- **Names and colors are set last** — OnShape resets body appearance if operations run after it, so the Coil/Sample naming and coloring happens at the very end of the feature

## Gotchas Learned

- Enum names `ProfileType` etc. clash with OnShape's own library; the enums here are prefixed `Coil...`.
- Parameter and dropdown **name labels must be plain ASCII** (no µ, Ω, en-dashes, middle dots). The info message text can use them.
- `setProperty(context, {...})` takes **two arguments** (no id).

## Changelog

- **2026-11-02 (insulated max temperature too high)** — The max-temperature balance lands far above the target (~1600–2000 °C), where the insulated-side model was too optimistic:
  1. **Insulation k was constant.** Ceramic fiber blanket k rises steeply with temperature (radiation through the fibers). It now follows a Kaowool-type 128 kg/m³ curve vs mean temperature: 260 °C 0.06, 538 °C 0.12, 816 °C 0.21, 1093 °C 0.30 W/m·K, extrapolated. The input is now "Insulation conductivity at 600 C mean", and the curve is scaled to it (≈ 0.14 for that blanket).
  2. **The outer-face film was a fixed 15 W/m²K.** The outer face temperature is now solved so that conduction = convection (10 W/m²K) + radiation (ε 0.9) to the surroundings and the water-cooled coil.
  Same for insulated ends and for the heat the coil water absorbs. Hand check for the 2 in × 3 in rod (12.7 mm insulation entered as 0.3, ε 0.4, bare ends, ~2.2 kW to the rod): the max drops from ~1965 °C to ~1650 °C. Not included: the copper probe draw (0.25–1 kW at 1000 °C), the flux pool and the ceramic support under the rod. Enter these as "Extra heat draw at target"; e.g. 0.5 kW brings the max to ~1450 °C. Not yet build-tested.

- **2026-11-02 (coil copper loss from a current-distribution solve)** — The coil-turn copper resistance assumed the current spreads evenly over a band one tube OD wide on the inner face. I solved the actual surface current on round turns numerically: axisymmetric, perfect-conductor limit, 96 filaments per turn, equal current in every turn, flux constant on each tube surface. Checks: an isolated ring gives 0.328 (theory ~1/π = 0.318 plus a little inner-side crowding), and the result converges within ~1 % from 64 → 128 filaments. The true loss as a factor of the old formula is now tabulated over ID/d 4–13, pitch/d 1.05–3 and 2–12 turns, and interpolated (round tube only).
  - **Result:** the old formula *overstated* copper loss for normally spaced coils. Examples: 0.48 at ID/d 8, pitch/d 2, 3.5 turns (≈ the 65.5 mm ID, 3-turn 5/16" coil); 0.65 at pitch/d 1.5, 8 turns. Tight long coils come out > 1 (1.2–1.6 at pitch/d 1.05, 12 turns). A load barely changes it (19 mm rod: 0.464 → 0.465; 50 mm rod: 0.49).
  - **Consequence:** for typical coils efficiency, work power and therefore the max temperature go **up**, not down. A max temperature that reads higher than reality is not caused by copper loss. Look at holder conduction, supply output vs rating, and hot-material coupling.
  - Rectangular tube is unchanged (factor 1). Not yet build-tested.

- **2026-11-02 (efficiency re-check: copper resistance)** — Checked the power path to the max temperature. The thermal estimate uses `work power = efficiency × delivered power`, with efficiency = `R_load / (R_load + R_copper)` at the loaded frequency. The load resistance formula reduces exactly to the classic long-coil result (`2πr·N²·ρ/(δ·l)`). Two copper-loss errors made the efficiency, and so the work power and max temperature, too high:
  1. Copper resistivity was pure copper (1.8e-8 Ω·m). It is now 2.15e-8 for C12200/DHP tube at ~40 °C (~85 % IACS). The skin-effect resistance goes as √ρ, so copper loss is ~9 % higher.
  2. Close lead pairs had no proximity effect. Lead resistance is now × `(s/2r)/√((s/2r)² − 1)`: 1.06 at 19 mm c-c with 6.35 mm tube, 1.4 at 9 mm.
  The coil-turn resistance still assumes the current spreads evenly over the tube's full axial width (OD for round tube). Real round tube crowds the current onto a narrower inner band, so copper loss may still read low. Not yet build-tested.

- **2026-11-02 (power capped by the current limit)** — Max temperature check. The heat balance itself is right: for a 19 mm × 100 mm stainless rod taking 2.76 kW, radiation + convection balance at ~1450 °C, which is what that power really does to a bare rod that small. But the work power always used the full set supply power, even when the coil current needed for it was over the supply's max coil current. The delivered power is now capped at `Imax²·(R_load + R_copper)`. It drives the work power, copper loss, water rise, coil current, tank voltage and the thermal estimate. The warning now says how much power is actually delivered. Not yet build-tested.

- **2026-11-02 (rectangular leads no longer squashed)** — After the axis lock fix, rectangular leads twisted and flattened on the near-vertical part of the S-curve: the profile was locked to the coil axis, which is almost parallel to the path there. With the tangent S-curve leads, the helix is now swept locked to the coil axis and each lead separately locked to the across direction, then all three are joined. At the helix ends the radial direction is the across direction, so the profile matches at the joints. The coil, sleeve and bore all use one shared sweep helper. Round tube, leads not together, and "Continue around" still sweep in one piece. Not yet build-tested.

- **2026-11-02 (rectangular tube SWEEP_FAILED)** — Rectangular tube with the tangent S-curve leads failed with `SWEEP_FAILED`; round tube built. Likely cause: the sweep's default profile frame flips at the S-curve's inflection points and twists the rectangle. Coil, sleeve and bore sweeps now use `ProfileControlMode.LOCK_DIRECTION` with the coil axis as `lockDirection`. That parameter takes a direction vector; the first version passed a query and failed with "Expected vector". The rectangle's L side stays parallel to the axis everywhere (W radial on the helix, L vertical on level leads). Because a lead running exactly parallel to the locked axis is undefined, rectangular tube uses an S-bend lean of at least 10° (a vertical drop would be parallel to the axis). Round tube is unchanged. **Build confirmed in OnShape 2026-11-02** (rectangular tube, tangent S-curve leads, side by side):
  ![[coil-rect-tube-build.png]]

- **2026-11-02 (pressure: bends, laminar, rectangular bore)** — Cooling pressure drop now adds:
  1. **Lead bend losses.** Total turning angle of both lead paths (helix excluded) × Weisbach smooth-bend coefficient per 90°, `K90 = 0.131 + 1.847·(Dh/2R)^3.5` at the bend radius. In laminar flow this is scaled by `1 + 30/√Re`.
  2. **Tube entry and exit** (K = 0.5 + 1.0).
  The Cooling message splits ΔP into friction and bends + ends and shows the bend angle.
  - **Laminar rectangular bore** uses Shah & London `f·Re` (square 56.9, 2:1 62.2, 4:1 72.9) instead of 64, in the straight and coil (Mishra & Gupta) laminar formulas.
  - **Straight-tube transition** (Re 2300–4000) blends laminar to turbulent instead of jumping at 2300.
  - The message shows the coil and lead flow regimes separately.
  - Not yet build-tested.

- **2026-11-02 (thermal & cooling review)** — Checked the formulas against their references: Haaland, Schmidt critical Re, Mishra & Gupta laminar and turbulent coil friction, Darcy-Weisbach, the heat-up integral, quasi-steady core lag `q″R/2k`, and the soak coefficients 1.108 (parabolic start) and 1.602 (step). Those were correct. Fixed:
  1. **Bare work had no convection.** Loss was radiation only; it now adds natural convection (h = 10 W/m²K). Max temperature and heat-up come out a little lower and longer, mostly at low and medium temperatures.
  2. **Insulated loss was overstated.** The insulation's outer face was taken at ambient (no surface resistance). It now has an outer film (h = 15 W/m²K) in series. Insulated runs now show higher max temperatures and lower hold power.
  3. **Crucible heat capacity was several times too high.** The whole crucible cylinder was counted as solid charge. It now uses the real crucible body (graphite ~1800 kg/m³ · 1300 J/kg-K, ceramic ~2300 · 1000) plus the charge volume. Crucible heat-up times come out much shorter.
  4. **Coil water missed the heat from the hot work.** At 1000 °C a bare sample radiates kW straight into the water-cooled turns. The Thermal message now shows coil water rise at the target (copper loss + that heat, `waterTempRiseAtTarget_C`). The Cooling message now shows the water rise, which it did not before.
  5. **Pressure drop:** the "Continue around coil" extra winding now counts as coiled tube, not straight.
  Still simplified: pressure drop excludes fittings and hoses and uses water at 20 °C; thermal is lumped with constant properties and power absorbed at room-temperature electrical values. Not yet build-tested.

- **2026-11-02 (climb at the join)** — Default (tangent S-curve) route with "Set join distance from coil OD" ticked: the lead now runs out level at its own height and climbs to the pair height just before the join, so the bend moves out with the join distance. Before, the climb always started right after the lead cleared the turns. Unticked is unchanged (climb right after clearing, tightest join). "Continue around coil" already put the extra length before its S-bend. Not yet build-tested.

- **2026-11-02 (lean for all routes, lead-loop inductance)** — "S-bend lean from vertical" now shows whenever Bring leads together is ticked and also shapes the default (tangent S-curve) route. That route's climb was an eased curve ≥ 1.5 × the climb (and ≥ 3 × 1.25 lead radius). It is now the same two-arc profile as Continue around, at the bend radius (at least 1.25 × lead radius), placed along the plan path right after the lead clears the turns, then level. Note: with a large bend radius this climb is now longer than before at 0°. That route's lead spline is resampled to even ~1.5 mm points. Lead inductance: the pair model now splits each path segment into lead-direction, across and height components and couples parallel components. Before, only components along the lead direction coupled, so the vertical legs of a wide lead loop added only self-inductance and their mutual coupling was missing. The default route now feeds its whole lead path into this model (it used to put the curved part into a two-wire formula at the mean spacing). Lead self-inductance is still that of a straight conductor of the same length, so it is a little high for strongly bent leads. Not yet build-tested.

- **2026-11-02 (S-bend lean angle)** — New input "S-bend lean from vertical" (0–89°, default 0, shown when "Continue around coil" is ticked). 0° = the steepest S the bend radius allows (two arcs plus a vertical middle, as before). A larger angle leans the middle straight over by that much, so each arc turns 90° − angle and the S is gentler and longer: length `2r·sinθ + m·cosθ` with θ = 90° − angle and middle `m = (drop − 2r(1 − cosθ)) / sinθ`. Small drops that the arcs alone cover are unchanged. 90° is impossible (the lead would never reach the pair height). Not yet build-tested.

- **2026-11-02 (continue-around: true two-arc S-bend)** — The vertical S-bend that brings each lead to the pair height was an eased curve `3 × bend radius` long (min 1.5 × the drop): 76 mm on the 65.5 mm ID, 3-turn coil with a 25.4 mm bend radius. It is now two arcs at the bend radius, with a vertical straight between them when the drop is more than 2 × bend radius. Its length is the shortest the bend radius allows: `2r` (drop ≥ 2r) or `√(drop·(4r − drop))`. Example, 33 mm drop: 25.4 mm radius → ~48 mm, 12.7 mm → ~25 mm, 8 mm → ~16 mm. With "Set join distance" ticked, the extra length goes into the straight before the S, and the too-short message gives the new minimum. Use a smaller Bend radius (e.g. ~12 mm for 5/16" tube) for leads that drop almost straight down next to the coil. Not yet build-tested.

- **2026-11-02 (continue-around SWEEP_SELF_INT at tight spacing)** — 50.8 mm ID, 6 turns, 101.6 mm long, 6.35 mm tube, 12.7 mm bend, side by side: 25 mm c-c built, 23 mm failed with `SWEEP_SELF_INT`. A hand check found no real collision (leads 23 mm apart, ≥ 15 mm above the turns), so the likely cause is the fitted lead spline rippling: its points were very unevenly spaced (0.45 mm on a 1.4 mm straight, 1.1 mm on the turn, ~7 mm on the S-bend). Each lead's points are now resampled to even ~1.5 mm spacing before fitting, a straight under 0.5 mm gets no points, and the fitted lead is checked for its tightest bend: below 1.05 × tube radius the feature stops with a message naming the lead and its bend radius. Removed unused `tgNearLen` / `tgFarSpacing`; sweep errors are printed to the console. Not yet build-tested.

- **2026-11-02 (model fixes: HF inductance, load coupling, tank voltage)** — Checked against independent numerical calculations (exact current-sheet integral, loop-by-loop elliptic integrals, a perfect-conductor current-distribution solve of the round turns, brute-force tank circuit). Tank frequency formula, sheet formula, loop field, lead and Bessel load models were all correct. Fixed:
  1. **Load coupling** was 1.3–1.8× too high for short, wide coils: it used the coil-shape factor at the *sample* radius times the overlap. It now integrates the coil's actual field² along the whole load (`G = mu0·pi·Ro²·∫H² dz`), field from loops at the fitted current radius ID/2 + 0.35·d. Effect: the 78.2 mm, 3-turn coil with the 50.8 × 76.2 mm rod goes from **59.9 % to ~44 % efficiency** (coupling × 0.52).
  2. **HF inductance** used a plain current sheet at ID/2, off by −27 % to +6 % (−4.6 % on the 78.2 mm coil, −8 % on the 8-loop coil). Now sheet at ID/2 + 0.19·d minus Rosa's correction with conductor GMD 0.38·d, fitted to the numerical solution: within ~4 % over 64 coil shapes, 0.5 % on the 65.5 / 78.2 mm coils. Tank frequencies come out ~1–4 % lower.
  3. **Tank voltage** now `I·|R + jwL|` (was `I·wL`; matters only at low Q).
  The uniformity estimate uses the same field model (loops at the fitted radius), and the "continue around coil" extra winding is now included in the field. Not yet build-tested.

- **2026-11-01 (tangent exit)** — The leads no longer spiral outward and then come back in. Each lead now leaves the helix end tangentially and follows one smooth curve in to the pair line, level until it is outside every turn, then climbs to the pair height; the curve is lengthened until every bend is ≥ 1.25 × lead radius and the climb has room. Used for all coils (the spiral hug route is kept in the code but not used). Checked offline: 65.5 mm ID 4 turns 30 mm c-c → smallest bend 12.9 mm, ≥ 12.1 mm from the other turns, leads meet ~36 mm from the OD; 78.2 mm ID 3 turns 19 mm c-c → 12.2 mm, ≥ 13.8 mm, ~36 mm. Not yet build-tested.

- **2026-11-01 (no S-jog for the climb)** — When the tight hug was too short for the climb to the pair height (e.g. 65.5 mm ID, 4 turns: ~31 mm of path for a ~28 mm climb needing ~42 mm), the leads switched to the wide route and made a small sideways S-jog onto the pair line. Now they keep the tight hug and finish the climb along the pair line (leads already side by side). The wide route is only used for a set join distance beyond the tight hug. **Build confirmed in OnShape 2026-11-01** (65.5 mm ID, 4 turns, 30 mm c-c, side by side):
  ![[coil-hug-leads-4turn-65mm.png]]
- **2026-11-01 (tank circuit & power supply)** — Tank frequency now includes the series resistance: `w² = 1/(LC) − (R/L)²` (the old `1/√(LC)` read ~1/(2Q²) high: < 0.2 % at Q 21, ~17 % at Q 1.8); "no resonance" when Q < ~1. New outputs: coil current and tank voltage at the supply power (`#coilCurrent_A`, `#tankVoltage_V`). New inputs: supply min / max frequency (default 150 / 400 kHz) and max coil current (default 550 A), with a WARNING in the Tank and Efficiency messages when outside them. Extra capacitor leads are now behind an "Extra leads to capacitor" checkbox (on by default, so existing features keep their leads). Group renamed "Tank circuit & power supply". Not yet build-tested.
- **2026-11-01 (exact joint tangent, sweep diagnostics)** — After the small-coil S-curve the main sweep reported `SWEEP_INVALID_PATH`. Hug / S-curve lead splines (and their sleeves) now start with the helix's exact tangent at the joint, like the working "Continue around" splines, instead of the first chord's direction. If the coil sweep still fails, the feature now shows the sweep path highlighted in red and names the lead route used ("hug", "S-curve (small coil)", "S-bend", "continue around"). Not yet build-tested.
- **2026-11-01 (small-coil S-curve)** — The hug spiral still failed with `SWEEP_SELF_INT` on the 25.4 mm ID coil (its outward curl at the helix end is too tight to sweep reliably). Small coils (when that curl would need more than ~0.6 rad of spiral) now use one smooth S-curve per lead: it leaves the helix end tangentially, curves in to the pair line, stays level until it is outside every turn, then climbs to the pair height; it is lengthened until every bend is ≥ 1.25 × lead radius and the climb has room (≥ 1.5 × the climb). No straight sections. Large coils (e.g. 78 mm ID) keep the hug route. Checked offline for the 25.4 mm coil (7 turns, 65 mm, 6.35 mm tube, 0.6 mm sleeve, 19 mm c-c): smallest bend ~12–13 mm, leads ≥ 7.8 mm from the other turns (tube + sleeve 7.55 mm), join ~42–43 mm from the OD (the climb needs that; a set distance shorter than this is pushed out). Not yet build-tested.
- **2026-11-01 (spiral start curl)** — Hug route still hit `SWEEP_SELF_INT` on the 25.4 mm ID coil. A numerical check found the cause: right at the helix end the spiral curls outward with a ~3.1 mm radius, less than the 3.175 mm tube radius. The spiral length now also limits that start curl (≥ 1.25 × lead radius), and the spiral is sampled with 64 points. Checked offline: 25.4 mm coil, smallest bend 5.0 mm, leads ≥ 9.7 mm from the turns (centre to centre), 19 mm apart; 78 mm coil keeps its tight shape. Not yet build-tested.
- **2026-11-01 (climb room, no straight)** — "Set join distance" unticked now also uses the hug route (tightest join that fits), so there is no straight section from the coil. Fixed `SWEEP_SELF_INT` on small coils: the climb to the pair height (~29 mm on a 65 mm coil) was squeezed into ~16 mm of lead. The route must now be ≥ 1.5 × the climb (and ≥ 3 × the minimum bend); if the tight route is too short the leads turn out after the spiral and one S-curve, lengthened until the climb and every bend fit, brings them to the pair. The 78 mm coil keeps its tight shape. Not yet build-tested.

- **2026-11-01 (bend limits)** — Fixes for `SWEEP_SELF_INT` and `opFitSpline: INVALID_INPUT`. Hug route: every bend is kept ≥ 1.25 × lead radius (tube + sleeve): the spiral out to the clearance radius is lengthened if needed but stops while there is room to turn out; for a longer join the lead turns out right after the spiral (bend radius, reduced if needed) and one S-curve brings it to the pair at the join distance; the tight route shrinks its turn radius if needed. If the leads truly can't hug the coil, a clear error says so. Sleeve: the straight sleeve lead is skipped when "Sleeve lead length" ≤ bend radius (two coincident points were invalid). New error when sleeves on neighbouring turns overlap (pitch < tube + 2 × sleeve). Not yet build-tested.
- **2026-11-01 (hug + join distance)** — Rebuilt on the user's working source. "Set join distance from coil OD" ticked: the leads always hug the coil (spiral just out to the clearance radius, no bulge). If the join is farther than the tightest turn reaches, each lead leaves the circle right after the spiral and follows one smooth curve to the join, so the leads stay apart and meet at the set distance (no straight run); if that curve would dip inside the clearance radius it falls back to the tight hug + a stretched turn. Unticked: straight out + S-bend, as before. Long final straights now get ~3 mm fit points. Not yet build-tested.

- **2026-11-01 (small-coil leads, reverted)** — Reverted: the source was reset to the user's working version. Hug route failed with `SWEEP_SELF_INT` on small coils (25.4 mm ID, 6.35 mm tube, 12.7 mm bend): the spiral out to the clearance radius bent tighter than the tube. The script now checks the bend; if the hug would be too tight it uses one smooth S-curve (heads straight out from the helix end, curves to the pair line, level until clear of the turns, then climbs), lengthened until every bend is ≥ 1.25 × lead radius. Join distance still applies (unticked = tightest). Not yet build-tested.
- **2026-11-01 (long lead fix)** — One-spline leads (hug, continue-around, and their sleeves) failed with `opFitSpline: INVALID_RESULT` for long leads (559 mm): the final straight had only 4 fit points. It now gets evenly spaced points (~3 mm, max 300). Not yet build-tested.
- **2026-11-01 (join distance consistent, superseded)** — "Set join distance from coil OD" Leads together (not "continue around") now always hug the coil with no straight sections. Unticked = tightest join; ticked = the outward turn becomes a tangent curve stretched to the set distance. Before, the route flipped to a straight + long S-bend once that fit (e.g. 77 mm hugged, 78 mm S-bent). The straight-out S-bend route is no longer used. Not yet build-tested.
- **2026-11-01 (wrap bulge fix)** — Wrap mode: leads stay at the clearance radius instead of a wider circle (fixes leads bulging outward). Not yet build-tested. Before the fix:
  ![[coil-wrap-bulge-bug.png]]
- **2026-11-01 (lead inductance)** — Lead inductance now uses a finite-length partial-inductance model along the actual modeled lead paths (all lead modes) instead of the infinite two-wire formula, which was ~20 % high for the short, widely spaced coil leads and ~5 % high for short, close pairs; in leads-together mode it also no longer averages the coil-end spacing (~102 mm) into the paired section. Proximity correction added and lead paths start at the helix ends (2026-11-01 review). Effect on the 3-turn design: not yet re-checked. Not yet build-tested.
- **2026-11-01 (lead spacing)** — "Gap between leads" replaced by "Lead spacing (center to center)" (default 13 mm); errors if it is less than one tube + both sleeves (leads would overlap). Note: existing features need the spacing re-entered (new parameter). Not yet build-tested.
- **2026-11-01 (continue around coil)** — New "Continue around coil, then straight out" option: each lead keeps winding at the coil radius and pitch (bottom lead downward, top lead upward, i.e. a little more coil) until it has come round to the exit side, turns outward with the bend radius, runs straight out until clear of every turn, makes a vertical S-bend to the pair height, then runs straight to the connection; one spline per lead. New arrangement "Own heights" skips the height change. The extra winding is counted as coil turns (inductance, copper loss, message "around coil +N turns") and removed from the lead length; the side-heating uniformity uses the main helix only. Not yet build-tested.
- **2026-11-01 (hug wrap, seamless leads)** — Wrap mode reworked: no straight run. From the helix end each lead keeps the coil curvature and spirals smoothly out to the clearance radius (coil OD + lead radius + 0.5 mm) over ~20–70°, follows it toward the centre, then turns outward with the bend radius into the pair; the helix slope is levelled over one bend radius and the height change happens only at the clearance radius. Each lead (helix end → far end, incl. the final straight) and each sleeve lead is now **one fitted spline** with matched end tangents, so there are no seams along the leads (only where lead meets helix). Tightest join for this coil ≈ 4.4 mm at a 5 mm bend radius. Not yet build-tested.
- **2026-11-01 (wrap route, superseded)** — Join distance is now the **gap from the coil OD to the outside of the paired leads** (0 = as tight as possible). When it is too short for an S-bend, the leads **wrap**: straight out to a clearance circle (coil OD + lead radius + 0.5 mm), a 90° arc toward the centre, a straight across the coil, and a bend-radius turn outward into the pair; the height change (smoothstep) happens along the wrap, which stays outside every turn. Tightest join ≈ bend radius + 0.5 mm. Message says "wrapped" when used. Not yet build-tested.
- **2026-11-01 (join distance)** — Leads together: optional "Set join distance from coil OD" (outer edge incl. sleeve → start of the parallel pair). The S-bend stretches to fit; errors with the minimum distance if the bend would be shorter than its sideways shift or 3 bend radii. Message reports the join distance used. Not yet build-tested.
- **2026-11-01 (leads together)** — "Bring leads together" option: each lead runs straight out until it clears the turns (axis distance > path radius + tube + sleeves + 1 mm), then a tangent S-bend (cubic Bezier, length max(1.5 × offset, 3 × bend radius)) moves it to a parallel pair, side by side at the coil's mid-height or stacked on its center plane, spaced tube + 2 × sleeve + gap; then "Lead length" of straight. Sleeve follows the route ("Sleeve lead length" measured on the final straight). Needs a whole number of turns. Lead inductance = route length at the average of the exit and final spacings + final straight at the final spacing; copper loss uses the modeled lead length. Not yet build-tested.
- **2026-11-01 (materials)** — Removed reading the part's OnShape material (not possible during regeneration; see Dummy Sample and Materials). The "Use part's assigned material" checkbox is gone; all samples use the script's material table, and the messages name the material used.
- **2026-11-01 (code review)** — Fixed material-name matching order (non-ferrous names before stainless so "Aluminum 3003" isn't read as 300-series stainless; copper UNS needs 5 digits so "C1018" steel isn't copper; stainless numbers must stand alone). Soak time now models the real run: after a full-power heat-up the core trails by the net-power lag, so `#thermSoak_s` is the soak from that state (≈ 41 s for this rig vs 96 s before); the old step-from-cold value is kept as `#thermSoakFromCold_s` (worst case). Guarded a zero tolerance at a 0 °C target.
- **2026-11-01 (tube wall)** — Round tube: "Specify wall thickness instead of ID" option (bore = OD − 2 × wall, default 0.032 in); Cooling message now shows the bore (or hydraulic diameter for rectangular tube).
- **2026-11-01 (part material)** — Part samples now read the part's assigned OnShape material (name → closest preset, plus the part's density for the thermal estimate), with a "Use part's assigned material" checkbox; Efficiency and Thermal messages report what was used. Build confirmed in OnShape 2026-11-01.
- **2026-11-01 (latest)** — Added "Stainless 304 - hot (~1000 C)" material preset (117 uOhm-cm); thermal estimate now reports the supply power needed to hold the target (`#thermHoldSupply_kW`) and warns when full power can exceed the material's melting point. Build confirmed in OnShape 2026-11-01.
- **2026-11-01 (later)** — UI: heating-uniformity inputs moved into their own collapsed group "Heating uniformity (side surface)" and relabelled "Heating band (+/- %)" / "Target zone length (centered on coil)"; thermal inputs moved into their own "Thermal estimate" group with a driving checkbox; tolerance relabelled "Core within (+/- % of target C)" so it is not confused with the heating band. Parameter IDs unchanged, so existing features keep their values.
- **2026-11-01** — (O'Malley Flux Setup copy) Added a thermal estimate: max reachable temperature from a lumped heat balance (radiation or insulated sides, end faces, extra heat draw), heat-up time, soak time for the core to be within a set % of a target, and core lag at full power. New "Thermal" message option, "Thermal estimate" inputs, custom thermal properties, five new variables. **Build confirmed in OnShape 2026-11-01.**

- **2026-10-30 (later)** — Added a heating uniformity estimate: axial heating profile on the sample's outer (side) surface from an off-axis loop field model. Reports uniform zone length, target-zone min/max, end drop-off, and turn ripple. Added a "Uniformity" message option, two new inputs (band %, target zone length), and four new variables. Build confirmed in OnShape 2026-10-31 (Uniformity message output not yet checked).

- **2026-10-30** — Merged the Gap and Length scripts (Length default, gap checkbox); added inductance estimates, dummy sample/crucible, materials, tank frequency, efficiency and cooling estimates, grouped dialog, Coil/Sleeve naming and colors, optional tube bore. Upgraded to FeatureScript 3044.

---

## Full Source Code

```featurescript
FeatureScript 3044;
import(path : "onshape/std/common.fs", version : "3044.0");
import(path : "onshape/std/geometry.fs", version : "3044.0");

export enum CoilProfileType
{
    annotation { "Name" : "Round" }
    ROUND,
    annotation { "Name" : "Rectangular" }
    RECTANGULAR
}

export enum CoilSampleType
{
    annotation { "Name" : "Cylinder" }
    CYLINDER,
    annotation { "Name" : "Prism" }
    PRISM,
    annotation { "Name" : "Part" }
    PART,
    annotation { "Name" : "Crucible" }
    CRUCIBLE
}

export enum CoilMaterial
{
    annotation { "Name" : "Steel - cold (magnetic)" }
    STEEL_COLD,
    annotation { "Name" : "Steel - hot (above Curie)" }
    STEEL_HOT,
    annotation { "Name" : "Stainless 304" }
    STAINLESS,
    annotation { "Name" : "Stainless 304 - hot (~1000 C)" }
    STAINLESS_HOT,
    annotation { "Name" : "Copper" }
    COPPER,
    annotation { "Name" : "Aluminum" }
    ALUMINUM,
    annotation { "Name" : "Brass" }
    BRASS,
    annotation { "Name" : "Titanium" }
    TITANIUM,
    annotation { "Name" : "Graphite" }
    GRAPHITE,
    annotation { "Name" : "Custom" }
    CUSTOM
}

export enum CoilCrucibleMaterial
{
    annotation { "Name" : "Graphite" }
    GRAPHITE,
    annotation { "Name" : "Ceramic / clay (non-conductive)" }
    CERAMIC,
    annotation { "Name" : "Custom" }
    CUSTOM
}

export enum CoilLeadJoin
{
    annotation { "Name" : "Side by side" }
    SIDE_BY_SIDE,
    annotation { "Name" : "Stacked" }
    STACKED,
    annotation { "Name" : "Own heights" }
    OWN_HEIGHTS
}

export enum CoilEstimateDisplay
{
    annotation { "Name" : "Inductance" }
    INDUCTANCE,
    annotation { "Name" : "Tank" }
    TANK,
    annotation { "Name" : "Efficiency" }
    EFFICIENCY,
    annotation { "Name" : "Cooling" }
    COOLING,
    annotation { "Name" : "Uniformity" }
    UNIFORMITY,
    annotation { "Name" : "Thermal" }
    THERMAL,
    annotation { "Name" : "All" }
    ALL
}

annotation { "Feature Type Name" : "Induction Coil" }
export const inductionCoil = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
        // =========================================================================
        // PLACEMENT
        // =========================================================================
        annotation { "Group Name" : "Placement", "Collapsed By Default" : false }
        {
            annotation { "Name" : "Center target (point or line)", "Filter" : EntityType.VERTEX || EntityType.EDGE, "MaxNumberOfPicks" : 1 }
            definition.target is Query;

            annotation { "Name" : "Reverse winding (CCW)", "UIHint" : UIHint.OPPOSITE_DIRECTION }
            definition.reverseDirection is boolean;

            annotation { "Name" : "Rotation angle" }
            isAngle(definition.rotationAngle, { (degree) : [-360, 0, 360] } as AngleBoundSpec);
        }

        // =========================================================================
        // COIL
        // =========================================================================
        annotation { "Group Name" : "Coil", "Collapsed By Default" : false }
        {
            annotation { "Name" : "Coil inner diameter (ID)" }
            isLength(definition.coilId, { (millimeter) : [0.1, 25, 2000] } as LengthBoundSpec);

            annotation { "Name" : "Number of turns" }
            isReal(definition.turns, { (unitless) : [0.01, 5, 500] } as RealBoundSpec);

            // Unchecked (default) = drive pitch from overall Coil Length
            // Checked             = drive pitch from Gap Between Winds
            annotation { "Name" : "Specify gap between winds", "Default" : false }
            definition.useGap is boolean;

            if (definition.useGap)
            {
                annotation { "Name" : "Gap between winds" }
                isLength(definition.gap, { (millimeter) : [0, 3, 500] } as LengthBoundSpec);
            }
            else
            {
                annotation { "Name" : "Coil length" }
                isLength(definition.coilLength, { (millimeter) : [0.1, 50, 2000] } as LengthBoundSpec);
            }
        }

        // =========================================================================
        // TUBE PROFILE
        // =========================================================================
        annotation { "Group Name" : "Tube Profile", "Collapsed By Default" : false }
        {
            annotation { "Name" : "Profile", "UIHint" : UIHint.HORIZONTAL_ENUM }
            definition.profileType is CoilProfileType;

            if (definition.profileType == CoilProfileType.ROUND)
            {
                annotation { "Name" : "Tube outer diameter (OD)" }
                isLength(definition.tubeOd, { (millimeter) : [0.1, 5, 200] } as LengthBoundSpec);
            }
            else
            {
                annotation { "Name" : "Tube axial length (L)" }
                isLength(definition.tubeL, { (millimeter) : [0.1, 5, 200] } as LengthBoundSpec);

                annotation { "Name" : "Tube radial width (W)" }
                isLength(definition.tubeW, { (millimeter) : [0.1, 5, 200] } as LengthBoundSpec);
            }
        }

        // =========================================================================
        // LEADS
        // =========================================================================
        annotation { "Group Name" : "Leads", "Collapsed By Default" : false }
        {
            annotation { "Name" : "Lead length" }
            isLength(definition.leadLength, { (millimeter) : [0, 25, 2000] } as LengthBoundSpec);

            annotation { "Name" : "Bend radius" }
            isLength(definition.bendRadius, { (millimeter) : [0.1, 5, 500] } as LengthBoundSpec);

            // Bring both leads together into a closely spaced parallel pair
            annotation { "Name" : "Bring leads together" }
            definition.leadsTogether is boolean;

            if (definition.leadsTogether)
            {
                annotation { "Name" : "Lead arrangement", "UIHint" : UIHint.HORIZONTAL_ENUM }
                definition.leadJoin is CoilLeadJoin;

                annotation { "Name" : "Continue around coil, then straight out" }
                definition.wrapAround is boolean;

                // Height change to the pair: 0 = steepest S-bend the bend radius
                // allows (vertical middle); larger = the middle leans over, gentler and longer
                annotation { "Name" : "S-bend lean from vertical" }
                isAngle(definition.sBendLean, { (degree) : [0, 0, 89] } as AngleBoundSpec);

                annotation { "Name" : "Lead spacing (center to center)" }
                isLength(definition.leadCtc, { (millimeter) : [0.1, 13, 1000] } as LengthBoundSpec);

                annotation { "Name" : "Set join distance from coil OD" }
                definition.setJoinDist is boolean;

                if (definition.setJoinDist)
                {
                    annotation { "Name" : "Join distance from coil OD" }
                    isLength(definition.joinDist, { (millimeter) : [0, 60, 5000] } as LengthBoundSpec);
                }
            }
        }

        // =========================================================================
        // OUTER SLEEVE
        // =========================================================================
        annotation { "Name" : "Outer sleeve" }
        definition.addSleeve is boolean;

        if (definition.addSleeve)
        {
            annotation { "Group Name" : "Outer sleeve", "Driving Parameter" : "addSleeve", "Collapsed By Default" : false }
            {
                annotation { "Name" : "Sleeve thickness" }
                isLength(definition.sleeveThickness, { (millimeter) : [0.1, 1, 200] } as LengthBoundSpec);

                annotation { "Name" : "Sleeve lead length" }
                isLength(definition.sleeveLeadLength, { (millimeter) : [0, 25, 2000] } as LengthBoundSpec);
            }
        }

        // =========================================================================
        // ESTIMATED VALUES
        // =========================================================================
        annotation { "Name" : "Estimated values" }
        definition.showEstimates is boolean;

        if (definition.showEstimates)
        {
            annotation { "Group Name" : "Estimated values", "Driving Parameter" : "showEstimates", "Collapsed By Default" : false }
            {
                annotation { "Name" : "Show in message", "UIHint" : UIHint.HORIZONTAL_ENUM }
                definition.estimateDisplay is CoilEstimateDisplay;
            }

            // --- Dummy sample ---
            annotation { "Name" : "Dummy sample" }
            definition.addSample is boolean;

            if (definition.addSample)
            {
                annotation { "Group Name" : "Dummy sample", "Driving Parameter" : "addSample", "Collapsed By Default" : false }
                {
                    annotation { "Name" : "Sample type", "UIHint" : UIHint.HORIZONTAL_ENUM }
                    definition.sampleType is CoilSampleType;

                    if (definition.sampleType == CoilSampleType.CYLINDER)
                    {
                        annotation { "Name" : "Diameter" }
                        isLength(definition.sampleDiameter, { (millimeter) : [0.1, 15, 2000] } as LengthBoundSpec);

                        annotation { "Name" : "Length" }
                        isLength(definition.sampleCylLength, { (millimeter) : [0.1, 100, 5000] } as LengthBoundSpec);
                    }
                    else if (definition.sampleType == CoilSampleType.PRISM)
                    {
                        annotation { "Name" : "Width" }
                        isLength(definition.samplePrismWidth, { (millimeter) : [0.1, 10, 2000] } as LengthBoundSpec);

                        annotation { "Name" : "Depth" }
                        isLength(definition.samplePrismDepth, { (millimeter) : [0.1, 10, 2000] } as LengthBoundSpec);

                        annotation { "Name" : "Length" }
                        isLength(definition.samplePrismLength, { (millimeter) : [0.1, 55, 5000] } as LengthBoundSpec);
                    }
                    else if (definition.sampleType == CoilSampleType.PART)
                    {
                        annotation { "Name" : "Sample part", "Filter" : EntityType.BODY && BodyType.SOLID, "MaxNumberOfPicks" : 1 }
                        definition.samplePart is Query;
                    }

                    if (definition.sampleType == CoilSampleType.CRUCIBLE)
                    {
                        annotation { "Name" : "Crucible outer diameter" }
                        isLength(definition.crucibleOd, { (millimeter) : [0.1, 20, 2000] } as LengthBoundSpec);

                        annotation { "Name" : "Crucible wall thickness" }
                        isLength(definition.crucibleWall, { (millimeter) : [0.1, 3, 500] } as LengthBoundSpec);

                        annotation { "Name" : "Crucible height" }
                        isLength(definition.crucibleHeight, { (millimeter) : [0.1, 50, 5000] } as LengthBoundSpec);

                        annotation { "Name" : "Crucible bottom thickness" }
                        isLength(definition.crucibleBottom, { (millimeter) : [0.1, 5, 500] } as LengthBoundSpec);

                        annotation { "Name" : "Crucible material" }
                        definition.crucibleMaterial is CoilCrucibleMaterial;

                        if (definition.crucibleMaterial == CoilCrucibleMaterial.CUSTOM)
                        {
                            annotation { "Name" : "Crucible resistivity (uOhm-cm)" }
                            isReal(definition.crucibleRho, { (unitless) : [0.1, 1000, 1e9] } as RealBoundSpec);
                        }

                        annotation { "Name" : "Charge material" }
                        definition.chargeMaterial is CoilMaterial;

                        if (definition.chargeMaterial == CoilMaterial.CUSTOM)
                        {
                            annotation { "Name" : "Charge resistivity (uOhm-cm)" }
                            isReal(definition.chargeRho, { (unitless) : [0.1, 17, 1e9] } as RealBoundSpec);

                            annotation { "Name" : "Charge relative permeability (ur)" }
                            isReal(definition.chargeMu, { (unitless) : [1, 1, 100000] } as RealBoundSpec);
                        }

                        annotation { "Name" : "Charge fill height" }
                        isLength(definition.chargeFill, { (millimeter) : [0, 25, 5000] } as LengthBoundSpec);
                    }
                    else
                    {
                        annotation { "Name" : "Sample material" }
                        definition.sampleMaterial is CoilMaterial;

                        if (definition.sampleMaterial == CoilMaterial.CUSTOM)
                        {
                            annotation { "Name" : "Sample resistivity (uOhm-cm)" }
                            isReal(definition.sampleRho, { (unitless) : [0.1, 17, 1e9] } as RealBoundSpec);

                            annotation { "Name" : "Sample relative permeability (ur)" }
                            isReal(definition.sampleMu, { (unitless) : [1, 1, 100000] } as RealBoundSpec);

                            // Used only by the thermal estimate
                            annotation { "Name" : "Thermal conductivity (W/m-K)" }
                            isReal(definition.sampleK, { (unitless) : [0.001, 25, 5000] } as RealBoundSpec);

                            annotation { "Name" : "Density (kg/m3)" }
                            isReal(definition.sampleDensity, { (unitless) : [1, 7800, 50000] } as RealBoundSpec);

                            annotation { "Name" : "Specific heat (J/kg-K)" }
                            isReal(definition.sampleCp, { (unitless) : [1, 600, 50000] } as RealBoundSpec);
                        }
                    }
                }

                // --- Heating uniformity (side surface, heat generated) ---
                annotation { "Group Name" : "Heating uniformity (side surface)", "Collapsed By Default" : true }
                {
                    annotation { "Name" : "Heating band (+/- %)" }
                    isReal(definition.uniformBand, { (unitless) : [0.1, 10, 100] } as RealBoundSpec);

                    annotation { "Name" : "Target zone length (centered on coil)" }
                    isLength(definition.targetZone, { (millimeter) : [0.1, 10, 5000] } as LengthBoundSpec);
                }

                // --- Thermal estimate (max temperature, heat-up, soak) ---
                annotation { "Name" : "Thermal estimate" }
                definition.thermalEst is boolean;

                if (definition.thermalEst)
                {
                    annotation { "Group Name" : "Thermal estimate", "Driving Parameter" : "thermalEst", "Collapsed By Default" : false }
                    {
                        annotation { "Name" : "Target temperature (C)" }
                        isReal(definition.thermTarget, { (unitless) : [-200, 1000, 5000] } as RealBoundSpec);

                        annotation { "Name" : "Start / ambient temperature (C)" }
                        isReal(definition.thermAmbient, { (unitless) : [-200, 20, 2000] } as RealBoundSpec);

                        annotation { "Name" : "Core within (+/- % of target C)" }
                        isReal(definition.thermTol, { (unitless) : [0.01, 2, 100] } as RealBoundSpec);

                        annotation { "Name" : "Surface emissivity" }
                        isReal(definition.thermEmissivity, { (unitless) : [0.01, 0.8, 1] } as RealBoundSpec);

                        annotation { "Name" : "Side insulation thickness (0 = bare)" }
                        isLength(definition.thermInsT, { (millimeter) : [0, 0, 500] } as LengthBoundSpec);

                        annotation { "Name" : "Insulation conductivity at 600 C mean (W/m-K)" }
                        isReal(definition.thermInsK, { (unitless) : [0.001, 0.15, 100] } as RealBoundSpec);

                        annotation { "Name" : "Insulate end faces too" }
                        definition.thermInsEnds is boolean;

                        annotation { "Name" : "Extra heat draw at target (W)" }
                        isReal(definition.thermExtraW, { (unitless) : [0, 0, 1e7] } as RealBoundSpec);
                    }
                }
            }

            // --- Cooling ---
            annotation { "Group Name" : "Cooling water", "Collapsed By Default" : false }
            {
                if (definition.profileType == CoilProfileType.ROUND)
                {
                    annotation { "Name" : "Specify wall thickness instead of ID" }
                    definition.roundUseWall is boolean;

                    if (definition.roundUseWall)
                    {
                        annotation { "Name" : "Tube wall thickness (round)" }
                        isLength(definition.roundWall, { (millimeter) : [0.01, 0.8128, 100] } as LengthBoundSpec);
                    }
                    else
                    {
                        annotation { "Name" : "Tube inner diameter (ID)" }
                        isLength(definition.tubeBoreId, { (millimeter) : [0.01, 3, 200] } as LengthBoundSpec);
                    }
                }
                else
                {
                    annotation { "Name" : "Tube wall thickness" }
                    isLength(definition.tubeWall, { (millimeter) : [0.01, 0.8, 100] } as LengthBoundSpec);
                }

                annotation { "Name" : "Model tube bore" }
                definition.modelBore is boolean;

                annotation { "Name" : "Flow rate (L/min)" }
                isReal(definition.flowRate, { (unitless) : [0.01, 2, 500] } as RealBoundSpec);
            }

            // --- Tank circuit & power ---
            annotation { "Group Name" : "Tank circuit & power supply", "Collapsed By Default" : false }
            {
                annotation { "Name" : "Capacitance (uF)" }
                isReal(definition.tankCap, { (unitless) : [0.001, 1, 100000] } as RealBoundSpec);

                annotation { "Name" : "Extra leads to capacitor", "Default" : true }
                definition.useTankLeads is boolean;

                if (definition.useTankLeads)
                {
                    annotation { "Name" : "Extra lead length to capacitor" }
                    isLength(definition.tankLeadLength, { (millimeter) : [0, 100, 10000] } as LengthBoundSpec);

                    annotation { "Name" : "Extra lead spacing (center to center)" }
                    isLength(definition.tankLeadSpacing, { (millimeter) : [0.1, 10, 1000] } as LengthBoundSpec);
                }

                annotation { "Name" : "Supply power (kW)" }
                isReal(definition.supplyPower, { (unitless) : [0.01, 3, 100000] } as RealBoundSpec);

                annotation { "Name" : "Supply min frequency (kHz)" }
                isReal(definition.supplyFmin, { (unitless) : [0, 150, 1e6] } as RealBoundSpec);

                annotation { "Name" : "Supply max frequency (kHz)" }
                isReal(definition.supplyFmax, { (unitless) : [0, 400, 1e6] } as RealBoundSpec);

                annotation { "Name" : "Supply max coil current (A, 0 = no check)" }
                isReal(definition.supplyImax, { (unitless) : [0, 550, 1e7] } as RealBoundSpec);
            }
        }
    }
    {
        // 1. Guard against empty target selection
        if (isQueryEmpty(context, definition.target))
        {
            throw regenError("Please select a vertex or edge target axis.", ["target"]);
        }

        // 2. Resolve Target and Establish Coordinate System
        var origin;
        var zAxis;
        var xAxis;

        var isVertex = !isQueryEmpty(context, qEntityFilter(definition.target, EntityType.VERTEX));
        var isEdge = !isQueryEmpty(context, qEntityFilter(definition.target, EntityType.EDGE));

        if (isVertex) {
            origin = evVertexPoint(context, { "vertex" : definition.target });
            zAxis = vector(0, 0, 1);
            xAxis = vector(1, 0, 0);
        } else if (isEdge) {
            var tangent = evEdgeTangentLine(context, { "edge" : definition.target, "parameter" : 0.5 });
            origin = tangent.origin;
            zAxis = tangent.direction;
            xAxis = perpendicularVector(zAxis);
        } else {
            throw regenError("Please select a vertex or an edge.", ["target"]);
        }

        var yAxis = cross(zAxis, xAxis);

        // 3. Fallback Type Guards
        var coilId = (definition.coilId is ValueWithUnits) ? definition.coilId : 25 * millimeter;
        var turns = (definition.turns is number) ? definition.turns : 5;
        var useGap = (definition.useGap == true);
        var gap = (definition.gap is ValueWithUnits) ? definition.gap : 3 * millimeter;
        var coilLength = (definition.coilLength is ValueWithUnits) ? definition.coilLength : 50 * millimeter;
        var leadLength = (definition.leadLength is ValueWithUnits) ? definition.leadLength : 25 * millimeter;
        var reverseDirection = (definition.reverseDirection == true);

        var bendRadius = (definition.bendRadius is ValueWithUnits && definition.bendRadius > 0*meter) ? definition.bendRadius : 5 * millimeter;
        if (bendRadius > leadLength * 0.8) { bendRadius = leadLength * 0.8; }

        var isRectangular = (definition.profileType == CoilProfileType.RECTANGULAR);
        var tubeOd = 5 * millimeter;
        var tubeL = 5 * millimeter;
        var tubeW = 5 * millimeter;

        if (!isRectangular) {
            if (definition.tubeOd is ValueWithUnits) { tubeOd = definition.tubeOd; }
            tubeL = tubeOd;
            tubeW = tubeOd;
        } else {
            if (definition.tubeL is ValueWithUnits) { tubeL = definition.tubeL; }
            if (definition.tubeW is ValueWithUnits) { tubeW = definition.tubeW; }
            tubeOd = tubeW;
        }

        // Sleeve Guards
        var addSleeve = (definition.addSleeve == true);
        var sleeveThickness = (definition.sleeveThickness is ValueWithUnits) ? definition.sleeveThickness : 1 * millimeter;

        // Default sleeve lead length to the main lead length if undefined, and ensure it doesn't break the bend curve
        var sleeveLeadLength = (definition.sleeveLeadLength is ValueWithUnits) ? definition.sleeveLeadLength : leadLength;
        if (sleeveLeadLength < bendRadius) { sleeveLeadLength = bendRadius; }

        // Estimated-values Guards
        var showEstimates = (definition.showEstimates == true);
        var addSample = showEstimates && (definition.addSample == true);
        var modelBore = showEstimates && (definition.modelBore == true);
        var estimateDisplay = (definition.estimateDisplay is CoilEstimateDisplay) ? definition.estimateDisplay : CoilEstimateDisplay.INDUCTANCE;

        var boreId = tubeOd * 0.6;
        var tubeWall = 0.8 * millimeter;
        if (showEstimates)
        {
            if (!isRectangular)
            {
                if (definition.roundUseWall == true)
                {
                    var rWall = (definition.roundWall is ValueWithUnits) ? definition.roundWall : 0.8128 * millimeter;
                    boreId = tubeOd - 2 * rWall;
                    if (boreId <= 0 * meter)
                    {
                        throw regenError("Wall thickness must be less than half the tube OD.", ["roundWall"]);
                    }
                }
                else
                {
                    if (definition.tubeBoreId is ValueWithUnits) { boreId = definition.tubeBoreId; }
                    if (boreId >= tubeOd)
                    {
                        throw regenError("Tube ID must be smaller than the tube OD.", ["tubeBoreId"]);
                    }
                }
            }
            else
            {
                if (definition.tubeWall is ValueWithUnits) { tubeWall = definition.tubeWall; }
                if (2 * tubeWall >= min(tubeL, tubeW))
                {
                    throw regenError("Wall thickness is too large for this tube size.", ["tubeWall"]);
                }
            }
        }

        var flowLpm = (definition.flowRate is number) ? definition.flowRate : 2;
        var tankCapUF = (definition.tankCap is number) ? definition.tankCap : 1;
        var useTankLeads = (definition.useTankLeads != false);   // older features without the checkbox keep their leads
        var tankLeadLength = (useTankLeads && definition.tankLeadLength is ValueWithUnits) ? definition.tankLeadLength
                                                                                          : (useTankLeads ? 100 * millimeter : 0 * meter);
        var tankLeadSpacing = (definition.tankLeadSpacing is ValueWithUnits) ? definition.tankLeadSpacing : 10 * millimeter;
        var supplyKW = (definition.supplyPower is number) ? definition.supplyPower : 3;
        var supplyFminKHz = (definition.supplyFmin is number) ? definition.supplyFmin : 150;   // HOTSHOT 5: 150-400 kHz
        var supplyFmaxKHz = (definition.supplyFmax is number) ? definition.supplyFmax : 400;
        var supplyImaxA = (definition.supplyImax is number) ? definition.supplyImax : 550;      // HOTSHOT current ceiling (measured)
        if (showEstimates && supplyFmaxKHz > 0 && supplyFminKHz > supplyFmaxKHz)
        {
            throw regenError("Supply min frequency must be below the max frequency.", ["supplyFmin"]);
        }
        var uniformBandPct = (definition.uniformBand is number) ? definition.uniformBand : 10;
        var targetZone = (definition.targetZone is ValueWithUnits) ? definition.targetZone : 10 * millimeter;

        // Thermal estimate inputs
        var thermalEst = addSample && (definition.thermalEst == true);
        var thermTargetC = (definition.thermTarget is number) ? definition.thermTarget : 1000;
        var thermAmbC = (definition.thermAmbient is number) ? definition.thermAmbient : 20;
        var thermTolPct = (definition.thermTol is number) ? definition.thermTol : 2;
        var thermEps = (definition.thermEmissivity is number) ? definition.thermEmissivity : 0.8;
        var thermInsT = (definition.thermInsT is ValueWithUnits) ? definition.thermInsT / meter : 0;
        var thermInsK = (definition.thermInsK is number) ? definition.thermInsK : 0.15;
        var thermInsEnds = (definition.thermInsEnds == true);
        var thermExtraW = (definition.thermExtraW is number) ? definition.thermExtraW : 0;
        if (thermalEst && thermTargetC <= thermAmbC)
        {
            throw regenError("Target temperature must be above the start / ambient temperature.", ["thermTarget"]);
        }

        // =========================================================================
        // COORDINATE SYSTEM PIVOT (Automatically counters the 180 degree flip)
        // =========================================================================
        var rotationAngle = (definition.rotationAngle is ValueWithUnits) ? definition.rotationAngle : 0 * degree;
        var finalRotation = rotationAngle;

        // If CCW is selected, apply a 180-degree correction to keep the leads aligned
        if (reverseDirection) {
            finalRotation += 180 * degree;
        }

        if (finalRotation != 0 * degree) {
            var rotTransform = rotationAround(line(vector(0, 0, 0) * meter, zAxis), finalRotation);
            var xLine = line(vector(0, 0, 0) * meter, xAxis);
            var yLine = line(vector(0, 0, 0) * meter, yAxis);

            xLine = rotTransform * xLine;
            yLine = rotTransform * yLine;

            xAxis = xLine.direction;
            yAxis = yLine.direction;
        }

        var pathRadius = (coilId / 2) + (tubeOd / 2);
        var revs = turns + 0.5;

        // =========================================================================
        // PITCH CALCULATION (GAP MODE vs. LENGTH MODE)
        // Gap mode:    pitch = tubeL + gap
        // Length mode: Coil Length = overall axial extent, outer edge of first
        //              turn to outer edge of last turn = revs * pitch + tubeL.
        //              The gap between winds is whatever is left: pitch - tubeL.
        // =========================================================================
        var pitch;
        if (useGap)
        {
            pitch = tubeL + gap;
        }
        else
        {
            pitch = (coilLength - tubeL) / revs;
            if (pitch < tubeL * 0.9999)
            {
                var minLength = (revs + 1) * tubeL;
                throw regenError("Coil length too short: the turns would overlap. Minimum length for this tube and turn count is " ~ roundToPrecision(minLength / millimeter, 1) ~ " mm.", ["coilLength"]);
            }
        }

        // Sleeves on neighbouring turns must not overlap (the sleeve sweep would intersect itself)
        if (addSleeve && pitch < (tubeL + 2 * sleeveThickness) * 0.9999)
        {
            throw regenError("Sleeves on neighbouring turns overlap: pitch " ~ roundToPrecision(pitch / millimeter, 2) ~
                             " mm is less than tube + 2 x sleeve (" ~ roundToPrecision((tubeL + 2 * sleeveThickness) / millimeter, 2) ~
                             " mm). Lengthen the coil, use fewer turns or a thinner sleeve.", ["sleeveThickness"]);
        }

        // Calculate total coil height to establish centering offset
        var totalHeight = revs * pitch;
        var centeredAxisStart = origin - zAxis * (totalHeight / 2.0);

        // Cubic Bezier math engine
        var sampleBezier = function(p0, p1, p2, p3, t) {
            var mt = 1.0 - t;
            return p0 * (mt * mt * mt) + p1 * (3.0 * mt * mt * t) + p2 * (3.0 * mt * t * t) + p3 * (t * t * t);
        };

        // =========================================================================
        // 4. CENTERED ROTATED HELIX GENERATION
        // =========================================================================
        var helixId = id + "helix";
        opHelix(context, helixId, {
            "direction" : zAxis,
            "axisStart" : centeredAxisStart,
            "startPoint" : centeredAxisStart + (xAxis * pathRadius),
            "interval" : [0, revs],
            "clockwise" : !reverseDirection,
            "helicalPitch" : pitch,
            "spiralPitch" : 0 * meter
        });
        var helixQuery = qCreatedBy(helixId, EntityType.EDGE);

        var startTarget = evEdgeTangentLine(context, { "edge" : helixQuery, "parameter" : 0.0 });
        var endTarget = evEdgeTangentLine(context, { "edge" : helixQuery, "parameter" : 1.0 });

        // Isolate horizontal tracing directions to make paths completely direction-agnostic
        var startHorizontalDir = normalize(startTarget.direction - zAxis * dot(zAxis, startTarget.direction));
        var endHorizontalDir = normalize(endTarget.direction - zAxis * dot(zAxis, endTarget.direction));

        var inletSlope = dot(zAxis, startTarget.direction);
        var outletSlope = dot(zAxis, endTarget.direction);

        // =========================================================================
        // 5. INLET SIDE SMOOTH RAMP
        // =========================================================================
        var ip0 = startTarget.origin;
        var ip1 = ip0 - startTarget.direction * (bendRadius * 0.5);
        var ip3 = ip0 - startHorizontalDir * bendRadius - zAxis * (inletSlope * bendRadius * 0.5);
        var ip2 = ip3 + startHorizontalDir * (bendRadius * 0.5);

        var inletCurvePts = [];
        for (var i = 0; i <= 5; i += 1) {
            inletCurvePts = append(inletCurvePts, sampleBezier(ip0, ip1, ip2, ip3, i / 5.0));
        }

        var inletCurveId = id + "inletCurve";
        opFitSpline(context, inletCurveId, { "points" : inletCurvePts });
        var inletCurveQuery = qCreatedBy(inletCurveId, EntityType.EDGE);


        // =========================================================================
        // 6. OUTLET SIDE SMOOTH RAMP
        // =========================================================================
        var op0 = endTarget.origin;
        var op1 = op0 + endTarget.direction * (bendRadius * 0.5);
        var op3 = op0 + endHorizontalDir * bendRadius + zAxis * (outletSlope * bendRadius * 0.5);
        var op2 = op3 - endHorizontalDir * (bendRadius * 0.5);

        var outletCurvePts = [];
        for (var i = 0; i <= 5; i += 1) {
            outletCurvePts = append(outletCurvePts, sampleBezier(op0, op1, op2, op3, i / 5.0));
        }

        var outletCurveId = id + "outletCurve";
        opFitSpline(context, outletCurveId, { "points" : outletCurvePts });
        var outletCurveQuery = qCreatedBy(outletCurveId, EntityType.EDGE);

        // =========================================================================
        // 6b. LEAD ROUTING
        // Default: each lead runs straight out from its tangent bend (the leads
        //   end up a coil diameter apart, at different heights).
        // "Bring leads together": each lead runs straight out until it clears
        //   the coil, then an S-bend (cubic Bezier, tangent at both ends) moves
        //   it to a closely spaced parallel pair, then a straight of "Lead
        //   length". Side by side = both at the coil's mid-height, spaced across;
        //   Stacked = both on the coil's center plane, one above the other.
        //   Needs a whole number of turns so both leads leave in the same direction.
        // =========================================================================
        var leadsTogether = (definition.leadsTogether == true);
        var leadJoin = (definition.leadJoin is CoilLeadJoin) ? definition.leadJoin : CoilLeadJoin.SIDE_BY_SIDE;
        var leadCtc = (definition.leadCtc is ValueWithUnits) ? definition.leadCtc : 13 * millimeter;   // pair spacing, center to center
        var sleeveExtra = addSleeve ? 2 * sleeveThickness : 0 * meter;

        var inletStartPoint;            // far end of the inlet lead (sweep starts here)
        var outletEndPoint;             // far end of the outlet lead
        var inletLeadEdges;             // all inlet lead edges except the tangent bend
        var outletLeadEdges;
        var inletRouteEdges = qNothing();    // clear + S-bend (shared with the sleeve)
        var outletRouteEdges = qNothing();
        var inletJoinPoint;             // start of the final straight
        var outletJoinPoint;
        var tgDir = -startHorizontalDir;     // lead direction (out from the coil)
        var tgSpacing = 0;              // final center-to-center spacing (m)
        var tgFinalLen = 0;             // final straight length (m)
        var tgJoin = 0;                 // gap from the coil OD to the outside of the paired leads (m)
        var tgWrap = false;             // leads wrapped around the coil (short join distance)
        var hugMode = false;            // wrap mode: each lead is one spline that hugs the coil
        var inletBendEdges = inletCurveQuery;     // tangent bends (replaced in wrap mode)
        var outletBendEdges = outletCurveQuery;
        var hugInletPts = [];           // wrap-mode lead points, helix end -> join (for the sleeve)
        var hugOutletPts = [];
        var extraCleanup = [];          // construction edges not in the sweep path
        var wrapAround = leadsTogether && (definition.wrapAround == true);   // continue around the coil, then straight out
        var oneSpline = false;          // each lead is one spline from the helix end (wrap or around-coil)
        var tgExtRevs = 0;              // extra coil revolutions added by continuing around (both leads)
        var tgExtLen = 0;               // length of that extra winding (m, both leads)
        var tgAround = false;
        var leadRouteText = leadsTogether ? (wrapAround ? "continue around" : "S-bend") : "straight leads";   // for the error message
        // Lead paths for the inductance estimate (finite-length model): each array
        // runs from where the lead is heading straight out along tgDir to its far
        // end. Wrap mode's curved part around the coil is estimated separately.
        var pairPtsIn = [];
        var pairPtsOut = [];
        var hugCurvedLen = 0;           // wrap mode: curved length per lead (m)
        var hugCurvedSpacing = 0;       // wrap mode: mean spacing along the curved part (m)

        if (!leadsTogether)
        {
            var inletLineId = id + "inletLine";
            inletStartPoint = ip3 - startHorizontalDir * (leadLength - bendRadius);
            opFitSpline(context, inletLineId, { "points" : [inletStartPoint, ip3] });
            inletLeadEdges = qCreatedBy(inletLineId, EntityType.EDGE);

            var outletLineId = id + "outletLine";
            outletEndPoint = op3 + endHorizontalDir * (leadLength - bendRadius);
            opFitSpline(context, outletLineId, { "points" : [op3, outletEndPoint] });
            outletLeadEdges = qCreatedBy(outletLineId, EntityType.EDGE);

            pairPtsIn = [ip0, ip3, inletStartPoint];
            pairPtsOut = [op0, op3, outletEndPoint];
        }
        else
        {
            if (abs(turns - round(turns)) > 1e-6)
            {
                throw regenError("Bring leads together needs a whole number of turns (so both leads leave on the same side).", ["turns"]);
            }

            // Pair spacing (center to center); must be at least one tube + sleeves or the leads overlap
            var minCtc = ((leadJoin == CoilLeadJoin.STACKED) ? tubeL : tubeW) + sleeveExtra;
            if (leadCtc < minCtc * 0.9999)
            {
                throw regenError("Lead spacing (center to center) must be at least " ~ roundToPrecision(minCtc / millimeter, 2) ~
                                 " mm (tube + sleeve) or the leads overlap.", ["leadCtc"]);
            }
            var cSpacing = leadCtc;
            var yClear = pathRadius + max(tubeL, tubeW) + sleeveExtra + 1 * millimeter;   // distance from the axis where a lead clears the turns
            var finalLen = max(leadLength, 1 * millimeter);

            // Coil outer edge (incl. sleeve) measured from the axis, lead outer
            // radius (incl. sleeve), and the optional join distance: the gap from
            // the coil OD to the outside of the paired leads (0 = as tight as possible)
            var coilOuter = pathRadius + tubeW / 2 + (addSleeve ? sleeveThickness : 0 * meter);
            var leadOuterR = max(tubeL, tubeW) / 2 + (addSleeve ? sleeveThickness : 0 * meter);
            var wrapR = coilOuter + leadOuterR + 0.5 * millimeter;   // a lead centerline outside this clears every turn
            var setJoin = (definition.setJoinDist == true);
            var joinDist = (definition.joinDist is ValueWithUnits) ? definition.joinDist : 60 * millimeter;
            var smoothStep = function(t) { return t * t * (3 - 2 * t); };

            // Height-change profile shared by the lead routes: two arcs of radius r,
            // with a middle straight leaning "S-bend lean" from vertical when the
            // drop h is more than the arcs give. Returns the arc angle, middle
            // length, horizontal length, and points [x, y] (m, y = 0 -> h) about
            // 1 mm apart along the curve (so a vertical middle is sampled too).
            var leanRad = (definition.sBendLean is ValueWithUnits) ? definition.sBendLean / radian : 0;
            leanRad = min(max(leanRad, isRectangular ? 10 * PI / 180 : 0), 89 * PI / 180);   // rectangular: >= 10 deg (a vertical run is parallel to the locked axis)
            var climbProfile = function(hM, rM) {
                var thMx = PI / 2 - leanRad;
                var hArc = 2 * rM * (1 - cos(thMx * radian));
                var th = (hM >= hArc) ? thMx : acos(1 - hM / (2 * rM)) / radian;
                var mid = (hM >= hArc) ? (hM - hArc) / sin(th * radian) : 0;
                var xLen = 2 * rM * sin(th * radian) + mid * cos(th * radian);
                var pp = [[0, 0]];
                if (hM > 1e-9)
                {
                    var nArc = max(8, ceil(rM * th / 0.001));
                    for (var k = 1; k <= nArc; k += 1)
                    {
                        var ph = th * k / nArc;
                        pp = append(pp, [rM * sin(ph * radian), rM * (1 - cos(ph * radian))]);
                    }
                    var x1 = rM * sin(th * radian);
                    var y1 = rM * (1 - cos(th * radian));
                    if (mid > 0.0005)
                    {
                        var nMid = max(1, ceil(mid / 0.002));
                        for (var k = 1; k <= nMid; k += 1)
                        {
                            var mk = mid * k / nMid;
                            pp = append(pp, [x1 + mk * cos(th * radian), y1 + mk * sin(th * radian)]);
                        }
                    }
                    for (var k = nArc - 1; k >= 0; k -= 1)
                    {
                        var ph = th * k / nArc;
                        pp = append(pp, [xLen - rM * sin(ph * radian), hM - rM * (1 - cos(ph * radian))]);
                    }
                }
                return { "th" : th, "mid" : mid, "len" : xLen, "pts" : pp };
            };

            // Pair targets: across position (inX / outX) and height (inZ / outZ).
            // Own heights: each lead stays at the height it leaves the coil at.
            var ownH = (leadJoin == CoilLeadJoin.OWN_HEIGHTS);
            var inX = (leadJoin == CoilLeadJoin.STACKED) ? 0 * meter : cSpacing / 2;
            var outX = -inX;
            var inZ = (leadJoin == CoilLeadJoin.STACKED) ? -cSpacing / 2 : 0 * meter;
            var outZ = -inZ;
            if (ownH)
            {
                inZ = dot(ip3 - origin, zAxis);
                outZ = dot(op3 - origin, zAxis);
            }

            // S-bend geometry for one lead (from its tangent-bend end pStart)
            var sBendGeom = function(pStart, latX, vertZ) {
                var along = dot(pStart - origin, tgDir);
                var clearLen = max(yClear - along, 0 * meter);
                var pA = pStart + tgDir * clearLen;
                var dX = latX - dot(pA - origin, xAxis);
                var dZ = vertZ - dot(pA - origin, zAxis);
                var shift = sqrt(dX * dX + dZ * dZ);
                var jogLen = max(max(1.5 * shift, 3 * bendRadius), 1 * millimeter);
                var fits = true;
                if (setJoin)
                {
                    // S-bend must be at least as long as its sideways shift (and 3 bend radii)
                    var minJog = max(max(shift, 3 * bendRadius), 1 * millimeter);
                    jogLen = coilOuter + leadOuterR + joinDist - dot(pA - origin, tgDir);
                    fits = (jogLen >= minJog);
                }
                return { "clearLen" : clearLen, "pA" : pA, "dX" : dX, "dZ" : dZ, "jogLen" : jogLen, "fits" : fits };
            };

            var gIn = sBendGeom(ip3, inX, inZ);
            var gOut = sBendGeom(op3, outX, outZ);
            hugMode = !wrapAround;   // always hug the coil, no straight sections (unticked = tightest join that fits)
            oneSpline = wrapAround || hugMode;

            if (wrapAround)
            {
                // ----- Continue around the coil, then straight out -----
                // Each lead keeps winding at the coil radius and pitch (the bottom
                // lead continues downward, the top lead upward, so it is just a
                // little more coil) until it has come round to the exit side, then
                // turns outward with the bend radius, runs straight out until it
                // clears every turn, makes a vertical S-bend to the pair height
                // (none for Own heights), and runs straight to the connection.
                // Helix end -> far end is one fitted spline per lead (no seams).
                var waLead = function(p0, outDir, latX, vertZ) {
                    var u0 = dot(p0 - origin, xAxis);
                    var su = (u0 >= 0 * meter) ? 1 : -1;
                    var a = abs(u0);
                    var wStart = dot(p0 - origin, zAxis);
                    var zsW = dot(outDir, zAxis);
                    var mW = zsW / sqrt(max(1 - zsW * zsW, 1e-9));     // helix height change per horizontal length
                    var uT = su * latX;
                    var rR = max(bendRadius, 0.5 * millimeter);
                    var cosE = (uT + rR) / (a + rR);
                    if (cosE >= 1 || cosE <= -1)
                    {
                        throw regenError("Leads can not come round to this arrangement. Try a smaller lead spacing or bend radius.", ["leadCtc"]);
                    }
                    var psiE = acos(cosE) / radian;           // extra wrap angle (rad)
                    var extLen = a * psiE;                    // horizontal length of the extra wrap
                    var lenT = rR * psiE;                     // horizontal length of the outward turn
                    var cE = a + rR;
                    var toWorld = function(uu, vv, ww) { return origin + xAxis * (su * uu) + tgDir * vv + zAxis * ww; };

                    var pts = [];
                    // Extra wrap: same radius and pitch as the coil
                    var nE = max(8, ceil(psiE / 0.08));
                    for (var k = 0; k <= nE; k += 1)
                    {
                        var psiK = psiE * k / nE;
                        pts = append(pts, toWorld(a * cos(psiK * radian), a * sin(psiK * radian), wStart + mW * a * psiK));
                    }
                    var wE = wStart + mW * extLen;
                    // Outward turn; the helix slope levels off over the turn
                    var nTn = 10;
                    for (var k = 1; k <= nTn; k += 1)
                    {
                        var betaK = psiE * k / nTn;
                        var gamK = psiE + PI - betaK;
                        var sT = rR * betaK;
                        pts = append(pts, toWorld(cE * cos(psiE * radian) + rR * cos(gamK * radian),
                                                  cE * sin(psiE * radian) + rR * sin(gamK * radian),
                                                  wE + mW * sT * (1 - sT / (2 * lenT))));
                    }
                    var wOwn = wE + mW * lenT / 2;
                    var vT = cE * sin(psiE * radian);         // along tgDir after the turn
                    var wT = ownH ? wOwn : vertZ;
                    var dW = wT - wOwn;
                    var hasS = abs(dW) > 1e-6 * meter;

                    // Straight out until clear (only needed before a height change), then the S-bend
                    var vClr = vT;
                    if (hasS && wrapR > abs(uT))
                    {
                        vClr = max(vT, sqrt(wrapR * wrapR - uT * uT));
                    }
                    // Vertical S-bend = two arcs at the bend radius (plus a vertical
                    // straight between them when the drop is more than 2 x bend radius).
                    // Shortest S that respects the bend radius:
                    //   drop >= 2r : length 2r (two 90 deg arcs + vertical straight)
                    //   drop <  2r : length sqrt(drop * (4r - drop)), arcs of acos(1 - drop / 2r)
                    // "S-bend lean from vertical" (alpha): the middle straight leans
                    // alpha from vertical, so each arc turns at most 90 deg - alpha.
                    var hS = abs(dW);
                    var leanA = (definition.sBendLean is ValueWithUnits) ? definition.sBendLean / radian : 0;
                    leanA = min(max(leanA, isRectangular ? 10 * PI / 180 : 0), 89 * PI / 180);
                    var thMax = PI / 2 - leanA;                                               // max arc angle (rad)
                    var hArcs = 2 * rR * (1 - cos(thMax * radian));                           // drop of the two full arcs
                    var thS = (hS >= hArcs) ? thMax : acos(1 - hS / (2 * rR)) / radian;       // arc angle (rad)
                    var midS = (hS >= hArcs) ? (hS - hArcs) / sin(thS * radian) : 0 * meter;  // middle straight length
                    var sLen = hasS ? 2 * rR * sin(thS * radian) + midS * cos(thS * radian) : 0 * meter;   // horizontal length of the S
                    if (setJoin)
                    {
                        var vReq = coilOuter + leadOuterR + joinDist;
                        if (hasS)
                        {
                            if (vReq < vClr + sLen - 1e-6 * meter)
                            {
                                throw regenError("Join distance too short to bring the leads to the same height. Minimum is about " ~
                                                 roundToPrecision((vClr + sLen - coilOuter - leadOuterR) / millimeter + 0.5, 0) ~
                                                 " mm from the coil OD (smaller bend radius, or use Own heights).", ["joinDist"]);
                            }
                            vClr = vReq - sLen;                // longer join: more straight before the S-bend
                        }
                        else
                        {
                            vClr = max(vT, vReq);              // no S-bend: join where requested (not closer than the turn)
                        }
                    }

                    // Straight section (collinear points)
                    // ~2 mm point spacing; a very short straight gets no extra points
                    // (tightly clustered points made the fitted spline ripple)
                    var nSt = max(1, ceil((vClr - vT) / (2 * millimeter)));
                    if (vClr - vT > 0.5 * millimeter)
                    {
                        for (var k = 1; k <= nSt; k += 1)
                        {
                            pts = append(pts, toWorld(uT, vT + (vClr - vT) * k / nSt, wOwn));
                        }
                    }
                    // Vertical S-bend to the pair height
                    if (hasS)
                    {
                        var sgS = (dW > 0 * meter) ? 1 : -1;
                        var nA = max(8, ceil(rR * thS / (1 * millimeter)));   // ~1 mm along each arc
                        // First arc: leaves level, bends toward the pair height
                        for (var k = 1; k <= nA; k += 1)
                        {
                            var ph = thS * k / nA;
                            pts = append(pts, toWorld(uT, vClr + rR * sin(ph * radian), wOwn + sgS * rR * (1 - cos(ph * radian))));
                        }
                        // Middle straight, leaning "S-bend lean" from vertical (only
                        // when the drop is more than the two arcs give)
                        if (midS > 0.5 * millimeter)
                        {
                            var nV = max(1, ceil(midS / (2 * millimeter)));
                            var v1 = vClr + rR * sin(thS * radian);
                            var w1 = wOwn + sgS * rR * (1 - cos(thS * radian));
                            for (var k = 1; k <= nV; k += 1)
                            {
                                var mk = midS * k / nV;
                                pts = append(pts, toWorld(uT, v1 + mk * cos(thS * radian), w1 + sgS * mk * sin(thS * radian)));
                            }
                        }
                        // Second arc: levels off at the pair height
                        for (var k = nA - 1; k >= 0; k -= 1)
                        {
                            var ph = thS * k / nA;
                            pts = append(pts, toWorld(uT, vClr + sLen - rR * sin(ph * radian), wT - sgS * rR * (1 - cos(ph * radian))));
                        }
                    }
                    var pB = pts[size(pts) - 1];
                    return { "pts" : pts, "B" : pB,
                             "extLen" : extLen * sqrt(1 + mW * mW) / meter,
                             "extRevs" : psiE / (2 * PI),
                             "iPair" : nE,                    // end of the extra winding (outward turn starts here)
                             "join" : (dot(pB - origin, tgDir) - coilOuter - leadOuterR) / meter };
                };

                // One spline per lead: helix end -> join -> final straight
                var waSpline = function(pts, startDir, endPoint, splineId) {
                    var all = pts;
                    var last = pts[size(pts) - 1];
                    // Straight part: evenly spaced points (~3 mm) so the fit stays
                    // straight and valid for long leads (4 points failed at 559 mm)
                    var nSt = min(max(4, ceil(norm(endPoint - last) / (3 * millimeter))), 300);
                    for (var k = 1; k <= nSt; k += 1)
                    {
                        all = append(all, last + (endPoint - last) * (k / nSt));
                    }
                    var polyLen = 0 * meter;
                    for (var k = 1; k < size(all); k += 1) { polyLen += norm(all[k] - all[k - 1]); }
                    // Resample to even spacing (~1.5 mm, max 600 points): uneven point
                    // spacing (0.45 mm straight next to ~7 mm S-bend steps) made the fit
                    // ripple tighter than the tube radius -> SWEEP_SELF_INT
                    var nRs = min(max(ceil(polyLen / (1.5 * millimeter)), 8), 600);
                    var even = [all[0]];
                    var segI = 1;
                    var accLen = 0 * meter;            // path length up to all[segI - 1]
                    for (var k = 1; k < nRs; k += 1)
                    {
                        var target = polyLen * k / nRs;
                        while (segI < size(all) - 1 && accLen + norm(all[segI] - all[segI - 1]) < target)
                        {
                            accLen += norm(all[segI] - all[segI - 1]);
                            segI += 1;
                        }
                        var segLen = norm(all[segI] - all[segI - 1]);
                        var tt = (segLen > 1e-9 * meter) ? (target - accLen) / segLen : 0;
                        even = append(even, all[segI - 1] + (all[segI] - all[segI - 1]) * min(max(tt, 0), 1));
                    }
                    even = append(even, all[size(all) - 1]);
                    opFitSpline(context, splineId, {
                        "points" : even,
                        "startDerivative" : startDir * polyLen,
                        "endDerivative" : tgDir * polyLen
                    });
                    // Check the tightest bend before sweeping (clear message instead of SWEEP_SELF_INT)
                    var edgeQ = qCreatedBy(splineId, EntityType.EDGE);
                    var minBendR = 1 * meter;
                    var nChk = 400;
                    for (var k = 0; k <= nChk; k += 1)
                    {
                        var cv = evEdgeCurvature(context, { "edge" : edgeQ, "parameter" : k / nChk }).curvature;
                        if (cv * meter > 1e-9) { minBendR = min(minBendR, 1 / cv); }
                    }
                    var tubeR = max(tubeL, tubeW) / 2;
                    if (minBendR < tubeR * 1.05)
                    {
                        var leadName = (splineId == id + "inletLine") ? "inlet" : "outlet";
                        throw regenError("Lead path bends too tightly to sweep (" ~ leadName ~ " lead, tightest bend " ~
                                         roundToPrecision(minBendR / millimeter, 2) ~ " mm, tube radius " ~
                                         roundToPrecision(tubeR / millimeter, 2) ~ " mm). Try a larger lead spacing or bend radius.",
                                         ["leadCtc"], edgeQ);
                    }
                    return edgeQ;
                };

                var waIn = waLead(ip0, -startTarget.direction, inX, inZ);
                var waOut = waLead(op0, endTarget.direction, outX, outZ);
                hugInletPts = waIn.pts;
                hugOutletPts = waOut.pts;
                inletJoinPoint = waIn.B;
                outletJoinPoint = waOut.B;

                inletStartPoint = inletJoinPoint + tgDir * finalLen;
                outletEndPoint = outletJoinPoint + tgDir * finalLen;
                inletLeadEdges = waSpline(waIn.pts, -startTarget.direction, inletStartPoint, id + "inletLine");
                outletLeadEdges = waSpline(waOut.pts, endTarget.direction, outletEndPoint, id + "outletLine");

                // Lead paths for the inductance estimate: from the end of the extra
                // winding (counted as coil turns), incl. the outward turn
                for (var k = waIn.iPair; k < size(waIn.pts); k += 1) { pairPtsIn = append(pairPtsIn, waIn.pts[k]); }
                pairPtsIn = append(pairPtsIn, inletStartPoint);
                for (var k = waOut.iPair; k < size(waOut.pts); k += 1) { pairPtsOut = append(pairPtsOut, waOut.pts[k]); }
                pairPtsOut = append(pairPtsOut, outletEndPoint);

                inletBendEdges = qNothing();
                outletBendEdges = qNothing();
                extraCleanup = [inletCurveQuery, outletCurveQuery];

                tgExtRevs = waIn.extRevs + waOut.extRevs;
                tgExtLen = waIn.extLen + waOut.extLen;
                tgJoin = max(waIn.join, waOut.join);
                tgAround = true;
            }
            else if (!hugMode)
            {
                // ----- S-bend: straight out until clear, then a tangent S-bend -----
                var routeLead = function(pStart, g, tag) {
                    var pB = g.pA + tgDir * g.jogLen + xAxis * g.dX + zAxis * g.dZ;
                    var jogPts = [];
                    for (var k = 0; k <= 10; k += 1)
                    {
                        jogPts = append(jogPts, sampleBezier(g.pA, g.pA + tgDir * (g.jogLen / 2), pB - tgDir * (g.jogLen / 2), pB, k / 10.0));
                    }
                    opFitSpline(context, id + (tag ~ "Jog"), { "points" : jogPts });
                    var edges = [qCreatedBy(id + (tag ~ "Jog"), EntityType.EDGE)];
                    if (g.clearLen > 1e-6 * meter)
                    {
                        opFitSpline(context, id + (tag ~ "Clear"), { "points" : [pStart, g.pA] });
                        edges = append(edges, qCreatedBy(id + (tag ~ "Clear"), EntityType.EDGE));
                    }
                    var nearLen = g.clearLen + evLength(context, { "entities" : qCreatedBy(id + (tag ~ "Jog"), EntityType.EDGE) });
                    // Path points for the inductance estimate: bend end -> (clear) -> S-bend
                    var legPts = [pStart];
                    if (g.clearLen > 1e-6 * meter) { legPts = append(legPts, g.pA); }
                    for (var k = 1; k <= 10; k += 1) { legPts = append(legPts, jogPts[k]); }
                    return { "edges" : qUnion(edges), "B" : pB, "near" : nearLen / meter, "pts" : legPts,
                             "join" : (dot(pB - origin, tgDir) - coilOuter - leadOuterR) / meter };
                };

                var inRoute = routeLead(ip3, gIn, "inletRoute");
                var outRoute = routeLead(op3, gOut, "outletRoute");

                inletJoinPoint = inRoute.B;
                outletJoinPoint = outRoute.B;
                inletRouteEdges = inRoute.edges;
                outletRouteEdges = outRoute.edges;

                inletStartPoint = inletJoinPoint + tgDir * finalLen;
                opFitSpline(context, id + "inletLine", { "points" : [inletStartPoint, inletJoinPoint] });
                inletLeadEdges = qUnion([qCreatedBy(id + "inletLine", EntityType.EDGE), inletRouteEdges]);

                outletEndPoint = outletJoinPoint + tgDir * finalLen;
                opFitSpline(context, id + "outletLine", { "points" : [outletJoinPoint, outletEndPoint] });
                outletLeadEdges = qUnion([qCreatedBy(id + "outletLine", EntityType.EDGE), outletRouteEdges]);

                pairPtsIn = concatenateArrays([[ip0], inRoute.pts, [inletStartPoint]]);
                pairPtsOut = concatenateArrays([[op0], outRoute.pts, [outletEndPoint]]);

                tgJoin = max(inRoute.join, outRoute.join);
            }
            else
            {
                // ----- Wrap: hug the coil, one smooth spline per lead -----
                // From the helix end the lead keeps the coil's curvature and
                // spirals smoothly out to the clearance radius rW (no straight
                // run), follows rW toward the centre, then turns outward with the
                // bend radius into the pair. The small helix slope is levelled
                // over one bend radius. The height change to the pair happens only
                // once the lead is at rW (outside every turn). Each lead (helix end
                // to far end, incl. the final straight) is one fitted spline, so
                // the sweep has no seams along the lead.
                var hugLead = function(p0, outDir, latX, vertZ) {
                    var u0 = dot(p0 - origin, xAxis);
                    var su = (u0 >= 0 * meter) ? 1 : -1;
                    var a = abs(u0);
                    var wStart = dot(p0 - origin, zAxis);
                    var zs = dot(outDir, zAxis);
                    var mH = zs / sqrt(max(1 - zs * zs, 1e-9));        // height change per horizontal length
                    var lb = max(bendRadius, 1 * millimeter);         // levelling length
                    var wFlat = wStart + mH * lb / 2;
                    var uT = su * latX;
                    var vJ = setJoin ? coilOuter + leadOuterR + joinDist : 0 * meter;   // requested join (0 = tightest)
                    var rW = wrapR;                                    // hug the coil (just clear of every turn)
                    var dR = rW - a;
                    var toWorld = function(uu, vv, ww) { return origin + xAxis * (su * uu) + tgDir * vv + zAxis * ww; };

                    // Every bend must be at least rMin, or the swept tube (and its
                    // sleeve) intersects itself.
                    var rMin = 1.25 * leadOuterR;
                    var rMinM = rMin / meter;

                    // Phase A (spiral out to rW) must be long enough for its bend:
                    //   curvature ~ (rW + 6 dR / psi1^2) / rW^2 <= 1 / rMin
                    var kRoom = rW / rMin - 1;
                    if (kRoom <= 0)
                    {
                        throw regenError("Coil is too small for the leads to hug it with this tube and sleeve. Untick Set join distance (S-bend leads).", ["setJoinDist"]);
                    }
                    // Both ends of the spiral: the inward bend at rW (above) and the
                    // outward curl right at the helix end, (6 dR / psi1^2 - a) / a^2 <= 1 / rMin
                    // (the curl folded the tube on a 25.4 mm ID coil)
                    var psiReq = max(sqrt(6 * dR / (rW * kRoom)), sqrt(6 * dR / (a * (1 + a / rMin))));
                    // Tangent exit (default for all coils): one smooth S-curve per lead.
                    // It leaves the helix end tangentially (no outward curl, so it never
                    // goes out and back in), curves in to the pair line, stays level
                    // until it is outside every turn, then climbs to the pair height.
                    // Lengthened until every bend is >= rMin and the climb has room.
                    // (The spiral "hug" route below is kept but no longer used.)
                    var tangentExit = true;
                    if (tangentExit || sqrt(6 * dR / (a * (1 + a / rMin))) > 0.6)
                    {
                        var nF = 48;
                        var lbM = lb / meter;
                        var wrapM = wrapR / meter;
                        var dWt = (vertZ - wFlat) / meter;
                        // Climb = shared two-arc profile at the bend radius (>= rMin), with the S-bend lean
                        var profT = climbProfile(abs(dWt), max(bendRadius, rMin) / meter);
                        var needT = (abs(dWt) > 1e-9) ? profT.len : 0;
                        var vEnd = max(vJ, coilOuter + leadOuterR) / meter;
                        var ptsF = [];
                        var okF = false;
                        for (var it = 0; it < 200 && !okF; it += 1)
                        {
                            var q0 = vector(a / meter, 0);
                            var q3 = vector(uT / meter, vEnd);
                            var plan = [];
                            for (var k = 0; k <= nF; k += 1)
                            {
                                plan = append(plan, sampleBezier(q0, q0 + vector(0, vEnd / 3), q3 - vector(0, vEnd / 3), q3, k / nF));
                            }
                            var sAcc = [0];
                            for (var k = 1; k <= nF; k += 1) { sAcc = append(sAcc, sAcc[k - 1] + norm(plan[k] - plan[k - 1])); }
                            var total = sAcc[nF];
                            var iC = nF;
                            var found = false;
                            for (var k = 0; k <= nF; k += 1)
                            {
                                if (!found && norm(plan[k]) >= wrapM && sAcc[k] >= lbM) { iC = k; found = true; }
                            }
                            var sC = sAcc[iC];
                            okF = (abs(dWt) < 1e-9 || (found && total - sC >= needT));
                            // plan-view bends
                            for (var k = 1; k < nF && okF; k += 1)
                            {
                                var e1 = plan[k] - plan[k - 1];
                                var e2 = plan[k + 1] - plan[k];
                                var turnA = acos(min(max(dot(e1, e2) / (norm(e1) * norm(e2)), -1), 1)) / radian;
                                if (turnA > 1e-9 && (norm(e1) + norm(e2)) / 2 / turnA < rMinM) { okF = false; }
                            }
                            if (okF)
                            {
                                ptsF = [];
                                var climbs = abs(dWt) > 1e-9;
                                // Set join distance: climb just before the join (lead runs out level at its own height first)
                                var sClimb = setJoin ? max(sC, total - profT.len) : sC;
                                var sgT = (dWt > 0) ? 1 : -1;
                                // Level part (incl. the helix-slope levelling) up to the clearance point
                                for (var k = 0; k <= nF; k += 1)
                                {
                                    var s = sAcc[k];
                                    if (!climbs || k <= iC || s < sClimb - 1e-9)
                                    {
                                        var wF = (s < lbM) ? wStart / meter + mH * s * (1 - s / (2 * lbM)) : wFlat / meter;
                                        ptsF = append(ptsF, toWorld(plan[k][0] * meter, plan[k][1] * meter, wF * meter));
                                    }
                                }
                                if (climbs)
                                {
                                    // Climb along the plan path: profile points placed by path length
                                    var planAt = function(sQ) {
                                        var j = 1;
                                        while (j < nF && sAcc[j] < sQ) { j += 1; }
                                        var segL = sAcc[j] - sAcc[j - 1];
                                        var tq = (segL > 1e-12) ? min(max((sQ - sAcc[j - 1]) / segL, 0), 1) : 0;
                                        return plan[j - 1] + (plan[j] - plan[j - 1]) * tq;
                                    };
                                    for (var k = 1; k < size(profT.pts); k += 1)
                                    {
                                        var pq = planAt(sClimb + profT.pts[k][0]);
                                        ptsF = append(ptsF, toWorld(pq[0] * meter, pq[1] * meter, (wFlat / meter + sgT * profT.pts[k][1]) * meter));
                                    }
                                    // Level at the pair height for the rest of the plan path
                                    for (var k = iC + 1; k <= nF; k += 1)
                                    {
                                        if (sAcc[k] > sClimb + profT.len + 0.0005)
                                        {
                                            ptsF = append(ptsF, toWorld(plan[k][0] * meter, plan[k][1] * meter, vertZ));
                                        }
                                    }
                                }
                            }
                            else
                            {
                                vEnd += 0.003;
                            }
                        }
                        if (!okF)
                        {
                            throw regenError("Leads can not be routed for this coil. Try a smaller lead spacing or a longer coil.", ["leadCtc"]);
                        }
                        var pBf = ptsF[size(ptsF) - 1];
                        return { "pts" : ptsF, "B" : pBf, "route" : "tangent S-curve", "join" : (dot(pBf - origin, tgDir) - coilOuter - leadOuterR) / meter };
                    }

                    // ...but must stop while the lead still has room to turn out to
                    // the pair line (x >= pair line + 2 rMin)
                    var psiCap = acos(min((uT + 2 * rMin) / rW, 1)) / radian;
                    var psi1 = max(min(min(max(3 * dR / a, 0.35), 1.2), psiCap), psiReq);
                    if (psi1 > 1.5)
                    {
                        throw regenError("Coil is too small for the leads to hug it with this tube and sleeve. Untick Set join distance (S-bend leads).", ["setJoinDist"]);
                    }

                    // Plan-view helpers (points in metres)
                    var planLen = function(plan) {
                        var len = 0;
                        for (var k = 1; k < size(plan); k += 1) { len += norm(plan[k] - plan[k - 1]); }
                        return len;
                    };
                    var bendOk = function(plan) {
                        for (var k = 1; k < size(plan) - 1; k += 1)
                        {
                            var e1 = plan[k] - plan[k - 1];
                            var e2 = plan[k + 1] - plan[k];
                            var n1 = norm(e1);
                            var n2 = norm(e2);
                            if (n1 > 1e-12 && n2 > 1e-12)
                            {
                                var turnA = acos(min(max(dot(e1, e2) / (n1 * n2), -1), 1)) / radian;
                                if (turnA > 1e-9 && (n1 + n2) / 2 / turnA < rMinM * 0.98) { return false; }
                            }
                        }
                        return true;
                    };
                    var staysOut = function(plan) {
                        for (var k = 0; k < size(plan); k += 1)
                        {
                            if (norm(plan[k]) < 0.999 * rW / meter) { return false; }
                        }
                        return true;
                    };
                    // Outward turn from the rW circle at angle psi with radius rT;
                    // ends heading along tgDir at x = (rW + rT) cos(psi) - rT
                    var turnPlan = function(psi, rT, n) {
                        var cc = rW + rT;
                        var plan = [];
                        for (var k = 1; k <= n; k += 1)
                        {
                            var beta = psi * k / n;
                            var gam = psi + PI - beta;
                            plan = append(plan, vector((cc * cos(psi * radian) + rT * cos(gam * radian)) / meter,
                                                       (cc * sin(psi * radian) + rT * sin(gam * radian)) / meter));
                        }
                        return plan;
                    };

                    var pts = [];
                    // Phase A: spiral out from the helix radius to rW (level);
                    // fine sampling so the fitted spline follows the start curl
                    var nA = 64;
                    for (var k = 0; k <= nA; k += 1)
                    {
                        var psiA = psi1 * k / nA;
                        var rho = a + dR * smoothStep(k / nA);
                        var sH = a * psiA;
                        var wA = (sH < lb) ? wStart + mH * sH * (1 - sH / (2 * lb)) : wFlat;
                        pts = append(pts, toWorld(rho * cos(psiA * radian), rho * sin(psiA * radian), wA));
                    }
                    var xA = rW * cos(psi1 * radian);
                    var startPlan = vector(rW * cos(psi1 * radian) / meter, rW * sin(psi1 * radian) / meter);

                    // ---- Wide route (longer join): right after the spiral, turn
                    // outward (radius rA) to run along tgDir with the leads still
                    // apart, then one smooth S-curve in to the pair at the join.
                    var tail = [];
                    var nTW = 12;
                    var nSW = 24;

                    // The climb to the pair height needs room: path >= 1.5 x the
                    // height change (and >= 3 rMin), or the tube folds on itself
                    var dWh = abs(vertZ - wFlat);
                    var needLen = (dWh > 1e-6 * meter) ? max(1.5 * dWh, 3 * rMin) / meter : 0;

                    // ---- Tight route: along rW, then the outward turn into the
                    // pair. Turn radius = bend radius, reduced if needed so the turn
                    // starts after the spiral (never below rMin).
                    var cNeed = cos(min(psi1 * 1.15, 1.55) * radian);
                    var rTmax = (cNeed * rW - uT) / (1 - cNeed);
                    var rT = min(bendRadius, rTmax);
                    if (rT >= rMin)
                    {
                        var psi2 = acos((uT + rT) / (rW + rT)) / radian;
                        var tight = [];
                        var nB = 24;
                        for (var k = 1; k <= nB; k += 1)
                        {
                            var psiB = psi1 + (psi2 - psi1) * k / nB;
                            tight = append(tight, vector(rW * cos(psiB * radian) / meter, rW * sin(psiB * radian) / meter));
                        }
                        tight = concatenateArrays([tight, turnPlan(psi2, rT, 12)]);
                        var vTight = tight[size(tight) - 1][1] * meter;
                        var lenTight = planLen(concatenateArrays([[startPlan], tight]));
                        if (vJ <= vTight + 1e-6 * meter)
                        {
                            // Not enough path for the climb: carry on along the pair
                            // line (leads already side by side) and finish the climb
                            // there, instead of switching to the wide route's S-jog
                            var extM = max(needLen - lenTight, 0);
                            if (extM > 1e-6)
                            {
                                var vT0 = vTight / meter;
                                var nExt = max(6, ceil(extM / 0.003));
                                for (var k = 1; k <= nExt; k += 1)
                                {
                                    tight = append(tight, vector(uT / meter, vT0 + extM * k / nExt));
                                }
                            }
                            tail = tight;
                        }
                    }

                    // ---- Wide route (longer join, or the tight route is too short
                    // for the climb): right after the spiral turn outward (radius
                    // rA) to run along tgDir with the leads still apart, then one
                    // smooth S-curve in to the pair. The S-curve is lengthened until
                    // the climb fits and every bend is >= rMin.
                    if (size(tail) == 0)
                    {
                        var rA = min(max(bendRadius, rMin), (xA - uT) / max(1 - cos(psi1 * radian), 1e-9));
                        if (rA >= rMin)
                        {
                            var arcW = turnPlan(psi1, rA, nTW);
                            var endW = arcW[nTW - 1];
                            var arcLen = planLen(concatenateArrays([[startPlan], arcW]));
                            var vEnd = max(vJ / meter, endW[1] + max(needLen - arcLen, 0.001));
                            var q3;
                            for (var it = 0; it < 200; it += 1)
                            {
                                var lS = vEnd - endW[1];
                                q3 = vector(uT / meter, vEnd);
                                var sPlan = [];
                                for (var k = 1; k <= nSW; k += 1)
                                {
                                    sPlan = append(sPlan, sampleBezier(endW, endW + vector(0, lS / 2), q3 - vector(0, lS / 2), q3, k / nSW));
                                }
                                var full = concatenateArrays([[startPlan], arcW, sPlan]);
                                if (bendOk(full) && staysOut(full))
                                {
                                    tail = concatenateArrays([arcW, sPlan]);
                                    break;
                                }
                                vEnd += 0.003;
                            }
                        }
                    }

                    if (size(tail) == 0)
                    {
                        throw regenError("Leads can not hug this coil at this lead spacing and size. Try a smaller lead spacing or bend radius.", ["leadCtc"]);
                    }

                    // Height change to the pair along the tail (outside every turn)
                    var lenT = planLen(concatenateArrays([[startPlan], tail]));
                    var sAcc = 0;
                    var prev = startPlan;
                    for (var k = 0; k < size(tail); k += 1)
                    {
                        sAcc += norm(tail[k] - prev);
                        prev = tail[k];
                        pts = append(pts, toWorld(tail[k][0] * meter, tail[k][1] * meter, wFlat + (vertZ - wFlat) * smoothStep(sAcc / lenT)));
                    }
                    var pB = pts[size(pts) - 1];
                    return { "pts" : pts, "B" : pB, "route" : "hug", "join" : (dot(pB - origin, tgDir) - coilOuter - leadOuterR) / meter };
                };

                // One spline per lead: helix end -> join -> final straight
                var makeLead = function(pts, startDir, endPoint, splineId) {
                    var all = pts;
                    var last = pts[size(pts) - 1];
                    // Straight part: evenly spaced points (~3 mm) so the fit stays
                    // straight and valid for long leads (4 points failed at 559 mm)
                    var nSt = min(max(4, ceil(norm(endPoint - last) / (3 * millimeter))), 300);
                    for (var k = 1; k <= nSt; k += 1)
                    {
                        all = append(all, last + (endPoint - last) * (k / nSt));
                    }
                    var polyLen = 0 * meter;
                    for (var k = 1; k < size(all); k += 1) { polyLen += norm(all[k] - all[k - 1]); }
                    // Resample to even spacing (~1.5 mm, max 600 points) so the fit
                    // does not ripple where 1 mm climb points meet the coarser plan points
                    var nRs = min(max(ceil(polyLen / (1.5 * millimeter)), 8), 600);
                    var even = [all[0]];
                    var segI = 1;
                    var accLen = 0 * meter;
                    for (var k = 1; k < nRs; k += 1)
                    {
                        var target = polyLen * k / nRs;
                        while (segI < size(all) - 1 && accLen + norm(all[segI] - all[segI - 1]) < target)
                        {
                            accLen += norm(all[segI] - all[segI - 1]);
                            segI += 1;
                        }
                        var segLen = norm(all[segI] - all[segI - 1]);
                        var tt = (segLen > 1e-9 * meter) ? (target - accLen) / segLen : 0;
                        even = append(even, all[segI - 1] + (all[segI] - all[segI - 1]) * min(max(tt, 0), 1));
                    }
                    even = append(even, all[size(all) - 1]);
                    opFitSpline(context, splineId, {
                        "points" : even,
                        "startDerivative" : startDir * polyLen,           // exact helix tangent at the joint
                        "endDerivative" : tgDir * polyLen
                    });
                    return qCreatedBy(splineId, EntityType.EDGE);
                };

                var hIn = hugLead(ip0, -startTarget.direction, inX, inZ);
                var hOut = hugLead(op0, endTarget.direction, outX, outZ);
                leadRouteText = hIn.route;
                hugInletPts = hIn.pts;
                hugOutletPts = hOut.pts;
                inletJoinPoint = hIn.B;
                outletJoinPoint = hOut.B;

                inletStartPoint = inletJoinPoint + tgDir * finalLen;
                outletEndPoint = outletJoinPoint + tgDir * finalLen;
                inletLeadEdges = makeLead(hIn.pts, -startTarget.direction, inletStartPoint, id + "inletLine");
                outletLeadEdges = makeLead(hOut.pts, endTarget.direction, outletEndPoint, id + "outletLine");

                // The tangent bends are replaced by the hug splines
                inletBendEdges = qNothing();
                outletBendEdges = qNothing();
                extraCleanup = [inletCurveQuery, outletCurveQuery];

                tgJoin = max(hIn.join, hOut.join);

                // Inductance estimate: the whole lead path (curve, climb, final
                // straight) goes into the 3-axis finite-filament pair model
                pairPtsIn = append(hIn.pts, inletStartPoint);
                pairPtsOut = append(hOut.pts, outletEndPoint);
            }

            tgSpacing = cSpacing / meter;
            tgWrap = hugMode;
            tgFinalLen = finalLen / meter;
        }

        // =========================================================================
        // 7. SKETCH PROFILE & COIL SWEEP
        // =========================================================================
        var unifiedPath = qUnion([inletLeadEdges, inletBendEdges, helixQuery, outletBendEdges, outletLeadEdges]);

        // Tube sweep helper (coil, sleeve and bore all use it).
        // Round tube: one sweep along the whole path.
        // Rectangular tube: the profile is locked to a direction so it does not
        // twist (the default frame flips at S-curve inflections -> SWEEP_FAILED).
        //  - Leads not brought together / "Continue around": one sweep locked to
        //    the coil axis (L side parallel to the axis, W radial on the helix).
        //  - Tangent S-curve leads: the helix is swept locked to the coil axis and
        //    each lead separately locked to the across direction (xAxis), then all
        //    three are joined. Locked to the axis, the near-vertical climb of the
        //    leads had the rectangle spin and flatten ("squashed" leads). At the
        //    helix ends the radial direction is xAxis, so both locks give the same
        //    profile orientation at the joints.
        var rectSplit = isRectangular && leadsTogether && hugMode;
        var tubeSweep = function(tag, hw, hl, rr, inPath, helPath, outPath, inStart) {
            var drawProfile = function(skId, pl) {
                var s = newSketchOnPlane(context, skId, { "sketchPlane" : pl });
                if (!isRectangular) {
                    skCircle(s, "c", { "center" : vector(0, 0) * meter, "radius" : rr });
                } else {
                    skRectangle(s, "r", { "firstCorner" : vector(-hw, -hl), "secondCorner" : vector(hw, hl) });
                }
                skSolve(s);
                return qSketchRegion(skId);
            };
            var startPl = plane(inStart, startHorizontalDir, xAxis);
            if (!rectSplit)
            {
                opSweep(context, id + tag, {
                    "profiles" : drawProfile(id + (tag ~ "Sk"), startPl),
                    "path" : qUnion([inPath, helPath, outPath]),
                    "profileControl" : isRectangular ? ProfileControlMode.LOCK_DIRECTION : ProfileControlMode.NONE,
                    "lockDirection" : zAxis
                });
                return { "body" : qCreatedBy(id + tag, EntityType.BODY),
                         "sketches" : [qCreatedBy(id + (tag ~ "Sk"), EntityType.BODY)] };
            }
            var radIn = normalize((ip0 - origin) - zAxis * dot(ip0 - origin, zAxis));
            var radOut = normalize((op0 - origin) - zAxis * dot(op0 - origin, zAxis));
            opSweep(context, id + tag, {
                "profiles" : drawProfile(id + (tag ~ "Sk"), plane(ip0, startTarget.direction, radIn)),
                "path" : helPath,
                "profileControl" : ProfileControlMode.LOCK_DIRECTION,
                "lockDirection" : zAxis
            });
            opSweep(context, id + (tag ~ "In"), {
                "profiles" : drawProfile(id + (tag ~ "SkIn"), startPl),
                "path" : inPath,
                "profileControl" : ProfileControlMode.LOCK_DIRECTION,
                "lockDirection" : xAxis
            });
            opSweep(context, id + (tag ~ "Out"), {
                "profiles" : drawProfile(id + (tag ~ "SkOut"), plane(op0, endTarget.direction, radOut)),
                "path" : outPath,
                "profileControl" : ProfileControlMode.LOCK_DIRECTION,
                "lockDirection" : xAxis
            });
            var parts = qUnion([qCreatedBy(id + tag, EntityType.BODY), qCreatedBy(id + (tag ~ "In"), EntityType.BODY),
                                qCreatedBy(id + (tag ~ "Out"), EntityType.BODY)]);
            opBoolean(context, id + (tag ~ "Join"), { "tools" : parts, "operationType" : BooleanOperationType.UNION });
            return { "body" : parts,
                     "sketches" : [qCreatedBy(id + (tag ~ "Sk"), EntityType.BODY), qCreatedBy(id + (tag ~ "SkIn"), EntityType.BODY),
                                   qCreatedBy(id + (tag ~ "SkOut"), EntityType.BODY)] };
        };

        // If the sweep fails, show the path (highlighted red) and the lead route
        // used, instead of only the OnShape error code
        var coilSw;
        try
        {
            coilSw = tubeSweep("seamlessSweep", tubeW / 2, tubeL / 2, tubeOd / 2,
                               qUnion([inletLeadEdges, inletBendEdges]), helixQuery, qUnion([outletBendEdges, outletLeadEdges]),
                               inletStartPoint);
        }
        catch (sweepErr)
        {
            println("Coil sweep error: " ~ sweepErr);
            throw regenError("Coil sweep failed (lead route: " ~ leadRouteText ~ "). The sweep path is highlighted in red.",
                             ["leadCtc"], unifiedPath);
        }
        var coilBodyQ = coilSw.body;

        // Measure path lengths now (wire geometry is deleted in cleanup)
        var pathLenTotal = evLength(context, { "entities" : unifiedPath }) / meter;   // m
        var pathLenHelix = evLength(context, { "entities" : helixQuery }) / meter;    // m

        // =========================================================================
        // 8. OUTER SLEEVE GENERATION (WITH INDEPENDENT LEAD LENGTHS)
        // =========================================================================
        var cleanupEntities = concatenateArrays([[unifiedPath], coilSw.sketches, extraCleanup]);

        if (addSleeve)
        {
            // Build separate shorter straight segments for the sleeve.
            // Leads together: the sleeve follows the clear run and S-bend, and
            // "Sleeve lead length" is measured along the final straight.
            var sleeveInletStartPoint;
            var sleeveOutletEndPoint;
            var sleeveInletFrom = ip3;
            var sleeveOutletFrom = op3;
            if (leadsTogether)
            {
                var sleeveFinal = min(max(sleeveLeadLength, 1 * millimeter), tgFinalLen * meter);
                sleeveInletFrom = inletJoinPoint;
                sleeveOutletFrom = outletJoinPoint;
                sleeveInletStartPoint = inletJoinPoint + tgDir * sleeveFinal;
                sleeveOutletEndPoint = outletJoinPoint + tgDir * sleeveFinal;
            }
            else
            {
                sleeveInletStartPoint = ip3 - startHorizontalDir * (sleeveLeadLength - bendRadius);
                sleeveOutletEndPoint = op3 + endHorizontalDir * (sleeveLeadLength - bendRadius);
            }

            var sleeveInletLineId = id + "sleeveInletLine";
            var sleeveOutletLineId = id + "sleeveOutletLine";
            var sleeveInletLineQuery;
            var sleeveOutletLineQuery;
            var sleeveInPath;
            var sleeveOutPath;
            if (oneSpline)
            {
                // Wrap mode: one spline per sleeve lead, same points as the coil
                // lead up to the join, then the shorter sleeve straight
                var sleeveSpline = function(pts, startDir, endPoint, splineId) {
                    var all = pts;
                    var last = pts[size(pts) - 1];
                    // Straight part: evenly spaced points (~3 mm) so the fit stays
                    // straight and valid for long leads (4 points failed at 559 mm)
                    var nSt = min(max(4, ceil(norm(endPoint - last) / (3 * millimeter))), 300);
                    for (var k = 1; k <= nSt; k += 1)
                    {
                        all = append(all, last + (endPoint - last) * (k / nSt));
                    }
                    var polyLen = 0 * meter;
                    for (var k = 1; k < size(all); k += 1) { polyLen += norm(all[k] - all[k - 1]); }
                    opFitSpline(context, splineId, {
                        "points" : all,
                        "startDerivative" : startDir * polyLen,           // exact helix tangent at the joint
                        "endDerivative" : tgDir * polyLen
                    });
                    return qCreatedBy(splineId, EntityType.EDGE);
                };
                sleeveInletLineQuery = sleeveSpline(hugInletPts, -startTarget.direction, sleeveInletStartPoint, sleeveInletLineId);
                sleeveOutletLineQuery = sleeveSpline(hugOutletPts, endTarget.direction, sleeveOutletEndPoint, sleeveOutletLineId);
                sleeveInPath = sleeveInletLineQuery;
                sleeveOutPath = sleeveOutletLineQuery;
            }
            else
            {
                // Skip a zero-length straight (sleeve lead length <= bend radius):
                // two coincident fit points are invalid
                sleeveInletLineQuery = qNothing();
                if (norm(sleeveInletStartPoint - sleeveInletFrom) > 1e-6 * meter)
                {
                    opFitSpline(context, sleeveInletLineId, { "points" : [sleeveInletStartPoint, sleeveInletFrom] });
                    sleeveInletLineQuery = qCreatedBy(sleeveInletLineId, EntityType.EDGE);
                }

                sleeveOutletLineQuery = qNothing();
                if (norm(sleeveOutletEndPoint - sleeveOutletFrom) > 1e-6 * meter)
                {
                    opFitSpline(context, sleeveOutletLineId, { "points" : [sleeveOutletFrom, sleeveOutletEndPoint] });
                    sleeveOutletLineQuery = qCreatedBy(sleeveOutletLineId, EntityType.EDGE);
                }

                // Create a custom sweep path just for the sleeve
                sleeveInPath = qUnion([sleeveInletLineQuery, inletRouteEdges, inletCurveQuery]);
                sleeveOutPath = qUnion([outletCurveQuery, outletRouteEdges, sleeveOutletLineQuery]);
            }

            // Sweep the solid oversized profile (profile at the tip of the shorter sleeve lead)
            var sleeveSw = tubeSweep("sleeveSweep", tubeW / 2 + sleeveThickness, tubeL / 2 + sleeveThickness,
                                     tubeOd / 2 + sleeveThickness, sleeveInPath, helixQuery, sleeveOutPath, sleeveInletStartPoint);
            var sleeveBody = sleeveSw.body;

            // Hollow it out
            opBoolean(context, id + "hollowSleeve", {
                "tools" : coilBodyQ,
                "targets" : sleeveBody,
                "operationType" : BooleanOperationType.SUBTRACTION,
                "keepTools" : true
            });

            // Name and color the sleeve body
            setProperty(context, {
                "entities" : sleeveBody,
                "propertyType" : PropertyType.NAME,
                "value" : "Sleeve"
            });
            setProperty(context, {
                "entities" : sleeveBody,
                "propertyType" : PropertyType.APPEARANCE,
                "value" : color(230 / 255, 230 / 255, 230 / 255)  // #E6E6E6
            });

            // Add new sleeve wireframes to the cleanup routine
            cleanupEntities = append(cleanupEntities, sleeveInletLineQuery);
            cleanupEntities = append(cleanupEntities, sleeveOutletLineQuery);
            cleanupEntities = concatenateArrays([cleanupEntities, sleeveSw.sketches]);
        }

        // =========================================================================
        // 8b. OPTIONAL TUBE BORE (done after the sleeve so the sleeve stays solid-walled)
        // =========================================================================
        if (modelBore)
        {
            var boreSw = tubeSweep("boreSweep", tubeW / 2 - tubeWall, tubeL / 2 - tubeWall, boreId / 2,
                                   qUnion([inletLeadEdges, inletBendEdges]), helixQuery, qUnion([outletBendEdges, outletLeadEdges]),
                                   inletStartPoint);

            opBoolean(context, id + "hollowCoil", {
                "tools" : boreSw.body,
                "targets" : coilBodyQ,
                "operationType" : BooleanOperationType.SUBTRACTION
            });

            cleanupEntities = concatenateArrays([cleanupEntities, boreSw.sketches]);
        }

        // =========================================================================
        // 8c. DUMMY SAMPLE / CRUCIBLE
        // Each "segment" is an axial slice of the load described as:
        //   outer radius Ro, optional conductive wall (thickness wallT), and an
        //   optional solid conductive core (radius coreR) inside it.
        // All radii/z positions in meters, z measured from the coil center.
        // =========================================================================
        var sampleSegments = [];
        var isCrucible = false;
        var sampleQuery = qNothing();
        var crucibleQuery = qNothing();
        var chargeQuery = qNothing();
        var crucibleColor = color(58 / 255, 58 / 255, 58 / 255);   // graphite #3A3A3A
        var aInnerM = (coilId / 2) / meter;

        // Heated region for the uniformity estimate (m, from the coil center)
        var heatZmin = 0;
        var heatZmax = 0;
        var heatR = 0;            // load surface radius (m)

        // Thermal estimate: outer radius and length of the whole load (m), and its
        // thermal properties. Representative averages over ~20-1000 C, not
        // temperature-dependent: k (W/m-K), density (kg/m3), cp (J/kg-K).
        var thermR = 0;
        var thermL = 0;
        var thermCapJK = 0;       // heat capacity override (J/K), crucible + charge
        // "melt" = approximate solidus / limit (C) for the max-temperature warning;
        // 0 = no check (custom material).
        var thermalProps = function(mat, kC, dC, cC) {
            if (mat == CoilMaterial.STEEL_COLD) { return { "k" : 45,  "d" : 7850, "cp" : 490, "melt" : 1420 }; }
            if (mat == CoilMaterial.STEEL_HOT)  { return { "k" : 27,  "d" : 7650, "cp" : 650, "melt" : 1420 }; }
            if (mat == CoilMaterial.STAINLESS || mat == CoilMaterial.STAINLESS_HOT)
                                                { return { "k" : 22,  "d" : 7900, "cp" : 560, "melt" : 1400 }; }
            if (mat == CoilMaterial.COPPER)     { return { "k" : 380, "d" : 8900, "cp" : 400, "melt" : 1085 }; }
            if (mat == CoilMaterial.ALUMINUM)   { return { "k" : 220, "d" : 2700, "cp" : 950, "melt" : 660 }; }
            if (mat == CoilMaterial.BRASS)      { return { "k" : 120, "d" : 8500, "cp" : 390, "melt" : 900 }; }
            if (mat == CoilMaterial.TITANIUM)   { return { "k" : 18,  "d" : 4500, "cp" : 580, "melt" : 1668 }; }
            if (mat == CoilMaterial.GRAPHITE)   { return { "k" : 80,  "d" : 1800, "cp" : 1300, "melt" : 3600 }; }
            return {
                "k" : (kC is number) ? kC : 25,
                "d" : (dC is number) ? dC : 7800,
                "cp" : (cC is number) ? cC : 600,
                "melt" : 0
            };
        };
        var thermP = thermalProps(CoilMaterial.STAINLESS, undefined, undefined, undefined);

        // Material text shown in the Efficiency / Thermal messages. The script uses
        // its own material table (dropdown); OnShape part materials set in the
        // Properties dialog are not visible to features during regeneration.
        var partMatText = "";
        var materialLabel = function(mat) {
            if (mat == CoilMaterial.STEEL_COLD) { return "Steel - cold"; }
            if (mat == CoilMaterial.STEEL_HOT) { return "Steel - hot"; }
            if (mat == CoilMaterial.STAINLESS) { return "Stainless 304"; }
            if (mat == CoilMaterial.STAINLESS_HOT) { return "Stainless 304 - hot"; }
            if (mat == CoilMaterial.COPPER) { return "Copper"; }
            if (mat == CoilMaterial.ALUMINUM) { return "Aluminum"; }
            if (mat == CoilMaterial.BRASS) { return "Brass"; }
            if (mat == CoilMaterial.TITANIUM) { return "Titanium"; }
            if (mat == CoilMaterial.GRAPHITE) { return "Graphite"; }
            return "Custom";
        };

        // Resistivity (ohm*m), relative permeability, conductive?
        var materialProps = function(mat, rhoCustom, muCustom) {
            if (mat == CoilMaterial.STEEL_COLD) { return { "rho" : 1.7e-7, "mu" : 100, "cond" : true }; }
            if (mat == CoilMaterial.STEEL_HOT)  { return { "rho" : 1.15e-6, "mu" : 1, "cond" : true }; }
            if (mat == CoilMaterial.STAINLESS)  { return { "rho" : 7.2e-7, "mu" : 1, "cond" : true }; }
            if (mat == CoilMaterial.STAINLESS_HOT) { return { "rho" : 1.17e-6, "mu" : 1, "cond" : true }; }
            if (mat == CoilMaterial.COPPER)     { return { "rho" : 1.7e-8, "mu" : 1, "cond" : true }; }
            if (mat == CoilMaterial.ALUMINUM)   { return { "rho" : 2.8e-8, "mu" : 1, "cond" : true }; }
            if (mat == CoilMaterial.BRASS)      { return { "rho" : 6.5e-8, "mu" : 1, "cond" : true }; }
            if (mat == CoilMaterial.TITANIUM)   { return { "rho" : 4.2e-7, "mu" : 1, "cond" : true }; }
            if (mat == CoilMaterial.GRAPHITE)   { return { "rho" : 1.0e-5, "mu" : 1, "cond" : true }; }
            var rc = (rhoCustom is number) ? rhoCustom : 17;
            var mc = (muCustom is number) ? muCustom : 1;
            return { "rho" : rc * 1e-8, "mu" : mc, "cond" : true };
        };

        var crucibleProps = function(mat, rhoCustom) {
            if (mat == CoilCrucibleMaterial.CERAMIC) { return { "rho" : 1, "mu" : 1, "cond" : false }; }
            if (mat == CoilCrucibleMaterial.CUSTOM)
            {
                var rc = (rhoCustom is number) ? rhoCustom : 1000;
                return { "rho" : rc * 1e-8, "mu" : 1, "cond" : true };
            }
            return { "rho" : 1.0e-5, "mu" : 1, "cond" : true };   // graphite
        };

        var airProps = { "rho" : 1, "mu" : 1, "cond" : false };

        var makeSeg = function(z0, z1, ro, wallT, wallP, coreR, coreP, isCharge) {
            return {
                "zMin" : z0, "zMax" : z1, "Ro" : ro,
                "wallT" : wallT, "wallRho" : wallP.rho, "wallCond" : wallP.cond,
                "coreR" : coreR, "coreRho" : coreP.rho, "coreMu" : coreP.mu, "coreCond" : coreP.cond,
                "isCharge" : isCharge
            };
        };

        if (addSample)
        {
            if (definition.sampleType == CoilSampleType.CRUCIBLE)
            {
                isCrucible = true;
                var cOd = (definition.crucibleOd is ValueWithUnits) ? definition.crucibleOd : 20 * millimeter;
                var cWall = (definition.crucibleWall is ValueWithUnits) ? definition.crucibleWall : 3 * millimeter;
                var cH = (definition.crucibleHeight is ValueWithUnits) ? definition.crucibleHeight : 50 * millimeter;
                var cBot = (definition.crucibleBottom is ValueWithUnits) ? definition.crucibleBottom : 5 * millimeter;
                var cFill = (definition.chargeFill is ValueWithUnits) ? definition.chargeFill : 25 * millimeter;

                if (cOd >= coilId) { throw regenError("Crucible OD must be smaller than the coil ID.", ["crucibleOd"]); }
                if (2 * cWall >= cOd) { throw regenError("Crucible wall is too thick for its OD.", ["crucibleWall"]); }
                if (cBot >= cH) { throw regenError("Crucible bottom must be thinner than the crucible height.", ["crucibleBottom"]); }
                if (cFill > cH - cBot) { throw regenError("Charge fill height is taller than the crucible cavity.", ["chargeFill"]); }

                var zBot = -cH / 2;
                var cInnerR = cOd / 2 - cWall;

                // Crucible = outer cylinder minus bore from the top
                fCylinder(context, id + "crucibleOuter", {
                    "bottomCenter" : origin + zAxis * zBot,
                    "topCenter" : origin + zAxis * (cH / 2),
                    "radius" : cOd / 2
                });
                fCylinder(context, id + "crucibleBore", {
                    "bottomCenter" : origin + zAxis * (zBot + cBot),
                    "topCenter" : origin + zAxis * (cH / 2 + 1 * millimeter),
                    "radius" : cInnerR
                });
                opBoolean(context, id + "crucibleHollow", {
                    "tools" : qCreatedBy(id + "crucibleBore", EntityType.BODY),
                    "targets" : qCreatedBy(id + "crucibleOuter", EntityType.BODY),
                    "operationType" : BooleanOperationType.SUBTRACTION
                });
                crucibleQuery = qCreatedBy(id + "crucibleOuter", EntityType.BODY);

                // Charge sits on the crucible floor
                if (cFill > 0 * meter)
                {
                    fCylinder(context, id + "charge", {
                        "bottomCenter" : origin + zAxis * (zBot + cBot),
                        "topCenter" : origin + zAxis * (zBot + cBot + cFill),
                        "radius" : cInnerR
                    });
                    chargeQuery = qCreatedBy(id + "charge", EntityType.BODY);
                }

                var crucP = crucibleProps(definition.crucibleMaterial, definition.crucibleRho);
                var chargeP = materialProps(definition.chargeMaterial, definition.chargeRho, definition.chargeMu);
                if (definition.crucibleMaterial == CoilCrucibleMaterial.CERAMIC)
                {
                    crucibleColor = color(232 / 255, 220 / 255, 200 / 255);  // ceramic beige
                }

                var roM = (cOd / 2) / meter;
                var tM = cWall / meter;
                var aiM = roM - tM;
                var z0 = zBot / meter;
                var z1 = (zBot + cBot) / meter;
                var z2 = (zBot + cBot + cFill) / meter;
                var z3 = (cH / 2) / meter;

                // Uniformity is reported over the charge zone, at the charge surface
                heatZmin = z1;
                heatZmax = z2;
                heatR = aiM;

                // Thermal: whole crucible as one body, charge properties
                thermR = roM;
                thermL = z3 - z0;
                thermP = thermalProps(definition.chargeMaterial, undefined, undefined, undefined);
                // Crucible body (graphite ~1800 kg/m3, 1300 J/kg-K avg; ceramic ~2300, 1000) + charge
                var crucDCp = (definition.crucibleMaterial == CoilCrucibleMaterial.CERAMIC) ? 2300 * 1000 : 1800 * 1300;
                var crucVol = PI * roM * roM * (z3 - z0) - PI * aiM * aiM * (z3 - z1);
                var chargeVol = PI * aiM * aiM * max(z2 - z1, 0);
                thermCapJK = crucVol * crucDCp + chargeVol * thermP.d * thermP.cp;
                partMatText = "charge " ~ materialLabel(definition.chargeMaterial);

                // Bottom: solid crucible-material disc
                sampleSegments = append(sampleSegments, makeSeg(z0, z1, roM, 0, airProps, roM, crucP, false));
                // Charge zone: crucible wall + metal core
                if (z2 > z1)
                {
                    sampleSegments = append(sampleSegments, makeSeg(z1, z2, roM, tM, crucP, aiM, chargeP, true));
                }
                // Empty zone above the charge: crucible wall only
                if (z3 > z2)
                {
                    sampleSegments = append(sampleSegments, makeSeg(z2, z3, roM, tM, crucP, 0, airProps, false));
                }
            }
            else
            {
                var sampleMatUsed = definition.sampleMaterial;
                partMatText = "material " ~ materialLabel(sampleMatUsed);
                var sampleP = materialProps(sampleMatUsed, definition.sampleRho, definition.sampleMu);
                var sampleB = 0;
                var sampleZmin = 0;
                var sampleZmax = 0;

                if (definition.sampleType == CoilSampleType.CYLINDER)
                {
                    var sDia = (definition.sampleDiameter is ValueWithUnits) ? definition.sampleDiameter : 15 * millimeter;
                    var sLen = (definition.sampleCylLength is ValueWithUnits) ? definition.sampleCylLength : 100 * millimeter;
                    if (sDia >= coilId)
                    {
                        throw regenError("Sample diameter must be smaller than the coil ID.", ["sampleDiameter"]);
                    }

                    fCylinder(context, id + "sampleCyl", {
                        "bottomCenter" : origin - zAxis * (sLen / 2),
                        "topCenter" : origin + zAxis * (sLen / 2),
                        "radius" : sDia / 2
                    });
                    sampleQuery = qCreatedBy(id + "sampleCyl", EntityType.BODY);

                    sampleB = (sDia / 2) / meter;
                    sampleZmin = -(sLen / 2) / meter;
                    sampleZmax = (sLen / 2) / meter;
                }
                else if (definition.sampleType == CoilSampleType.PRISM)
                {
                    var sW = (definition.samplePrismWidth is ValueWithUnits) ? definition.samplePrismWidth : 10 * millimeter;
                    var sD = (definition.samplePrismDepth is ValueWithUnits) ? definition.samplePrismDepth : 10 * millimeter;
                    var sL = (definition.samplePrismLength is ValueWithUnits) ? definition.samplePrismLength : 55 * millimeter;
                    if (sqrt(sW * sW + sD * sD) >= coilId)
                    {
                        throw regenError("Sample diagonal must be smaller than the coil ID.", ["samplePrismWidth"]);
                    }

                    var samplePlane = plane(origin - zAxis * (sL / 2), zAxis, xAxis);
                    var sampleSketchId = id + "sampleSketch";
                    var skSample = newSketchOnPlane(context, sampleSketchId, { "sketchPlane" : samplePlane });
                    skRectangle(skSample, "s", {
                        "firstCorner" : vector(-sW / 2, -sD / 2),
                        "secondCorner" : vector(sW / 2, sD / 2)
                    });
                    skSolve(skSample);

                    opExtrude(context, id + "samplePrism", {
                        "entities" : qSketchRegion(sampleSketchId),
                        "direction" : zAxis,
                        "endBound" : BoundingType.BLIND,
                        "endDepth" : sL
                    });
                    sampleQuery = qCreatedBy(id + "samplePrism", EntityType.BODY);
                    cleanupEntities = append(cleanupEntities, qCreatedBy(sampleSketchId, EntityType.BODY));

                    sampleB = sqrt((sW / meter) * (sD / meter) / PI);
                    sampleZmin = -(sL / 2) / meter;
                    sampleZmax = (sL / 2) / meter;
                }
                else
                {
                    if (!(definition.samplePart is Query) || isQueryEmpty(context, definition.samplePart))
                    {
                        throw regenError("Select a sample part.", ["samplePart"]);
                    }

                    // Axial extent in the coil's own coordinate system
                    var coilCsys = coordSystem(origin, xAxis, zAxis);
                    var sampleBox = evBox3d(context, { "topology" : definition.samplePart, "cSys" : coilCsys, "tight" : true });
                    sampleZmin = sampleBox.minCorner[2] / meter;
                    sampleZmax = sampleBox.maxCorner[2] / meter;

                    // Average cross-section area = volume / axial span
                    var sVol = evVolume(context, { "entities" : definition.samplePart }) / (meter ^ 3);
                    var sSpan = sampleZmax - sampleZmin;
                    if (sSpan <= 0)
                    {
                        throw regenError("Could not measure the sample part along the coil axis.", ["samplePart"]);
                    }
                    sampleB = sqrt((sVol / sSpan) / PI);
                }

                var bClamped = min(sampleB, aInnerM * 0.999);
                sampleSegments = [makeSeg(sampleZmin, sampleZmax, bClamped, 0, airProps, bClamped, sampleP, true)];

                heatZmin = sampleZmin;
                heatZmax = sampleZmax;
                heatR = bClamped;

                // Thermal: equivalent cylinder (same volume and length)
                thermR = sampleB;
                thermL = sampleZmax - sampleZmin;
                thermP = thermalProps(sampleMatUsed, definition.sampleK, definition.sampleDensity, definition.sampleCp);
            }
        }

        // =========================================================================
        // 9. CLEANUP CONSTRUCTION GEOMETRY
        // =========================================================================
        opDeleteBodies(context, id + "cleanupCurves", {
            "entities" : qUnion(cleanupEntities)
        });

        // =========================================================================
        // 10. ESTIMATED INDUCTANCE (EMPTY COIL)
        // Base: thin-sheet single-layer coil (Nagaoka, Lundin approx.)
        //   L = mu0 * pi * a^2 * N^2 * K / l
        // LF: a = tube centerline radius, minus conductor-shape correction
        //     L = L_sheet - mu0 * a * N * (kS + kM)
        //     kS = 1.5 - ln(pitch / GMD_conductor)   (self term, any cross-section)
        //     kM = Rosa's N-turn mutual term
        //     GMD_conductor: round = d*e^(-1/4)/2 ; rectangle ~ 0.2235*(W+L)
        // HF: perfect-conductor (surface current) coil, fitted to a numerical
        //     solution of the current distribution on round turns:
        //     L = L_sheet(aHF) - mu0 * aHF * N * (kS(gHF) + kM)
        //     aHF = ID/2 + 0.19 * d,  gHF = 0.38 * d   (d = tube OD, or the
        //     equal-perimeter round size 2(W+L)/pi for rectangular tube)
        //     Within ~4 % for ID/d 3.5-13, pitch/d 1.05-3, 2-14 turns
        //     (the plain sheet at ID/2 used before was off by up to 33 %).
        // =========================================================================
        var mu0 = 4 * PI * 1e-7;                     // H/m
        var nTurns = revs + tgExtRevs;               // helix revolutions (turns + 0.5), plus any extra winding from "continue around coil"
        var windLen = (totalHeight + pitch * tgExtRevs) / meter;   // m (winding length incl. extra winding)

        var f1 = function(x) { return (1 + 0.383901 * x + 0.017108 * x * x) / (1 + 0.258952 * x); };
        var f2 = function(x) { return 0.093842 * x + 0.002029 * x * x - 0.000801 * x * x * x; };

        // Thin-sheet inductance (H) for mean radius a (m)
        var sheetL = function(a) {
            var d = 2 * a;
            var k;
            if (windLen >= d) {
                var dOverL = d / windLen;
                k = f1(dOverL * dOverL) - (4 * dOverL) / (3 * PI);
            } else {
                var lOverD = windLen / d;
                k = (2 / PI) * lOverD * ((log(4 / lOverD) - 0.5) * f1(lOverD * lOverD) + f2(lOverD * lOverD));
            }
            return mu0 * PI * a * a * nTurns * nTurns * k / windLen;
        };

        // --- LF estimate: centerline radius + conductor-shape correction ---
        var aMean = pathRadius / meter;
        var gmdCond;                                 // conductor self-GMD (m)
        if (isRectangular)
        {
            gmdCond = 0.2235 * (tubeW + tubeL) / meter;
        }
        else
        {
            gmdCond = 0.5 * exp(-0.25) * tubeOd / meter;
        }
        var kS = 1.5 - log((pitch / meter) / gmdCond);
        var kM = log(2 * PI) - 1.5 - log(nTurns) / (6 * nTurns) - 0.33084 / (nTurns * nTurns)
                 - 1 / (120 * nTurns * nTurns * nTurns);
        var lfH = sheetL(aMean) - mu0 * aMean * nTurns * (kS + kM);

        // --- HF estimate: surface current, fitted to a perfect-conductor solution ---
        var aInner = aInnerM;                        // inner face (copper loss)
        var dCondEq = isRectangular ? 2 * (tubeW + tubeL) / PI / meter : tubeOd / meter;
        var dRadial = isRectangular ? tubeW / meter : tubeOd / meter;
        var aHF = aInner + 0.19 * dRadial;
        var gHF = 0.38 * dCondEq;
        var hfH = max(sheetL(aHF) - mu0 * aHF * nTurns * (1.5 - log((pitch / meter) / gHF) + kM),
                      0.25 * sheetL(aHF));       // guard for odd shapes (e.g. a fraction of one turn)
        // Field loops for the load coupling (radius fitted to the same solution)
        var aField = aInner + 0.35 * dRadial;

        var lfUH = lfH * 1e6;
        var hfUH = hfH * 1e6;

        setVariable(context, "coilInductanceLF_uH", lfUH);
        setVariable(context, "coilInductanceHF_uH", hfUH);

        var inductanceText = "Inductance " ~ roundToPrecision(hfUH, 3) ~ " µH (HF) – " ~ roundToPrecision(lfUH, 3) ~ " µH (LF)";
        var msg = inductanceText ~ " · " ~ roundToPrecision(nTurns, 2) ~ " turns · no workpiece";

        // =========================================================================
        // 11. ESTIMATED VALUES
        // =========================================================================
        if (showEstimates)
        {
            // ---------------------------------------------------------------------
            // 11a. Lead inductance: two-wire helper (wrap mode's curved part)
            //   and the finite-length pair model below
            // ---------------------------------------------------------------------
            var rCond = isRectangular ? ((tubeW + tubeL) / PI) / meter : (tubeOd / 2) / meter;
            var acoshFn = function(x) { return log(x + sqrt(x * x - 1)); };
            var twoWireL = function(len, s) {
                var x = s / (2 * rCond);
                if (x < 1.0001) { x = 1.0001; }
                return (mu0 * len / PI) * acoshFn(x);
            };

            if (tankLeadLength > 0 * meter && tankLeadSpacing <= 2 * rCond * meter)
            {
                throw regenError("Extra lead spacing must be larger than the tube size.", ["tankLeadSpacing"]);
            }

            // Finite-length (partial-inductance) model of a lead pair carrying
            // opposite currents, following the modeled lead paths:
            //   L = L_self(A) + L_self(B) - 2 * M_AB
            // M_AB: each path segment is treated as a filament parallel to the
            //   lead direction (tgDir) at its own across/height position, using the
            //   exact mutual inductance of parallel finite filaments:
            //   M = (mu0/4pi) [F(a2-b1) + F(a1-b2) - F(a2-b2) - F(a1-b1)],
            //   F(x) = x asinh(x/d) - sqrt(x^2 + d^2)
            //   d = (s + sqrt(s^2 - 4r^2)) / 2 for centre spacing s: accounts for
            //   current crowding between close tubes, so long pairs match the
            //   two-wire formula (mu0 l / pi) acosh(s / 2r)
            // L_self: HF (surface current) inductance of a straight round
            //   conductor of the lead's path length (Rosa):
            //   (mu0 l / 2pi) [asinh(l/r) - sqrt(1 + (r/l)^2) + r/l]
            // (The infinite-wire formula used before was ~20 % high for short,
            // widely spaced leads and ~5 % high for short close pairs.)
            var asinhFn = function(x) { return log(x + sqrt(x * x + 1)); };
            var fFil = function(x, d) { return x * asinhFn(x / d) - sqrt(x * x + d * d); };
            var selfL = function(len) {
                if (len <= 0) { return 0; }
                return 2e-7 * len * (asinhFn(len / rCond) - sqrt(1 + (rCond / len) * (rCond / len)) + rCond / len);
            };
            var leadCoords = function(pts) {
                var c = [];
                for (var k = 0; k < size(pts); k += 1)
                {
                    c = append(c, [dot(pts[k] - origin, tgDir) / meter, dot(pts[k] - origin, xAxis) / meter,
                                   dot(pts[k] - origin, zAxis) / meter]);
                }
                return c;
            };
            var pathLen = function(pts) {
                var len = 0;
                for (var k = 1; k < size(pts); k += 1) { len += norm(pts[k] - pts[k - 1]) / meter; }
                return len;
            };
            var pairL = function(ptsA, ptsB) {
                if (size(ptsA) < 2 || size(ptsB) < 2) { return 0; }
                var cA = leadCoords(ptsA);
                var cB = leadCoords(ptsB);
                // Each segment is split into its components along the three axes
                // (lead direction, across, height); parallel components couple,
                // perpendicular ones have no mutual inductance. So vertical climbs
                // (e.g. the S-bend legs of a wide lead loop) count too.
                var mAB = 0;
                for (var ax = 0; ax < 3; ax += 1)
                {
                    var q1 = (ax == 0) ? 1 : 0;
                    var q2 = (ax == 2) ? 1 : 2;
                    for (var i = 0; i < size(cA) - 1; i += 1)
                    {
                        var a1 = cA[i][ax];
                        var a2 = cA[i + 1][ax];
                        if (abs(a2 - a1) > 1e-9)
                        {
                            var uA = (cA[i][q1] + cA[i + 1][q1]) / 2;
                            var wAs = (cA[i][q2] + cA[i + 1][q2]) / 2;
                            for (var j = 0; j < size(cB) - 1; j += 1)
                            {
                                var b1 = cB[j][ax];
                                var b2 = cB[j + 1][ax];
                                if (abs(b2 - b1) > 1e-9)
                                {
                                    var uB = (cB[j][q1] + cB[j + 1][q1]) / 2;
                                    var wBs = (cB[j][q2] + cB[j + 1][q2]) / 2;
                                    var dRaw = sqrt((uA - uB) * (uA - uB) + (wAs - wBs) * (wAs - wBs));
                                    var dAB = (dRaw > 2 * rCond) ? (dRaw + sqrt(dRaw * dRaw - 4 * rCond * rCond)) / 2 : rCond;
                                    mAB += 1e-7 * (fFil(a2 - b1, dAB) + fFil(a1 - b2, dAB) - fFil(a2 - b2, dAB) - fFil(a1 - b1, dAB));
                                }
                            }
                        }
                    }
                }
                return selfL(pathLen(ptsA)) + selfL(pathLen(ptsB)) - 2 * mAB;
            };

            // Coil leads along their modeled paths (+ wrap mode's curved part around
            // the coil, estimated at its mean spacing)
            var coilLeadsH = pairL(pairPtsIn, pairPtsOut);
            if (hugCurvedLen > 0)
            {
                coilLeadsH += twoWireL(hugCurvedLen, hugCurvedSpacing);
            }

            // Extra leads to the capacitor: straight parallel pair, same model
            var tankLeadsH = 0;
            if (tankLeadLength > 0 * meter)
            {
                var tOff = xAxis * (tankLeadSpacing / 2);
                tankLeadsH = pairL([origin + tOff, origin + tOff + tgDir * tankLeadLength],
                                   [origin - tOff, origin - tOff + tgDir * tankLeadLength]);
            }
            var leadsH = coilLeadsH + tankLeadsH;

            // ---------------------------------------------------------------------
            // 11b. Complex math helpers ([re, im])
            // ---------------------------------------------------------------------
            var cMul = function(a, b) { return [a[0] * b[0] - a[1] * b[1], a[0] * b[1] + a[1] * b[0]]; };
            var cDiv = function(a, b) {
                var den = b[0] * b[0] + b[1] * b[1];
                return [(a[0] * b[0] + a[1] * b[1]) / den, (a[1] * b[0] - a[0] * b[1]) / den];
            };
            var cSub = function(a, b) { return [a[0] - b[0], a[1] - b[1]]; };
            var cScale = function(a, s) { return [a[0] * s, a[1] * s]; };

            // ---------------------------------------------------------------------
            // 11c. Solid conducting cylinder in a uniform axial AC field
            // Normalized flux F = mu_r * 2*J1(kr) / (kr*J0(kr)),  k = (1-j)/delta
            // With u = r^2 / (2 delta^2):
            //   J0 -> S0 = sum (j u)^n / (n!)^2
            //   2*J1/(kr) -> S1 = sum (j u)^n / (n!(n+1)!)
            // Re(F) = flux kept (inductance), -Im(F) = power absorbed (resistance)
            // For r/delta > 30 use the thick-body limit (1 - j) * delta / r.
            // ---------------------------------------------------------------------
            var kelvinRatio = function(rOverDelta) {
                if (rOverDelta > 30)
                {
                    return [1 / rOverDelta, -1 / rOverDelta];
                }
                var u = rOverDelta * rOverDelta / 2;
                var s0r = 0; var s0i = 0; var s1r = 0; var s1i = 0;
                var t0 = 1; var t1 = 1;
                var ph = 0;
                var nMax = floor(4 * sqrt(u)) + 25;
                for (var n = 0; n <= nMax; n += 1)
                {
                    if (n > 0)
                    {
                        t0 = t0 * u / (n * n);
                        t1 = t1 * u / (n * (n + 1));
                    }
                    if (ph == 0) { s0r += t0; s1r += t1; }
                    else if (ph == 1) { s0i += t0; s1i += t1; }
                    else if (ph == 2) { s0r -= t0; s1r -= t1; }
                    else { s0i -= t0; s1i -= t1; }
                    ph += 1;
                    if (ph == 4) { ph = 0; }
                }
                return cDiv([s1r, s1i], [s0r, s0i]);
            };

            // ---------------------------------------------------------------------
            // 11d. Layered load (crucible wall + charge), normalized flux F
            // Core -> air gap -> conductive wall (planar slab transfer) -> outside.
            // Returns F (normalized to pi*Ro^2) and the fraction of absorbed
            // power that lands in the core when the core is the "charge".
            // ---------------------------------------------------------------------
            var layeredF = function(seg, omega) {
                var ro = seg.Ro;
                var ai = ro - seg.wallT;
                var rc = seg.coreR;

                var fCore = [1, 0];
                if (rc > 0 && seg.coreCond)
                {
                    var dCore = sqrt(2 * seg.coreRho / (omega * mu0 * seg.coreMu));
                    fCore = cScale(kelvinRatio(rc / dCore), seg.coreMu);
                }

                // Effective flux area inside the wall (complex, m^2)
                var aIn = [PI * (ai * ai - rc * rc) + PI * rc * rc * fCore[0], PI * rc * rc * fCore[1]];
                var aOut;
                var hGain2 = 1;   // |H at core / H outside|^2

                if (seg.wallCond && seg.wallT > 0)
                {
                    // Surface impedance at the inner wall face: Z = -j*w*mu0*A / (2*pi*a)
                    var zIn = cScale(cMul([0, -1], aIn), omega * mu0 / (2 * PI * ai));
                    var dW = sqrt(2 * seg.wallRho / (omega * mu0));
                    var x = min(seg.wallT / dW, 30);
                    var ex = exp(x);
                    var emx = exp(-x);
                    var chx = (ex + emx) / 2;
                    var shx = (ex - emx) / 2;
                    var cx = cos(x * radian);
                    var sx = sin(x * radian);
                    var coshG = [chx * cx, shx * sx];     // cosh((1+j)x)
                    var sinhG = [shx * cx, chx * sx];     // sinh((1+j)x)
                    var eta = [seg.wallRho / dW, seg.wallRho / dW];

                    var hT = cSub(coshG, cMul(cDiv(zIn, eta), sinhG));   // H_out / H_in
                    var eT = cSub(cMul(zIn, coshG), cMul(eta, sinhG));   // E_out / H_in
                    var zOut = cDiv(eT, hT);

                    aOut = cScale(cMul([0, 1], zOut), 2 * PI * ro / (omega * mu0));
                    hGain2 = 1 / (hT[0] * hT[0] + hT[1] * hT[1]);
                }
                else
                {
                    aOut = [aIn[0] + PI * (ro * ro - ai * ai), aIn[1]];
                }

                var coreLoss = (rc > 0 && seg.coreCond) ? -PI * rc * rc * fCore[1] : 0;
                var totalLoss = -aOut[1];
                var frac = (totalLoss > 0 && seg.isCharge) ? min(hGain2 * coreLoss / totalLoss, 1) : 0;

                return { "F" : [aOut[0] / (PI * ro * ro), aOut[1] / (PI * ro * ro)], "chargeFrac" : frac };
            };

            // ---------------------------------------------------------------------
            // Coil field model (shared by the load coupling and the uniformity):
            // each helix revolution = one circular loop at its mean height (the
            // last half revolution = a half-weight loop; extra winding from
            // "continue around coil" = a part-weight loop beyond each end), at the
            // effective HF current radius aField. Axial field from the off-axis
            // loop formula (complete elliptic integrals K, E via AGM).
            // ---------------------------------------------------------------------
            // Complete elliptic integrals K(m), E(m), m = k^2 (AGM method)
            var ellipKE = function(m) {
                var aG = 1;
                var bG = sqrt(max(1 - m, 0));
                var cSum = m / 2;             // sum of 2^(n-1) * c_n^2, n = 0 term
                var w2 = 0.5;
                for (var n = 0; n < 10; n += 1)
                {
                    var cG = (aG - bG) / 2;
                    var aNext = (aG + bG) / 2;
                    bG = sqrt(aG * bG);
                    aG = aNext;
                    w2 = w2 * 2;
                    cSum += w2 * cG * cG;
                }
                var kK = PI / (2 * aG);
                return { "K" : kK, "E" : kK * (1 - cSum) };
            };

            // Axial field (A/m) of a 1 A loop of radius aL at radius r, axial offset z
            var loopHz = function(aL, r, z) {
                var sP = (aL + r) * (aL + r) + z * z;
                var dM = (aL - r) * (aL - r) + z * z;
                var ke = ellipKE(4 * aL * r / sP);
                return (ke.K + (aL * aL - r * r - z * z) / dM * ke.E) / (2 * PI * sqrt(sP));
            };

            var pitchM = pitch / meter;
            var halfH = (totalHeight / meter) / 2;
            var loopZ = [];
            var loopW = [];
            var nFull = floor(revs);
            var fracRev = revs - nFull;
            for (var k = 0; k < nFull; k += 1)
            {
                loopZ = append(loopZ, -halfH + (k + 0.5) * pitchM);
                loopW = append(loopW, 1);
            }
            if (fracRev > 1e-6)
            {
                loopZ = append(loopZ, -halfH + (nFull + fracRev / 2) * pitchM);
                loopW = append(loopW, fracRev);
            }
            if (tgExtRevs > 1e-6)
            {
                var extEach = tgExtRevs / 2;
                loopZ = append(loopZ, -halfH - extEach * pitchM / 2);
                loopW = append(loopW, extEach);
                loopZ = append(loopZ, halfH + extEach * pitchM / 2);
                loopW = append(loopW, extEach);
            }

            // Axial field per amp of coil current at radius r, height z (one
            // filament per loop, or 3 across the tube's inner face for the ripple)
            var spread = isRectangular ? 0.25 * tubeL / meter : 0.15 * tubeOd / meter;
            var coilHz = function(r, z, threeFil) {
                var hSum = 0;
                for (var k = 0; k < size(loopZ); k += 1)
                {
                    if (threeFil)
                    {
                        for (var fl = -1; fl <= 1; fl += 1)
                        {
                            hSum += (loopW[k] / 3) * loopHz(aField, r, z - loopZ[k] - fl * spread);
                        }
                    }
                    else
                    {
                        hSum += loopW[k] * loopHz(aField, r, z - loopZ[k]);
                    }
                }
                return hSum;
            };

            // Coupling weight of each load segment (H):
            //   G = mu0 * pi * Ro^2 * integral over the segment of H(z)^2 dz
            // H = coil field per amp at the segment surface. The impedance change
            // of a slice dz in a local axial field H is j w mu0 pi Ro^2 (F - 1) H^2 dz
            // (reciprocity), so the whole segment counts, including parts outside
            // the coil. Long coil, long sample: H = N / l and G -> mu0 pi Ro^2 N^2 / l.
            // (The old (overlap / l) * L_sheet(Ro) weight was 1.3-1.8 x too high for
            // short, wide coils.) Within ~2 % of the field from a numerical
            // perfect-conductor current solution for the coils checked.
            var segG = [];
            for (var s = 0; s < size(sampleSegments); s += 1)
            {
                var sg = sampleSegments[s];
                var span = sg.zMax - sg.zMin;
                var gSeg = 0;
                if (span > 0 && sg.Ro > 0)
                {
                    var nZ = min(max(ceil(span / (pitchM / 4)), 24), 240);
                    nZ = max(24, min(nZ, floor(30000 / size(loopZ))));
                    var dzG = span / nZ;
                    var h2Sum = 0;
                    for (var i = 0; i < nZ; i += 1)
                    {
                        var hG = coilHz(sg.Ro, sg.zMin + (i + 0.5) * dzG, false);
                        h2Sum += hG * hG * dzG;
                    }
                    gSeg = mu0 * PI * sg.Ro * sg.Ro * h2Sum;
                }
                segG = append(segG, gSeg);
            }

            // Load seen by the coil: dL (H), R_load (ohm), R into charge (ohm)
            var computeLoad = function(omega) {
                var dL = 0;
                var rLoad = 0;
                var rCharge = 0;
                for (var s = 0; s < size(sampleSegments); s += 1)
                {
                    if (segG[s] > 0)
                    {
                        var res = layeredF(sampleSegments[s], omega);
                        dL += segG[s] * (res.F[0] - 1);
                        var rSeg = -omega * segG[s] * res.F[1];
                        rLoad += rSeg;
                        rCharge += rSeg * res.chargeFrac;
                    }
                }
                return { "dL" : dL, "R" : rLoad, "Rc" : rCharge };
            };

            // ---------------------------------------------------------------------
            // Copper resistance at angular frequency w (ohm)
            // Coil: current on inner face, R = rho * N * 2*pi*a / (delta * w)
            // Leads: current on conductor surface, R = rho * len / (delta * perimeter)
            // Copper tube (C12200 / DHP, ~85 % IACS) at ~40 °C (water-cooled):
            // rho = 2.15e-8 ohm*m (was 1.8e-8 = pure copper, ~20 % low for tube,
            // which put copper loss low and efficiency / work power high)
            // Close lead pairs: proximity crowds the current onto the facing sides,
            // R x (s/2r) / sqrt((s/2r)^2 - 1)  (exact for two parallel round wires)
            // ---------------------------------------------------------------------
            var rhoCu = 2.15e-8;
            var wEff = tubeL / meter;
            var proxK = function(sM) {
                var xP = sM / (2 * rCond);
                return (xP > 1.0001) ? xP / sqrt(xP * xP - 1) : 50;
            };
            // Leads together: use the modeled lead path length (both leads, incl. bends)
            var coilLeadLen = leadsTogether ? (pathLenTotal - pathLenHelix - tgExtLen) : 2 * leadLength / meter;
            var tankLeadLen = 2 * tankLeadLength / meter;
            var leadLenEff = coilLeadLen * (leadsTogether ? proxK(tgSpacing) : 1)
                             + ((tankLeadLen > 0) ? tankLeadLen * proxK(tankLeadSpacing / meter) : 0);
            // Round tube: real current spread on the turns. The simple formula above
            // puts the current evenly over a band one tube OD wide on the inner
            // face. A numerical perfect-conductor solve of the surface current on
            // round turns (axisymmetric, 96 filaments per turn, equal current in
            // every turn, flux constant on each tube surface) gives the true loss as
            // a factor of that, tabulated below over ID/d, pitch/d and turns.
            // Widely spaced turns spread the current round most of the tube (an
            // isolated ring -> ~1/pi), so the factor is mostly < 1 (~0.48 for
            // ID/d 8, pitch/d 2, 3.5 turns); tightly wound long coils crowd it onto
            // the inner face and the ends (> 1). A load in the coil changes it
            // little (19 mm rod in a 65.5 mm ID coil: 0.464 -> 0.465).
            // Rectangular tube keeps the simple formula (factor 1).
            var cuIdD = [4, 6, 8, 10, 13];
            var cuPD = [1.05, 1.1, 1.2, 1.35, 1.5, 2, 3];
            var cuN = [2, 3, 4, 6, 8, 12];
            var cuF = [
                [[0.577, 0.721, 0.837, 0.991, 1.073, 1.13], [0.571, 0.69, 0.78, 0.891, 0.947, 0.992], [0.558, 0.654, 0.722, 0.802, 0.844, 0.883],
                 [0.541, 0.618, 0.669, 0.729, 0.763, 0.798], [0.527, 0.59, 0.632, 0.681, 0.709, 0.741], [0.495, 0.533, 0.558, 0.588, 0.606, 0.626],
                 [0.464, 0.483, 0.494, 0.508, 0.516, 0.525]],
                [[0.528, 0.678, 0.813, 1.018, 1.144, 1.252], [0.522, 0.645, 0.747, 0.888, 0.968, 1.033], [0.51, 0.608, 0.683, 0.779, 0.831, 0.877],
                 [0.495, 0.572, 0.627, 0.695, 0.733, 0.772], [0.482, 0.545, 0.589, 0.643, 0.674, 0.709], [0.452, 0.49, 0.516, 0.549, 0.569, 0.593],
                 [0.425, 0.444, 0.457, 0.474, 0.483, 0.494]],
                [[0.501, 0.653, 0.799, 1.04, 1.207, 1.376], [0.495, 0.619, 0.727, 0.891, 0.994, 1.086], [0.484, 0.582, 0.66, 0.769, 0.831, 0.887],
                 [0.47, 0.546, 0.603, 0.677, 0.719, 0.761], [0.457, 0.519, 0.564, 0.621, 0.655, 0.691], [0.429, 0.466, 0.492, 0.525, 0.546, 0.572],
                 [0.404, 0.423, 0.436, 0.453, 0.464, 0.476]],
                [[0.485, 0.637, 0.789, 1.056, 1.257, 1.486], [0.479, 0.602, 0.714, 0.894, 1.016, 1.138], [0.468, 0.565, 0.645, 0.762, 0.834, 0.901],
                 [0.454, 0.529, 0.587, 0.665, 0.711, 0.757], [0.442, 0.502, 0.548, 0.607, 0.642, 0.68], [0.414, 0.45, 0.476, 0.509, 0.53, 0.557],
                 [0.39, 0.409, 0.422, 0.439, 0.45, 0.464]],
                [[0.469, 0.621, 0.778, 1.07, 1.31, 1.622], [0.464, 0.585, 0.701, 0.896, 1.04, 1.204], [0.453, 0.548, 0.63, 0.756, 0.839, 0.923],
                 [0.439, 0.512, 0.571, 0.654, 0.705, 0.757], [0.427, 0.486, 0.531, 0.593, 0.631, 0.671], [0.4, 0.435, 0.459, 0.493, 0.514, 0.541],
                 [0.378, 0.395, 0.407, 0.424, 0.436, 0.45]]];
            var bracket = function(arr, x) {
                var xc = min(max(x, arr[0]), arr[size(arr) - 1]);
                var i0 = 0;
                while (i0 < size(arr) - 2 && xc > arr[i0 + 1]) { i0 += 1; }
                return { "i" : i0, "t" : (xc - arr[i0]) / (arr[i0 + 1] - arr[i0]) };
            };
            var cuFactor = 1;
            if (!isRectangular)
            {
                var bA = bracket(cuIdD, coilId / tubeOd);
                var bB = bracket(cuPD, pitch / tubeOd);
                var bC = bracket(cuN, nTurns);
                cuFactor = 0;
                for (var ia = 0; ia <= 1; ia += 1)
                {
                    for (var ib = 0; ib <= 1; ib += 1)
                    {
                        for (var ic2 = 0; ic2 <= 1; ic2 += 1)
                        {
                            var wgt = (ia == 0 ? 1 - bA.t : bA.t) * (ib == 0 ? 1 - bB.t : bB.t) * (ic2 == 0 ? 1 - bC.t : bC.t);
                            cuFactor += wgt * cuF[bA.i + ia][bB.i + ib][bC.i + ic2];
                        }
                    }
                }
                cuFactor = cuFactor * 0.99;   // table solved at 96 filaments/turn, ~1 % above the converged value
            }
            var rCopperAt = function(w) {
                var dCu = sqrt(2 * rhoCu / (w * mu0));
                return cuFactor * rhoCu * nTurns * 2 * PI * (aInner + dCu / 2) / (dCu * wEff)
                       + rhoCu * leadLenEff / (dCu * 2 * PI * rCond);
            };

            // ---------------------------------------------------------------------
            // 11e. Tank frequency: parallel tank (C across the coil + leads, whose
            // series resistance is R). Zero-phase frequency:
            //   w^2 = 1/(L C) - (R/L)^2
            // (the old 1/sqrt(LC) ignored R: ~1/(2 Q^2) high, 17 % at Q = 1.8).
            // No resonance when R/L >= 1/sqrt(LC) (Q < ~1). Iterated with the
            // load's effect on L and R, which depend on frequency.
            // ---------------------------------------------------------------------
            var capF = tankCapUF * 1e-6;
            var noResonance = false;
            var tankW = function(lTot, rTot) {
                return 1 / (lTot * capF) - (rTot / lTot) * (rTot / lTot);
            };
            // Empty coil (copper resistance only)
            var lEmpty = hfH + leadsH;
            var omegaE = 1 / sqrt(lEmpty * capF);
            for (var it = 0; it < 4; it += 1)
            {
                var w2E = tankW(lEmpty, rCopperAt(omegaE));
                if (w2E > 0) { omegaE = sqrt(w2E); }
            }
            var fEmpty = omegaE / (2 * PI);
            // Loaded
            var omega = omegaE;
            var load = computeLoad(omega);
            for (var it = 0; it < 10; it += 1)
            {
                var lTot = max(hfH + load.dL, hfH * 0.05) + leadsH;
                var w2 = tankW(lTot, load.R + rCopperAt(omega));
                if (w2 <= 0)
                {
                    noResonance = true;
                    omega = 1 / sqrt(lTot * capF);   // keep the undamped value for the estimates
                }
                else
                {
                    noResonance = false;
                    omega = sqrt(w2);
                }
                load = computeLoad(omega);
            }
            var hfLoadedH = max(hfH + load.dL, hfH * 0.05);
            var fLoaded = omega / (2 * PI);

            // ---------------------------------------------------------------------
            // 11f. Copper losses & efficiency (at the loaded frequency)
            // ---------------------------------------------------------------------
            var rCopper = rCopperAt(omega);

            var eff = (load.R > 0) ? load.R / (load.R + rCopper) : 0;
            var effCharge = (load.R > 0) ? load.Rc / (load.R + rCopper) : 0;
            var qFactor = omega * (hfLoadedH + leadsH) / (load.R + rCopper);

            // ---------------------------------------------------------------------
            // 11g. Cooling water pressure drop (Darcy-Weisbach)
            // Water at 20 °C. Straight sections: laminar 64/Re, turbulent Haaland
            // (drawn copper, eps = 1.5 µm). Helix: curved-pipe correlations
            // (Mishra & Gupta) with Schmidt critical Re. Leads + bends treated
            // as straight. Fittings / hoses / manifolds NOT included.
            // ---------------------------------------------------------------------
            var flowQ = flowLpm / 60000;             // m^3/s
            var rhoW = 998.2;                        // kg/m^3
            var muW = 1.002e-3;                      // Pa*s

            var boreArea;
            var dH;
            var li = 0;      // rectangular bore sides (m)
            var wi = 0;
            if (!isRectangular)
            {
                var dB = boreId / meter;
                boreArea = PI * dB * dB / 4;
                dH = dB;
            }
            else
            {
                li = (tubeL - 2 * tubeWall) / meter;
                wi = (tubeW - 2 * tubeWall) / meter;
                boreArea = li * wi;
                dH = 2 * li * wi / (li + wi);
            }

            var vel = flowQ / boreArea;
            var reyn = rhoW * vel * dH / muW;
            var log10f = function(x) { return log(x) / log(10); };

            // Laminar friction constant f*Re (Darcy): 64 for round bore; rectangular
            // bore from Shah & London, aspect ratio a = short / long side
            // (square 56.9, 2:1 62.2, 4:1 72.9, 8:1 82.3; flat slot -> 96)
            var fRe = 64;
            if (isRectangular)
            {
                var asp = min(li, wi) / max(li, wi);
                fRe = 96 * (1 - 1.3553 * asp + 1.9467 * asp * asp - 1.7012 * asp * asp * asp
                            + 0.9564 * asp * asp * asp * asp - 0.2537 * asp * asp * asp * asp * asp);
            }

            var fStraight;
            var regimeStraight;
            if (reyn < 2300)
            {
                fStraight = fRe / reyn;
                regimeStraight = "laminar";
            }
            else
            {
                var eps = 1.5e-6;
                var inv = -1.8 * log10f((eps / dH / 3.7) ^ 1.11 + 6.9 / reyn);
                // Transition 2300-4000: blend laminar -> turbulent (no jump at 2300)
                var fTurb = 1 / (inv * inv);
                var tBl = min((reyn - 2300) / 1700, 1);
                fStraight = (1 - tBl) * (fRe / reyn) + tBl * fTurb;
                regimeStraight = (reyn < 4000) ? "transitional" : "turbulent";
            }

            var curv = dH / (2 * pathRadius / meter);          // tube bore / coil diameter
            var reCrit = 2300 * (1 + 8.6 * (curv ^ 0.45));
            var fHelix;
            var regime;
            if (reyn < reCrit)
            {
                // Mishra & Gupta laminar coil factor on the straight laminar value
                // (fRe/64 carries the rectangular-bore shape over)
                var dean = max(reyn * sqrt(curv), 1);
                var lgDe = log10f(dean);
                fHelix = (fRe / reyn) * (1 + 0.033 * lgDe * lgDe * lgDe * lgDe);
                regime = "laminar";
            }
            else
            {
                fHelix = 0.3164 * (reyn ^ -0.25) + 0.03 * sqrt(curv);
                regime = "turbulent";
            }

            var dynP = 0.5 * rhoW * vel * vel;
            // "Continue around coil" extra winding is coiled too (was counted as straight)
            var lenCoiled = pathLenHelix + tgExtLen;
            var lenStraight = max(pathLenTotal - lenCoiled, 0);

            // Bend losses in the leads (on top of friction along their length):
            // total turning angle of each lead path (helix excluded), Weisbach
            // smooth-bend coefficient per 90 deg: K90 = 0.131 + 1.847 (Dh / 2R)^3.5
            // at the bend radius R. Laminar flow: K scaled by 1 + 30/sqrt(Re)
            // (bends lose relatively more at low Re). Plus tube entry (0.5) and
            // exit (1.0) at the two lead ends.
            var turnAngle = function(pts) {
                var total = 0;
                for (var k = 1; k < size(pts) - 1; k += 1)
                {
                    var e1 = pts[k] - pts[k - 1];
                    var e2 = pts[k + 1] - pts[k];
                    var n1 = norm(e1);
                    var n2 = norm(e2);
                    if (n1 > 1e-9 * meter && n2 > 1e-9 * meter)
                    {
                        total += acos(min(max(dot(e1, e2) / (n1 * n2), -1), 1)) / radian;
                    }
                }
                return total;
            };
            var bendDeg = (turnAngle(pairPtsIn) + turnAngle(pairPtsOut)) * 180 / PI;
            var rBendM = max(bendRadius / meter, dH);
            var k90 = 0.131 + 1.847 * ((dH / (2 * rBendM)) ^ 3.5);
            var kLam = (reyn < reCrit) ? 1 + 30 / sqrt(reyn) : 1;
            var kBends = k90 * kLam * bendDeg / 90;
            var kEnds = 1.5;
            var dPfric = (fStraight * lenStraight / dH + fHelix * lenCoiled / dH) * dynP;
            var dPbends = (kBends + kEnds) * dynP;
            var dP = dPfric + dPbends;   // Pa

            // ---------------------------------------------------------------------
            // 11h. Power split & water temperature rise
            // ---------------------------------------------------------------------
            // Delivered power is capped by the supply's max coil current: at the set
            // power the needed current is sqrt(P / R); above the limit the supply
            // can only deliver Imax^2 * R (before, the full set power was used for
            // the work power and the thermal estimate even when the current was
            // over the limit, so low-resistance loads read too hot).
            var pIn = supplyKW * 1000;
            var iWanted = sqrt(pIn / (load.R + rCopper));
            var pDel = (supplyImaxA > 0 && iWanted > supplyImaxA) ? supplyImaxA * supplyImaxA * (load.R + rCopper) : pIn;
            var pWork = eff * pDel;
            var pCopper = pDel - pWork;
            var waterRise = pCopper / (flowQ * rhoW * 4180);

            // Coil current and tank (capacitor) voltage at the delivered power:
            //   P = I^2 (R_load + R_copper),  V = I * |R + j w (L_coil + L_leads)|
            var iCoil = sqrt(pDel / (load.R + rCopper));
            var xTank = omega * (hfLoadedH + leadsH);
            var vTank = iCoil * sqrt(xTank * xTank + (load.R + rCopper) * (load.R + rCopper));

            // Supply limits (frequency window, coil current)
            var fCheckKHz = (addSample ? fLoaded : fEmpty) / 1000;
            var limitWarn = "";
            if (noResonance)
            {
                limitWarn = "NO RESONANCE (load too lossy for this L and C, Q < 1)";
            }
            else if (supplyFminKHz > 0 && fCheckKHz < supplyFminKHz)
            {
                limitWarn = "BELOW supply min " ~ roundToPrecision(supplyFminKHz, 0) ~ " kHz";
            }
            else if (supplyFmaxKHz > 0 && fCheckKHz > supplyFmaxKHz)
            {
                limitWarn = "ABOVE supply max " ~ roundToPrecision(supplyFmaxKHz, 0) ~ " kHz";
            }
            if (supplyImaxA > 0 && iWanted > supplyImaxA * 1.0001)
            {
                limitWarn = limitWarn ~ (limitWarn == "" ? "" : ", ") ~ "current-limited: " ~ roundToPrecision(iWanted, 0) ~ " A needed, " ~ roundToPrecision(supplyImaxA, 0) ~ " A limit -> only " ~ roundToPrecision(pDel / 1000, 2) ~ " kW delivered";
            }

            // ---------------------------------------------------------------------
            // 11i. Heating uniformity along the load (axial power profile)
            // Uses the shared coil field model (loops at the effective HF current
            // radius aField), each loop split into 3 filaments across the tube's
            // inner face so the current is not a single line. Axial field Hz at
            // the load surface radius.
            // Heating per unit length ~ Hz^2 (thick body, r/delta >> 1).
            // Smoothed profile = average over exactly one pitch (turn ripple removed).
            // Empty-coil field: the load's own end effects and end-face heating
            // are NOT included. This is heat generated, not temperature.
            // ---------------------------------------------------------------------
            var uniformZoneMm = 0;
            var endRatioPct = 0;
            var zoneMinMaxPct = 0;
            var ripplePct = 0;
            var uniformWhole = false;
            var heatSpan = heatZmax - heatZmin;
            var hasUniformity = addSample && heatSpan > 0 && heatR > 0;

            if (hasUniformity)
            {
                // Shared coil field model (see the load coupling above), 3 filaments
                var hzAt = function(z) { return coilHz(heatR, z, true); };

                // Sample the profile (about 8 points per pitch, limited for big coils)
                var nPts = ceil(heatSpan / (pitchM / 8)) + 1;
                nPts = min(max(nPts, 41), 401);
                nPts = min(nPts, max(41, floor(60000 / (27 * size(loopZ)))));
                var dzS = heatSpan / (nPts - 1);

                // pRaw = local heating; pSm = average over exactly one pitch
                // (8 midpoint samples, only those inside the load)
                var zs = [];
                var pRaw = [];
                var pSm = [];
                for (var i = 0; i < nPts; i += 1)
                {
                    var zP = heatZmin + dzS * i;
                    var hP = hzAt(zP);
                    zs = append(zs, zP);
                    pRaw = append(pRaw, hP * hP);

                    var sumP = 0;
                    var cntP = 0;
                    for (var q = 0; q < 8; q += 1)
                    {
                        var zQ = zP + ((q + 0.5) / 8 - 0.5) * pitchM;
                        if (zQ >= heatZmin - 1e-12 && zQ <= heatZmax + 1e-12)
                        {
                            var hQ = hzAt(zQ);
                            sumP += hQ * hQ;
                            cntP += 1;
                        }
                    }
                    pSm = append(pSm, (cntP > 0) ? sumP / cntP : hP * hP);
                }

                // Reference = coil center (or the nearest point of the load)
                var zRef = min(max(0, heatZmin), heatZmax);
                var ic = 0;
                for (var i = 1; i < nPts; i += 1)
                {
                    if (abs(zs[i] - zRef) < abs(zs[ic] - zRef)) { ic = i; }
                }
                var pC = pSm[ic];
                var band = uniformBandPct / 100;

                // Uniform zone: walk out from the center while within +/- band
                var iLo = ic;
                while (iLo > 0 && abs(pSm[iLo - 1] / pC - 1) <= band) { iLo -= 1; }
                var iHi = ic;
                while (iHi < nPts - 1 && abs(pSm[iHi + 1] / pC - 1) <= band) { iHi += 1; }
                uniformZoneMm = (zs[iHi] - zs[iLo]) * 1000;
                uniformWhole = (iLo == 0 && iHi == nPts - 1);

                // Ends of the load vs center (the colder end)
                endRatioPct = min(pSm[0], pSm[nPts - 1]) / pC * 100;

                // Target zone (centered on zRef): weakest / strongest
                var tHalf = (targetZone / meter) / 2;
                var zoneMin = -1;
                var zoneMax = 0;
                for (var i = 0; i < nPts; i += 1)
                {
                    if (abs(zs[i] - zRef) <= tHalf + 1e-12)
                    {
                        if (zoneMin < 0 || pSm[i] < zoneMin) { zoneMin = pSm[i]; }
                        if (pSm[i] > zoneMax) { zoneMax = pSm[i]; }
                    }
                }
                zoneMinMaxPct = (zoneMax > 0) ? zoneMin / zoneMax * 100 : 100;

                // Turn ripple: raw profile within one pitch of the center
                var rMin = -1;
                var rMax = 0;
                for (var i = 0; i < nPts; i += 1)
                {
                    if (abs(zs[i] - zRef) <= pitchM / 2 + 1e-12)
                    {
                        if (rMin < 0 || pRaw[i] < rMin) { rMin = pRaw[i]; }
                        if (pRaw[i] > rMax) { rMax = pRaw[i]; }
                    }
                }
                ripplePct = (rMax > 0) ? (rMax - rMin) / rMax * 100 : 0;
            }

            // ---------------------------------------------------------------------
            // 11i-2. Thermal estimate (lumped heat balance + radial soak time)
            // Power into the load = pWork (room-temperature electrical model).
            // Losses at load temperature T (K), ambient Ta (K):
            //   Sides: bare  -> eps*sigma*2*pi*R*L*(T^4 - Ta^4)
            //          insul -> 2*pi*k_ins*L*(T - Ta) / ln((R + t)/R)
            //                   (outer face of the insulation taken at ambient)
            //   Ends (both faces): radiation, or k_ins*A*(T - Ta)/t if insulated
            //   Extra draw: user value at the target, scaled with (T - Ta)
            // Max temperature: T where losses = pWork (bisection).
            // Heat-up: t = integral of m*cp dT / (pWork - loss(T)), lumped.
            // Soak (a): after a full-power heat-up the core trails by the net-power
            //   lag; it decays as 1.108*lag*exp(-lambda1^2 * Fo) (parabolic profile).
            // Soak (b), worst case: surface held at the target from a uniform start; the center
            //   of an infinite cylinder lags by C1*exp(-lambda1^2 * Fo)
            //   (lambda1 = 2.4048, C1 = 1.6020, one-term series, Fo = alpha*t/R^2).
            //   Soak = time for that lag to fall within the tolerance band.
            // Full-power lag: quasi-steady center-to-surface difference
            //   q''*R/(2k) while heating at full power (q'' = pWork / side area).
            // ---------------------------------------------------------------------
            var hasThermal = thermalEst && thermR > 0 && thermL > 0;
            var coilAbsorbW = 0;       // heat from the hot work into the coil water at the target (W)
            var waterRiseHot = 0;      // water temperature rise at the target (copper loss + that heat)
            var tMaxC = 0;
            var tMaxCapped = false;
            var heatupS = -1;           // -1 = target not reachable
            var soakS = 0;             // after full-power heat-up
            var soakColdS = 0;         // worst case, from cold
            var lagNetC = 0;
            var lagC = 0;
            var lossTargetW = 0;
            var tolC = 0;
            var holdInKW = 0;

            if (hasThermal)
            {
                var sigmaSB = 5.670374e-8;
                var taK = thermAmbC + 273.15;
                var ttK = thermTargetC + 273.15;
                var areaSide = 2 * PI * thermR * thermL;
                var areaEnds = 2 * PI * thermR * thermR;

                // Insulation (Kaowool-type ceramic fiber blanket): k rises steeply
                // with temperature (radiation through the fibers). Typical 128 kg/m3
                // blanket, k vs mean temperature: 260 C 0.06, 538 C 0.12, 816 C 0.21,
                // 1093 C 0.30 W/m-K (linear between, extrapolated). The user value
                // is taken as k at a 600 C mean and the curve is scaled to it.
                // Before, k was constant, so at the very high temperatures where the
                // max-temperature balance lands the insulation was far too good.
                var kTabT = [260, 538, 816, 1093];
                var kTabK = [0.06, 0.12, 0.21, 0.30];
                var kCurve = function(tmC) {
                    var j = 0;
                    while (j < 2 && tmC > kTabT[j + 1]) { j += 1; }
                    var kk = kTabK[j] + (kTabK[j + 1] - kTabK[j]) * (tmC - kTabT[j]) / (kTabT[j + 1] - kTabT[j]);
                    return max(kk, 0.02);
                };
                var kRef = kCurve(600);
                var kIns = function(tHotK, tColdK) { return thermInsK * kCurve((tHotK + tColdK) / 2 - 273.15) / kRef; };

                // Heat through insulation of resistance-per-k rK (K/W per W/m-K), outer
                // area aOut: outer face temperature found so that conduction =
                // convection (10 W/m2-K) + radiation (fiber emissivity 0.9) to the
                // surroundings and the water-cooled coil at ambient (bisection).
                // Before, a fixed 15 W/m2-K outer film was used, which at high outer
                // temperatures badly understated the outer loss.
                var insFlow = function(tK, rK, aOut) {
                    var lo = taK;
                    var hi = tK;
                    for (var it2 = 0; it2 < 40; it2 += 1)
                    {
                        var ts = (lo + hi) / 2;
                        var qIn = (tK - ts) * kIns(tK, ts) / rK;
                        var qOut = aOut * (10 * (ts - taK) + 0.9 * sigmaSB * (ts * ts * ts * ts - taK * taK * taK * taK));
                        if (qIn > qOut) { lo = ts; } else { hi = ts; }
                    }
                    var tsF = (lo + hi) / 2;
                    return (tK - tsF) * kIns(tK, tsF) / rK;
                };
                var rOutI = thermR + thermInsT;
                var sideIns = function(tK) { return insFlow(tK, log(rOutI / thermR) / (2 * PI * thermL), 2 * PI * rOutI * thermL); };
                var endsIns = function(tK) { return insFlow(tK, thermInsT / areaEnds, areaEnds); };

                var lossAt = function(tK) {
                    var dT = tK - taK;
                    var rad4 = tK * tK * tK * tK - taK * taK * taK * taK;
                    // Bare: radiation + natural convection (10 W/m2-K)
                    var hConv = 10;
                    var side = (thermInsT > 0) ? sideIns(tK) : thermEps * sigmaSB * areaSide * rad4 + hConv * areaSide * dT;
                    var ends = (thermInsEnds && thermInsT > 0) ? endsIns(tK) : thermEps * sigmaSB * areaEnds * rad4 + hConv * areaEnds * dT;
                    var extra = thermExtraW * dT / (ttK - taK);
                    return side + ends + extra;
                };

                // Max temperature (bisection, capped at ambient + 5000 K)
                var thLo = taK;
                var thHi = taK + 5000;
                if (lossAt(thHi) < pWork)
                {
                    tMaxCapped = true;
                    thLo = thHi;
                }
                else
                {
                    for (var thIt = 0; thIt < 60; thIt += 1)
                    {
                        var thMid = (thLo + thHi) / 2;
                        if (lossAt(thMid) < pWork) { thLo = thMid; } else { thHi = thMid; }
                    }
                }
                tMaxC = thLo - 273.15;
                lossTargetW = lossAt(ttK);

                // Heat from the hot work that lands in the coil water at the target:
                // the side faces the coil, so its radiation (bare) or the heat through
                // the insulation is absorbed by the water-cooled turns
                var r4T = ttK * ttK * ttK * ttK - taK * taK * taK * taK;
                coilAbsorbW = (thermInsT > 0) ? sideIns(ttK) : thermEps * sigmaSB * areaSide * r4T;
                waterRiseHot = (pCopper + coilAbsorbW) / (flowQ * rhoW * 4180);

                // Heat-up time to the target (lumped), midpoint rule in temperature
                // Crucible: real crucible + charge heat capacity (the whole cylinder
                // was counted as solid charge before, several times too much)
                var heatCap = (thermCapJK > 0) ? thermCapJK : thermP.d * PI * thermR * thermR * thermL * thermP.cp;   // J/K
                if (lossTargetW < pWork)
                {
                    var nT = 400;
                    var dTs = (ttK - taK) / nT;
                    var tSum = 0;
                    for (var i = 0; i < nT; i += 1)
                    {
                        var tMid = taK + (i + 0.5) * dTs;
                        tSum += heatCap * dTs / (pWork - lossAt(tMid));
                    }
                    heatupS = tSum;
                }

                // Core lag while heating at full power from cold (quasi-steady)
                lagC = (pWork / areaSide) * thermR / (2 * thermP.k);

                // Soak time for the core to be within tolerance of the target.
                //  (a) After a full-power heat-up (how the rig is run): when the
                //      surface reaches the target the profile is about parabolic,
                //      core behind by the net-power lag (pWork - loss at target).
                //      Center of a parabolic profile decays as
                //      A1 * lag * exp(-lambda1^2 * Fo), A1 = 8 / (lambda1^3 * J1(lambda1)) = 1.108
                //  (b) Worst case, uniform cold body with its surface suddenly held
                //      at the target: C1 * (target - start) * exp(...), C1 = 1.602
                var alpha = thermP.k / (thermP.d * thermP.cp);
                var lam1 = 2.4048;
                var tauR = thermR * thermR / (alpha * lam1 * lam1);       // s
                tolC = max(thermTolPct / 100 * abs(thermTargetC), 0.01);   // guard: 0 C target
                if (heatupS >= 0)
                {
                    lagNetC = max(pWork - lossTargetW, 0) / areaSide * thermR / (2 * thermP.k);
                    soakS = (1.108 * lagNetC > tolC) ? tauR * log(1.108 * lagNetC / tolC) : 0;
                }
                var thFrac = tolC / (thermTargetC - thermAmbC);
                soakColdS = (thFrac < 1.602) ? tauR * log(1.602 / thFrac) : 0;

                // Supply power needed to hold the target (losses / efficiency)
                holdInKW = (eff > 0) ? lossTargetW / eff / 1000 : 0;
            }

            // ---------------------------------------------------------------------
            // 11j. Variables (always available in the variable table)
            // ---------------------------------------------------------------------
            setVariable(context, "coilInductanceLoaded_uH", hfLoadedH * 1e6);
            setVariable(context, "leadInductance_uH", leadsH * 1e6);
            setVariable(context, "tankFreqEmpty_kHz", fEmpty / 1000);
            setVariable(context, "tankFreqLoaded_kHz", fLoaded / 1000);
            setVariable(context, "efficiency_pct", eff * 100);
            setVariable(context, "efficiencyCharge_pct", effCharge * 100);
            setVariable(context, "workPower_kW", pWork / 1000);
            setVariable(context, "copperLoss_kW", pCopper / 1000);
            setVariable(context, "waterTempRise_C", waterRise);
            setVariable(context, "waterTempRiseAtTarget_C", waterRiseHot);
            setVariable(context, "coilQ", qFactor);
            setVariable(context, "coilCurrent_A", iCoil);
            setVariable(context, "tankVoltage_V", vTank);
            setVariable(context, "coolantDeltaP_kPa", dP / 1000);
            setVariable(context, "coolantVelocity_mps", vel);
            setVariable(context, "tubeLength_mm", pathLenTotal * 1000);
            setVariable(context, "heatUniformZone_mm", uniformZoneMm);
            setVariable(context, "heatZoneMinMax_pct", zoneMinMaxPct);
            setVariable(context, "heatEndRatio_pct", endRatioPct);
            setVariable(context, "heatRipple_pct", ripplePct);
            setVariable(context, "thermMaxTemp_C", tMaxC);
            setVariable(context, "thermHeatup_s", heatupS);
            setVariable(context, "thermSoak_s", soakS);
            setVariable(context, "thermSoakFromCold_s", soakColdS);
            setVariable(context, "thermLossAtTarget_W", lossTargetW);
            setVariable(context, "thermFullPowerLag_C", lagC);
            setVariable(context, "thermHoldSupply_kW", holdInKW);

            // ---------------------------------------------------------------------
            // 11k. Message text (pick one topic, or All)
            // ---------------------------------------------------------------------
            var sampleText = addSample ? " · with load " ~ roundToPrecision(hfLoadedH * 1e6, 3) ~ " µH" : "";
            var leadsText = " · leads " ~ roundToPrecision(leadsH * 1e6, 3) ~ " µH";
            if (leadsTogether)
            {
                leadsText = leadsText ~ " (together, " ~ ((leadJoin == CoilLeadJoin.STACKED) ? "stacked" : ((leadJoin == CoilLeadJoin.OWN_HEIGHTS) ? "own heights" : "side by side")) ~
                            " " ~ roundToPrecision(tgSpacing * 1000, 1) ~ " mm c-c, joined " ~
                            roundToPrecision(tgJoin * 1000, 1) ~ " mm from coil OD" ~ (tgWrap ? ", wrapped" : "") ~
                            (tgAround ? ", around coil +" ~ roundToPrecision(tgExtRevs, 2) ~ " turns" : "") ~ ")";
            }

            var tankText = "Tank @ " ~ roundToPrecision(tankCapUF, 3) ~ " µF: " ~ roundToPrecision(fEmpty / 1000, 1) ~ " kHz";
            if (addSample)
            {
                tankText = tankText ~ " empty → " ~ (noResonance ? "no resonance" : roundToPrecision(fLoaded / 1000, 1) ~ " kHz loaded");
            }
            tankText = tankText ~ ((supplyFminKHz > 0 || supplyFmaxKHz > 0)
                                   ? " (supply " ~ roundToPrecision(supplyFminKHz, 0) ~ "–" ~ roundToPrecision(supplyFmaxKHz, 0) ~ " kHz)" : "") ~
                       " · coil current " ~ roundToPrecision(iCoil, 0) ~ " A" ~
                       ((supplyImaxA > 0) ? " (limit " ~ roundToPrecision(supplyImaxA, 0) ~ " A)" : "") ~
                       " · tank voltage " ~ roundToPrecision(vTank, 0) ~ " V at " ~ roundToPrecision(supplyKW, 2) ~ " kW" ~
                       (addSample ? "" : " (no workpiece: all power in copper)") ~
                       ((limitWarn != "") ? " · WARNING: " ~ limitWarn : "");

            var effText;
            if (addSample)
            {
                effText = "Efficiency " ~ roundToPrecision(eff * 100, 1) ~ "% @ " ~ roundToPrecision(fLoaded / 1000, 1) ~ " kHz";
                if (isCrucible)
                {
                    effText = effText ~ " (charge " ~ roundToPrecision(effCharge * 100, 1) ~ "%, crucible " ~
                              roundToPrecision((eff - effCharge) * 100, 1) ~ "%)";
                }
                effText = effText ~ " · " ~ roundToPrecision(pDel / 1000, 2) ~ " kW in → " ~ roundToPrecision(pWork / 1000, 2) ~
                          " kW to work, " ~ roundToPrecision(pCopper / 1000, 2) ~ " kW copper loss (water +" ~
                          roundToPrecision(waterRise, 1) ~ " °C) · Q " ~ roundToPrecision(qFactor, 1);
            }
            else
            {
                effText = "Efficiency: add a dummy sample to estimate";
            }

            if (partMatText != "")
            {
                effText = effText ~ " · " ~ partMatText;
            }
            if (addSample && limitWarn != "")
            {
                effText = effText ~ " · WARNING: " ~ limitWarn;
            }

            var velNote = (vel > 3) ? " (high – erosion risk)" : "";
            var coolText = "Cooling @ " ~ roundToPrecision(flowLpm, 2) ~ " L/min: ΔP " ~ roundToPrecision(dP / 1000, 1) ~
                           " kPa (" ~ roundToPrecision(dP / 6894.76, 1) ~ " psi) · " ~ roundToPrecision(vel, 2) ~ " m/s" ~ velNote ~
                           " (friction " ~ roundToPrecision(dPfric / 1000, 1) ~ " + bends " ~ roundToPrecision(bendDeg, 0) ~
                           "° & tube ends " ~ roundToPrecision(dPbends / 1000, 1) ~ " kPa)" ~
                           " · Re " ~ roundToPrecision(reyn, 0) ~ " (coil " ~ regime ~ ", leads " ~ regimeStraight ~ ")" ~
                           " · tube " ~ roundToPrecision(pathLenTotal * 1000, 0) ~ " mm" ~
                           (isRectangular ? " · hydraulic dia " : " · bore ") ~ roundToPrecision(dH * 1000, 2) ~ " mm" ~
                           " · water +" ~ roundToPrecision(waterRise, 1) ~ " °C from copper loss" ~
                           (hasThermal ? " (+" ~ roundToPrecision(waterRiseHot, 1) ~ " °C at target incl. heat from the hot work)" : "");

            var uniText;
            if (hasUniformity)
            {
                var loadName = isCrucible ? "charge" : "sample";
                var zoneText = uniformWhole ? "whole " ~ loadName ~ " (" ~ roundToPrecision(uniformZoneMm, 1) ~ " mm)"
                                            : roundToPrecision(uniformZoneMm, 1) ~ " mm";
                uniText = "Heating uniformity: within ±" ~ roundToPrecision(uniformBandPct, 1) ~ "% over " ~ zoneText ~
                          " · " ~ roundToPrecision(targetZone / millimeter, 1) ~ " mm target zone min/max " ~
                          roundToPrecision(zoneMinMaxPct, 1) ~ "% · " ~ loadName ~ " ends " ~ roundToPrecision(endRatioPct, 1) ~
                          "% of center · turn ripple " ~ roundToPrecision(ripplePct, 1) ~ "% (heat generated, not temperature)";
            }
            else if (addSample)
            {
                uniText = "Heating uniformity: no charge in the crucible";
            }
            else
            {
                uniText = "Heating uniformity: add a dummy sample to estimate";
            }

            var fmtTime = function(sec) {
                if (sec < 120) { return roundToPrecision(sec, 0) ~ " s"; }
                return roundToPrecision(sec / 60, 1) ~ " min";
            };

            var thermText;
            if (hasThermal)
            {
                var maxText = tMaxCapped ? "> " ~ roundToPrecision(tMaxC, 0) ~ " °C" : "~" ~ roundToPrecision(tMaxC, 0) ~ " °C";
                var heatText = (heatupS >= 0) ? "heat-up to " ~ roundToPrecision(thermTargetC, 0) ~ " °C ~" ~ fmtTime(heatupS)
                                              : "target " ~ roundToPrecision(thermTargetC, 0) ~ " °C NOT reachable (losses " ~
                                                roundToPrecision(lossTargetW / 1000, 2) ~ " kW > " ~ roundToPrecision(pWork / 1000, 2) ~ " kW in)";
                var soakText = (heatupS >= 0) ? "soak " ~ fmtTime(soakS) ~ " after full-power heat-up (" ~ fmtTime(soakColdS) ~ " worst case from cold)"
                                              : "soak from cold " ~ fmtTime(soakColdS);
                thermText = "Thermal: max " ~ maxText ~ " at " ~ roundToPrecision(pWork / 1000, 2) ~ " kW in · " ~ heatText ~
                            " · loss at target " ~ roundToPrecision(lossTargetW / 1000, 2) ~ " kW · " ~ soakText ~
                            " for core within ±" ~ roundToPrecision(thermTolPct, 2) ~ "% (±" ~ roundToPrecision(tolC, 1) ~
                            " °C) · core lags ~" ~ roundToPrecision(lagC, 0) ~ " °C at full power";
                if (heatupS >= 0)
                {
                    thermText = thermText ~ " · hold ~" ~ roundToPrecision(holdInKW, 2) ~ " kW supply (" ~
                                roundToPrecision(holdInKW / supplyKW * 100, 0) ~ "%)";
                }
                if (thermP.melt > 0 && tMaxC >= thermP.melt)
                {
                    thermText = thermText ~ " · WARNING: full power can exceed ~" ~ thermP.melt ~
                                " °C (melting) – limit power / add over-temp cutoff";
                }
                thermText = thermText ~ " · coil water +" ~ roundToPrecision(waterRiseHot, 1) ~ " °C at target (incl. " ~
                            roundToPrecision(coilAbsorbW / 1000, 2) ~ " kW from the hot work)";
                thermText = thermText ~ " (lumped estimate)";
                if (partMatText != "")
                {
                    thermText = thermText ~ " · " ~ partMatText;
                }
            }
            else if (addSample)
            {
                thermText = "Thermal: tick Thermal estimate (below Dummy sample)";
            }
            else
            {
                thermText = "Thermal: add a dummy sample to estimate";
            }

            if (estimateDisplay == CoilEstimateDisplay.INDUCTANCE)
            {
                msg = inductanceText ~ sampleText ~ leadsText ~ " · " ~ roundToPrecision(nTurns, 2) ~ " turns";
            }
            else if (estimateDisplay == CoilEstimateDisplay.UNIFORMITY)
            {
                msg = uniText;
            }
            else if (estimateDisplay == CoilEstimateDisplay.THERMAL)
            {
                msg = thermText;
            }
            else if (estimateDisplay == CoilEstimateDisplay.TANK)
            {
                msg = tankText ~ " · total L " ~ roundToPrecision((hfLoadedH + leadsH) * 1e6, 3) ~ " µH";
            }
            else if (estimateDisplay == CoilEstimateDisplay.EFFICIENCY)
            {
                msg = effText;
            }
            else if (estimateDisplay == CoilEstimateDisplay.COOLING)
            {
                msg = coolText;
            }
            else
            {
                msg = inductanceText ~ sampleText ~ leadsText ~ "   |   " ~ tankText ~ "   |   " ~ effText ~ "   |   " ~ coolText ~ "   |   " ~ uniText ~ "   |   " ~ thermText;
            }
        }

        reportFeatureInfo(context, id, msg);

        // =========================================================================
        // 12. NAMES & COLORS (done last so later operations can't reset them)
        // =========================================================================
        var coilBody = coilBodyQ;
        setProperty(context, {
            "entities" : coilBody,
            "propertyType" : PropertyType.NAME,
            "value" : "Coil"
        });
        setProperty(context, {
            "entities" : coilBody,
            "propertyType" : PropertyType.APPEARANCE,
            "value" : color(184 / 255, 115 / 255, 51 / 255)      // #B87333
        });

        var steelGray = color(168 / 255, 169 / 255, 173 / 255);   // #A8A9AD

        if (!isQueryEmpty(context, sampleQuery))
        {
            setProperty(context, { "entities" : sampleQuery, "propertyType" : PropertyType.NAME, "value" : "Sample" });
            setProperty(context, { "entities" : sampleQuery, "propertyType" : PropertyType.APPEARANCE, "value" : steelGray });
        }
        if (!isQueryEmpty(context, crucibleQuery))
        {
            setProperty(context, { "entities" : crucibleQuery, "propertyType" : PropertyType.NAME, "value" : "Crucible" });
            setProperty(context, { "entities" : crucibleQuery, "propertyType" : PropertyType.APPEARANCE, "value" : crucibleColor });
        }
        if (!isQueryEmpty(context, chargeQuery))
        {
            setProperty(context, { "entities" : chargeQuery, "propertyType" : PropertyType.NAME, "value" : "Charge" });
            setProperty(context, { "entities" : chargeQuery, "propertyType" : PropertyType.APPEARANCE, "value" : steelGray });
        }
    });
```
