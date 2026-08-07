---
tags: [vacuum-chamber, current-design, upcycled, active]
---

# Used Vacuum Chamber (Current Design)

**Status**: Primary candidate for initial testing. Repurposed industrial resin-degassing chamber adapted for induction quench testing.

## Chamber Overview

### Base Vessel
- **Construction** — Stainless steel bucket
- **Diameter** — ~11 inches
- **Height** — ~11 inches  
- **Capacity** — ~4 liters (water + sample)
- **History** — Previously used for resin bubble-out operations

### Original Lid (Pre-Modification)
- **Material** — Acrylic (12.5" diameter × 0.7" thick)
- **Current State** — Requires replacement
- **Original Ports**:
  - 1 large hole with rubber stopper (unusable)
  - 1 un-threaded hole with expired pressure gauge (non-functional)

## Current Design Plan

### New Lid Specifications
**Goal**: Replace acrylic lid with custom design incorporating all necessary feedthroughs

#### Central Sample Holder
- **Seal Type** — Press-fit seal rated to 50 psi
- **Shaft** — 1/2" diameter rod (through-seal)
- **Purpose** — Holds modified ceramic charpy holder

#### Structural Reinforcement  
To support bucket load with ~4L water and pressure:
- **Aluminum Plate Stack** — 3 sheets (1/4" or 3/8" thick)
- **Fastening** — 8× 1/4"-20 screws per sheet into new acrylic lid
- **Load Path** — Each plate connects via 3/8" screw to 1.5" aluminum extrusion frame above

#### Feedthrough Ports
- **Coil Power Leads** — 2 holes for high-frequency power connections
- **Air Control Assembly** — 1 hole for vacuum/argon manifold
- **Pressure Monitoring** — New pressure gauge port (location: TBD)
- **Thermocouple** — Feedthrough in chamber wall (planned)

### Support Mechanism
- **Scissor Lift** — Supports bucket from below
- **Purpose** — Easier manual movement for water changes/maintenance
- **Load Capacity** — Must support ~40-50 lbs (bucket + 4L water)

## Status & Next Steps

### Outstanding Decisions
- ⏳ **Structural Design Review** — Confirm aluminum plate reinforcement strategy with Dr. Buchely
- ⏳ **Lid Fabrication** — Timeline and vendor for custom acrylic lid
- ⏳ **Feedthrough Design** — Finalize port layouts for coil leads, thermocouple, pressure gauge

## Visual Reference

![[main-assembly.png]]

---

## Related Documentation

- [[Design/Vacuum Chamber/Vacuum Chamber CAD|Vacuum Chamber CAD]] — 3D model and design specifications
- [[Design/Coil Geometry/Round Coil|Round Coil]] — Must fit within chamber envelope
- [[Design/Plumbing/Fluid Systems|Plumbing Systems]] — Gas/vacuum line connections