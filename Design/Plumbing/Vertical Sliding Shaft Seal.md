---
subsystem: plumbing
tags: [design, plumbing, sealing, linear-actuation]
---

# Vertical Sliding Shaft Seal

**Design Plan:**
- Press-fit spring-loaded lip seal rated for 50 psi
- Allows 1/2" shaft linear actuation with minimal horizontal load
- Optimizes seal performance and longevity

**Lip Seal:**
- Spring-Loaded Rotary Shaft Seal, McMaster **5154T48**
- Sized for 1/2" shaft
- Material: Buna-N (Nitrile), Durometer 80A; steel case, steel spring
- Dimensions: 0.500" ID / 0.875" OD / 0.313" width (0.875" bore)
- Rated: 50 psi max pressure, 2,500 rpm max rotation
- **Temperature range: -40°F to 210°F** — this is the T_max ceiling for the thermal margin gauge on this seal

**Thermal Margin (2026-09-12)**:
- Real thermocouple test: seal location stabilizes at **~100°C (212°F)** with sample held at 850°C — right at the rated limit, near-zero steady-state margin
- Extensive search for a higher-temp drop-in replacement (McMaster, AVX, SKF/CR, Kurt J. Lesker) found no clean match combining correct dimensions + adequate pressure rating + confirmed linear-motion service — decision made to **keep this seal** and fix the thermal problem at the source instead (standoff distance + heat-break at the rod/ceramic-holder joint — titanium screw + mica washer, see [[Design/Mechanisms/Ceramic Mount.md]])
- Full derivation, sensitivity analysis, and gauge formula: [[Design/Plumbing/Seal Thermal Margin Analysis.md]]

**Shaft:**
- 1/2" OD 304 stainless steel rod, McMaster **8934K31**
  - Tolerance: ±0.0005" (tight-tolerance)
  - Mechanical finish: Precision Ground
  - Nonmagnetic (safe near induction coil field)
  - Hardness: Rockwell B80
  - $24.05/ft, in stock
  - Quench media is water — 304 corrosion resistance is sufficient; 316/316L (McMaster 8936K6, same tolerance/finish) would be the upgrade path if quenchant chemistry ever changes to something with chlorides (brine, aggressive polymer quenchant, etc.), but was temporarily unavailable at time of selection
- Attached to ball screw for linear drive