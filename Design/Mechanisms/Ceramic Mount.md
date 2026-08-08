---
tags: [mechanisms, sample-mounting, current-design, active]
---

# Ceramic Mount (Current Design)

**Status**: Primary candidate for sample mounting and positioning.

Simple two-part sample holder using a threaded boron nitride cylinder and stainless steel seal shaft.

## Design Concept

### Components

**Part 1: Boron Nitride Cylinder**
- **Material**: Boron nitride (20mm OD)
- **Top Port**: 1/4" NPT thread for attachment to seal shaft
- **Bottom Feature**: Groove for sliding in modified charpy sample
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

**Version 1**: Initial concept based on available boron nitride stock  
**Iterations**: Refined groove depth and thread interface  
**Status**: Locked in as primary approach

## Assembly Notes

- Mount weight: minimal (just ceramic + shaft)
- Thermal isolation: Excellent (boron nitride is poor conductor)
- Vacuum compatibility: Good (polished steel creates good seal)
- Failure mode: Ceramic may chip or crack under sample thermal stress

## Manufacturing Specifications

| Feature | Spec | Purpose |
|---------|------|---------|
| Cylinder OD | 20mm | Fit in quartz tube clearance |
| Groove Depth | TBD | Hold charpy sample |
| Groove Width | TBD | Friction fit sample |
| Thread | 1/4" NPT | Attach seal shaft |
| Shaft Polish | Mirror finish | Vacuum seal quality |

## Visual Reference & CAD

![[ceramic-mount-rendering.png]]

**CAD Model**: https://cad.onshape.com/documents/c9bb59bfc991b1182ce6c971/w/cfd3d1ecd4a4ee8129c4b7d7/e/a16968b7dbf6956bf30b9506?renderMode=0&uiState=6a74d7825d9455abadcdb775

## Related Mechanisms

- [[Design/Mechanisms/Control System|Control System]] — Part of overall automation architecture
- [[Design/Mechanisms/Ball Screw|Ball Screw]] — Potential future linear actuation
- [[Design/Mechanisms/Bottom Lift|Bottom Lift]] — Alternative lifting mechanism (rejected)
- [[Design/Mechanisms/Titanium Claw|Titanium Claw]] — Multi-part gripper approach (rejected due to heat sink effect)
- [[Design/Mechanisms/Trapdoor|Trapdoor]] — Sliding plate release concept (deferred)
- [[Design/Coil Geometry/Induction Coil|Induction Coil]] — Mount positions sample inside coil
- [[Design/Vacuum Chamber/Vacuum Enclosure|Vacuum Chamber]] — Mounts inside chamber