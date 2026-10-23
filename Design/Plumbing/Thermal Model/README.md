---
subsystem: plumbing
tags: [thermal, model, code, seal]
---

# Thermal Model (seal / ceramic holder / rod)

Code behind the numbers in [[Design/Plumbing/Seal Thermal Margin Analysis.md]]. Saved 2026-10-18.

## Files

| File | What it is |
|---|---|
| `plume_model.cs` | The model itself (C#, one file, classes `TP4` = parameters and `TM4` = solver). Final version; it includes every earlier version (conduction-only → sample bar → plume → argon). |
| `run_examples.ps1` | Driver that compiles the model and reproduces the key results (open-air calibration, planned build in air vs. argon, final design vs. cooling-block resistance). |

**Run it** (Windows PowerShell 5.1, no installs needed — it uses the C# compiler built into PowerShell):

```
powershell -ExecutionPolicy Bypass -File "Design/Plumbing/Thermal Model/run_examples.ps1"
```

Takes about a minute. To use your own cases, call `Run-P (New-P <sampleC> <rodBottom_in> <copper?> <clamp?> <argon?> <blockR>)` after the first lines of the script.

## What it models

A 1-D transient finite-difference model (mesh 0.5mm, steady/transient) of one vertical line:

`10×10×55mm steel sample (held at the set temperature) → contact resistance → Lava (or BN) holder, four zones → ceramic-to-rod joint (gap + screw) → rod (R_bottom) → lid/seal (0.313in) → rod above the lid (→ optional water-cooled block)`

- **Conduction** along the line with each zone's real cross-section (orange 3.175mm/219 mm², gold 4.175mm/118 mm², teal variable/314 mm², purple 10mm/293 mm², rod ⌀12.7mm).
- **Radiation** from every surface to 300K surroundings (Lava ε 0.85, 304SS 0.35, copper 0.15).
- **Hot-gas plume heating** of the holder and rod below the lid: the gas temperature next to the surface is the Heskestad plume centerline rise `ΔT = 25·(Q_c^0.4 / z)^(5/3)` K (Q_c in kW, z in m above the sample top plus a 0.02m virtual origin, capped at 60% of the sample-to-ambient difference), scaled by `fc`, with convection coefficient `hg`. Q_c = 18 W at 800°C (scaled with temperature).
- **Lid underside heating:** convection from the same gas plus a fixed extra heat `sealQ` (W at 800°C, scaled with temperature) at the first lid node.
- **Above the lid:** natural convection `hair` = 7 W/m²K plus radiation to room air; optional cooling block (`clamp=true`) holding the rod at 16°C through total resistance `rClamp` over a 2.5in length starting `clampStart` above the lid top.
- **Argon:** set `hg = 15.4`, `fc = 1.353`, and the hot-layer rise `dL0 → dLinf` (K) with time constant `tauL` (confined gas under the lid).
- **Air jet:** present in the code (`jet`, `hjet`, `jetHalf`) but **tested ineffective in the real hardware — keep `jet = false`**.

## Key parameters (class `TP4`)

| Parameter | Default | Meaning |
|---|---|---|
| `Th` | 800 | Sample temperature, °C |
| `wholeSampleHot` | false | true = whole sample at `Th` (used for all calibrated runs); false = only a 0.5in band at `hotCenterFromCeramic` |
| `tealMm`, `kCer` | 21.65, 1.265 | Teal length (mm; **26.65 for the original 1.732in holder**) and ceramic conductivity (Lava datasheet 1.265, fired estimate 2.0) |
| `rContact` | 39.54 | Sample-to-ceramic contact, K/W (scales as 1/k; 2.24 for BN) |
| `rJoint` | 80 (driver: 45) | Ceramic-to-rod joint (gap + screw), K/W |
| `rodBottom` | 0.0508 | Rod length from ceramic top to **bottom** of the lid, m (tested 2.66in; planned **1.603in** at the decided Margin 1.5in with the 1.732in holder, 2.103in at Margin 1in) |
| `kRod`, `rhoRod`, `cRod`, `eRod` | 304SS | Rod properties (110 copper: 391, 8960, 385, 0.15) |
| `fc`, `qcW`, `z0`, `hg` | 1.0, 18, 0.02, 15 | Plume strength factor, convective heat from sample, virtual origin, gas-to-surface h |
| `sealQ` | 12 | Extra heat at the lid underside (W at 800°C) |
| `coolantC` | 16 | Cooling-block coolant temperature, °C (open-loop tap water ≈5–30°C; added 2026-10-23) |
| `clamp`, `rClamp`, `clampStart` | false, 0.8, 0.1066 | Cooling block on/off, its total resistance (contact + coolant) K/W, and its near edge above the lid top, m (**0.1193 m = 4.697in at the decided Margin 1.5in**; 0.1066 m = 4.197in at Margin 1in) |

**Calibrated values (open air):** `fc = 1.36`, `hg = 22`, `sealQ = 2.0`, `rJoint = 45`, `wholeSampleHot = true`, Lava 1.732in → reproduces 197°C (sandwiched thermocouple, steady) and 85–90°C (rod at lid bottom, 750 s).

## Output

`TM4.Sim(p)` returns one row per `recEvery` seconds: `[t, sampleEnd, sealMid, rod 6mm below seal, ceramicTopFace, lidTop, rod 1in above lid, rod 0.5in below seal, rodAtLidBottom, rodEndAboveGap, gasAtLidBottom]` (all °C). A sandwiched thermocouple reads about the average of `ceramicTopFace` and `rodEndAboveGap`.

## Validation status (read before trusting the numbers)

- **Numerically converged:** halving the mesh or time step changes results by <0.5°C; energy balance closes exactly.
- **Matches the two open-air measurements — but only after fitting** two terms (`fc`, `sealQ`) to them.
- **Fails an independent check:** against the 2026-09-18 as-built BN measurement (850°C, ≈120°C at the seal) it predicts 194–323°C. Likely missing the ball-screw carriage as a heat sink. **Use results to compare designs, not as absolute temperatures.**
- No transient time trace has been measured; nothing has been tested in argon.

## History

Earlier versions (conduction-only → sample bar and 0.5in hot band → fitted gas heating → physical plume → argon) lived in a temp folder and are not kept; the final file contains all of their features (set `fc = 0, sealQ = 0` for conduction + radiation only).
