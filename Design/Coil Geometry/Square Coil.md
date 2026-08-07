---
tags: [coil-design, square, rejected, theoretical, archived]
---

# Square Coil Design

## Concept

Square/rectangular coil geometry matching the charpy sample's cross-section. **Theoretical advantage**: better field alignment with sample geometry. **Status**: Currently deferred due to manufacturing and electrical concerns.

## Design Rationale

### Theoretical Benefits
- ✓ Better field geometry matching square sample cross-section
- ✓ Potentially more uniform heating across sample faces
- ✓ Reduced field fringing at corners

## Manufacturing Challenges

### Primary Concern: Price
* Significant manufacturing cost
- Each 90° corner turn in the tubing **severely reduces coolant flow**
- Cooling performance becomes critical bottleneck
- May require multiple tubing passes or external cooling

### Electrical Concerns
- **Self-coupling effects** — Square field geometry may create unforeseen interactions with itself
- Difficult to model and predict performance
- Unknown impedance behavior at operating frequency

## Current Status

🟡 **Deferred** — Possible to implement, but not practical at current time

- Round coil is simpler and proven to work
- Square coil requires:
  - Advanced flow analysis
  - Electromagnetic simulation
  - Prototype testing (high risk)
  
## Reconsideration Criteria

Could revisit if:
- Round coil performance is insufficient
- Better cooling tube materials/designs emerge
- FEA analysis shows significant heating advantage
- Manufacturing methods improve (additive manufacturing?)

## Related Work

- [[Design/Coil Geometry/Coil Feature Script|Coil Feature Script]] includes parametric support for rectangular profiles
- Can rapidly prototype once manufacturing method is defined