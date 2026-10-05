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

**Note**: this table reflects the original v1/v2 two-part concept. The "Thread" row (1/4" NPT) is superseded by the Version 3 screw joint (1/4-20 × 0.4"L screw; the deferred titanium-screw thermal break is documented in [[Design/Mechanisms/Hybrid Ceramic Holder|Hybrid Ceramic Holder]]) — kept here for history, not current build reference.

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

## Holder Material Change: Hybrid BN + Alumina Silicate ("Lava") — concept

> **Moved to its own note (2026-09-21):** [[Design/Mechanisms/Hybrid Ceramic Holder|Hybrid Ceramic Holder]]

**Concept only — not committed, not quoted, machined, or tested.** Summary:

- Keep the **orange wedge + gold ring in BN** (thermal-shock tolerance at the sample-contact interface); switch the upper holder to **alumina silicate ("Lava", McMaster 8479K69)**, k = 1.265 W/(m·°C) vs. BN's ~60-250 W/m·K.
- **Why**: the shaft seal already measures ~120°C / 248°F against a 210°F rating — the swap projects the seal back into positive margin (+51.0% to +80.6%, configuration pending confirmation).
- **Joint**: pinned clevis — Lava fork/cap + BN tongue + AS ceramic cross-pin in double shear. Metal never contacts BN.
- The **titanium screw + mica washer thermal break** is documented there too, as a deferred fallback option.

See the [[Design/Mechanisms/Hybrid Ceramic Holder|Hybrid Ceramic Holder]] note for geometry, load check, machinability, firing schedule, margin projections, and open items.

## Visual Reference & CAD

**Current (Version 3)**: see the section-view, isometric, and dimensioned-drawing renders under [[#Current Geometry (Version 3, from CAD)|Current Geometry]] above — those reflect the actual multi-zone (orange/gold/teal/purple + steel shaft, screw joint) geometry currently in use.

**Superseded (Version 1/2, kept for history only)**: the image below (also duplicated as `ceramic-mount-assembly.png` and `ceramic-mount-detail.png`) shows the earlier single-piece BN cylinder with a plain T-slot groove and square shaft — it predates the Version 3 split-zone redesign and does not reflect the current geometry, material, or joint hardware. Do not use it as a build reference.

![[ceramic-mount-rendering.png]]

**CAD Model**: https://cad.onshape.com/documents/c9bb59bfc991b1182ce6c971/w/cfd3d1ecd4a4ee8129c4b7d7/e/a16968b7dbf6956bf30b9506?renderMode=0&uiState=6a74d7825d9455abadcdb775

## Related Mechanisms

- [[Design/Mechanisms/Hybrid Ceramic Holder|Hybrid Ceramic Holder]] — BN + Lava material-change concept (seal thermal margin)
- Control System — Part of overall automation architecture
- [[Design/Mechanisms/Ball Screw|Ball Screw]] — Potential future linear actuation
- [[Design/Mechanisms/Bottom Lift|Bottom Lift]] — Alternative lifting mechanism (rejected)
- [[Design/Mechanisms/Titanium Claw|Titanium Claw]] — Multi-part gripper approach (rejected due to heat sink effect)
- [[Design/Mechanisms/Trapdoor|Trapdoor]] — Sliding plate release concept (deferred)
- [[Design/Coil Geometry/Induction Coil|Induction Coil]] — Mount positions sample inside coil
- [[Design/Vacuum Chamber/Vacuum Enclosure|Vacuum Chamber]] — Mounts inside chamber