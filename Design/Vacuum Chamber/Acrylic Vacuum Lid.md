---
subsystem: vacuum_chamber
tags: [design, vacuum, chamber, lid, acrylic, fabrication]
---

# Acrylic Vacuum Lid

Custom-fabricated lid for the [[Design/Vacuum Chamber/Used Vacuum Chamber|repurposed bucket vacuum chamber]]. Mounts via 3 brackets; the bucket itself is supported separately from below by the scissor (X-) lift, so the lid brackets only carry the lid's own weight, not the vacuum seal load.

## Material & Stock

- **Material**: Cast acrylic sheet
- **Thickness**: 11/16" (0.6875")
- **Stock**: 12"×12" square — sufficient to cut a 12.5" (or larger) diameter disc to clear the bucket seal
- **Bucket seal OD**: 12"
- **Sourcing**: McMaster-Carr, Tap Plastics, or local plastics supplier (search "acrylic sheet 11/16")

## Edge Treatment

- **Chamfer**: ~1mm on the lid perimeter
- Primarily a deburr/handling-safety feature at this size — not significant structural stress relief, but adequate since the lid isn't under high edge load in this application (weight-only support via brackets, not a vacuum-sealed edge).

## Mounting: 3-Bracket System

- **Layout**: 3 triangular brackets at 120° spacing around the lid perimeter
- **Purpose**: Support the lid only — the bucket is held from below via the scissor lift, so bracket loads are lid weight + handling, not full vacuum/seal force
- **Fastening**: 1/4" bolts through the acrylic into an **aluminum backing plate** on the underside of each bracket
  - Aluminum plate acts as a load-spreading washer — distributes clamp force across the full plate footprint rather than concentrating it at each hole, and stiffens the acrylic against local flex at the bolt line
  - Reduces point-stress cracking risk compared to washers at individual holes alone

### Hole Pattern
- Two rows of holes per bracket near the lid's curved edge (bolt-circle pattern), evenly spread for load distribution
- Center hole in the lid for a separate fitting (not part of the bracket mounting)

### Design Guidelines Applied
- **Edge distance**: target ≥2-3× hole diameter (~1/2"–3/4" min for 1/4" holes) from the lid's cut edge to hole center — verify this accounts for the chamfer's angled face, not just top-down plan view
- **Seal groove clearance**: confirm the inner row of bracket holes doesn't intersect or sit too close to the seal channel where brackets meet the lid — grooves are stress risers themselves
- **Bolt fit**: slight clearance (e.g. 17/64"–9/32" hole for 1/4" bolt) to avoid binding from thermal expansion mismatch between acrylic and aluminum/steel fasteners
- **Torque**: don't overtighten — even with the aluminum spreader plate, excessive clamping force can crack acrylic locally

## Drilling Notes

- Use a **brad-point or plastic-specific drill bit** (standard twist bits tend to grab and crack acrylic)
- **Peck drill** through the 11/16" thickness — clear chips periodically rather than drilling straight through, to avoid heat buildup/crack
- **Sacrificial backer** (wood/MDF) under the exit side to prevent blowout
- Clamp securely but avoid overtightening clamps (localized stress)
- Chips should curl, not melt to dust — if melting, slow down or add cutting fluid/water

## Status

🟡 In progress — design finalized, fabrication pending (per [[Design/Vacuum Chamber/Vacuum Enclosure|Vacuum Enclosure]] status: "custom acrylic lid, to be fabricated")

## Related

- [[Design/Vacuum Chamber/Vacuum Enclosure|Vacuum Enclosure]] — Parent chamber overview
- [[Design/Vacuum Chamber/Used Vacuum Chamber|Used Vacuum Chamber]] — Bucket base specifications
- [[Design/Mechanisms/Control System|Mechanisms & Automation]] — Scissor (X-) lift supporting the bucket from below
- [[Design/Plumbing/Vertical Sliding Shaft Seal|Shaft Seal Design]] — Sample port seal in lid center
