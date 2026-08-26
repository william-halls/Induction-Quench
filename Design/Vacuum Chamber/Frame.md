---
subsystem: vacuum_chamber
tags: [vacuum-chamber, frame, extrusion, hardware, structural]
---

# Frame — Series 15 Aluminum Extrusion Hardware

Notes on fastening aluminum plates to the Series 15 aluminum extrusion frame (support structure above the vacuum chamber lid).

## Thread Engagement — 3/8-28 Screw into Extrusion

**Setup**: 3/8-28 × 3/4" long screw, passing through a clearance hole in a 1/4" thick aluminum plate, threading into the Series 15 extrusion's center hole.

- **Engagement** = screw length − plate thickness = 3/4" − 1/4" = **0.5"** into the extrusion
  - (Extrusion center bore runs the full length of the profile, so depth isn't the limiting factor — only screw length is.)
- **Recommended engagement for aluminum**: 1.5–2× screw diameter → 0.56"–0.75" for a 3/8" screw
- **Verdict**: 0.5" (≈1.33× diameter) is **just under the recommended range, but acceptable** for general fixturing/structural loads — not marginal, just no extra strength margin. Don't over-torque.
- **For full-strength margin**: use a **7/8"–1" long screw** instead → bumps engagement to 0.625"–0.75", solidly in the recommended range.

### Important
- Tapping threads directly into the **1/4" plate itself** (instead of passing through into the extrusion) only gives **1/4" engagement (0.67× diameter)** — this is **insufficient**, prone to stripping under load/vibration. Avoid; use a clearance hole in the plate and thread into the extrusion instead.
- If attaching via a **T-nut in the extrusion's T-slot** (rather than the center hole) instead, real engagement is the thread length inside the T-nut — typically ≥3/8", generally adequate on its own.

## Related Documentation

- [[Design/Vacuum Chamber/Used Vacuum Chamber|Used Vacuum Chamber]] — aluminum plate stack bolts to this frame
- [[Design/Vacuum Chamber/Vacuum Chamber CAD|Vacuum Chamber CAD]] — extrusion frame connections in CAD model
