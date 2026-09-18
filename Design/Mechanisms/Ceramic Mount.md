---
subsystem: mechanisms
tags: [mechanisms, sample-mounting, current-design, active]
---

# Ceramic Mount (Current Design)

**Status**: Primary candidate for sample mounting and positioning.

> **Superseded description, kept for history**: the two-part NPT-threaded design below (Part 1/Part 2) was the original concept. It was superseded by the **Version 3 split-zone design** (orange wedge / gold ring / teal main body / purple cap, connected to the shaft via a 1/4-20 screw joint rather than a 1/4" NPT thread) — see [[#Current Geometry (Version 3, from CAD)|Current Geometry]] and [[#Design Evolution|Design Evolution]] below for the current build. The two-part description is retained here only as background on how the design got started.

Simple two-part sample holder using a threaded boron nitride cylinder and stainless steel seal shaft.

## Design Concept

### Components

**Part 1: Boron Nitride Cylinder**
- **Material**: Boron nitride (20mm OD)
- **Top Port**: 1/4" NPT thread for attachment to seal shaft
- **Bottom Feature**: T-slot retaining groove for sliding in modified charpy sample (updated 2026-08-17 — see below)
- **Advantages**: Non-magnetic, low thermal conductivity, chemically inert

**Part 2: Polished Stainless Steel Shaft**
- **Material**: Stainless steel (mirror polished)
- **Purpose**: Threads into ceramic mount, forms seal interface
- **Finish**: Highly polished to minimize vacuum leakage

### Operation

1. Sample prep: Charpy modified to 6mm length (cut to spec before experiment)
2. Sample loading: Slide charpy into bottom groove of ceramic cylinder
3. Assembly: Screw shaft into cylinder—acts as sample cap/lock
4. Positioning: Slide complete assembly into quartz tube
5. Centering: Quartz tube walls naturally center the ceramic mount

## Key Advantages

✓ **Simplicity** — Only 2 parts (ceramic + shaft)  
✓ **Natural Centering** — Tube wall guides keep mount centered  
✓ **Easy Replacement** — If ceramic cracks, swap quickly  
✓ **Existing Material** — Uses boron nitride pieces already available  
✓ **Minimal Thermal Load** — Ceramic conducts away minimal heat from sample  

## Design Challenges

### 1. Thermal Expansion
- **Issue**: Charpy undergoes thermal expansion under 1000°C heating
- **Risk**: Could crack the ceramic mount
- **Solution**: Machine tight tolerances to allow for thermal cycling
- **Status**: Solvable with precision machining

### 2. Material Cost
- **Concern**: Boron nitride can be expensive
- **Advantage**: Already have scrap pieces on hand
- **Economics**: Favorable since using existing inventory

### 3. Machining Complexity
- **Groove**: Requires precision cutting into ceramic
- **Thread**: 1/4" NPT thread must be accurate
- **Solution**: Procure specialty tool for groove cutting
- **Status**: Solves complexity with correct tooling

### 4. Sample Preparation
- **Requirement**: Charpy must be pre-cut to 6mm length
- **Workflow**: Extra pre-experiment preparation step
- **Status**: Acceptable—one-time setup per sample

## Design Evolution

**Version 1**: Initial concept based on available boron nitride stock, retaining groove sized for a round button-head sample  
**Version 2 (2026-08-17)**: Retaining groove updated to match new [[Design/Sample Quenching/Modified Charpy|Modified Charpy]] T-slot head geometry (button head → T-slot, changed for mass water-jet manufacturability). Because the sample head is now a square-shouldered tab seated in what is otherwise a circular bore, the slot had to be cut a bit farther/deeper into the cylinder to fully capture the square feature.  
**Version 3 (2026-09-12)**: Holder body split into 4 boron nitride sub-sections (bottom to top: orange wedge/clamp, gold ring, teal main body, purple cap), connected to the 1/2" steel shaft via a 1/4-20 × 0.4"L screw rather than the 1/4" NPT thread described above — supersedes the simple two-part description for the current CAD. See geometry table below. Real dimensions came from CAD mass/section properties and a dimensioned section drawing.  
**Iterations**: Refined groove depth and thread interface  
**Status**: Locked in as primary approach; thermal path through this stack is now the limiting factor on the seal (see below)

## Assembly Notes

- Mount weight: minimal (just ceramic + shaft)
- Thermal isolation: Excellent (boron nitride is poor conductor)
- Vacuum compatibility: Good (polished steel creates good seal)
- Failure mode: Ceramic may chip or crack under sample thermal stress

## Manufacturing Specifications

**Note**: this table reflects the original v1/v2 two-part concept. The "Thread" row (1/4" NPT) is superseded by the Version 3 screw joint (1/4-20 × 0.4"L screw, see [[#Thermal Break at the Screw Joint (2026-09-12)|Thermal Break at the Screw Joint]] below) — kept here for history, not current build reference.

| Feature | Spec | Purpose |
|---------|------|---------|
| Cylinder OD | 20mm | Fit in quartz tube clearance |
| Groove Depth | TBD (deeper than v1, to fully seat square T-slot tab in round bore) | Hold charpy sample |
| Groove Width | TBD | Friction fit sample |
| Groove Profile | T-slot (square-shouldered) | Match modified charpy T-slot head |
| Thread | ~~1/4" NPT~~ superseded — see note above | Attach seal shaft (v1/v2 only) |
| Shaft Polish | Mirror finish | Vacuum seal quality |

## Current Geometry (Version 3, from CAD)

![[ceramic-mount-v3-section-view.png]]
![[ceramic-mount-v3-isometric.png]]
![[ceramic-mount-v3-dimensions.png]]

Bottom (sample) to top (shaft), all boron nitride unless noted:

| Zone | Length | Mass | Notes |
|---|---|---|---|
| Orange wedge | 3.175mm | 1.392g | Sample contact, 2×21.308mm² contact patches |
| Gold ring | 4.175mm | 0.986g | |
| Teal main body | 26.65mm | 16.724g | Full ⌀20mm OD cylinder |
| Purple cap | 10.0mm | 5.858g | Bored for screw/dowel connection to shaft |
| Screw | 1/4-20 × 0.4"L | 2.486g | Steel — see thermal break below |
| Shaft | ⌀12.7mm (1/2") | — | Steel, connects up to the [[Design/Plumbing/Vertical Sliding Shaft Seal\|shaft seal]] ~1-1.5in above the cap |

BN density backs out to ~2.0 g/cm³ (consistent, hot-pressed BN — same stock previously used for hot-rolling mill guides).

<details>
<summary>Source CAD mass/section property measurements (click to expand)</summary>

![[ceramic-mount-orange-wedge-mass-props.png]]
![[ceramic-mount-gold-ring-mass-props.png]]
![[ceramic-mount-teal-body-mass-props.png]]
![[ceramic-mount-purple-cap-mass-props.png]]
![[ceramic-mount-screw-mass-props.png]]
![[ceramic-mount-shaft-mass-props.png]]

</details>

## Holder Material Change: Alumina Silicate ("Lava") — recommended (2026-09-12, updated 2026-09-17)

Replacing the boron nitride holder body (orange/gold/teal/purple zones) with **alumina silicate ceramic ("Lava"), McMaster 8479K69** (3/4" dia rod, $79.36) was evaluated as a much bigger thermal lever than the joint heat-break alone:

- **k = 1.265 W/(m·°C) at 20°C** (datasheet-confirmed) vs. BN's ~50 W/m·K — roughly **40x lower conductivity**, dropping the holder's own resistance contribution from ~3.4 K/W to ~130 K/W using the existing zone geometry
- Combined with the titanium-screw + 1/4" mica-washer joint below, full-assembly modeled margin improves from +2% (BN holder) to **~+75%** for the 800°C/2min-hold mock cycle
- **Fabrication note**: sold "Unfired," rated to 2,010°F even unfired per this datasheet — may not require firing at all for this application (peak ~1,562°F/850°C), which would avoid the ~2% fire-expansion dimensioning issue and kiln dependency generally associated with this material class. Needs direct confirmation with McMaster/supplier before relying on the unfired rating.
- **Mechanical properties are modest** — tensile strength only 2,500 psi (compressive 25,000 psi, flexural 10,000 psi) — weaker than BN; fine for this application's light clamp loads but avoid tension/bending-loaded features

**Firing schedule** (if firing is done rather than relying on the as-machined unfired rating — published schedule for this material family, Foundry Service & Supplies Grade A aluminum silicate):
- **Heating ramp**: 111-139°C/hour for standard sections (max 167°C/hour). For thick sections (≥13mm cross-section — applies to the teal main body at ⌀20mm) slow to **28-83°C/hour**, and consider stress-relief holes to reduce cracking risk during the firing itself.
- **Maturing (soak) temperature**: **1,010-1,093°C** — do not exceed 1,093°C (causes crystallization, distortion, shrinkage, loss of properties).
- **Soak time**: 30 min for sections up to ~6mm thick, 45 min for sections ≥13mm thick — the teal body needs the longer soak.
- **Cooling**: no mandated rate published; pull parts once below 93°C. Given the thermal-shock discussion below, cool passively (furnace-closed) rather than open-air, even though the manufacturer doesn't require it.
- **Atmosphere**: ordinary air, nothing special.
- **Dimensional change**: expands ~2% during firing (not shrinkage) — build green-state machining dimensions around growth, not shrinkage.
- **Open issue**: this part has widely varying cross-sections (thin orange/gold wedges vs. the thick teal body) — the whole firing schedule needs to be built around the thickest section's slower ramp rate, which may over-stress the thinner sections. Worth discussing with whoever does the firing.

**Thermal shock risk — reassessed and narrowed (2026-09-17)**: initial concern was unverified thermal shock resistance under repeated rapid heat/quench cycling. Reassessed given the actual quench process — **the holder itself is never submerged; only the sample is quenched in water**:
- Ceramic thermal-shock cracking is driven by rapid *cooling* putting the surface in tension (ceramics, including this one, are much weaker in tension — 2,500 psi — than compression — 25,000 psi). Full immersion in cold water is the severe version of this. Since the holder never contacts the quench bath directly, it never experiences that event — it only sees the gradual, conduction-limited transient already modeled (τ≈89 min for holder+joint), the same duty cycle McMaster's own use-case examples describe ("standoffs and welding jigs" — repeatedly heated by proximity, cooled between cycles, never submerged).
- Even incidental water/steam contact (splash, condensation creeping up the shaft) would be **preheated by the sample's own heat dump into the bath**, not cold bulk-bath-temperature water — a much smaller ΔT than the worst case, and if it's condensing steam specifically, that's a *heating* event (safe compressive stress), not a cooling one.
- **Remaining open item, narrowed**: the **orange wedge** (direct clamped sample contact) still sees the fastest/steepest gradient in the assembly — conducted, not immersion-driven, but worth a validation test specifically on that geometry/contact condition before committing the full holder, rather than testing the whole assembly against full immersion.
- **Fallback if that test doesn't hold up**: machine the holder from **Grade 5 Titanium** instead (same alloy as the screw below) — no ceramic brittle-fracture risk at all, still gives **+49% margin** (vs. Lava's +75%), fully ductile/well-characterized, ordinary shop-machinable. Comfortable margin either way; Lava is the higher-upside option worth trying first given the risk is now well-bounded.
- Full derivation: [[Design/Plumbing/Seal Thermal Margin Analysis.md]]

## Thermal Break at the Screw Joint (2026-09-12)

Real-world thermocouple testing showed the shaft seal **already exceeding** its 210°F rated limit (~120°C / 248°F measured with sample held at 850°C, confirmed value as of 2026-09-18) — see [[Design/Plumbing/Seal Thermal Margin Analysis.md]] for the full derivation. Rather than redesigning the seal, decided to add a heat break at the rod-to-ceramic-holder screw joint:

- **Screw: Grade 5 Titanium (Ti-6Al-4V), 1/4-20 × 3/4"L — McMaster 94081A112.** k≈6.7 W/m·K vs. steel's 16-45 W/m·K (confirmed this needs to be Grade 5 specifically — Grade 2/CP titanium runs k≈17-22 W/m·K, essentially no better than steel; a ceramic-alumina screw option considered along the way was ruled out for the same reason, alumina's k≈20-30 W/m·K isn't actually low). Nonmagnetic, corrosion-resistant, 130,000 psi tensile. Original screw was 0.4"L; confirm the assembly has clearance for the longer 3/4"L part (or trim to length).
- **Washer: Muscovite mica, cut from tube stock — McMaster 5067K56** (1/2" OD × 1/4" ID × 1/8" wall tube; saw thin cross-sectional rings off the end). Datasheet-confirmed k=0.3 W/(m·°C) — blocks the parallel direct-contact heat path between rod end and purple cap face (not just the screw itself). Target thickness ~0.01-0.025" (stack multiple thin slices if needed) adds a meaningful 25-100% to the joint's thermal resistance. Ream the as-cut 1/4" ID slightly for screw clearance (it's an exact nominal fit, not a clearance hole) — mica files/sands easily, just avoid forcing an undersized screw through it. OD (0.5") deliberately matches the rod's own OD so the washer fully covers the rod's end face with no gap or wasted overhang. Chosen over phlogopite mica (marginally higher temp ceiling; k is essentially the same between the two — a 2026-09-18 double-check found the real difference is negligible and, if anything, points the other way from what was first assumed — muscovite was picked for cost and because 499°C already has huge margin over anything this joint sees, not a real k advantage), Macor, Vespel, and PEEK. Light clamp torque at this joint suits mica's main weakness (delamination under shear/point load isn't triggered by pure axial compression).

## Visual Reference & CAD

**Current (Version 3)**: see the section-view, isometric, and dimensioned-drawing renders under [[#Current Geometry (Version 3, from CAD)|Current Geometry]] above — those reflect the actual multi-zone (orange/gold/teal/purple + steel shaft, screw joint) geometry currently in use.

**Superseded (Version 1/2, kept for history only)**: the image below (also duplicated as `ceramic-mount-assembly.png` and `ceramic-mount-detail.png`) shows the earlier single-piece BN cylinder with a plain T-slot groove and square shaft — it predates the Version 3 split-zone redesign and does not reflect the current geometry, material, or joint hardware. Do not use it as a build reference.

![[ceramic-mount-rendering.png]]

**CAD Model**: https://cad.onshape.com/documents/c9bb59bfc991b1182ce6c971/w/cfd3d1ecd4a4ee8129c4b7d7/e/a16968b7dbf6956bf30b9506?renderMode=0&uiState=6a74d7825d9455abadcdb775

## Related Mechanisms

- Control System — Part of overall automation architecture
- [[Design/Mechanisms/Ball Screw|Ball Screw]] — Potential future linear actuation
- [[Design/Mechanisms/Bottom Lift|Bottom Lift]] — Alternative lifting mechanism (rejected)
- [[Design/Mechanisms/Titanium Claw|Titanium Claw]] — Multi-part gripper approach (rejected due to heat sink effect)
- [[Design/Mechanisms/Trapdoor|Trapdoor]] — Sliding plate release concept (deferred)
- [[Design/Coil Geometry/Induction Coil|Induction Coil]] — Mount positions sample inside coil
- [[Design/Vacuum Chamber/Vacuum Enclosure|Vacuum Chamber]] — Mounts inside chamber