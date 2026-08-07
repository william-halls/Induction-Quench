---
tags: [mechanisms, sample-mounting, archived, iterations, rejected]
---

# Titanium Claw Gripper

Multi-part clamping mechanism for holding charpy sample. Designed primarily for the quartz tube chamber phase.

**Status**: Abandoned — excessive complexity and thermal performance issues.

## Design Rationale

For the quartz glass chamber concept, the sample mounting had to fit inside the narrow tube while:
- Allowing thermocouple measurement
- Permitting air/steam expansion during quenching
- Minimizing thermal contact (avoiding heat sink effect)

## Design Evolution

### Version 1: 9-Part Assembly

**Components:**
- 2 screw tilt adjusters (top alignment)
- 2 titanium grip pads
- 1 ceramic mount (center)
- 2 long screws (tension/alignment)
- 2 nuts (fasteners)

**Operation:**
- Top screw set: Controls grip alignment and pressure
- Bottom screw set: Adjusts grip tension on sample

**Issues:**
- ✗ Unnecessary complexity
- ✗ Does not fit inside quartz glass tube diameter
- ✗ Over-engineered for initial prototype

**Decision**: Too complicated; moved to V2 simplification

---

### Version 2: 7-Part Assembly

**Components:**
- 2 titanium claw grips
- 1 ceramic mount
- 2 long screws
- 2 nuts

**Changes from V1:**
- Removed tilt adjusters
- Streamlined fastening
- Kept grip tension adjustment

**Issues:**
- ✗ Nut size: Could not find nuts small enough to fit in quartz tube
- ✗ Grip thickness: Material too thin; risk of warping when quenched
- ✗ Diameter still exceeded tube constraints

**Decision**: Thickness and fastener sizing unresolved; advanced to V3

---

### Version 3: 7-Part Refined

**Components:**
- 2 titanium grips (Grade 5)
- 1 titanium center mount (Grade 2)
- 2 grub screws (grip alignment, top holes)
- 2 flat-head screws (black oxide stainless steel, hex socket)

**Design Changes:**
- Increased grip thickness to prevent warping
- Changed center mount material: Grade 2 titanium (softer, more ductile than Grade 5)
- Kept grips as Grade 5 (stronger)
- Counter-sunk grips to reduce profile for tube fitting
- Used flat-head fasteners instead of nuts (lower profile)

**Improvements:**
- ✓ Reduced warp risk
- ✓ Better stress distribution with material grade separation
- ✓ Lower profile fits better in tube
- ✓ Grub screws for alignment, hex socket screws for strength

**Critical Issue Discovered: Heat Sink Effect**
- Thermal simulation showed titanium grips draw significant heat away from charpy
- Temperature gradient across sample became uneven
- Primary heating surface heated; gripped areas stayed cooler
- **Effect**: Compromised experimental validity (uneven microstructure)

**Decision**: Rejected despite engineered refinements

---

## Why Abandoned

### 1. **Excessive Complexity**
- 7 parts per sample holder
- Multiple screw types (grub screws, flat-head, varying grades)
- Machining requirements extensive

### 2. **Galvanic Corrosion Risk**
- Stainless screws inside titanium parts
- Risk of seizing over time
- Difficult to disassemble after thermal cycling

### 3. **Thermal Performance**
- **Heat Sink Effect**: Titanium grips conduct heat away from sample
- Charpy clamped region stays cooler than free surfaces
- Creates uneven temperature gradient → uneven microstructure
- This is a **fundamental flaw** not solvable by design refinement

### 4. **Degrees of Freedom**
- Too many adjustment points (tension, alignment, positioning)
- Adds variability to experiments
- Initial prototype doesn't need fine-tuning capability

## Key Lessons

**Material Selection Matters**: Thermal properties outweigh mechanical refinement.  
**Simulation Catches Flaws**: FEA discovered heat sink effect before prototype testing.  
**Simplicity > Optimization**: Core concept was flawed; refinement can't fix fundamental issues.  
**Thermal Isolation Critical**: Need materials that don't conduct away sample heat.

## Visual References & CAD

**Claw V1:**
![[titanium-claw-v1.png]]
Model: https://cad.onshape.com/documents/c9bb59bfc991b1182ce6c971/w/cfd3d1ecd4a4ee8129c4b7d7/e/734c1f49de98495526b388ae?renderMode=0&uiState=6a74ce6e5d9455abadcd888f

**Claw V2:**
![[titanium-claw-v2.png]]
Model: https://cad.onshape.com/documents/c9bb59bfc991b1182ce6c971/w/cfd3d1ecd4a4ee8129c4b7d7/e/31b32c13307e270c7606af4c?renderMode=0&uiState=6a74cfef5d9455abadcd8fa7

**Claw V3 (Final):**
![[titanium-claw-v3-final.png]]
Model: https://cad.onshape.com/documents/c9bb59bfc991b1182ce6c971/w/cfd3d1ecd4a4ee8129c4b7d7/e/e6b95b592806d4d918246f5d?renderMode=0&uiState=6a74d1ec5d9455abadcd9e99

## Related Mechanisms

- [[Design/Mechanisms/Ceramic Mount|Ceramic Mount]] — Current preferred approach (avoids heat sink)
- [[Design/Mechanisms/Bottom Lift|Bottom Lift]] — Alternative sample positioning (also rejected)
- [[Design/Mechanisms/Trapdoor|Trapdoor]] — Sliding release concept (deferred)
- [[Design/Vacuum Chamber/Quartz Glass Tube|Quartz Glass Tube]] — Original chamber design these grips were meant for
