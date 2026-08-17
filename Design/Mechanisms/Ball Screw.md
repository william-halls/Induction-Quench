---
subsystem: mechanisms
tags: [design, mechanisms, automation, motor-control, stepper-motor]
---

# Ball Screw Assembly

Similar to the FUYU FSK40E series but unbranded. The actual parts and stepper are identical except with no logo attached. This is what we currently use to move the shaft up and down.

## Identified Components

**Complete electrical control system now documented.** See [[Design/Mechanisms/Ball Screw Motor Control|Ball Screw Motor Control]] for full specifications, wiring diagrams, and system architecture.

| Component | Model / SN | Purpose |
|-----------|-----------|---------|
| **Power Supply** | SDN 10-24-100P (SolaHD) | AC→DC conversion (240W, 24V @ 10A) |
| **Motion Controller** | ST-PMC1 (SN: 170120011) | Programmable stepper sequencer (99 programs, 40 kHz max) |
| **Stepper Driver** | TB6600 or equiv. (SN: 170120011) | Coil amplifier (5-10A @ 24V) |
| **Stepper Motor** | NEMA 23 or 34 (SN: 161104226) | Rotary actuator (200 steps/rev, integrated ball screw) |

**Key Capability:** ±0.025mm positioning per full step, or ±0.006mm with 1/16 microstepping.

## Motor Control

**Current Setup**: [[Design/Mechanisms/Ball Screw Motor Control|ST-PMC1 programmable controller]] + NEMA 23/34 stepper motor with ball screw linear actuator

**Electrical Specifications:**
- ✅ Power: 24V DC, 10A supply (SDN 10-24-100P) provides 2× safety margin
- ✅ Control: Pulse+direction signals (1–40 kHz) from ST-PMC1 to stepper driver
- ✅ Motion: Up to 200 full-steps per revolution = ~0.025mm per step (5–10mm ball screw lead)
- ✅ Holding: Motor coils always energized → fail-safe position hold (sample cannot drift)

**Automated Control** (in development):
- See [[Design/Wiring/NI-DAQ Control Architecture|NI-DAQ Control Architecture]] for integration with NI-9263 signal output
- ST-PMC1 can receive start/stop commands from external triggers; coordinate motor timing with quench valve relay outputs
- See [[Design/Mechanisms/Ball Screw Motor Control|Motor Control documentation]] for detailed commissioning guide

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
