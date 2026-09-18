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

## Material properties reference

Full comparison table (thermal conductivity, max temp, tensile strength) for every material evaluated during this analysis: [[Design/Plumbing/Material Properties Reference.md]]

## Known open items
- Confirm exact "6440 steel" shaft/screw grade (matters for k_steel — swings result significantly)
- Confirm real seal-to-holder standoff distance precisely (was using 1.0in conservative assumption)
- No transient (time-to-plateau) data recorded from the real thermocouple test — only the steady-state 120°C point is known; a time trace would allow fitting real τ and replacing the conservative steady-state-bound gauge formula with a proper dynamic one
- Validate Lava's thermal shock behavior specifically for the orange-wedge (direct sample contact) geometry before committing the full holder — see narrowed risk assessment in [[Design/Mechanisms/Ceramic Mount.md]]
- Confirm whether the 8479K69 Lava rod's 2,010°F unfired rating is reliable as-is, or whether firing is still advisable for this application
