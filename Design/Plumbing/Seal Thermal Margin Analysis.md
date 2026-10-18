---
subsystem: plumbing
tags: [design, plumbing, sealing, thermal, analysis]
---

# Seal Thermal Margin Analysis

Thermal analysis for the [[Design/Plumbing/Vertical Sliding Shaft Seal|Vertical Sliding Shaft Seal]] (McMaster 5154T48, Buna-N, rated -40°F to 210°F). Goal: estimate how close the seal runs to its thermal limit during a heat/quench cycle, so a live "thermal margin" gauge can be built, and decide whether the seal/joint needs redesign.

## Conduction path geometry (from CAD mass properties + section drawing)

Sample → seal, bottom to top:

| Zone | Material | Length | Cross-section | Mass |
|---|---|---|---|---|
| Orange wedge (sample contact, 2×21.308mm² contact patches) | Boron nitride | 3.175mm | ~219.2mm² avg | 1.392g |
| Gold ring | Boron nitride | 4.175mm | ~118.1mm² avg | 0.986g |
| Teal main body | Boron nitride | 26.65mm | ~313.8mm² (≈full ⌀20mm) | 16.724g |
| Purple cap | Boron nitride | 10.0mm | ~292.9mm² (bore for screw/dowel) | 5.858g |
| Screw (1/4-20 × 0.4"L) | Steel | — | — | 2.486g |
| Shaft (rod to seal) | Steel | **~1.0–1.5in** (seal sits this far from ceramic holder — confirmed 1.0in used as conservative case) | ⌀12.7mm (126.7mm²) | ~25.3g @ 1.0in |
| **Seal** | Buna-N | — | — | T_max = **210°F** |

BN density backs out to ~2.0 g/cm³ across all 4 zones (consistent, hot-pressed BN, previously used for hot-rolling mill guides per user).

Note: the shaft is actually ~1ft long overall, but the seal itself sits only ~1.0-1.5in from the ceramic holder — the remaining ~11in of rod is downstream of the seal and acts as a secondary heat sink, not part of the primary resistive path (see 2-node model below).

## Lumped RC thermal model

Single-node model: `dT_seal/dt = (T_sample(t) - T_seal(t)) / τ`

- `R_total = R_BN + R_shaft = ΣL/(k·A)` per zone, in series
- `C_total = Σ(mass × specific heat)` per zone
- `τ = R_total × C_total`

**Closed-form per segment** (hold or ramp), carrying `T_seal` forward between segments (compounding — this is essential, not optional):
- Hold at `T_h` for duration `Δt`, starting from `T_start`: `T(Δt) = T_h + (T_start - T_h)·e^(-Δt/τ)`
- Ramp from `A` at rate `r` for duration `Δt`, starting from `T_start`: `T(Δt) = A + r·(Δt-τ) + (T_start - A + r·τ)·e^(-Δt/τ)`

**Important correction found during derivation**: the peak within a segment is not always at its endpoints. A ramp-down segment where `T_seal` starts cooler than the segment's own starting input value can produce an interior local maximum (seal keeps heating while sample is already cooling, until it catches up). For a LabVIEW implementation, sample the full curve within each segment (e.g. via Generate Preview Curve.vi) rather than checking only segment endpoints, then take the array max over the whole run.

### Assumed material properties (used before real data was available)
- k_BN: swept 20-70 W/m·K (anisotropic, grade unknown — was previously used for hot-rolling guides)
- k_steel: swept 16-45 W/m·K (exact "6440 steel" grade unconfirmed — could be 440-series stainless ~24, 304/316 ~16, or carbon steel ~45)
- c_BN ≈ 800 J/kg·K, c_steel ≈ 470 J/kg·K, ρ_BN ≈ 2000 kg/m³, ρ_steel ≈ 7850 kg/m³

### Sensitivity sweep result (mock cycle: 20°C→800°C in 30s, hold 2min, quench to 20°C, L=1.0in to seal)
Across the full plausible k_BN × k_steel grid, **margin was negative in every combination** (range: -83% to -358%). This is a robust finding — the conclusion (seal exceeds rating) doesn't flip sign across the material uncertainty, even though the magnitude does.

## Real calibration data (supersedes the modeled sweep)

**Actual thermocouple measurement at the seal location**: with the sample held at 850°C, the seal location stabilized at **~120°C (248°F)** — this **already exceeds the seal's 210°F/99°C rating** at steady state, even with the original as-built hardware (no heat-break installed). (Corrected 2026-09-18 — an earlier version of this note used 100°C; 120°C is the confirmed real value.)

This is a smaller gap from the modeled sweep (predicting 328-554°F for a much shorter 2-min hold) than the earlier 100°C figure suggested, but the real assembly still clearly has heat-loss paths (convection/radiation off the shaft, larger downstream thermal mass) that the simple closed conduction-path model omits — the model is still too pessimistic, just not as dramatically as first calculated.

**Calibrated steady-state gain** (usable without knowing k_BN/k_steel at all):
```
f = (T_seal_ss - T_ambient) / (T_sample_ss - T_ambient) = (120-20)/(850-20) = 0.1205
```

**Recommended gauge formula** (conservative upper bound, no time-constant/material data needed):
```
T_seal_worst_case = T_ambient + 0.1205 × (T_hold - T_ambient)
margin% = (210°F - T_seal_worst_case) / (210°F - T_ambient) × 100
```
This works because T_seal can never exceed its steady-state asymptote during a finite hold (monotonic approach) — so this is a guaranteed upper bound per hold segment, and ramp segments are short enough not to matter much next to holds.

## Calibrated ("more realistic") estimate for the simulated heat curve

The modeled sweep above (BN, assumed k, no heat-break) gave -83% to -358% margin for the 800°C/2min-hold mock cycle — but that model has no path for heat to escape to ambient (pure series conduction only), so it structurally can't reproduce the real measured behavior. Applying the **calibrated steady-state gain** from the real thermocouple test to that same mock cycle instead:

```
f = (T_seal_ss - T_ambient) / (T_sample_ss - T_ambient) = (248°F - 68°F) / (1562°F - 68°F) = 0.1205
T_seal_worst_case = T_ambient + f × (T_hold - T_ambient) = 68 + 0.1205 × (1472 - 68) = 237.2°F (114.0°C)
margin% = (210 - 237.2) / (210 - 68) × 100 = -19.1%
```

**This is still a much smaller overshoot than the modeled sweep** (-19.1% vs. -83% to -358%) for the *exact same* 800°C/2min-hold cycle, but unlike the earlier 100°C-based calculation, **this is now a genuinely negative margin, not a positive one** — the calibrated, real-data-anchored estimate confirms the seal is over its rating for this cycle on the as-built hardware, just less severely than the unanchored model claimed. The reason for the remaining gap between calibrated and modeled numbers is the same as before: this estimate correctly accounts for the ambient heat-loss path (convection/radiation off the shaft and surrounding hardware) that the pure series-conduction model omits entirely.

**Important scope caveat**: this calibration was measured on the **original, as-built hardware** — boron nitride holder, whatever standoff distance currently exists, no heat-break installed. It is **not** validated for the Titanium or Lava holder variants, or for a longer standoff, since those configurations haven't been physically tested. Use this calibrated number as the "realistic baseline for what's already built," and the modeled sweep's *relative* improvements (e.g. the ~80-160% resistance increase from the joint heat-break, the ~40x conductivity drop from switching to Lava) as directionally trustworthy multipliers on top of it — not the modeled sweep's absolute peak-temperature numbers, which are calibrated to nothing. **Given the calibrated estimate is now negative even at the real-data anchor point, the heat-break/holder mitigation is not optional headroom — it's needed to bring the as-built system into positive margin at all.**

### Comparison: user-supplied real measurement vs. modeled approaches

| Source | Condition | Result |
|---|---|---|
| **Real thermocouple measurement (user-supplied)** | Sample held at 850°C (sustained/steady state), original as-built BN holder, thermocouple placed at the seal location | **Seal stabilized at ~120°C (248°F) — already -26.8% margin at this measured condition alone** |
| Modeled sweep (assumed k, pure series conduction, no ambient loss path) | 800°C sample, 2min hold, mock cycle | Peak seal temp 328-554°F (165-290°C) depending on k_BN/k_steel assumption — margin -83% to -358% |
| **Calibrated estimate (this section)** — derived directly from the real measurement above | Same 800°C/2min-hold mock cycle | **Peak seal temp 237.2°F (114.0°C) — margin -19.1%** |

The calibrated estimate (114.0°C) lands close to the user's real measured value (120°C), for a comparable but not identical condition — the real measurement was a sustained 850°C hold, the calibrated estimate applies that same ratio to a shorter 800°C/2min mock hold — which is the expected relationship, since the calibration coefficient (`f = 0.1205`) is derived directly from that 120°C measurement. The point of this comparison is to show how far off the pure series-conduction model was (328-554°F) versus anything anchored to the real data (~114-120°C) — a ~3-5x overestimate from the unanchored model.

## 2-node model (seal + downstream rod mass)

For completeness: added a second node representing the ~11in of rod beyond the seal (277.8g, C₂≈130.6 J/K), coupled via R₁₂≈68.9 K/W (half-length approximation). For a single ~155s cycle, this only reduced peak seal temp by ~10°F (400.3°F → 390.4°F) because the downstream mass's own time constant (R₁₂·C₂≈9,000s) is much longer than one cycle — it doesn't have time to help on a single fast pulse. Would matter more for long soaks or repeated back-to-back cycles without full cooldown.

## Seal replacement search — dead end, use thermal break instead

Extensively searched for a higher-temperature drop-in replacement (same ID/OD/width, ~15psi rough-vacuum rating instead of the original 50psi spec — confirmed only rough vacuum is actually needed, and only when the system is cold, not during heating):

- **McMaster wiper-lip family** (same family as 5154T48): fluoroelastomer/Viton option exists in general (rated to 390°F) but exact dimensional match not confirmed via catalog search
- **McMaster PTFE V-ring packing (9572K35)**: 500°F, 400psi — great specs, but it's a *packing system* (needs separate male/female adapters + compression retention), not a drop-in single-piece seal — would require housing redesign
- **AVX TC12.7x22.2x6.3 (Viton option)**: right size family, but only 5psi rated and 0.248" width (vs needed 0.313") — wrong performance class
- **SKF/CR 12X22X7 HMSA10 V**: real Viton lip design, but 12mm ≠ 12.7mm (1/2") — dimensional mismatch, metric/inch false-friend
- **Kurt J. Lesker FMH-50A**: real vacuum feedthrough, 1/2" shaft, fluorocarbon (Viton) O-ring — but confirmed via Lesker's own technical notes to be a **rotary-only** feedthrough (rpm-rated, bronze bushings for rotation), not validated for linear/reciprocating motion. Lesker's actual linear motion feedthroughs are bellows-sealed, a bigger architecture change than needed.

**Conclusion**: no clean catalog part combines the exact dimensions + adequate pressure rating + high temp + confirmed linear-motion service. Decision: **keep the Buna-N 5154T48 seal, fix the thermal problem at the source instead of the seal.**

## Mitigation plan (decided)

1. **Increase seal standoff distance** where practical — sensitivity check showed 1.0in→1.5in swung margin from -134% to -49% (single biggest lever found).
2. **Heat-break at the rod-to-ceramic-holder screw joint**:
   - Replace the steel 1/4-20 screw with a **titanium (Ti-6Al-4V) screw** — k≈6.7 W/m·K vs steel's 16-45 W/m·K, nonmagnetic (safe near induction field, same reasoning as the 304SS shaft), corrosion-resistant, standard stock item.
   - Add a **mica washer** between the rod end face and the ceramic holder cap face — addresses the parallel direct-contact heat path (not just the screw). Chosen over Macor/Vespel/PEEK specifically because clamp torque at this joint is very light, and mica's main weakness (delamination under shear/point loading) isn't triggered by pure light-axial-compression. Muscovite mica (common, cheap) is stable to ~500°C; phlogopite mica is available if more margin is wanted (~900-1000°C). Size washer OD generously to bridge the full rod-end-to-cap contact face, not just the screw shank, so heat can't detour around it at the edges.
3. Convective fins in the ~1in gap were evaluated and **deprioritized**: gap is inert-gas-backfilled near-atmospheric during heating (not vacuum, so convection is physically possible), but modest/achievable finning (2-3x area) only gives ~20-30% resistance reduction (natural convection h≈10 W/m²K gives R_conv≈33-49 K/W vs. axial R≈12.5-35 K/W — conduction still dominates unless fins are very aggressive, 5-8x area, which may not fit in the available annular clearance and risks weakening the shaft's structural cross-section in the actuation load path). Worth adding only as a secondary bonus if space allows, not a primary fix.

## Final configuration comparison (2026-09-17)

Full-assembly modeled results for the 800°C/2min-hold mock cycle, with the joint heat-break (Ti screw + mica washer, corrected to properly account for the screw as a parallel bypass path around the washer) plus holder material options:

| Holder material | R_total | τ | Peak T_seal | Margin |
|---|---|---|---|---|
| Boron Nitride (original, no heat-break) | 15.9 K/W | 506s | 400.7°F | -134.3% |
| Boron Nitride + Ti screw/1/4" mica joint | 41.3 K/W | 1,315s (21.9min) | 207.2°F (97.3°C) | +2.0% |
| **Titanium (Grade 5) + joint** (fallback option) | 63.1 K/W | 2,585s (43.1min) | 140.7°F (60.4°C) | **+48.8%** |
| **Alumina Silicate "Lava" + joint** (recommended) | 167.8 K/W | 5,344s (89.1min) | 103.6°F (39.8°C) | **+74.9%** |

Joint resistance note: the mica washer alone would add far more resistance than shown (e.g. 167 K/W for a 1/4" thickness) if it were the only path — but the titanium screw is a parallel bypass through the same joint, and titanium (k=6.7) still conducts ~22x better than mica (k=0.3), so despite its smaller cross-section the screw ends up carrying most of the heat through that joint. The parallel combination (mica ∥ screw) is what's shown above (25.4 K/W for the 1/4" washer case) — always compute both paths together, not the washer in isolation.

**Recommended path**: Lava holder + Ti screw + 1/4" mica washer, with Grade 5 Titanium holder as a fully-characterized fallback if Lava's thermal-shock behavior doesn't validate (see [[Design/Mechanisms/Ceramic Mount.md]] for the immersion-vs-conduction risk reassessment — holder is never directly water-quenched, only the sample is, which substantially de-risks the ceramic option).

## Active-cooled clamp mitigation (proposed, not yet validated, 2026-10-13)

A second, independent mitigation explored on top of the joint heat-break above: an actively chilled clamp (16°C coolant loop) wrapped 2in around the rod, mounted **above the seal** (outside the vacuum/argon chamber, past the lid — the seal sits in the lid, so this segment of rod is in room air, not argon). Unlike the existing joint heat-break (which raises resistance to slow heat reaching the seal), this adds a second active heat sink past the seal to pull its temperature down directly.

### Geometry — derived from real travel/packing constraints, not assumed

> **GEOMETRY CORRECTED 2026-10-15 (user-confirmed).** An earlier version of this section (a) bundled the 5mm clamp clearance into Margin (1.197in), (b) double-counted Margin in RodTop (5.394in), (c) put the 4.197in rod on the *sample* side of the seal instead of the clamp side, and (d) used a 2in clamp 3in from the seal. All formulas below are the corrected ones. **Numbers computed from the old geometry elsewhere in this note (the 3.638in CeramicL, 4.197in "Movement" standoff, 3in clamp-side rod) are superseded — see "Corrected-geometry results" below.**

**Stack at the home (heating) position, top to bottom** (chamber interior = **10in**):

| # | Section | Length |
|---|---|---|
| 1 | Clamp (rod-wrapping, 16°C water cooling, connected to the same rod; rides down with it) | **2.5in** |
| 2 | **RodTop** — rod from clamp to seal | **5mm + Margin + WaterHeight = 4.197in** |
| 3 | Seal (thickness of the lid) | 0.313in (seal width, 5154T48) — above the 10in interior |
| 4 | **RodBottom** — rod from seal down to top of ceramic | **= RodBudget − CeramicL** (see below) |
| 5 | Ceramic holder | **CeramicL** |
| 6 | Sample (exposed Charpy body) | 2.165in (55mm) |
| 7 | **Margin** — air gap, bottom of sample to water surface | **1in** |
| 8 | Water | **3in** |

```
10in         = RodBottom + CeramicL + Sample + Margin + WaterHeight
RodBottom + CeramicL = 10 − 2.165 − 1 − 3 = 3.835in          (RodBudget)
Stroke       = Margin + WaterHeight = 4.0in                   (travel to fully submerge the sample)
RodTop       = 5mm + Margin + WaterHeight = 4.197in           (the 5mm is clamp-to-lid clearance at bottom of stroke; it sits above the lid, NOT inside the 10in)
```

**Thermal-path assignment (this is what the corrected geometry changes):**
- **Sample-side rod** (ceramic top → seal) = **RodBottom** = 3.835in − CeramicL.
- **Clamp-side rod** (seal → clamp near edge) = **RodTop = 4.197in** (not 3in).
- RodBottom therefore **is** part of the sample-to-seal thermal network. (An earlier statement that it wasn't was wrong; see correction in the "Largest-margin" section.)

- **Sample = 55mm (2.165in)** — the exposed Modified Charpy body (not the 6mm head feature, which is retained inside the ceramic and doesn't count against this budget — see [[Design/Sample Quenching/Modified Charpy.md]])
- **WaterHeight = 3in** (quench bath depth)
- **Maximum buildable CeramicL = 3.835in** (RodBottom = 0). The old 3.638in figure came from putting the 5mm inside the budget — it was 0.197in short.
- **Clamp**: 2.5in long, above RodTop, same rod (the old "2in clamp, near edge 3in from seal" is superseded — the clamp's near edge is 4.197in from the seal).

The current Lava holder design is 1.73in; extending it adds length to the main/teal body zone (the lowest-resistance-per-inch zone, so this is a conservative, not optimal, allocation; see open items below).

### Thermal result (idealized conduction model, no ambient convection)

Same lumped-resistor method as the rest of this analysis, split into two legs from the seal:

**Toward the sample (seal → ceramic → sample), using Lava (k=1.265 W/m·K):**

| Segment | R |
|---|---|
| Orange wedge | 11.45 K/W |
| Gold ring | 27.98 K/W |
| Teal body (lengthened to accommodate CeramicL=3.638in) | 189.0 K/W |
| Purple cap | 26.99 K/W |
| Standoff rod (Movement=4.197in, 304SS) | 51.96 K/W |
| **Sample/ceramic contact resistance** (worst-case minimum — see below) | 75.9 K/W |
| **Total (R_sample-side)** | **≈383 K/W** |

**No joint heat-break in this design** — unlike the original mitigation plan (steel/Ti screw + mica washer between rod and ceramic cap, see "Mitigation plan" above), this active-clamp configuration omits it. Testing found the joint contributes only a small fraction of Lava's total resistance (21.85 K/W out of ~405 K/W, ~5%) — removing it costs little here, in exchange for simpler/cheaper hardware (no Ti screw, no mica washer). **This is holder-material-specific**: the same test with a Boron Nitride holder found the joint carries ~81% of total resistance there instead (BN's own zones barely insulate, k being so much higher than Lava's) — so this simplification applies to Lava, not to BN, if BN is ever reconsidered as the holder material for this mitigation.

**Toward the clamp (seal → clamp), 304SS rod, 3in, clamp assumed to hold its contact surface at exactly 16°C:**

| Segment | R |
|---|---|
| Standoff rod (3in) | **≈37 K/W** |

**Sample/ceramic contact resistance derivation**: the sample rests on the ceramic under gravity alone (no clamping) — weight = 0.424N (10×10×55mm steel, ρ=7.85g/cm³) over the documented 2×21.308mm² contact patches ≈ 9.9kPa (~1.4psi) contact pressure. Using the ideal circular-contact-spot spreading-resistance formula (R=1/(2ka) per patch, two patches in parallel) gives **75.9 K/W** — this is the *minimum physically possible* resistance at that joint (perfect full-area contact); real resistance at such light, unclamped pressure is almost certainly higher (less heat gets through), so this value is the conservative/worst-case choice for that term specifically.

**Result**, with T_sample=1000°C (above the documented real process peak of 800-850°C) and T_clamp=16°C:

```
T_seal = 1000 − [383/(383+37)] × (1000−16) ≈ 103°C
```

**Against the seal's ~99-100°C rating, this is right at the edge — slightly over, not passing.**

### Caveats on this estimate

- **This is a pure-conduction, no-convection model** — the same class of model that this file's own earlier sections showed running dramatically hotter than reality (328-554°F modeled vs. 120°C real, pre-heat-break). 103°C should be read as a pessimistic ceiling on that axis, not a confirmed number.
- **The sample-contact resistance (76 K/W) is a worst-case minimum** — real resistance, given the light gravity-only contact, is likely higher, which would improve (lower) the real T_seal further.
- **The clamp-side assumption is the opposite — an idealized best case.** R_up=37 K/W only models rod conduction; it assumes the clamp's contact surface sits at exactly 16°C with zero added resistance for (a) rod-to-clamp contact and (b) clamp-to-coolant convection. Neither has been estimated — the clamp's actual contact design (split-block? clamping force? thermal interface material?) and the chiller's real behavior under load are unknowns that could push the real T_seal higher than 103°C if the clamp underperforms this ideal.
- **Net effect**: one side of the estimate is conservatively pessimistic, the other is optimistically idealized — they don't necessarily cancel, and the real number could land on either side of 103°C.

### Open items from this mitigation

- **Lava holder structural check needed**: 3.638in is a substantial extension from the current 1.73in design. Lava's modest strength (2,500psi tensile per the McMaster 8479K69 datasheet; 10,000psi flexural per [[Design/Plumbing/Material Properties Reference.md]]) hasn't been checked against this length under the actuator's motion loads.
- **Clamp contact/coolant-side resistance unquantified** — see caveat above; needed to know whether 103°C is realistic or optimistic.
- **Teal-body-only lengthening is not the most resistance-efficient allocation** — the narrower gold-ring zone offers ~2.7x more resistance per inch of added length (170 K/W/in vs. 64 K/W/in for teal), if that zone can mechanically be lengthened instead. Not pursued here per explicit preference for the simpler teal-only change, but flagged as a lower-cost-of-length option if more margin is needed later.
- **Without the joint, this result (103°C) is slightly over the seal's rating** — the joint removal traded a small amount of margin (98°C→103°C) for simpler/cheaper hardware. If margin is needed back, re-adding the joint recovers it (~5°C), or other levers in this section (standoff, clamp distance, holder length) can be revisited.
- **Recommended next step, consistent with how every other thermal question in this file got actually resolved**: bench test this exact geometry (CeramicL=3.638in, Movement=4.197in, clamp 3in from seal at 16°C, no joint) with a thermocouple at the seal, rather than refining the hand calculation further.

## 101 Copper rod + active-cooled clamp — Lava vs. BN holder comparison (2026-10-13)

Same active-clamp geometry and 16°C clamp assumption as the section above, but the rod material is swapped from 304SS (k=16.2 W/m·K) to **101 Copper** (k=391 W/m·K, see [[Design/Plumbing/Material Properties Reference.md]], McMaster 8965K82, 1/2" dia.), evaluated for both the Lava and Boron Nitride holder options. Goal: find the margin against the seal's **100°C** limit, and the sample temperature at which that margin hits zero.

> **CORRECTION (2026-10-13, caught during follow-on sanity check)**: the "75.9 K/W" sample/ceramic contact (spreading) resistance used below was **not actually material-independent** as originally labeled. Reverse-engineering the spreading-resistance formula (`R=1/(2ka)` per patch, two 21.308mm² patches in parallel, a=2.605mm) shows 75.9 K/W only comes out of that formula for **k=1.265 W/m·K — exactly Lava's conductivity**. It was computed for spreading into Lava and wrongly reused unchanged for the BN rows (and, further down, for a hypothetical zero-ceramic/direct-copper-contact case). **The BN results and the CeramicL=0 conclusion below are corrected in-place; Lava's numbers are unaffected** (its contact resistance was always correctly tied to its own k).
>
> **SECOND CORRECTION (2026-10-15):** the 75.9 K/W value itself is ~2× too high — the per-patch formula should be `1/(4ka)` on the ceramic side (plus a small steel-side term), not `1/(2ka)`. Corrected Lava contact ≈ **39.5 K/W**. Every number in this section that uses 75.9 (or a k-scaling of it) is affected. See "BUG FIX + FINAL RESULTS" under "Corrected-geometry results" below.

**Explicit assumption, re-confirmed**: this configuration has **no joint heat-break** — no titanium screw, no mica washer. The ceramic purple cap bears directly on the rod end face (metal-to-ceramic contact only), exactly as specified in the active-cooled-clamp section above (which replaced the original screw+washer mitigation plan). Every resistance below is re-derived from raw `R = L/(k·A)` on the documented zone dimensions, not carried over from a table, specifically so no stray joint term sneaks in.

**Rod** (⌀12.7mm, A=1.2668×10⁻⁴ m², k_Cu=391 W/m·K):
- Sample-side standoff (Movement = 4.197in = 0.1066m): R = 0.1066/(391×1.2668×10⁻⁴) = **2.15 K/W** (vs. 51.96 K/W for 304SS — ~24x drop)
- Clamp-side standoff (3in = 0.0762m): R = 0.0762/(391×1.2668×10⁻⁴) = **1.54 K/W** (vs. ≈37 K/W for 304SS — ~24x drop)

**Ceramic zones** (dimensions from the conduction-path geometry table at top of file), computed directly for each k, confirming no joint term exists between any of them:

| Segment | L | A | R, Lava (k=1.265) | R, BN (k=75, cross-plane) | R, BN (k=200, in-plane) |
|---|---|---|---|---|---|
| Orange wedge (sample contact) | 3.175mm | 219.2mm² | 3.175e-3/(1.265×2.192e-4)=**11.45 K/W** | 3.175e-3/(75×2.192e-4)=**0.19 K/W** | 3.175e-3/(200×2.192e-4)=**0.07 K/W** |
| Gold ring | 4.175mm | 118.1mm² | 4.175e-3/(1.265×1.181e-4)=**27.95 K/W** | 4.175e-3/(75×1.181e-4)=**0.47 K/W** | 4.175e-3/(200×1.181e-4)=**0.18 K/W** |
| Teal body (lengthened, CeramicL=3.638in total → teal=75.11mm) | 75.11mm | 313.8mm² | 0.07511/(1.265×3.138e-4)=**189.2 K/W** | 0.07511/(75×3.138e-4)=**3.19 K/W** | 0.07511/(200×3.138e-4)=**1.20 K/W** |
| Purple cap | 10.0mm | 292.9mm² | 0.010/(1.265×2.929e-4)=**26.99 K/W** | 0.010/(75×2.929e-4)=**0.46 K/W** | 0.010/(200×2.929e-4)=**0.17 K/W** |
| *(no joint segment — rod bears directly on purple cap)* | — | — | — | — | — |
| Rod, sample-side (copper) | 0.1066m | 1.2668e-4m² | **2.15 K/W** | **2.15 K/W** | **2.15 K/W** |
| **Sample/ceramic contact resistance (spreading — corrected, scaled by each holder's own k)** | — | — | **75.9 K/W** (k=1.265) | **1.28 K/W** (75.9×1.265/75) | **0.48 K/W** (75.9×1.265/200) |
| **Total R_sample-side** | | | **≈333.5 K/W** | **≈7.74 K/W** | **≈4.25 K/W** |
| Rod, clamp-side (copper, 3in) | 0.0762m | 1.2668e-4m² | **≈1.54 K/W** | **≈1.54 K/W** | **≈1.54 K/W** |

(BN bound: orientation through this stack is unconfirmed, see Corrections note above — k=75 is cross-plane mid, k=200 is in-plane mid. The contact resistance is now scaled per-holder — see correction note above — so the BN totals are dramatically smaller than previously reported, not merely a rounding difference.)

### T_seal at T_sample = 1000°C, 16°C clamp

(Updated 2026-10-13 — raised from the 850°C real process peak to the 1000°C stress-case value used elsewhere in this file's active-clamp analysis, for an apples-to-apples comparison against the 304SS+joint "≈103°C" result above.)

`T_seal = T_clamp + [R_clamp/(R_sample+R_clamp)] × (T_sample − T_clamp)`

| Holder | R_sample | R_clamp | R_total | T_seal | Margin to 100°C |
|---|---|---|---|---|---|
| **Lava + Cu rod** | 333.5 K/W | 1.54 K/W | 335.0 K/W | **≈20.5°C** | **Passes** — ≈79.5°C headroom |
| **BN + Cu rod** (cross-plane, k=75, corrected) | 7.74 K/W | 1.54 K/W | 9.28 K/W | **≈179°C** | **Fails** — ≈79°C over the limit |
| **BN + Cu rod** (in-plane, k=200, corrected) | 4.25 K/W | 1.54 K/W | 5.79 K/W | **≈278°C** | **Fails** — ≈178°C over the limit |

**Corrected conclusion (supersedes the previous "both holders pass" statement): only Lava passes.** With the contact resistance properly scaled to each holder's own conductivity, BN's total sample-side resistance collapses to single digits (7.74 or 4.25 K/W) — barely bigger than the rod's own clamp-side resistance (1.54 K/W) — so BN no longer dominates the divider at all; a large share of the 984°C drop now lands on the seal instead of the ceramic. This is actually consistent with the file's much earlier (pre-copper-rod) finding that BN without a heat-break ran strongly negative margin (-134.3%) — BN was never a good insulator on its own; the copper rod's extreme conductivity doesn't change that, it just changes where in the chain the heat ends up. **Lava remains the only validated holder option for this active-clamp + copper-rod configuration.**

### Breakeven: sample temperature that drives T_seal to exactly 100°C

Solving `T_sample = T_clamp + (100 − T_clamp) × (R_sample+R_clamp)/R_clamp`:

| Holder | T_sample for T_seal = 100°C |
|---|---|
| Lava + Cu rod | ≈18,300°C (unaffected by the contact-resistance correction) |
| **BN + Cu rod (k=75, corrected)** | **≈522°C** |
| **BN + Cu rod (k=200, corrected)** | **≈332°C** |

**Lava**: unreachable — 101 Copper melts at **1,081°C** (per datasheet, [[Design/Plumbing/Material Properties Reference.md]]), far below the Lava breakeven temperature, so **the copper rod itself, not the seal, is the binding constraint for Lava.**

**BN (corrected)**: the opposite conclusion. **≈332–522°C is well inside the real process range (800-850°C peaks)** — BN's seal-breakeven point is now below the actual operating temperature, confirming BN fails in normal use, not just in some extreme hypothetical. This matches the T_seal≈179-278°C already found at 1000°C above.

### Caveats

- Same pure-conduction, no-ambient-loss model as the rest of this file — real T_seal is expected to run cooler than modeled (see calibration section above), so these numbers are already conservative in the seal's favor.
- Copper is electrically highly conductive (very low surface resistivity vs. titanium), which was previously flagged as a reason copper/aluminum rods were set aside for eddy-current heating risk in the induction field — that concern is **not resolved by this thermal result** and should be checked before adopting 101 Copper as the rod material, independent of the seal-margin conclusion above.
- Clamp contact/coolant-side resistance is still idealized (perfect 16°C sink, no added rod-to-clamp or clamp-to-coolant resistance) — same open caveat as the 304SS active-clamp case above.
- BN orientation through the actual part geometry remains unconfirmed; the k=75/200 bound is a proxy, not a measured value.

### Temperature at the top of the ceramic holder (rod/ceramic interface), and max-margin length check (2026-10-13)

> **This whole subsection is corrected below** — it originally reused the Lava-derived 75.9 K/W contact resistance for BN too (see correction note at the top of this section), which understated BN's top-of-ceramic temperature by roughly 350-580°C. Recomputed with each holder's own contact resistance:

Extending the same model to a third node: the top of the ceramic holder, i.e. where the purple cap meets the rod (no joint hardware there — see assumption above). `T_top-ceramic = T_sample − I×R_upstream`, where `R_upstream` is everything from the sample down to (and including) the purple cap, and `I = (T_sample−T_clamp)/R_total` as before. At T_sample=1000°C, T_clamp=16°C:

| Holder | R_upstream (contact+orange+gold+teal+purple) | R_total | I | **T_top-ceramic** |
|---|---|---|---|---|
| Lava | 75.9+11.45+27.95+189.2+26.99 = 331.5 K/W | 335.0 K/W | 2.937 W | **≈26.4°C** |
| **BN (k=75, cross-plane, corrected)** | 1.28+0.19+0.47+3.19+0.46 = **5.59 K/W** | 9.28 K/W | 106.0 W | **≈407°C** |
| **BN (k=200, in-plane, corrected)** | 0.48+0.07+0.18+1.20+0.17 = **2.10 K/W** | 5.79 K/W | 170.0 W | **≈643°C** |

**Max-margin length check against the 80%-of-copper-melting constraint** (0.8 × 1,081°C = **864.8°C** — the top of the ceramic must stay **at or below** this; we're solving for where it would exceed it. Wording corrected 2026-10-15 — it previously said "allowed to run hotter," which was backwards):

**Lava**: solving `T_top-ceramic = 864.8°C` for teal length still returns a negative/unphysical length — even zero ceramic only reaches ≈61.5°C using Lava's own (correctly-scaled) contact resistance. No finite critical length exists; Lava is unconstrained by this check at any achievable length (margin ≈838°C at the current 3.638in design).

**BN**: now much closer to the threshold, but still technically unconstrained. As teal length (x) increases from 0, `T_top-ceramic` *decreases* (more resistance upstream of the node cools it down), so the worst case is the shortest possible length:
- At **x=0** (zero teal, BN k=75): `T_top-ceramic ≈ 612°C` — margin to 864.8°C ≈ **253°C**, still passing, but far tighter than Lava's ≈838°C.
- At the current extended length (x=3.19, BN k=75): `T_top-ceramic ≈ 407°C` — margin ≈ **458°C**.
- BN k=200 case is cooler at the top-of-ceramic than k=75 in this metric (counterintuitively) because its much lower contact resistance drops more of the ∆T right at the sample/ceramic interface itself, before the node being measured — margin ≈**222-414°C** depending on length.

**So the 80%-melting constraint still doesn't set a hard minimum/maximum length for either holder** — but this check is now almost academic for BN, because **BN already fails the seal's actual 100°C limit** (see corrected main table above) at any length, by a wide margin, regardless of what's happening at the top of the ceramic. The real length ceiling for Lava (the only viable holder here) remains the **non-thermal constraints already on record**: the 10in total travel budget (Geometry section above) and the **open structural check on the 3.638in Lava extension** (flagged in Open Items — Lava's 2,500psi tensile rating hasn't been validated against actuation loads at this length).

### Largest-margin CeramicL/RodBottom split — Lava + 101 Copper rod (2026-10-13)

> **CORRECTED 2026-10-15.** The paragraph that stood here claimed RodBottom is not in the thermal network and that the 4.197in "Movement" rod connects the ceramic to the seal. Both were wrong: RodBottom **is** the rod between the ceramic top and the seal (sample side), and the 4.197in RodTop is on the **clamp** side. The conclusion below survives, but for a different reason.

The Geometry section's corrected budget (`10in = RodBottom + CeramicL + Sample + Margin + WaterHeight`) fixes **RodBottom + CeramicL = 3.835in** (not 3.638in) — the two trade off 1:1, since Sample (2.165in), Margin (1in) and WaterHeight (3in) are fixed independently of both.

**Per inch of length, ceramic resists far more than copper rod** (teal Lava ≈ 64 K/W/in at k=1.265, ≈ 40 K/W/in at fired k=2.0, vs. copper rod ≈ 0.5 K/W/in; 304SS rod ≈ 12.4 K/W/in). So shifting budget from CeramicL into RodBottom *removes* more resistance than it adds — margin can only fall as RodBottom increases from zero, for either rod.

**Largest-margin point is therefore the corner RodBottom=0, CeramicL=3.835in** (the maximum buildable length). Note the table below and the sections after it were computed with CeramicL=3.638in / the old standoff lengths — see "Corrected-geometry results" for the redone numbers.

| Parameter | Value |
|---|---|
| RodBottom | **0 in** |
| CeramicL (teal body lengthened to 75.11mm) | **3.638 in** |
| R_sample-side (Lava + Cu rod) | ≈333.5 K/W |
| R_clamp-side (Cu rod, 3in) | ≈1.54 K/W |
| T_seal @ T_sample=1000°C, T_clamp=16°C | ≈20.5°C |
| **Margin to seal's 100°C rating** | **≈99.3%** (≈79.5°C headroom) — the largest achievable within this travel budget |

Any RodBottom>0 (equivalently CeramicL<3.638in) strictly reduces margin from this maximum, since it shortens the one resistive segment (teal body) that's actually doing useful work insulating the seal.

**(Lava-specific — does not extend to BN or to zero ceramic.)** This analysis is only valid for a holder material actually present in the contact — it does **not** mean CeramicL can be driven toward zero, or that BN can substitute for Lava, without consequence. See the correction below: the sample/ceramic contact resistance depends on whatever material the sample is actually touching, and collapses badly for high-conductivity materials.

### Can the sample sit directly on the bare copper rod (CeramicL→0, no ceramic at all)? No. (2026-10-13)

A natural extension of the "RodBottom=0, CeramicL=3.638in is optimal" result above is to ask whether pushing further — CeramicL=0, no ceramic holder at all, sample resting directly on the copper rod — is even more thermally favorable, since removing the ceramic would also remove its bulk resistance from a naive reading of the earlier (incorrect) CeramicL=0 analysis. **It is not, and the earlier claim that it was thermally fine was wrong** — it's the same contact-resistance bug described in the correction note at the top of this section, now carried to its most extreme case.

The sample/ceramic contact (spreading) resistance is **not a fixed 75.9 K/W regardless of what's in contact** — it scales with the conductivity of whatever material the sample is actually touching (`R_contact ∝ 1/k`). If the sample touches bare copper instead of ceramic:

```
R_contact(copper) = 75.9 × (k_Lava/k_Cu) = 75.9 × (1.265/391) ≈ 0.25 K/W
```

Almost no spreading resistance at all — copper conducts so well there's essentially nothing to spread into. With no ceramic stack either, the sample-to-seal path is just this tiny contact resistance plus the rod itself:

| Quantity | Value |
|---|---|
| R_contact (copper) | ≈0.25 K/W |
| R_sample-side (contact + rod, no ceramic) | ≈2.40 K/W |
| R_clamp-side (rod, 3in) | ≈1.54 K/W |
| R_total | ≈3.94 K/W |
| I = (1000−16)/R_total | ≈250 W |
| **T_seal** | **≈401°C — fails the 100°C limit by ~300°C** |
| **Rod temperature at the sample contact point** | **≈939°C — ~87% of copper's 1,081°C melting point** |

**Conclusion: no, the sample cannot be held at 1000°C directly on the bare copper rod.** Removing the ceramic doesn't just delete a length term from the travel-budget equation — it deletes the only thing in the stack with low enough conductivity to create meaningful resistance in the first place, both along its length *and* at the contact spot. Without it, heat dumps straight into the copper: the seal blows through its rating by ~300°C, and the rod itself is driven to within ~140°C of melting right where it touches the sample. **A low-conductivity ceramic holder (Lava) between the sample and the copper rod is load-bearing for this entire thermal strategy, not an optional/removable length — the CeramicL=0 "thermal floor" conclusion earlier in this section is retracted.**

### Largest Margin length, corrected (2026-10-13)

Redoing the largest-margin-length question properly: the distinction that matters is **which zones shrink**. The **orange wedge** (sample-contact zone) must stay present and Lava for the 75.9 K/W contact resistance to remain valid — collapsing it to zero is the retracted direct-copper-contact case above. The **teal body**, however, is pure bulk ceramic with no contact-resistance role of its own — it can shrink to zero while orange/gold/purple stay at their original (as-built) Lava dimensions, with the contact physics fully intact.

**Solving with teal→0, orange/gold/purple at original baseline lengths (still genuine Lava contact):**

```
R_sample-side = R_contact(75.9) + R_orange(11.45) + R_gold(27.95) + R_purple(26.99) + R_teal(0) + R_rod(2.15) = 144.44 K/W
R_total = 145.98 K/W
T_seal = 16 + 1.54×984/145.98 ≈ 26.4°C — passes
T_top-ceramic = 1000 − (984/145.98)×142.29 ≈ 40.9°C — also clears the 864.8°C check  (corrected 2026-10-15 from 41.2°C — arithmetic error)
```

**Minimum CeramicL = orange(3.175mm) + gold(4.175mm) + teal(0) + purple(10.0mm) = 17.35mm = 0.683in.**

**Largest Margin length = 4.835in − 0.683in = 4.152in.**

| | Current design | Largest-margin design |
|---|---|---|
| CeramicL | 3.638in | **0.683in** |
| Margin | 1.197in | **4.152in** |
| T_seal @ 1000°C | ≈20.5°C | ≈26.4°C |
| T_top-ceramic @ 1000°C | ≈26.4°C | ≈40.9°C |

Both configurations pass comfortably — the current 3.638in design spends far more of the 10in travel budget on ceramic length than the seal/rod constraints actually require once the teal body (the only zone without a load-bearing contact role) is pared back to zero. **This supersedes the earlier (retracted) CeramicL=0/Margin=4.835in claim**: that version wrongly removed the orange wedge too, which is why it failed; this version keeps it, which is why it passes.

**Model-validity caveat (double-checked 2026-10-13, re-derivation confirmed self-consistent but flagged as optimistic)**: re-verifying the teal=0 arithmetic two independent ways (direct T_top-ceramic calc, and T_seal + I×R_rod cross-check) both land on ≈40.9°C/≈26.4°C, so the numbers are computed correctly from the stated model. **But the model itself is being pushed outside where it was validated.** The 75.9 K/W contact resistance comes from a semi-infinite-medium spreading-resistance formula, which assumes the ceramic behind the contact patch is deep enough for heat to fully spread out before hitting anything else — reasonable when a real teal body (26.65–75.11mm) backs it up, increasingly questionable at teal=0, where only 17.35mm of Lava separates the contact patch from the copper rod (a much better conductor). In that thin-backing regime, the true resistance is likely **lower** than the semi-infinite formula predicts, which would push the real T_top-ceramic and T_seal **higher** than 40.9°C/26.4°C — i.e. **less real margin than this idealized result suggests**, same direction of error as the retracted direct-copper-contact case, just far smaller in magnitude since Lava is still physically present here. Treat the 0.683in/4.152in-Margin result as **optimistic, not a validated design point** — bench-test before building to it, same standard this file has applied to every other extreme conclusion.

## Corrected-geometry results (2026-10-15) — supersedes numbers computed from the old geometry above

> **⚠️ Conduction-only (2026-10-18 note):** absolute temperatures in this section and everything below it up to "Open-air test calibration" under-predict the real seal/ceramic temperatures by a factor of 3–5 because they omit hot-air heating of the holder and rod. Use the "Open-air test calibration and recalibrated model" section for temperatures.

Uses the corrected stack in the Geometry section: **sample-side rod = RodBottom (3.835in − CeramicL)**, **clamp-side rod = RodTop = 4.197in**, clamp 2.5in (ideal 16°C sink assumed, same idealization as before), budget **RodBottom + CeramicL = 3.835in**. Rod resistance per inch: **copper 0.513 K/W/in** (R=L/(kA), k=391, A=1.2668×10⁻⁴m²); **304SS 12.4 K/W/in** (k=16.2). Clamp-side: copper 2.15 K/W, 304SS 51.95 K/W.

**Safety factors used (set 2026-10-15): both at 80%** — seal ≤ 0.8×100°C = **80°C**; copper top-of-ceramic ≤ 0.8×1,081°C = **864.8°C**. Fired Lava k ≈ **2.0 W/m·K** (estimate — see [[Design/Plumbing/Material Properties Reference.md]]); unfired k = 1.265 (datasheet). Contact/ceramic resistances scale as 1/k (contact 75.9 K/W at k=1.265).

### BUG FIX + FINAL RESULTS — unfired Lava, copper rod, water-cooled clamp, 1000°C (double-checked 2026-10-15)

Every resistance was recomputed from scratch in a script (raw `R = L/(kA)` for each zone and rod segment). The arithmetic in this note was confirmed correct (orange 11.45, gold 27.95, purple 26.99, teal 2.519 K/W per mm, copper rod 0.513 K/W/in, all match). **Bugs found:**

1. **The sample/ceramic contact (spreading) resistance was ~2× too high.** The note used `R = 1/(2ka)` per patch. For a circular contact spot on a half-space, the constriction resistance on **one** side of the joint is `1/(4ka)` (isothermal-disk result). `1/(2ka)` is the *combined* value when *both* bodies have conductivity k, but the sample side is steel (k≈30), not Lava. Corrected, with two patches in parallel (a = 2.604mm from 21.308mm² each):
   - Lava side: 1/(4×1.265×0.002604)/2 = **37.94 K/W**
   - Steel side: 1/(4×30×0.002604)/2 = **1.60 K/W**
   - **Corrected contact R = 39.5 K/W (was 75.9 K/W).** This lowers R_sample by ~36 K/W everywhere the 75.9 value was used (every table in this note, including the BN/fired scalings, inherits the same factor-of-2 error). It makes the seal run **hotter**, not cooler.
   - This is still idealized: the orange wedge is only 3.175mm thick, so part of this spreading happens *inside* the wedge whose bulk resistance is also counted. That overlap means real resistance is probably somewhat **lower** again (unquantified).
2. **The 0.313in of rod inside the seal/lid was left out.** The seal node is now placed at mid-thickness (0.156in of copper on each side, 0.08 K/W each). Small: raises T_seal by ~0.2–0.7°C.
3. **The 80% seal limit used 100°C as the rating.** The datasheet rating is 210°F = **98.9°C**, so 80% is **79.1°C**. Doesn't change any conclusion below.

**Rod material (2026-10-15): McMaster 8966K13, 110 Copper** — k = 391 W/m·K (226 BTU/hr·ft·°F), same as 101 Cu, so every result below applies unchanged. Melting point is 1,065°C (not 1,081°C), so the 80% copper limit is **852°C**; the rod end at the ceramic runs ≈34–50°C, so this doesn't bind. Details and the oxygen/atmosphere caveat: [[Design/Plumbing/Material Properties Reference.md]].

**Final results** (T_sample = 1000°C, ideal 16°C clamp, clamp-side rod 4.197in = 2.15 K/W, RodBottom = 3.835in − CeramicL, contact = 39.5 K/W):

| Design | CeramicL | Teal | RodBottom | R_sample | **T_seal** | T_top-ceramic | Passes 79.1°C seal + 864.8°C top? |
|---|---|---|---|---|---|---|---|
| **Minimum** | **0.683in (17.35mm)** | 0 | 3.152in | 107.5 K/W | **≈36°C** | ≈50°C | Yes |
| **Original** | **1.732in (44.0mm)** | 26.65mm | 2.103in | 174.1 K/W | **≈28.5°C** | ≈34°C | Yes |
| **Maximum** | **3.835in (97.4mm)** | 80.06mm | 0 | 307.6 K/W | **≈23°C** | ≈23°C | Yes |

- Seal limit (79.1°C) requires R_sample ≥ **31.4 K/W**; the minimum stack has 107.5 K/W (~3.4×). **The minimum length is still the physical stack, 0.683in.**
- Clamp tolerance at the minimum length: the real clamp-side resistance (rod + rod-to-clamp contact + coolant side) can be up to **≈7.5 K/W** before the seal reaches 80°C (ideal = 2.15). This replaces the earlier ≈10 K/W figure.
- **Ambient air only (no clamp), corrected contact**, 4.197in bare rod, lumped: original 1.732in → **≈141°C (h=10) / ≈106°C (h=15) — fails**; maximum 3.835in → ≈92.5°C / ≈71°C. These supersede the ambient numbers given earlier in this note for unfired Lava.
- The fired-Lava and 1300°C rows below still use the old 75.9 K/W contact value — scale them with the same correction before relying on them.

### Planned build: original 1.732in Lava holder + copper water-cooled clamp (2026-10-16)

**Stack at the home (heating) position, chamber interior = 10in:**

| # | Section | Length |
|---|---|---|
| 1 | Copper clamp, 16°C water (internal coolant passage area **4.02789 in² = 2,599 mm²**, read as wetted area) | 2.5in |
| 2 | RodTop, clamp to seal (5mm + Margin 1in + Water 3in) | 4.197in |
| 3 | Seal / lid (above the 10in interior) | 0.313in |
| 4 | RodBottom, seal to ceramic top | **2.103in** (= 3.835 − 1.732) |
| 5 | Lava holder (orange 3.175 + gold 4.175 + teal 26.65 + purple 10.0mm) | **1.732in (44.0mm)** |
| 6 | Sample | 2.165in |
| 7 | Margin (sample bottom to water) | 1in |
| 8 | Water | 3in |

Check: 2.103 + 1.732 + 2.165 + 1 + 3 = 10.0in. Stroke = Margin + Water = 4in. Rod length needed ≈ 2.103 + 0.313 + 4.197 + 2.5 = **9.11in** (assumes the rod runs up through the clamp; a 12in McMaster 8966K13 rod covers it).

**Clamp-side resistance model** (copper clamp body ≈ 0 K/W; coolant side = 1/(h·A) with A = 2.599×10⁻³ m²; h is an estimate, not measured):

| Piece | Best | Typical | Pessimistic |
|---|---|---|---|
| Rod, seal to clamp near edge (4.197in) | 2.15 | 2.15 | 2.15 |
| Rod-to-clamp contact (thermal paste; unknown) | 0.04 | 0.1 | 0.4 |
| Coolant side (h = 5000 / 1000 / 500 W/m²K) | 0.08 | 0.39 | 0.77 |
| **R_clamp-side (K/W)** | **2.27** | **2.64** | **3.32** |

Heat load is only ~5–6 W, so the coolant warms <1°C at even a slow flow.

**Seal temperature, T_sample = 1000°C, 16°C coolant, full contact** (contact resistance 39.5 K/W corrected; R_sample 174.1 K/W unfired Lava k=1.265, 111.1 K/W fired k≈2.0):

| Case | Unfired Lava | Fired Lava |
|---|---|---|
| Best | ≈ 29°C | ≈ 36°C |
| Typical | ≈ 31°C | ≈ 39°C |
| Pessimistic | ≈ 34°C | ≈ 45°C |

All far below the 79.1°C (80%) limit.

**If only half of the rod is in contact with the water-cooled block** (contact area halved; contact resistance doubles; coolant-side area unchanged; azimuthal constriction in the copper rod is ≈ 0.01 K/W, ignored):

*(a) Half the contact area, same position (near edge still 4.197in from the seal):*

| Case | R_clamp-side | Unfired Lava | Fired Lava |
|---|---|---|---|
| Best (contact 0.08) | 2.31 | ≈ 29°C | ≈ 36°C |
| Typical (0.2) | 2.74 | ≈ 31°C | ≈ 40°C |
| Pessimistic (0.8) | 3.72 | ≈ 37°C | ≈ 48°C |

*(b) Worst case: contact only on the far half of the clamp (near edge pushed out to 4.197 + 1.25 = 5.447in; rod conduction 2.79 K/W):*

| Case | R_clamp-side | Unfired Lava | Fired Lava |
|---|---|---|---|
| Best | 2.95 | ≈ 32°C | ≈ 42°C |
| Typical | 3.38 | ≈ 35°C | ≈ 45°C |
| Pessimistic | 4.36 | ≈ 40°C | ≈ 53°C |

**Takeaway:** halving the contact adds only ~1–7°C (case a) or ~4–11°C (case b). The seal stays well under 79.1°C even in the worst combination (53°C, fired Lava). The clamp-side budget before the seal hits 80°C is ≈ 7.5 K/W at the 0.683in minimum holder, and larger for the 1.732in holder, so there is wide margin.

**Caveats:** pure-conduction model; ideal 16°C coolant bulk temperature; h and the rod-to-clamp contact resistance are assumptions; thin-backing and wedge-overlap caveats on the ceramic contact resistance still apply. Bench-test with a thermocouple at the seal.

### Minimum CeramicL (teal = 0; orange 3.175 + gold 4.175 + purple 10.0 = 17.35mm = 0.683in), copper rod, active clamp

> ⚠️ Computed with the **old 75.9 K/W** contact value (2× too high) — see the bug fix directly above. Unfired row superseded by the final-results table.

| Holder | T_sample | R_sample (incl. RodBottom 3.152in ≈ 1.62 K/W) | R_clamp | T_seal | T_top-ceramic | Passes 80°C seal + 864.8°C top? |
|---|---|---|---|---|---|---|
| Lava, unfired k=1.265 | 1000°C | 143.9 K/W | 2.15 | ≈30.5°C | ≈41.4°C | Yes |
| **Lava, fired k≈2.0** | 1000°C | **91.6 K/W** | 2.15 | **≈38.6°C** | **≈55.6°C** | **Yes** |
| **Lava, fired k≈2.0** | 1300°C | **91.6 K/W** | 2.15 | **≈45.5°C** | **≈67.6°C** | Yes thermally — **but Lava's ~1,100°C ceiling is exceeded at the sample contact; see below** |

- **Seal limit requires** R_sample ≥ 30.9 K/W at 1000°C and ≥ 41.0 K/W at 1300°C (80°C limit, R_clamp = 2.15). The zero-teal stack is 2–3× that, so **neither safety factor sets a length floor — the minimum is the physical stack, 0.683in (17.35mm)**, and RodBottom becomes 3.152in.
- The seal limit would only bind for fired k ≳ 6.1 W/m·K (1000°C) or ≳ 4.6 W/m·K (1300°C).
- **1300°C exceeds Lava's service ceiling in any state** (~1,100°C working; 1,093°C is the *firing* limit). The orange wedge touches the sample directly, so it sees ~sample temperature. A 1300°C regime needs a different contact-zone material (e.g. BN) — not yet analyzed.
- **Thin-backing caveat still applies**: the 75.9 K/W contact value assumes a deep ceramic behind the patch; at 17mm of Lava in front of copper the real contact resistance is likely lower, so real temperatures run hotter than shown. Bench-test before building to this.

### Maximum-length (RodBottom = 0, CeramicL = 3.835in), copper rod, active clamp, 1000°C, unfired Lava k=1.265

R_sample = 344.0 K/W, R_clamp = 2.15 K/W → **T_seal ≈ 22°C**, T_top-ceramic ≈ 22°C. (Old figure with CeramicL=3.638in and wrong standoffs: ≈20.5°C — essentially unchanged for copper because rod resistance is tiny.)

### 304SS rod — corrected result is much worse than the old table

With the standoff lengths corrected (sample-side = RodBottom, clamp-side = 4.197in = 51.95 K/W), **304SS + active clamp, unfired Lava, 1000°C: T_seal ≈ 145°C at CeramicL=3.835in** (≈148°C at 3.638in) — not the ≈103°C previously reported. Keeping T_seal ≤ 100°C would need R_sample ≈ 557 K/W, well beyond the 344 K/W reachable at the maximum buildable length. **304SS is not viable with this geometry; copper (or another high-k rod with the clamp) is required.**

### Copper rod, ambient air only (no active clamp) — recomputed, rough

Exposed rod area is the bare rod above the seal (not the old 3in): π×0.0127×0.1066 = 0.00425m² for RodTop alone (4.197in), 0.00679m² if the 2.5in clamp section is also bare rod (6.697in). Same lumped model as the 2026-10-15 correction below (R_conv = 1/(hA), copper rod treated as isothermal, radiation ignored, ambient 20°C), 1000°C.

| Holder | Exposed rod | h | T_seal @ CeramicL=3.835in | CeramicL for 100°C | CeramicL for 80°C |
|---|---|---|---|---|---|
| Unfired k=1.265 | 4.197in | 10 | ≈85°C | ≈2.8in | not reachable (needs R_sample ≈377 K/W > 344 max) |
| Unfired k=1.265 | 4.197in | 15 | ≈66°C | ≈1.4in | ≈2.5in |
| **Fired k≈2.0** | 4.197in | 10 | **≈120°C — fails** | **≈5.3in — exceeds 3.835in budget** | not reachable |
| **Fired k≈2.0** | 4.197in | 15 | ≈90°C | ≈3.1in | ≈4.8in — exceeds budget |

Takeaway: **fired Lava's higher k makes ambient-air-only cooling marginal-to-failing within the 3.835in budget** (the heat-loss assumption h is the swing factor); the active clamp is what makes the short fired holder work. Treat these as ballpark — they depend entirely on the assumed h and on how much bare rod is actually exposed.

## No-active-cooling comparison, Lava holder, T_sample=1000°C (2026-10-13/15)

> **Superseded numbers (2026-10-15):** the table below was computed with the old geometry (4.197in "Movement" rod on the sample side, 3in clamp-side rod, CeramicL budget 3.638in). The 304SS rows in particular are materially optimistic — see "Corrected-geometry results" above. The 304SS "no active cooling" row also still assumed a perfect ambient sink with no convective resistance (the same flaw retracted for copper just below), so it understates seal temperature further. Kept for history.

Real-world cross-check first: the user has **actually tested a Lava holder at its original (un-extended) length, 304SS rod, no active clamp, and measured the seal exceeding 100°C.** Reproducing that exact configuration with plain series resistors (original teal=26.65mm, R_teal=67.12 K/W, 304SS rod, 16°C active-clamp standoffs used as the closest fully-specified proxy) gives T_seal≈138°C and T_top-ceramic≈309°C — **both over 100°C, consistent with the real measurement.** *(Corrected 2026-10-15: this only shows the model also predicts "over 100°C"; it does **not** validate the ceramic/contact resistance values. The calculation used the old, wrong standoff lengths (4.197in on the sample side) and a 16°C clamp-sink proxy that was not present in the real test, so agreement is weak evidence at best. A real validation needs the actual RodBottom standoff and no clamp.)*

**Length summary, T_sample=1000°C, Lava holder, all four rod/cooling combinations** (R_fixed = contact 75.9 + orange 11.45 + gold 27.95 + purple 26.99 = 142.29 K/W, plus whichever rod):

| Rod + cooling | R_fixed (ceramic+contact+rod) | Current design (CeramicL=3.638in) T_seal | Required CeramicL for T_seal=100°C | Fits in 3.638in budget? |
|---|---|---|---|---|
| 304SS, 16°C active clamp | 231.25 K/W | ≈102.6°C — fails | 3.844in | No — 0.21in over |
| 304SS, no active cooling | 231.25 K/W | ≈106.3°C — fails | 4.154in | No — 0.52in over |
| Copper, 16°C active clamp | 145.98 K/W | ≈20.5°C — passes | unconstrained (passes at any length) | Yes |
| Copper, no active cooling | 145.98 K/W | ≈24.5°C — passes (**retracted, see below**) | — | — |

**With 304SS, no length/RodBottom split within the 10in travel envelope keeps the seal under 100°C at 1000°C**, regardless of cooling — you'd need to extend the travel budget itself, add real active cooling, or change rod material.

## CORRECTION (2026-10-15): the "copper + no active cooling ≈ 24.5°C" result above is wrong — retracted

> **Follow-up (2026-10-15, after the geometry fix):** the calculation below assumes only **3in** of exposed rod. With the corrected stack the bare rod above the seal is **4.197in** (6.697in if the clamp section is also bare rod), and the 111°C/84°C figures are recomputed in "Corrected-geometry results" above (≈85°C / ≈66°C for unfired Lava at 3.835in CeramicL, h=10 / 15). The retraction itself still stands — the original 24.5°C was wrong — only the 3in exposure length is superseded.

That number assumed ambient air acts as a **perfect, zero-resistance heat sink** at 20°C wherever the rod ends. It doesn't — natural convection off a bare rod has its own real, and in this case large, thermal resistance that was left out of the series chain entirely.

**Check the actual heat flow**: at that result, I ≈ 980/335.18 ≈ 2.92 W. For a bare ⌀12.7mm copper rod with ~3in exposed to room air (A ≈ π×0.0127×0.0762 ≈ 0.00304 m²) to shed even that modest 2.9W by natural convection (h≈10 W/m²K, typical for a small cylinder in still air):

```
Required ΔT (rod surface above ambient) = Q/(h×A) = 2.9/(10×0.00304) ≈ 95°C
```

The rod's surface would need to sit **~95°C above ambient**, not at ambient, to actually convect that heat away — i.e. there's a real convective resistance of `R_conv ≈ 1/(h×A) ≈ 33 K/W` that was never added to the series chain. That's comparable to or larger than the rod's own 1.54 K/W conduction resistance, and was silently treated as zero in the "no active cooling" copper calc above.

**Redone with that resistance included** (R_clamp-side = rod conduction 1.54 + convective-to-ambient ≈33 ≈ 34.4 K/W):

```
R_total = 335.18 + 34.4 = 369.6 K/W
T_seal = 20 + [34.4/369.6]×980 ≈ 111°C — fails
```

Using a more generous convection coefficient (h=15, since natural convection strengthens somewhat with ΔT) instead gives ≈84°C — passes, but only marginally, and the result swings by ~27°C just from a plausible-range guess at h. **The honest range for "copper rod, passive ambient convection only, no active clamp" is roughly 84–111°C — on the edge or failing, nowhere near the ≈24.5°C first reported.** This is exactly why the active-clamp mitigation exists in the first place: bare natural convection off a rod this size is not a strong enough heat sink on its own. **The "copper + no active cooling" row above should be read as retracted, not just uncertain — passive convection has not been shown to work, and the copper-rod result throughout this file depends on the 16°C active clamp actually being built, not merely a bare rod in room air.**

## Alternative path (new research starting, 2026-10-13): switch rod to Grade 5 Titanium, instead of actively cooling it

Rather than adding active cooling to the existing 304SS rod (above), a second direction being explored is **replacing the rod material itself** with a lower-conductivity metal, so the rod resists heat transfer along its own length rather than relying on a clamp to pull heat out partway along it. This research thread is just starting — no thermal recalculation has been run yet for this option.

**Candidate part**: McMaster **89055K56** — High-Strength Grade 5 Titanium Rod, 1/2" diameter, 1ft length, $63.30 each.

Key properties from the datasheet:
- **Thermal conductivity: 45–50 BTU·in/(hr·ft²·°F) = 6.49–7.21 W/m·K** — converting to the units used elsewhere in this file. This independently cross-checks the **6.7 W/m·K** figure already used in this analysis for the Ti-6Al-4V heat-break screw (see "Mitigation plan" above and [[Design/Plumbing/Material Properties Reference.md]]) — same alloy, now confirmed via a rod-stock datasheet rather than just the screw product page.
- **~2.4× lower k than the current 304SS rod (16.2 W/m·K)** — every inch of rod would become a better insulator with no geometry change needed.
- **Yield strength 120,000 psi** — stronger than the current 304SS rod (75,000 psi minimum), so no structural downside expected.
- **Nonmagnetic**, consistent with the shaft's existing requirement (safe near the induction coil field). Surface resistivity (171 cir. mil·ohm/ft) is high relative to copper/aluminum — relevant to an earlier open question (not yet resolved) about whether a replacement rod material could pick up eddy-current heating from the coil field; titanium's resistivity is far less of a concern there than the high-conductivity metals (copper, aluminum) considered and set aside earlier for that reason.

**Open/TBD for this path**:
- No thermal recalculation run yet — need to rework R_bottom using Ti's k instead of 304SS for the standoff/downstream rod segments, and see how T_seal compares to the active-clamp result above, either as a replacement or combined with it.
- Cost/availability check beyond the single part found so far.
- Whether this fully replaces the active-clamp mitigation, or whether both get combined (lower-k rod *and* a clamp) for more margin.

## Open-air test calibration and recalibrated model (2026-10-18)

> **This section supersedes the absolute seal/ceramic temperatures in every conduction-only section above** (the ones that predicted ≈20–60°C at the seal for the Lava holder). Those models left out heating of the holder and rod by hot air rising from the sample, and they under-predict the real measurements by a factor of 3–5. Relative comparisons (e.g. copper conducts far better than 304SS) still hold.

### Measurements used (open air with the lid in place but no chamber, Lava holder, whole sample treated as held at 800°C, no cooling)

| Measurement | Value | Notes |
|---|---|---|
| **Top of ceramic** (thermocouple sandwiched between the rod and the ceramic holder) | **197°C** | The thermocouple sandwich creates a **1.5mm air gap**; the **screw is the only mechanical connection** between ceramic and rod. Trusted regardless of time. |
| **R_bottom touching the bottom of the lid** | **85°C at ~750 s** | R_bottom = **2.66in** (ceramic top to bottom of lid). Treat 85°C as an **underestimate**; the real value was probably closer to **90°C**. |

### Experimental setup and lengths (open-air tests, 2026-10-18)

| Item | Tested setup | Notes |
|---|---|---|
| Environment | Open air, **lid in place, no chamber** | No argon, no bucket |
| Sample | 10×10×55mm steel (Charpy blank), held at 800°C | Coil heats only a ~0.5in band at the sample center; the sample was heated so the connection between the ceramic and the sample sat at the 800°C level, i.e. the whole sample is treated as 800°C in the model |
| Ceramic holder | **Lava**; zones orange 3.175 + gold 4.175 + teal 26.65 + purple 10.0mm = **44.0mm (1.732in)** | Length is the model's assumption — confirm the length actually used |
| Ceramic-to-rod joint | **1.5mm air gap**, caused by the thermocouple sandwiched between ceramic top and rod end; **steel screw is the only mechanical connection** | Thermocouple reads the interface (197°C) |
| Rod | **304SS**, 1/2" dia | |
| **R_bottom** | **2.66in** from the top of the ceramic to the bottom of the lid | Rod end touching the lid bottom read 85°C (probably ~90°C) at ~750 s |
| Lid / seal | Lid thickness ≈0.313in (the seal width, 5154T48 in the lid); lid is acrylic | Rod runs through the seal |
| Cooling | **None** for the 197°C and 85°C readings | |
| Air-jet test (separate) | Compressed air through a hose and **6ft of 1/4" OD, 0.023" wall tube (0.204" ID)**, aimed at the rod **0.25in above the lid** | Rod above the lid ≈30°C or less, rod below ≈100°C |

**Planned build lengths (for comparison; chamber interior 10in, Margin 1in):** clamp/block 2.5in → RodTop (block to lid) 4.197in → lid/seal 0.313in → **RodBottom 2.103in** → Lava holder **1.732in** → sample 2.165in → Margin 1in → water 3in. The tested R_bottom (2.66in) is 0.557in longer than the planned RodBottom (2.103in), so the tested rod end sat farther from the sample than the final build will.

**Air jet — tested, INEFFECTIVE (recorded 2026-10-18):** compressed air blown on the rod just above the lid did **not** cool the seal well. The rod **above** the lid never gets above ~30°C, while the rod **below** the lid (inside the chamber) gets much hotter, so the jet only acts on rod that is already cool and cannot reach the hot section or the seal. **An earlier model that credited the jet with large reductions was far too optimistic and has been removed.** An earlier test was also discarded because it was run with incorrect lengths.

### Model (rebuilt 2026-10-18, no jet)

1-D transient finite-difference model: 10×10×55mm steel sample bar (held isothermal at 800°C) → contact → Lava holder (orange 3.175 + gold 4.175 + teal 26.65 + purple 10.0mm = 1.732in) → gap-and-screw joint → 304SS or copper rod (R_bottom 2.66in) → lid (0.313in, seal inside) → rod above.
- Surface losses: radiation to 300K surroundings (Lava ε=0.85, 304SS ε=0.35, copper ε=0.15) plus convection.
- **Hot-air plume heating** along the holder and rod: Heskestad centerline temperature rise ΔT = 25·(Q_c^0.4 / z)^(5/3) K (Q_c in kW, z in m from a 0.02m virtual origin, capped at 60% of the sample-to-ambient difference), with Q_c ≈ 18 W of convection from the sample at 800°C; convection coefficient in the plume h = 22 W/m²K.
- Ceramic joint (gap + screw): ≈45 K/W (sensitivity 20–200 K/W). Sample–ceramic contact 39.5 K/W. Lava k = 1.265 (datasheet).
- Reading the sandwiched thermocouple as the average of the ceramic-top face and the rod-end face.

### Does it align with the measurements?

| Run | Interface TC, steady (target 197°C) | Rod at lid bottom, 750 s (target 85–90°C) |
|---|---|---|
| **A priori, no fitted parameters** | 128–156°C | 50–55°C |
| (insensitive to joint 20–200 K/W, ceramic 1.535 vs 1.732in, or fired Lava k=2) | | |
| Plume strengthened ×1.4 (within the uncertainty of the plume formula) | **197°C** | 66–71°C |
| Plume ×1.36 **plus ≈2 W of extra heat at the lid underside** (radiation + ceiling flow) | **197°C** | **85–90°C** |

**The first-principles model undershoots both readings, but a 40% stronger plume plus about 2 W at the lid underside reproduces both.** Two fitted terms, two data points, so this is a fit, not a validation.

### Conclusion

The rod and seal region is heated mainly by **hot air rising from the sample**, not by conduction through the Lava: with the plume and lid heat switched off, the same model gives only **≈39°C** at the ceramic/rod interface (≈57°C with fired Lava k=2.0) and ≈23–25°C at the seal, against the measured 197°C. (Corrected 2026-10-18 — an earlier wording here said "under 45°C even with fired Lava," which was wrong for the fired case.)

**Numerical checks (2026-10-18):** results change by <0.5°C when the mesh is halved (0.5 → 0.25mm) or the time step is cut (3 → 1 s), so the numbers are converged; the differences in the tables come from the physics inputs, not the numerics.

**Independent check — FAILS:** the recalibrated model was also run, without refitting, against the 2026-09-18 measurement in "Real calibration data" above (as-built BN holder, 850°C, no cooling, ≈120°C at the seal). It predicts **194–323°C** (standoff 1.0–1.5in, joint 5–45 K/W) — 1.6–2.7× too hot. Likely reasons, none confirmed: that test's rod may have been bolted to the ball-screw carriage (a large heat sink the model doesn't include — it treats the rod top as insulated), its real standoff and joint are not recorded, and the BN grade's actual conductivity is unknown. (Confirmed 2026-10-18: these tests were all run **in open air with the lid in place but no chamber**, so a chamber/argon difference is **not** the explanation.) **Until this is resolved, treat the predictions below as design comparisons, not absolute temperatures.**

### Predictions from the recalibrated model (no jet, Lava 1.732in, R_bottom 2.66in)

| Case | Rod at lid bottom, 750 s | Seal, 750 s | Seal, steady |
|---|---|---|---|
| 800°C, 304SS, no cooling (the test) | 88°C | 81°C | **116°C** |
| 800°C, copper, no cooling | 77°C | 76°C | 118°C |
| 800°C, **copper + water clamp** | 51°C | 50°C | **51°C** |
| 1000°C, 304SS, no cooling | 100°C | 93°C | 133°C |
| 1000°C, copper, no cooling | 87°C | 86°C | 133°C |
| 1000°C, **copper + water clamp** | 56°C | 55°C | **57°C** |

**Same model at the planned build geometry** (Lava 1.732in, RodBottom 2.103in from the corrected geometry with Margin = 1in, clamp 4.197in above the lid, no jet):

| Case | Seal, steady, 800°C | Seal, steady, 1000°C |
|---|---|---|
| 304SS, no cooling | 128°C | 146°C |
| Copper, no cooling | 121°C | 137°C |
| **Copper + water clamp** | **50°C** | **56°C** |

- Left uncooled, the seal keeps rising past its ~99°C rating (steady ≈116–146°C) for either rod.
- Copper alone does not help at steady state, since the heat comes from the air along the rod. **Copper with the water clamp brings the seal to about 50–57°C.**

### Caveats
- Two fitted terms (plume strength ×1.36, 2 W lid heat) from two data points; the plume strength and the lid heat are partly interchangeable.
- The 85°C reading is a lower bound (closer to 90°C); the 197°C is trusted at any time, and the model's value at 750 s is ≈180°C.
- Holder length assumed 1.732in; the screw joint (45 K/W) and h in the plume are estimates.
- The **sandwiched thermocouple reads between** the ceramic top face and the rod end, so it reads lower than the ceramic face itself: in the calibrated 304SS case the model has the ceramic top face ≈225°C, the rod end above the gap ≈172°C, and the thermocouple (average) ≈198°C.
- Model script is not stored in the vault (kept as a temporary file outside it).

### Argon chamber model (2026-10-18)

The same validated model (it reproduces the open-air baseline exactly) with two changes for the closed argon-filled chamber:
1. **Argon gas properties**: conductivity 0.68× air, specific heat 0.52× → heat shed by the sample into the gas 0.71×, plume temperature rise per watt 1.25× (Heskestad scaling), so the **net plume temperature is ≈ the same as in air (0.995×)**; gas-to-rod/holder convection coefficient **0.70×** (h = 22 → 15.4 W/m²K).
2. **Confined hot layer under the lid** (hot gas cannot escape): local gas temperature raised by ≈10 K early and ≈20 K at steady state (time constant ≈3,000 s), from ≈9 W of plume heat into ≈0.15 m² of lid/wall at ≈0.46 W/K. Sensitivity run over 0–60 K.

**Planned build (Lava 1.732in, RodBottom 2.103in, 800°C): open air vs. argon**

| Case | Open air, seal steady | **Argon, seal steady** | Argon, seal at 750 s |
|---|---|---|---|
| 304SS, no cooling | 128°C | **133°C** | 85°C |
| Copper, no cooling | 121°C | **121°C** | 70°C |
| Copper + water clamp | 50°C | **47°C** | 45°C |

- Argon changes the seal temperature by only about ±5–10°C: the weaker convection and the warmer trapped layer roughly cancel. **Argon (closed chamber) is not expected to be materially worse than the open-air tests for the seal.**
- Gas at the lid bottom, steady: ≈109°C in open air, **≈129°C in argon** (layer 20 K). Sensitivity over the hot-layer rise (0–60 K) and h (15.4–22): 304SS seal 124–156°C steady; copper + clamp seal 45–59°C steady.
- **Acrylic lid:** a rough steady-state estimate put the lid underside near the seal at ≈95–110°C (near PMMA's softening range). The user believes the lid does **not** absorb most of that heat (low gas-to-lid heat transfer, plate stack above the lid) and does not expect a problem; open-air tests with the lid in place showed no issue. Unmeasured — check the lid temperature with an IR thermometer after a long hold.

### Final design configuration (2026-10-18): 110 copper rod + copper cooling block, argon

- Rod: **McMaster 8966K13 110 Copper**, k = 391 W/m·K (see [[Design/Plumbing/Material Properties Reference.md]]); holder: Lava 1.732in; RodBottom 2.103in; cooling block near edge 4.197in above the lid, 16°C coolant.
- Cooling block: coolant-side surface area **4.02789 in² (2.5986×10⁻³ m²)**, so the coolant-side resistance is 1/(h·A): **0.77 K/W at h = 500, 0.38 at h = 1000, 0.13 at 3000, 0.08 at 5000 W/m²K** (h not measured; 500–1000 expected for the likely flow).

| Block total R (contact + coolant) | Seal, steady, 800°C | Seal, steady, 1000°C |
|---|---|---|
| 0.3 K/W (very good) | 43°C | 48°C |
| **0.8 K/W (typical estimate)** | **47°C** | **53°C** |
| 2.0 K/W | 55°C | 61°C |
| 5.0 K/W (poor) | 68°C | 77°C |

- Seal limit: rated 98.9°C (210°F); 80% = 79.1°C, 90% = 89°C. **All cases pass the 80% limit**, even a poor 5 K/W block (77°C at 1000°C, just under). Seal at 750 s is within a few °C of the steady value.
- **Top of ceramic in this design:** the **ceramic top face ≈170°C (800°C) and ≈189°C (1000°C)**, steady; the rod end just above the gap ≈53–60°C; a sandwiched thermocouple would read only ≈112°C (800°C) / ≈125°C (1000°C). The ceramic face stays hot because little heat leaves it; the gap and screw show a large temperature step to the cooled rod.
- Keep the rod-to-block contact snug with thermal paste (aim for the block's total resistance below ≈1 K/W).

## Material properties reference

Full comparison table (thermal conductivity, max temp) for every material evaluated during this analysis: [[Design/Plumbing/Material Properties Reference.md]]

## Known open items
- Confirm exact "6440 steel" shaft/screw grade (matters for k_steel — swings result significantly)
- Confirm real seal-to-holder standoff distance precisely (was using 1.0in conservative assumption)
- **Still no transient data**: no time trace (temperature vs. time) has been recorded from any real thermocouple test. All that exists is the 120°C steady point (2026-09-18) and a single spot reading of 85–90°C at ~750 s (2026-10-18). A logged trace would allow fitting real τ and replacing the conservative steady-state-bound gauge formula with a proper dynamic one
- Validate Lava's thermal shock behavior specifically for the orange-wedge (direct sample contact) geometry before committing the full holder — see narrowed risk assessment in [[Design/Mechanisms/Ceramic Mount.md]]
- Confirm whether the 8479K69 Lava rod's 2,010°F unfired rating is reliable as-is, or whether firing is still advisable for this application
- **(From the active-cooled clamp mitigation, see section above)** Joint resistance figure inconsistency (21.85 vs. 25.4 K/W) unresolved — small effect, not yet fixed
- **(From the active-cooled clamp mitigation)** Clamp's real contact resistance (rod-to-clamp) and coolant-side convective resistance are unquantified — current estimate assumes an idealized perfect 16°C sink, which could be optimistic
- **(From the active-cooled clamp mitigation)** Structural check needed for a 3.638in Lava holder length (substantial extension from the current 1.73in design) against actuation loads — *max buildable length is now 3.835in (corrected 2026-10-15); the minimum thermally-allowed length is 0.683in, which shrinks this concern if the short holder is adopted*
- **(2026-10-15)** Confirm fired-Lava k and the unfired temperature rating with the supplier (McMaster 1,099°C vs. MakeItFrom ~650°C conflict) — see [[Design/Plumbing/Material Properties Reference.md]]
- **(2026-10-15)** If a 1300°C sample regime is ever needed, redesign the sample-contact zone (Lava ceiling ~1,100°C) — e.g. BN orange wedge + Lava remainder; not analyzed yet
- **(2026-10-15)** Clamp mechanics: the clamp rides down with the rod (5mm lid clearance at bottom of stroke) — confirm water-line routing and that clamp contact stays tight/cold over the 4in stroke
- **(From the active-cooled clamp mitigation)** Bench test recommended before committing to this mitigation — same pattern as every other thermal claim in this file that ultimately needed real measurement to confirm
- **(2026-10-18, open-air calibration)** The recalibrated hot-air model fails against the 2026-09-18 BN measurement (predicts 194–323°C vs. ≈120°C). Test conditions confirmed 2026-10-18: open air with the lid, no chamber. Still to record for that test: rod attached to the carriage or not, actual standoff and joint. Adding the carriage as a heat sink to the model is the first thing to try.
- **(2026-10-18)** Confirm the Lava holder length used in the open-air tests (model assumes 1.732in).
- **(2026-10-18)** Log a full temperature-vs-time run (ceramic/rod interface, rod at lid bottom, seal, rod 1in above lid) to separate plume strength from lid heat — the current fit has two partly interchangeable terms.
- **(2026-10-18)** All tests so far were in **open air with the lid but no chamber**. Behavior inside the closed argon chamber is untested; the argon model (see "Argon chamber model") predicts a seal temperature within ≈±5–10°C of the open-air value, but a hotter gas layer under the lid (≈129°C vs. ≈109°C). Verify with a thermocouple at the seal in the real chamber.
- **(2026-10-18)** Measure the cooling block's real flow rate, water-side h, and rod-to-block contact; the final-design seal temperature is 43–68°C (800°C) as the block's total resistance varies from 0.3 to 5 K/W.
- **(2026-10-18) Compressed-air jet test — RESULT: INEFFECTIVE.** Compressed air (hose + 6ft of **1/4" OD tube with 0.023" wall, i.e. 0.204" (5.2mm) ID**) aimed at the rod 0.25in above the lid did **not** cool the seal: the rod above the lid stayed at ≈30°C or less, while the rod below the lid (the hot part, inside the lid/chamber side) was much hotter (≈100°C). The jet only cools rod that is already cool and cannot reach the hot section or the seal. **Not a mitigation; do not credit it in any model.**
