---
subsystem: plumbing
tags: [reference, materials, thermal]
---

# Material Properties Reference

Thermal conductivity, max temperature, and tensile strength for every material evaluated during the [[Design/Plumbing/Seal Thermal Margin Analysis|shaft seal thermal margin analysis]] and the [[Design/Mechanisms/Ceramic Mount|Ceramic Mount]] heat-break/holder redesign (2026-09-17). **Bold** rows are what's actually in the final design. "Datasheet" source = pulled from the actual McMaster/manufacturer product page; "Literature" = general reference value, not verified against a specific datasheet — worth a confirmation pass before relying on those for a final go/no-go decision.

## Metals

| Material | k (W/m·K) | Max Temp (°C) | Tensile Strength | Source |
|---|---|---|---|---|
| 304 Stainless Steel (shaft, confirmed McMaster 8934K31) | ~16 | ~870°C (oxidation limit, continuous service) | ~75,000 psi | Literature |
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
| Boron Nitride (current holder) | 20-70 (anisotropic, grade-dependent) | Very high (refractory) | Not rated | Unconfirmed range |
| **Ceramic Alumina** (screw option, rejected — not actually low-k) | ~20-30 | **1,649°C** | Not Rated | **Datasheet (94555A321)**, k literature |
| **Muscovite Mica** — used (washer) | **0.3** | **499°C cont. / 799°C intermittent** | **21,000 psi** | **Datasheet (5067K56)** |
| Phlogopite Mica (extra-high-temp option, not used — muscovite has lower k) | ~0.5-0.6 | 699°C cont. / 999°C intermittent | ~similar to muscovite | Datasheet (temp), k literature |
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
