---
subsystem: plumbing
tags: [design, plumbing, vacuum-chamber, air-control, automation]
---

# Air System Control Assembly

This assembly currently uses an assortment of bronze 1/4" threaded pipes. There is a 3-way valve where the two inputs are the inert gas (Argon) and the vacuum pump, and the output connects into a 4-way cross pipe. On the top of the cross pipe is a pressure gauge, the opposite side of the cross pipe from the 3-way valve is a pressure relief valve, and the bottom connects to the top of the acrylic lid.

### Vacuum Pump

**Model**: JB Industries DV-85N Platinum — 2-stage, 3 CFM displacement, 1/2 HP, 115VAC.
- **Rated ultimate vacuum**: 15 microns (0.015 torr) — i.e. essentially full vacuum (~29.92" Hg) when working properly.
- **Observed max pull (2026-09-04)**: only ~28" Hg on the chamber gauge — well short of the pump's rated capability.
- **Diagnosis**: A 2-stage pump stalling at 28" almost always means air is leaking into the system faster than the pump can remove it, not a pump limitation. Suspect the shaft seal, lid gasket/acrylic lid fit, or a fitting in this 1/4" bronze assembly (3-way valve, cross pipe, pressure relief valve).
- **Other possible causes**: gauge accuracy/resolution near the top of its scale, contaminated/wet pump oil, insufficient pump run time before reading.
- **Suggested check**: Pull vacuum, close off the pump (isolate via the 3-way valve), and watch the gauge — holding steady at 28" points to the pump/oil; decaying back toward atmospheric confirms a leak on the chamber side.

**Pump-down estimate** (ideal, ~4 L chamber volume, no leaks/moisture):
- To 28" Hg (current real-world ceiling): ~8 sec
- To near pump's rated 15-micron ultimate: ~20-25 sec
- Real-world time will be longer due to 1/4" fitting conductance and any water/moisture in the chamber (see [[Design/Plumbing/Fluid Systems|Fluid Systems]])
- For inert-gas purging, multiple evacuate/backfill cycles (e.g. 2-3 pulls to 28" Hg + argon backfill) reduce residual O2 far more effectively than a single deeper pull

## Automated Control Integration

**Planned Enhancements** (via [[Design/Wiring/NI-DAQ Control Architecture|NI-DAQ Control Architecture]]):
- Pressure transducer monitoring (NI-9219 input) for vacuum safety interlocks
- Solenoid valve control for automatic argon backfill and air purging (future)
- Feedback signals to laptop for closed-loop pressure monitoring
Picture:
![[air-control-assembly.png]]
CAD:https://cad.onshape.com/documents/c9bb59bfc991b1182ce6c971/w/cfd3d1ecd4a4ee8129c4b7d7/e/1fb988f31ac00a38dc1f003a?renderMode=0&uiState=6a75393b615cd4d242af34f6