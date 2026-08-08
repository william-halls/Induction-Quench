---
tags: [design, plumbing, vacuum-chamber, air-control, automation]
---

# Air System Control Assembly

This assembly currently uses an assortment of bronze 1/4" threaded pipes. There is a 3-way valve where the two inputs are the inert gas (Argon) and the vacuum pump, and the output connects into a 4-way cross pipe. On the top of the cross pipe is a pressure gauge, the opposite side of the cross pipe from the 3-way valve is a pressure relief valve, and the bottom connects to the top of the acrylic lid.

## Automated Control Integration

**Planned Enhancements** (via [[Design/Wiring/NI-DAQ Control Architecture|NI-DAQ Control Architecture]]):
- Pressure transducer monitoring (NI-9219 input) for vacuum safety interlocks
- Solenoid valve control for automatic argon backfill and air purging (future)
- Feedback signals to laptop for closed-loop pressure monitoring
Picture:
![[air-control-assembly.png]]
CAD:https://cad.onshape.com/documents/c9bb59bfc991b1182ce6c971/w/cfd3d1ecd4a4ee8129c4b7d7/e/1fb988f31ac00a38dc1f003a?renderMode=0&uiState=6a75393b615cd4d242af34f6