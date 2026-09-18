---
subsystem: plumbing
tags: [design, plumbing, gas-lines, fluid-systems, vacuum]
---

# Plumbing & Fluid Systems

Gas delivery, vacuum evacuation, and quenching medium circulation systems for chamber environment control.

## Current Design: Multi-Purpose Plumbing Assembly

**Status**: In progress — Core components specified; final routing TBD.

### Vacuum System
- **Pump**: Existing equipment (details TBD)
- **Target**: Achieve adequate vacuum for inert gas backfill and prevent water boiling
- **Concern**: Water boil-off at vacuum — impact on backfill requirements (TBD)
- **Configuration**: Roughing pump + fine pump lines (standard practice)

### Inert Gas System
- **Gas**: Argon (primary) or nitrogen (alternative)
- **Supply**: Via air control manifold (3-way valve)
- **Purpose**: Backfill after evacuation; maintain inert atmosphere during heating
- **Integration**: Air System Control Assembly in lid (bronze 1/4" pipe networks)

### Quench System
- **Medium**: Water (decided) — see [[Design/Sample Quenching/Quenching Methods|Quenching Methods]] for full rationale. Oil/gas remain candidate alternatives, not implemented.
- **Delivery (decided)**: Pre-filled static water bath; the [[Design/Mechanisms/Ball Screw|ball screw]] lowers the sample (still clamped in its [[Design/Mechanisms/Ceramic Mount|ceramic mount]]) directly into the bath — no valve, spray, or release mechanism involved.
- **Opportunistic alternative**: A custom nozzle on the 24V diaphragm pump inlet could enable spray quenching (overcomes Leidenfrost effect) if pursued later — this would require sample release, which the current always-clamped design doesn't do, so it's not on the current path.
- **Temperature Control**: Pending coolant system design

## Key Design Factors

- **Vacuum Performance** — Pump selection, line diameter, leak prevention
- **Inert Atmosphere** — Gas purity, backfill rate, pressure stability during heating
- **Quench Delivery** — Medium choice (oil/water/air), flow uniformity, thermal control
- **Line Sizing** — Pressure drop, flow rates, material compatibility at temperature
- **Filtration** — Particle removal from gas and quench medium
- **Pressure Relief** — Over-pressure protection, emergency venting
- **Thermal Management** — Fluid cooling if required, insulation of hot lines

## Integration Points

- [[Design/Vacuum Chamber/Vacuum Enclosure|Vacuum Chamber]] — Chamber connections and feedthroughs
- [[Design/Sample Quenching/Quenching Methods|Sample Quenching Routes]] — Quench delivery method
- Mechanisms & Automation — Flow control valve actuation
- [[Design/Wiring/Electrical System|Wiring & Electrical]] — Pump power and control signals
- [[Design/Archive/Design History|Design Archive]] — Previous fluid system configurations
