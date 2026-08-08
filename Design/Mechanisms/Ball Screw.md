---
tags: [design, mechanisms, automation, motor-control]
---

# Ball Screw Assembly

Similar to the FUYU FSK40E series but unbranded. The actual parts and stepper are identical except with no logo attached. This is what we currently use to move the shaft up and down.

## Motor Control

**Current Setup**: NEMA 23 2-phase stepper motor with external controller (model TBD)

**Automated Control** (in development):
- See [[Design/Wiring/NI-DAQ Control Architecture|NI-DAQ Control Architecture]] for integration with NI-9263 signal output
- Awaiting stepper controller identification to determine best control method (pulse/direction vs. analog speed vs. serial)

## Shaft Clamp Design

Current plan is to 3D print a clamp that attaches to the ball screw mount. When screwing in the 4 screws, it clamps the shaft to the ball screw mount.

## Component Images

### Ball Screw
![[ball-screw-stepper-assembly.png]]

[CAD Model](https://cad.onshape.com/documents/c9bb59bfc991b1182ce6c971/w/cfd3d1ecd4a4ee8129c4b7d7/e/bf36553ecacc1a73d342fb18?renderMode=0&uiState=6a768005f27b0a87b4b71d59)

### Shaft Clamp Mount
![[shaft-clamp-mount.png]]

[CAD Model](https://cad.onshape.com/documents/c9bb59bfc991b1182ce6c971/w/cfd3d1ecd4a4ee8129c4b7d7/e/b4787fd1cac9f9febf2767d6?renderMode=0&uiState=6a76804ef27b0a87b4b71ec4)

### Ball Screw Assembly
![[ball-screw-full-assembly.png]]

[CAD Model](https://cad.onshape.com/documents/c9bb59bfc991b1182ce6c971/w/cfd3d1ecd4a4ee8129c4b7d7/e/312df3f72b112ac5329c8313?renderMode=0&uiState=6a768081f27b0a87b4b71f2e)
