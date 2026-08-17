---
subsystem: mechanisms
tags: [mechanisms, sample-mounting, archived, rejected]
---

# Bottom Lift Mechanism

Linear actuator-based sample positioning system using an offset shaft to raise/lower the sample into the coil.

## Design Concept

**Operating Principle:**
1. Linear actuator moves a shaft horizontally
2. Shaft wraps around coil perimeter
3. Ceramic platform (poor thermal conductor, non-magnetic) attaches to shaft end
4. Charpy sample rests on or hangs from platform
5. Actuator raises/lowers sample into coil for heating

**Advantages:**
- ✓ Simple actuation mechanism
- ✓ Sample accessible from side
- ✓ Ceramic avoids induction heating

## Design Challenges

### 1. Cantilever Stress
- **Issue**: Shaft offset from coil center creates uneven load on ceramic platform
- **Effect**: Unnecessary stress on mounting point
- **Solution**: Use boron nitride or similar material (mitigates but doesn't eliminate)
- **Status**: Was unaware boron nitride was available at the time

### 2. Seal Integrity
- **Issue**: Lip seal performs best with zero horizontal force
- **Concern**: Charpy weight creates cantilever loading on platform
- **Risk**: May accelerate seal wear or cause leakage
- **Assessment**: Possible but requires monitoring

### 3. Shaft Material & Cost
- **Challenge**: Reduce cantilever requires shaft very close to coil
- **Problem**: Proximity increases induction heating of shaft itself
- **Material Issue**: Non-inductive, hard-machinable materials (titanium, etc.) very expensive
- **Polish Requirement**: Shaft needs mirror finish to minimize vacuum leakage
- **Potential Solution**: Composite approach—non-magnetic material near coil, cheaper polished steel for seal

### 4. Temperature Limitations
- **Seal Rating**: Only rated to 210°F
- **Concern**: Far below 1000°C target heating temperature
- **Issue**: Seal integrity uncertain during high-temperature operation

### 5. Sample Centering
- **Challenge**: Keeping charpy centered in coil during heating
- **Approach Tried**: Spacers to center sample in tube
- **Problem**: Would require tedious setup per experiment

## Status

🟡 **Deferred** — Possible but requires additional development

**Revisit If:**
- Better seal materials available (rated to 1000°C+)
- Non-magnetic material costs decrease
- Simplified centering mechanism developed

## Visual & CAD

![[bottom-lift-design.png]]

**CAD Model**: https://cad.onshape.com/documents/c9bb59bfc991b1182ce6c971/w/cfd3d1ecd4a4ee8129c4b7d7/e/0f8909e75a921dc5d7804835?renderMode=0&uiState=6a74d4385d9455abadcda866

## Related

- [[Design/Mechanisms/Ceramic Mount|Ceramic Mount]] — Current preferred approach
- [[Design/Mechanisms/Titanium Claw|Titanium Claw]] — Previous multi-part gripper design
- [[Design/Vacuum Chamber/Vacuum Enclosure|Vacuum Chamber]] — Sample positioning interface
