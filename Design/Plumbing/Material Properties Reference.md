---
subsystem: plumbing
tags: [reference, materials, thermal]
---

# Material Properties Reference

Thermal conductivity, max temperature, and tensile strength for every material evaluated during the [[Design/Plumbing/Seal Thermal Margin Analysis|shaft seal thermal margin analysis]] and the [[Design/Mechanisms/Ceramic Mount|Ceramic Mount]] heat-break/holder redesign (2026-09-17). **Bold** rows are what's actually in the final design. "Datasheet" source = pulled from the actual McMaster/manufacturer product page; "Literature" = general reference value, not verified against a specific datasheet — worth a confirmation pass before relying on those for a final go/no-go decision.

## Metals

| Material | k (W/m·K) | Max Temp (°C) | Tensile Strength | Source |
|---|---|---|---|---|
| 304 Stainless Steel (shaft, confirmed McMaster 8934K31) | ~16.2 (confirmed) | 870°C intermittent / 925°C continuous oxidation resistance — **870°C is the relevant figure here since the actual duty cycle is intermittent/cyclic, not steady-state** (see sensitization note below) | 75,000 psi (confirmed, ASTM A240 minimum) | Literature, verified 2026-09-18 |
| Grade 2 Titanium (CP) | ~17-22 | High (oxidation-limited, similar order to Ti-6Al-4V) | ~50,000 psi | Literature |
| **Grade 5 Titanium (Ti-6Al-4V)** — used (screw) | **6.7** | High (well above process temps) | **130,000 psi** | **Datasheet (94081A112)** |

## Seal elastomer

| Material | k (W/m·K) | Max Temp (°C) | Tensile Strength | Source |
|---|---|---|---|---|
| **Buna-N (Nitrile)** — current seal | Not rated on datasheet | **-40°C to 99°C** | N/A (elastomer) | **Datasheet (5154T48)** |
| Fluoroelastomer (Viton/FKM) | ~0.2-0.3 | Up to ~199°C | N/A (elastomer) | Literature |

## Low-k ceramics / thermal-break candidates

| Material | k (W/m·K) | Max Temp (°C) | Tensile Strength | Source |
|---|---|---|---|---|
| Boron Nitride (current holder) | 60-90 cross-plane / 150-250 in-plane for dense hot-pressed h-BN (highly anisotropic — orientation for this part's actual geometry was never confirmed, so true worst-case could exceed what the sensitivity sweep tested with 20-70) | Very high (refractory) | Not rated | Corrected 2026-09-18 — see note below |
| **Ceramic Alumina** (screw option, rejected — not actually low-k) | ~20-30 | **1,649°C** | Not Rated | **Datasheet (94555A321)**, k literature |
| **Muscovite Mica** — used (washer) | **0.3 (datasheet)**; ~0.46 perpendicular-to-cleavage per general literature | **499°C cont. / 799°C intermittent** | **21,000 psi** | **Datasheet (5067K56)** |
| Phlogopite Mica (extra-high-temp option, not used) | ~0.44 perpendicular-to-cleavage per general literature — essentially identical to muscovite, actually *very slightly lower*, not higher as previously stated here | 699°C cont. / 999°C intermittent | ~similar to muscovite | Corrected 2026-09-18 — see note below |
| **Alumina Silicate "Lava"** — recommended holder | **1.265** | **1,099°C (unfired)** | **2,500 psi** | **Datasheet (8479K69)** |
| Macor | ~1.46 | ~799-982°C | ~5,000 psi | Literature |
| Cordierite | ~1.5-3 | ~982-1,149°C | ~3,000-5,000 psi | Literature |
| Fused Silica (dense) | ~1.3-1.5 | ~982°C+ | ~7,000-8,000 psi | Literature |
| Fused Silica Foam (shuttle-tile type, not structurally viable — for reference only) | ~0.03-0.05 | ~1,204°C+ | Negligible (~10-15 psi) | Literature |
| Aluminum Titanate | ~1-2 | ~982-1,593°C | ~3,000-5,000 psi | Literature |
| Zirconia (PSZ) | ~2-3 | ~799°C+ | High compressive, modest tensile | Literature |
| Steatite | ~2-3 | ~982°C | ~5,000-8,000 psi | Literature |
| PTFE (V-ring seal material, rejected — no pressure rating) | ~0.25 | -79°C to 260°C | ~3,000-4,000 psi | Datasheet (temp), k/tensile literature |
| Vespel (Polyimide) | ~0.35 | ~260°C | ~10,000-13,000 psi | Literature |
| PEEK | ~0.25 | ~149-249°C | ~14,000-16,000 psi | Literature |

## Open item

The main actuation shaft was briefly referred to as "6440 steel" mid-session, which would have been a magnetic low-alloy steel (AMS 6440 covers 4340 or 52100) — conflicting with the shaft's established nonmagnetic requirement (it runs through the induction coil field). The project BOM confirms the shaft ("Pole" line item) is actually **McMaster 8934K31 (304 stainless)**, so the "6440 steel" reference doesn't apply to the shaft. Unresolved which (if any) component it was meant to refer to — flagged here in case it resurfaces.

## New risk identified during double-check: 304 SS sensitization (2026-09-18)

304 stainless steel has a **sensitization range of 425-860°C** — sustained or repeated exposure in this band causes chromium carbide precipitation at grain boundaries, which depletes chromium locally and **degrades the material's resistance to subsequent aqueous (intergranular) corrosion**. This is separate from the oxidation/scaling temperature limits above.

**Why this matters for the shaft specifically**: it's repeatedly heated through exactly this range via conduction from the sample (process peaks at 800-850°C+), *and* it's in contact with water quenchant. Sensitization + later water exposure is the classic failure combination for this effect (intergranular corrosion/cracking at grain boundaries in welds and heat-affected zones is the most common real-world manifestation, though this applies to any 304 SS repeatedly cycled through this range, not just welds).

**Not yet assessed**: how many thermal cycles through this range it takes to meaningfully sensitize the shaft, or whether the fast conduction/cooling in this application (versus slow furnace cooling, which is the worse case for sensitization) reduces the practical risk. Worth flagging as a shaft *longevity/corrosion* item to watch, independent of the seal thermal margin problem this document otherwise focuses on — if the shaft ever shows unexpected pitting or cracking near the hot end, this is the mechanism to suspect.

## Corrections (2026-09-18, from a k-value double-check pass)

- **Boron Nitride**: original entry (20-70 W/m·K) was too low. Published data for dense (>98%) hot-pressed h-BN is 60-90 W/m·K cross-plane, 150-250 W/m·K in-plane. This doesn't change the sensitivity sweep's conclusion in [[Design/Plumbing/Seal Thermal Margin Analysis.md]] (margin was already negative across the whole tested range) — but the true worst-case may have been understated, since the part's actual crystal orientation relative to the heat-flow direction was never confirmed.
- **Muscovite vs. phlogopite mica**: the original note claimed muscovite was chosen for having *lower* k than phlogopite. Real literature values (perpendicular to cleavage, the relevant direction for the washer) are muscovite ≈0.46 W/m·K vs. phlogopite ≈0.44 W/m·K — muscovite is actually marginally *higher*, and the difference is small enough (~4.5%) to be practically irrelevant either way. **This does not change the part choice** — the actual purchased tube's datasheet-confirmed value (0.3 W/m·K) is lower than both generic literature figures and is what the thermal calculations actually use. The real reasons muscovite (5067K56) was picked over phlogopite (5094N56) are cost and that 930°F/499°C already has huge margin over anything in this application — not a k advantage that doesn't actually exist.
