---
tags: [design, plumbing, coil-connections, electrical, power-delivery]
---

# Coil Lead Pass-Throughs

**Status: Redesigned (2026-08-17)** — Original bronze/brass flared-fitting pass-through concept **rejected**; see [[Design/Archive/INDEX|Archive]] for details.

## Current Design: SSLK-14-14 Compression Fittings

We are using **Omega SSLK-14-14** 1/4" x 1/4" NPT stainless steel compression fittings for the coil lead pass-throughs.

**Rationale:** The original brass flared-fitting pass-throughs would resist too much high-frequency power, forcing roughly double the input power to heat the sample at the same rate. Stainless compression fittings give a much better electrical connection through the chamber wall/lid.

**Trade-off:** To get that better connection, the lead between the coil and the power supply must now be **one continuous piece** running through the fitting — there's no independent flared-nut joint to disconnect at. This means:
- Swapping coils, coil extensions, or making any iteration/change now requires un-soldering (destroying) the existing lead
- A new/repaired lead must be soldered back on for each change
- Slower iteration cycle, but a much lower-resistance, more reliable power connection

## Electrical Integration

**High-Frequency Power Delivery:**
- Connections: [[Design/Coil Geometry/Induction Coil|Induction Coil]] to external power supply
- Power frequency: ~1 MHz (high-frequency AC)
- Current: Hundreds of amps (requires robust, low-resistance conductor path — driver for moving off brass)
- Fitting: Omega SSLK-14-14, 1/4" x 1/4" NPT compression
- See [[Design/Wiring/Electrical System|Wiring & Electrical System]] for power system specifications
- See [[Design/Wiring/NI-DAQ Control Architecture|NI-DAQ Control Architecture]] for PID control of power level

## Previous Design (Rejected)

These pass-throughs were originally comprised of a bronze tube with a flared fitting nut on both ends. The brass tube is threaded. Next to the flared fitting nuts there is a flanged nut that threads onto the brass pipe, then an O-ring, then the acrylic lid. The configuration allowed the nut to be tightened and squeezed onto the lid, making a seal and allowing the flared fittings to move independent of the flanged nuts, allowing for ease of swapping and installing coils and coil extensions in and out of the chamber.

**Why rejected:** Pumping hundreds of amps through brass (copper pipe in the required size was unavailable) caused too much resistive loss, requiring roughly double the power to heat the sample at the same rate. Copper might also have been too soft for this application anyway.

Picture: 
![[coil-lead-passthrough-1.png]]
![[coil-lead-passthrough-2.png]]
CAD:https://cad.onshape.com/documents/c9bb59bfc991b1182ce6c971/w/cfd3d1ecd4a4ee8129c4b7d7/e/2eeb2d23eebc4101a7919a14?renderMode=0&uiState=6a753b64615cd4d242af3e47