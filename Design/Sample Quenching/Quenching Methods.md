---
subsystem: sample_quenching
tags: [design, quenching, sample-handling, heat-treatment, thermal-cycling]
---

# Sample Quenching Routes & Methods

Rapid cooling strategy after heating to 1000°C. Critical for achieving desired microstructure and test objectives.

## Candidate Methods & Decision Criteria

**Status**: TBD — Material properties and experimental requirements will determine cooling rate targets.

### Liquid Quenching
**Oil Quench**
- Rapid cooling, good control
- Risk: Oxidation, post-quench cleanup
  
**Water Quench**
- Very rapid cooling
- Risk: Oxidation, thermal shock, boiling at vacuum
- Note: Water boil-off in vacuum chamber is a design concern (see Plumbing notes)

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
- [[Design/Plumbing/Fluid Systems|Plumbing & Fluid Systems]] — Medium delivery and circulation
- [[Design/Mechanisms/Control System|Mechanisms & Automation]] — Quench triggering and timing
- [[Design/Wiring/Electrical System|Wiring & Electrical]] — Quench initiation signals
- [[Design/Wiring/NI-DAQ Control Architecture|NI-DAQ Control Architecture]] — Automated quench triggering via NI-9263; pressure spike monitoring via NI-9219
- [[Design/Archive/Design History|Design Archive]] — Previous quench methods and results
