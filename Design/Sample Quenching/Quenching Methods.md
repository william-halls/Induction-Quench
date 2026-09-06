---
subsystem: sample_quenching
tags: [design, quenching, sample-handling, heat-treatment, thermal-cycling]
---

# Sample Quenching Routes & Methods

Rapid cooling strategy after heating to 1000°C. Critical for achieving desired microstructure and test objectives.

## Delivery Mechanism (Decided)

Quenching is performed by driving the [[Design/Mechanisms/Ball Screw|ball screw]] to lower the sample — still clamped in its [[Design/Mechanisms/Ceramic Mount|ceramic mount]] — directly down into a water bath. **There is no separate release/drop mechanism**: the sample never disconnects from the mount/shaft at any point in the cycle. This is why the release-style mechanisms in [[Design/Mechanisms/INDEX|Mechanisms & Automation]] (Bottom Lift, Titanium Claw, Trapdoor) are deferred/abandoned rather than developed further — the ball screw's existing vertical travel does the job directly.

## Candidate Methods & Decision Criteria

**Status**: Water quench (via ball-screw immersion, see above) is the current decided approach. Oil/gas/spray remain candidate alternatives, not currently implemented.

### Liquid Quenching
**Oil Quench**
- Rapid cooling, good control
- Risk: Oxidation, post-quench cleanup
  
**Water Quench (current)**
- Very rapid cooling
- Risk: Oxidation, thermal shock, boiling at vacuum
- Note: Water boil-off in vacuum chamber is a design concern (see Plumbing notes)
- Delivery: sample is lowered into the water bath by the ball screw (see Delivery Mechanism above), not sprayed/released onto a stationary sample

### Gas Quenching
**Argon/Nitrogen Quench**
- Clean environment (no oxidation)
- Slower cooling rate
- Advantage: Maintains inert atmosphere

### Advanced Method
**Spray Quenching**
- 24V diaphragm pump available for custom nozzle
- Could overcome Leidenfrost effect for improved cooling uniformity
- Concept: Modify pump inlet to create spray pattern
- Status: Opportunistic if other systems allow

## Key Design Factors

- **Cooling Rate** — Material-dependent; affects final microstructure and properties
- **Medium Selection** — Oil (high quench power, contamination risk), water (fast, oxidation), gas (clean, slow), air (slowest)
- **Uniformity** — Even cooling across entire charpy sample geometry
- **Temperature Control** — Quench medium preheat/cooling for repeatability
- **Safety** — Hot sample handling, medium volatility/toxicity, overflow containment
- **Data Integrity** — Thermocouple measurement during quench process

## Integration Points

- [[Design/Vacuum Chamber/Vacuum Enclosure|Vacuum Chamber]] — Sample positioning and access
- [[Design/Plumbing/Fluid Systems|Plumbing & Fluid Systems]] — Water bath fill/circulation (bath is pre-filled/static; not sprayed onto the sample)
- [[Design/Mechanisms/Ball Screw|Ball Screw]] — Drives sample immersion into the quench bath; no separate release mechanism
- [[Design/Wiring/Electrical System|Wiring & Electrical]] — Quench initiation signals
- [[Design/Wiring/NI-DAQ Control Architecture|NI-DAQ Control Architecture]] — Automated quench triggering via NI-9263; pressure spike monitoring via NI-9219
- [[Design/Archive/Design History|Design Archive]] — Previous quench methods and results
