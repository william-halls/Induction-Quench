---
subsystem: plumbing
tags: [reference, materials, thermal]
---

# Material Properties Reference

Thermal conductivity and max temperature for every material evaluated during the [[Design/Plumbing/Seal Thermal Margin Analysis|shaft seal thermal margin analysis]] and the [[Design/Mechanisms/Ceramic Mount|Ceramic Mount]] heat-break/holder redesign (2026-09-17). **Bold** rows are what's actually in the final design. "Datasheet" source = pulled from the actual McMaster/manufacturer product page; "Literature" = general reference value, not verified against a specific datasheet — worth a confirmation pass before relying on those for a final go/no-go decision. *(Tensile-strength column removed 2026-10-15 at user request; other mechanical values that were mixed into that column are kept in the Source/notes column.)*

## Metals

| Material                                                | k (W/m·K)         | Max Temp (°C)                                                                                                                                                                                          | Source                          |
| ------------------------------------------------------- | ----------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | ------------------------------- |
| 304 Stainless Steel (shaft, confirmed McMaster 8934K31) | ~16.2 (confirmed) | 870°C intermittent / 925°C continuous oxidation resistance — **870°C is the relevant figure here since the actual duty cycle is intermittent/cyclic, not steady-state** (see sensitization note below) | Literature, verified 2026-09-18 |
| Grade 2 Titanium (CP)                                   | ~17-22            | High (oxidation-limited, similar order to Ti-6Al-4V)                                                                                                                                                   | Literature                      |
| **Grade 5 Titanium (Ti-6Al-4V)** — used (screw)         | **6.7**           | High (well above process temps)                                                                                                                                                                        | **Datasheet (94081A112)**       |
| 101 Copper (Super-Conductive, McMaster 8965K82, 1/2" rod) — rod stock candidate, **not** the coil material | **391 (confirmed; 226 BTU/(hr·ft·°F) @ 68°F, 101% IACS)** | Melting point 1,081°C (1,981°F) — not a meaningful service-temp ceiling for this application, just where the metal liquefies | **Datasheet (8965K82)** — elongation 18%, Rockwell F65, modulus 17,000 ksi |
| **110 Copper (Multipurpose, McMaster 8966K13, 1/2" rod, $20.08/ft) — the rod actually being used (2026-10-15)** | **391 (confirmed; 226 BTU/(hr·ft·°F) @ 68°F × 1.7307, 101% IACS)** — identical to 101 Cu, so all rod resistances in [[Design/Plumbing/Seal Thermal Margin Analysis.md]] apply unchanged (0.513 K/W/in) | Melting point **1,949°F = 1,065°C** (vs. 101 Cu's 1,081°C) → 80% safety limit = **852°C** (vs. 864.8°C) | **Datasheet (8966K13)** — ASTM B187, ETP copper. H04 full hard, elongation 15%, Rockwell F65, modulus 17,000 ksi, density 0.322 lb/in³, surface resistivity 10.3 cir. mil·ohm/ft. Up to **0.04% oxygen** (101 is oxygen-free) → hydrogen-embrittlement risk if heated in a hydrogen-bearing atmosphere (argon backfill is fine; avoid forming gas). Low resistivity: the eddy-current heating question in the induction field is still open. |
| Steel sample (10×10×55mm Charpy blank) — **assumed**, used for the steel-side contact term | **~30 (assumed, not measured; hot carbon/low-alloy steel)** | — | **Assumption (2026-10-15)** — only enters the contact-resistance correction (steel side ≈1.6 K/W vs. ≈37.9 K/W Lava side); result is insensitive to this value. Confirm the actual sample grade if precision matters. |

## Seal elastomer

| Material | k (W/m·K) | Max Temp (°C) | Source |
|---|---|---|---|
| **Buna-N (Nitrile)** — current seal | Not rated on datasheet | **-40°C to 99°C** | **Datasheet (5154T48)** |
| Fluoroelastomer (Viton/FKM) | ~0.2-0.3 | Up to ~199°C | Literature |

## Low-k ceramics / thermal-break candidates

| Material | k (W/m·K) | Max Temp (°C) | Source |
|---|---|---|---|
| Boron Nitride (current holder) | 60-90 cross-plane / 150-250 in-plane for dense hot-pressed h-BN (highly anisotropic — orientation for this part's actual geometry was never confirmed, so true worst-case could exceed what the sensitivity sweep tested with 20-70) | Very high (refractory) | Corrected 2026-09-18 — see note below |
| **Ceramic Alumina** (screw option, rejected — not actually low-k) | ~20-30 | **1,649°C** | **Datasheet (94555A321)**, k literature |
| **Muscovite Mica** — used (washer) | **0.3 (datasheet)**; ~0.46 perpendicular-to-cleavage per general literature | **499°C cont. / 799°C intermittent** | **Datasheet (5067K56)** |
| Phlogopite Mica (extra-high-temp option, not used) | ~0.44 perpendicular-to-cleavage per general literature — essentially identical to muscovite, actually *very slightly lower*, not higher as previously stated here | 699°C cont. / 999°C intermittent | Corrected 2026-09-18 — see note below |
| **Alumina Silicate "Lava"** — recommended holder | **1.265** | **1,099°C (unfired)** | **Datasheet (8479K69)** |
| ↳ Additional datasheet values for the same part (8479K69), not yet used elsewhere: density **0.09 lb/in³ (2.49 g/cm³)**, hardness **Rockwell H59**, CTE **1.6×10⁻⁶/°F**, compressive strength **25,000 psi**, flexural strength **10,000 psi**, dielectric strength **100 V/mil**, porosity **2%**, composition ~59% SiO₂ / ~29% Al₂O₃ / balance Fe₂O₃, K₂O, TiO₂ | | | Confirmed 2026-09-25 from full McMaster product-page text (not just the summary spec pulled earlier) |
| **Alumina Silicate "Lava", FIRED** (estimate, 2026-10-15) | **≈2.0 (range 1.3-2.0)** — design value 2.0 (higher k = hotter seal, so conservative) | **~1,100°C working ceiling** (sources: 1,130°C MakeItFrom, ~1,149°C / 2,100°F Professional Plastics) | **MakeItFrom database (not a supplier datasheet)** |
| ↳ Fired Lava other values | Density ≈ 2.3 g/cm³, specific heat ≈ 840 J/kg·K | | MakeItFrom. Firing expands the part ~2% (Professional Plastics) — machine green-state dimensions undersize |
| ↳ **Lava max-temp notes (2026-10-15)** | | | |
| | **Unfired rating conflicts between sources**: McMaster 8479K69 datasheet says 1,099°C (2,010°F); MakeItFrom says ~650°C. Unresolved — **design to the lower figure, i.e. fire the part** (fired ceiling ≈1,100°C in every source, so the conflict doesn't matter for a fired holder). Confirm with McMaster/supplier before relying on the unfired rating. | | |
| | **The 1,093°C "maturing" temperature is a firing limit, not a service rating** — exceeding it during firing causes crystallization, distortion and loss of properties (Professional Plastics / Foundry Service & Supplies Grade A schedule). | | |
| | **1300°C at the sample contact exceeds Lava's ceiling in any state.** The orange wedge touches the sample directly, so it sees ~sample temperature. Real process peaks (800-850°C) have ~250°C margin on fired Lava. A 1300°C regime would need a different contact-zone material (e.g. BN). | | |
| | Fired k is **not** known to differ much from unfired: MakeItFrom lists 2.0 W/m·K for both fired and unfired, vs. McMaster's 1.265 (unfired datasheet). Confirm fired k with the supplier or a bench test. | | |
| Macor | ~1.46 | ~799-982°C | Literature |
| Cordierite | ~1.5-3 | ~982-1,149°C | Literature |
| Fused Silica (dense) | ~1.3-1.5 | ~982°C+ | Literature |
| Fused Silica Foam (shuttle-tile type, not structurally viable — for reference only) | ~0.03-0.05 | ~1,204°C+ | Literature |
| Aluminum Titanate | ~1-2 | ~982-1,593°C | Literature |
| Zirconia (PSZ) | ~2-3 | ~799°C+ | Literature |
| Steatite | ~2-3 | ~982°C | Literature |
| PTFE (V-ring seal material, rejected — no pressure rating) | ~0.25 | -79°C to 260°C | Datasheet (temp), k literature |
| Vespel (Polyimide) | ~0.35 | ~260°C | Literature |
| PEEK | ~0.25 | ~149-249°C | Literature |

## Open item

The main actuation shaft was briefly referred to as "6440 steel" mid-session, which would have been a magnetic low-alloy steel (AMS 6440 covers 4340 or 52100) — conflicting with the shaft's established nonmagnetic requirement (it runs through the induction coil field). The project BOM confirms the shaft ("Pole" line item) is actually **McMaster 8934K31 (304 stainless)**, so the "6440 steel" reference doesn't apply to the shaft. Unresolved which (if any) component it was meant to refer to — flagged here in case it resurfaces.

## New risk identified during double-check: 304 SS sensitization (2026-09-18)

304 stainless steel has a **sensitization range of 425-860°C** — sustained or repeated exposure in this band causes chromium carbide precipitation at grain boundaries, which depletes chromium locally and **degrades the material's resistance to subsequent aqueous (intergranular) corrosion**. This is separate from the oxidation/scaling temperature limits above.

**Why this matters for the shaft specifically**: it's repeatedly heated through exactly this range via conduction from the sample (process peaks at 800-850°C+), *and* it's in contact with water quenchant. Sensitization + later water exposure is the classic failure combination for this effect (intergranular corrosion/cracking at grain boundaries in welds and heat-affected zones is the most common real-world manifestation, though this applies to any 304 SS repeatedly cycled through this range, not just welds).

**Not yet assessed**: how many thermal cycles through this range it takes to meaningfully sensitize the shaft, or whether the fast conduction/cooling in this application (versus slow furnace cooling, which is the worse case for sensitization) reduces the practical risk. Worth flagging as a shaft *longevity/corrosion* item to watch, independent of the seal thermal margin problem this document otherwise focuses on — if the shaft ever shows unexpected pitting or cracking near the hot end, this is the mechanism to suspect.

## Corrections (2026-09-18, from a k-value double-check pass)

- **Boron Nitride**: original entry (20-70 W/m·K) was too low. Published data for dense (>98%) hot-pressed h-BN is 60-90 W/m·K cross-plane, 150-250 W/m·K in-plane. This doesn't change the sensitivity sweep's conclusion in [[Design/Plumbing/Seal Thermal Margin Analysis.md]] (margin was already negative across the whole tested range) — but the true worst-case may have been understated, since the part's actual crystal orientation relative to the heat-flow direction was never confirmed.
- **Muscovite vs. phlogopite mica**: the original note claimed muscovite was chosen for having *lower* k than phlogopite. Real literature values (perpendicular to cleavage, the relevant direction for the washer) are muscovite ≈0.46 W/m·K vs. phlogopite ≈0.44 W/m·K — muscovite is actually marginally *higher*, and the difference is small enough (~4.5%) to be practically irrelevant either way. **This does not change the part choice** — the actual purchased tube's datasheet-confirmed value (0.3 W/m·K) is lower than both generic literature figures and is what the thermal calculations actually use. The real reasons muscovite (5067K56) was picked over phlogopite (5094N56) are cost and that 930°F/499°C already has huge margin over anything in this application — not a k advantage that doesn't actually exist.
