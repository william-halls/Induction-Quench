---
subsystem: coil_geometry
tags: [coil-design, cad, parametric, feature-script, automation, inductance, efficiency]
---

# Coil Feature Script

Parametric OnShape FeatureScript for rapid generation of induction coil geometries. Supports round and rectangular tube profiles, an optional outer sleeve, an optional dummy sample or crucible, and a set of estimated electrical and cooling values.

> **Status (2026-10-31):** the source below is the latest version (FeatureScript 3044). It **builds in OnShape**: the ASCII-label dialog loads, including the new "Uniformity" option, and the coil, dummy sample and Efficiency message all generate (screenshot below). The **Uniformity output itself hasn't been looked at in OnShape yet**. Its math was checked offline in C#: the elliptic integrals match reference values and the on-axis loop formula.
>
> ![[coil-featurescript-onshape-build.png]]
> *5-turn coil, 53.34 mm ID, 7.938 mm (5/16") round tube, 2 mm gap, cylinder dummy sample. Message: "Efficiency 93.8% @ 293.7 kHz · 5 kW in → 4.69 kW to work, 0.31 kW copper loss (water +2.2 °C) · Q 1.8".*

## Quick Access

🔗 **OnShape Document**: https://cad.onshape.com/documents/f8c5aa176fe5d3504340f7c7/v/e52646392fe2bef23f6bfba3/e/91393e0f3187f0dfdba36f16

## Features

✓ **Tube profile** — Round (OD) or rectangular (axial L x radial W) toggle  
✓ **Spacing mode** — Coil Length (default) or Gap Between Winds (checkbox)  
✓ **Orientation** — Automatic centering, reverse winding, rotation angle  
✓ **Smart leads** — Bezier-smooth inlet/outlet bends  
✓ **Outer sleeve** — Optional, with independent sleeve lead length  
✓ **Named / colored bodies** — Coil `#B87333`, Sleeve `#E6E6E6`, Sample/Charge `#A8A9AD`, Crucible graphite `#3A3A3A`  
✓ **Optional tube bore** — Hollow the coil (round ID or rectangular wall thickness)  
✓ **Dummy sample** — Cylinder, prism, existing part, or crucible with charge  
✓ **Estimated values** — Inductance, leads, tank frequency, efficiency, cooling  
✓ **Heating uniformity** — Axial heating profile along the sample's outer (side) surface, not the top/bottom end faces: uniform zone, target-zone min/max, end drop-off, turn ripple  

## Dialog Layout

| Group | Contents |
| ----- | -------- |
| Placement | Center target (point/line), reverse winding, rotation angle |
| Coil | Coil ID, number of turns, "Specify gap between winds" checkbox → Coil length (default) or Gap |
| Tube Profile | Round / Rectangular toggle, OD or L and W |
| Leads | Lead length, bend radius |
| Outer sleeve | Checkbox → sleeve thickness, sleeve lead length |
| Estimated values | Checkbox → "Show in message" (Inductance / Tank / Efficiency / Cooling / Uniformity / All) |
| Dummy sample | Checkbox → type, dimensions, material (see below), uniformity band (%) (default 10), target zone length (default 10 mm, centered on the coil) |
| Cooling water | Tube ID (round) or wall thickness (rectangular), "Model tube bore", flow rate |
| Tank circuit & power | Capacitance (uF), extra lead length and spacing to the capacitor, supply power (kW) |

## Key Parameters

| Parameter | Range | Purpose |
| --------- | ----- | ------- |
| Coil Inner Diameter | 0.1-2000 mm | Center hole diameter |
| Number of Turns | 0.01-500 | Winding count (helix builds turns + 0.5) |
| Coil Length | 0.1-2000 mm | Overall axial extent (default mode) |
| Gap Between Winds | 0-500 mm | Spacing, used when the gap checkbox is on |
| Tube OD / L / W | 0.1-200 mm | Tube size |
| Lead Length | 0-2000 mm | Supply line length |
| Bend Radius | 0.1-500 mm | Smooth entry/exit curves |
| Sleeve | Optional | Water-jacket visualization |

### Spacing modes
- **Length mode (default):** `pitch = (Coil Length - tube axial size) / (turns + 0.5)`. Errors with the minimum length if the turns would overlap.
- **Gap mode:** `pitch = tube axial size + gap`.

## Dummy Sample and Materials

- **Sample types:** Cylinder, Prism, Part (pick an existing body), Crucible.
- **Material presets:** Steel cold (magnetic, ur ~100), Steel hot (above Curie), Stainless 304, Copper, Aluminum, Brass, Titanium, Graphite, Custom (resistivity in uOhm-cm and relative permeability).
- **Crucible:** outer diameter, wall, height, bottom thickness; crucible material Graphite / Ceramic (non-conductive) / Custom; separate charge material (default steel) and fill height. Models a hollow body named "Crucible" and a body named "Charge".
- Prism and Part samples are treated as an equivalent round cylinder for the electrical model.

## Estimated Values (what the script computes)

| Value | Method |
| ----- | ------ |
| Coil inductance (empty) | Single-layer coil formula (Nagaoka via Lundin approximation). **HF** uses radius ID/2 (current on the inner face of the turns, the right number for induction). **LF** uses the tube centerline radius minus a conductor-shape correction (Rosa; round tube and rectangular via conductor self-GMD) |
| Lead inductance | Two parallel conductors, opposite currents: the coil's own leads at their modeled spacing, plus extra lead length/spacing to the capacitor |
| Tank frequency | `f = 1 / (2*pi*sqrt(L*C))` with HF coil inductance plus leads; iterated with the sample's effect on inductance to give a loaded frequency |
| Sample / crucible loading | Solid cylinder in an AC axial field (Kelvin/Bessel solution, series expansion, thick-body limit above r/delta = 30); crucible wall modeled as a conductive slab layer in front of the charge, giving the charge vs crucible power split |
| Efficiency | Load resistance / (load resistance + copper resistance). Electrical only: inverter, capacitor and radiated losses are not included |
| Power split, water rise | From the supply power entered; water temperature rise from copper loss only |
| Cooling pressure drop | Darcy-Weisbach, water at 20 C: straight sections laminar 64/Re or Haaland (drawn copper); helix via curved-pipe correlations (Mishra and Gupta, Schmidt critical Re). Also reports velocity, Reynolds number, regime, and total tube length. Flags water speed above 3 m/s |
| Heating uniformity | Each helix revolution is treated as a circular loop at its mean height (last half turn = half-weight loop), split into 3 filaments across the tube's inner face. The axial field at the sample's outer cylindrical surface (radius = sample radius, i.e. the side of the rod, not the top face) comes from the off-axis loop formula (elliptic integrals K, E by AGM). Heating per unit length ∝ H², sampled at about 8 points per pitch along the sample (charge zone for a crucible). See the table below for the reported numbers |

### Heating uniformity outputs

| Output | Meaning |
| ------ | ------- |
| Uniform zone (`#heatUniformZone_mm`) | Length around the coil center where the pitch-averaged heating stays within ± band % of the center value. Says "whole sample" if the band covers the full length |
| Target zone min/max (`#heatZoneMinMax_pct`) | Weakest ÷ strongest pitch-averaged heating inside the centered target zone (e.g. the Charpy notch region). 100% = perfectly even |
| Ends (`#heatEndRatio_pct`) | Heating at the colder end of the sample ÷ heating at the center. Low values mean cold ends, e.g. when the sample sticks out past the coil |
| Turn ripple (`#heatRipple_pct`) | (max - min) ÷ max of the local (unaveraged) heating within one pitch of the center: banding under each turn vs between turns |

Offline check of the algorithm, using a 10×10×55 mm Charpy sample (equivalent radius 5.64 mm) in a 25.4 mm ID coil made from 4.76 mm round tube, with a ±10% band:

| Coil | Uniform zone | 10 mm zone min/max | Ends vs center | Turn ripple |
| ---- | ------------ | ------------------ | -------------- | ----------- |
| 7 turns, 65 mm long (8-loop coil) | 29 mm | 99% | 55% | 2.4% |
| 4 turns, 40 mm long | 13.5 mm | 96% | 5% | 3.4% |
| 10 turns, 100 mm long | whole 55 mm | 92% (full 55 mm zone) | 92% | 4.3% |

With the current 8-loop coil, only about the middle 29 mm of a 55 mm Charpy sample gets even heating. The ends get about half the center's heating. Most of that difference is smoothed out by heat conduction along the bar, but expect the ends to lag.

All results are also saved as part studio variables: `#coilInductanceLF_uH`, `#coilInductanceHF_uH`, `#coilInductanceLoaded_uH`, `#leadInductance_uH`, `#tankFreqEmpty_kHz`, `#tankFreqLoaded_kHz`, `#efficiency_pct`, `#efficiencyCharge_pct`, `#workPower_kW`, `#copperLoss_kW`, `#waterTempRise_C`, `#coilQ`, `#coolantDeltaP_kPa`, `#coolantVelocity_mps`, `#tubeLength_mm`, `#heatUniformZone_mm`, `#heatZoneMinMax_pct`, `#heatEndRatio_pct`, `#heatRipple_pct` (the heat variables are 0 without a dummy sample).

### Assumptions and limits
- **HF is the number to trust** for induction heating; LF is mostly an upper bound.
- **Cold steel ur = 100** is a rough effective value and falls toward saturation; use "Steel - hot" above the Curie point (~770 C) or Custom.
- Resistivities are **room-temperature** values except "Steel - hot". Hot metal has higher resistivity, which usually raises efficiency.
- Coil copper loss assumes current across the full tube width on the inner face, which is **optimistic for round tube**; real efficiency is somewhat lower.
- The crucible wall uses a flat-wall approximation, best when the wall is thin compared with the crucible radius.
- Cooling pressure drop **excludes hoses, fittings, and manifolds**; add margin on top for real supply pressure.
- Rectangular GMD (0.2235 x (W+L)) is exact for a square and a few percent off for very flat tube; effect on total is well under 1%.
- Leads and workpiece effects are only included when the matching options are enabled.
- **Uniformity is heat generated, not temperature.** Heat conduction along the sample and radiation from its ends are not modeled. Temperature is always more even than the heating profile, but the hot zone shape still follows it.
- **Uniformity is measured on the outside of the rod (the side surface), not the top surface.** Every number in the uniformity outputs is a position along the rod's length on that outer surface. The top and bottom end faces aren't evaluated at all.
- Uniformity uses the **empty-coil field**. Field crowding at the sample's ends and the extra heating at its end faces and corners are not included, and neither are the leads, the helix's uneven start and finish, or variation around the coil.
- Material properties are assumed constant along the sample. In reality, sections that pass the Curie point heat differently from colder sections.
- **Turn ripple is overstated when the sample nearly touches the coil** (gap smaller than about the tube size), because the current is modeled as thin filaments. Example: a 24 mm sample in a 25.4 mm ID coil reads about 89% ripple.

## Usage

1. Open the OnShape document
2. Set desired parameters in the FeatureScript feature panel
3. Geometry auto-generates with smooth Bezier transition curves
4. Tick "Estimated values" and (optionally) "Dummy sample" to see inductance, tank frequency, efficiency and cooling estimates in the feature message and the variable table
5. Export STL for 3D printing molds or STEP for manufacturing

## Implementation Highlights

- **Centered Helix** — Auto-calculates total height and centers coil vertically
- **Bezier Transitions** — Smooth inlet/outlet curves with configurable bend radius
- **Dual-Lead Support** — Separate lead lengths for main coil and optional sleeve
- **Cleanup Routine** — Removes construction geometry for clean final model
- **Names and colors are set last** — OnShape resets body appearance if operations run after it, so the Coil/Sample naming and coloring happens at the very end of the feature

## Gotchas Learned

- Enum names `ProfileType` etc. clash with OnShape's own library; the enums here are prefixed `Coil...`.
- Parameter and dropdown **name labels must be plain ASCII** (no µ, Ω, en-dashes, middle dots). The info message text can use them.
- `setProperty(context, {...})` takes **two arguments** (no id).

## Changelog

- **2026-10-30 (later)** — Added a heating uniformity estimate: axial heating profile on the sample's outer (side) surface from an off-axis loop field model. Reports uniform zone length, target-zone min/max, end drop-off, and turn ripple. Added a "Uniformity" message option, two new inputs (band %, target zone length), and four new variables. Build confirmed in OnShape 2026-10-31 (Uniformity message output not yet checked).

- **2026-10-30** — Merged the Gap and Length scripts (Length default, gap checkbox); added inductance estimates, dummy sample/crucible, materials, tank frequency, efficiency and cooling estimates, grouped dialog, Coil/Sleeve naming and colors, optional tube bore. Upgraded to FeatureScript 3044.

---

## Full Source Code

```featurescript
FeatureScript 3044;
import(path : "onshape/std/common.fs", version : "3044.0");
import(path : "onshape/std/geometry.fs", version : "3044.0");

export enum CoilProfileType
{
    annotation { "Name" : "Round" }
    ROUND,
    annotation { "Name" : "Rectangular" }
    RECTANGULAR
}

export enum CoilSampleType
{
    annotation { "Name" : "Cylinder" }
    CYLINDER,
    annotation { "Name" : "Prism" }
    PRISM,
    annotation { "Name" : "Part" }
    PART,
    annotation { "Name" : "Crucible" }
    CRUCIBLE
}

export enum CoilMaterial
{
    annotation { "Name" : "Steel - cold (magnetic)" }
    STEEL_COLD,
    annotation { "Name" : "Steel - hot (above Curie)" }
    STEEL_HOT,
    annotation { "Name" : "Stainless 304" }
    STAINLESS,
    annotation { "Name" : "Copper" }
    COPPER,
    annotation { "Name" : "Aluminum" }
    ALUMINUM,
    annotation { "Name" : "Brass" }
    BRASS,
    annotation { "Name" : "Titanium" }
    TITANIUM,
    annotation { "Name" : "Graphite" }
    GRAPHITE,
    annotation { "Name" : "Custom" }
    CUSTOM
}

export enum CoilCrucibleMaterial
{
    annotation { "Name" : "Graphite" }
    GRAPHITE,
    annotation { "Name" : "Ceramic / clay (non-conductive)" }
    CERAMIC,
    annotation { "Name" : "Custom" }
    CUSTOM
}

export enum CoilEstimateDisplay
{
    annotation { "Name" : "Inductance" }
    INDUCTANCE,
    annotation { "Name" : "Tank" }
    TANK,
    annotation { "Name" : "Efficiency" }
    EFFICIENCY,
    annotation { "Name" : "Cooling" }
    COOLING,
    annotation { "Name" : "Uniformity" }
    UNIFORMITY,
    annotation { "Name" : "All" }
    ALL
}

annotation { "Feature Type Name" : "Induction Coil" }
export const inductionCoil = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
        // =========================================================================
        // PLACEMENT
        // =========================================================================
        annotation { "Group Name" : "Placement", "Collapsed By Default" : false }
        {
            annotation { "Name" : "Center target (point or line)", "Filter" : EntityType.VERTEX || EntityType.EDGE, "MaxNumberOfPicks" : 1 }
            definition.target is Query;

            annotation { "Name" : "Reverse winding (CCW)", "UIHint" : UIHint.OPPOSITE_DIRECTION }
            definition.reverseDirection is boolean;

            annotation { "Name" : "Rotation angle" }
            isAngle(definition.rotationAngle, { (degree) : [-360, 0, 360] } as AngleBoundSpec);
        }

        // =========================================================================
        // COIL
        // =========================================================================
        annotation { "Group Name" : "Coil", "Collapsed By Default" : false }
        {
            annotation { "Name" : "Coil inner diameter (ID)" }
            isLength(definition.coilId, { (millimeter) : [0.1, 25, 2000] } as LengthBoundSpec);

            annotation { "Name" : "Number of turns" }
            isReal(definition.turns, { (unitless) : [0.01, 5, 500] } as RealBoundSpec);

            // Unchecked (default) = drive pitch from overall Coil Length
            // Checked             = drive pitch from Gap Between Winds
            annotation { "Name" : "Specify gap between winds", "Default" : false }
            definition.useGap is boolean;

            if (definition.useGap)
            {
                annotation { "Name" : "Gap between winds" }
                isLength(definition.gap, { (millimeter) : [0, 3, 500] } as LengthBoundSpec);
            }
            else
            {
                annotation { "Name" : "Coil length" }
                isLength(definition.coilLength, { (millimeter) : [0.1, 50, 2000] } as LengthBoundSpec);
            }
        }

        // =========================================================================
        // TUBE PROFILE
        // =========================================================================
        annotation { "Group Name" : "Tube Profile", "Collapsed By Default" : false }
        {
            annotation { "Name" : "Profile", "UIHint" : UIHint.HORIZONTAL_ENUM }
            definition.profileType is CoilProfileType;

            if (definition.profileType == CoilProfileType.ROUND)
            {
                annotation { "Name" : "Tube outer diameter (OD)" }
                isLength(definition.tubeOd, { (millimeter) : [0.1, 5, 200] } as LengthBoundSpec);
            }
            else
            {
                annotation { "Name" : "Tube axial length (L)" }
                isLength(definition.tubeL, { (millimeter) : [0.1, 5, 200] } as LengthBoundSpec);

                annotation { "Name" : "Tube radial width (W)" }
                isLength(definition.tubeW, { (millimeter) : [0.1, 5, 200] } as LengthBoundSpec);
            }
        }

        // =========================================================================
        // LEADS
        // =========================================================================
        annotation { "Group Name" : "Leads", "Collapsed By Default" : false }
        {
            annotation { "Name" : "Lead length" }
            isLength(definition.leadLength, { (millimeter) : [0, 25, 2000] } as LengthBoundSpec);

            annotation { "Name" : "Bend radius" }
            isLength(definition.bendRadius, { (millimeter) : [0.1, 5, 500] } as LengthBoundSpec);
        }

        // =========================================================================
        // OUTER SLEEVE
        // =========================================================================
        annotation { "Name" : "Outer sleeve" }
        definition.addSleeve is boolean;

        if (definition.addSleeve)
        {
            annotation { "Group Name" : "Outer sleeve", "Driving Parameter" : "addSleeve", "Collapsed By Default" : false }
            {
                annotation { "Name" : "Sleeve thickness" }
                isLength(definition.sleeveThickness, { (millimeter) : [0.1, 1, 200] } as LengthBoundSpec);

                annotation { "Name" : "Sleeve lead length" }
                isLength(definition.sleeveLeadLength, { (millimeter) : [0, 25, 2000] } as LengthBoundSpec);
            }
        }

        // =========================================================================
        // ESTIMATED VALUES
        // =========================================================================
        annotation { "Name" : "Estimated values" }
        definition.showEstimates is boolean;

        if (definition.showEstimates)
        {
            annotation { "Group Name" : "Estimated values", "Driving Parameter" : "showEstimates", "Collapsed By Default" : false }
            {
                annotation { "Name" : "Show in message", "UIHint" : UIHint.HORIZONTAL_ENUM }
                definition.estimateDisplay is CoilEstimateDisplay;
            }

            // --- Dummy sample ---
            annotation { "Name" : "Dummy sample" }
            definition.addSample is boolean;

            if (definition.addSample)
            {
                annotation { "Group Name" : "Dummy sample", "Driving Parameter" : "addSample", "Collapsed By Default" : false }
                {
                    annotation { "Name" : "Sample type", "UIHint" : UIHint.HORIZONTAL_ENUM }
                    definition.sampleType is CoilSampleType;

                    if (definition.sampleType == CoilSampleType.CYLINDER)
                    {
                        annotation { "Name" : "Diameter" }
                        isLength(definition.sampleDiameter, { (millimeter) : [0.1, 15, 2000] } as LengthBoundSpec);

                        annotation { "Name" : "Length" }
                        isLength(definition.sampleCylLength, { (millimeter) : [0.1, 100, 5000] } as LengthBoundSpec);
                    }
                    else if (definition.sampleType == CoilSampleType.PRISM)
                    {
                        annotation { "Name" : "Width" }
                        isLength(definition.samplePrismWidth, { (millimeter) : [0.1, 10, 2000] } as LengthBoundSpec);

                        annotation { "Name" : "Depth" }
                        isLength(definition.samplePrismDepth, { (millimeter) : [0.1, 10, 2000] } as LengthBoundSpec);

                        annotation { "Name" : "Length" }
                        isLength(definition.samplePrismLength, { (millimeter) : [0.1, 55, 5000] } as LengthBoundSpec);
                    }
                    else if (definition.sampleType == CoilSampleType.PART)
                    {
                        annotation { "Name" : "Sample part", "Filter" : EntityType.BODY && BodyType.SOLID, "MaxNumberOfPicks" : 1 }
                        definition.samplePart is Query;
                    }

                    if (definition.sampleType == CoilSampleType.CRUCIBLE)
                    {
                        annotation { "Name" : "Crucible outer diameter" }
                        isLength(definition.crucibleOd, { (millimeter) : [0.1, 20, 2000] } as LengthBoundSpec);

                        annotation { "Name" : "Crucible wall thickness" }
                        isLength(definition.crucibleWall, { (millimeter) : [0.1, 3, 500] } as LengthBoundSpec);

                        annotation { "Name" : "Crucible height" }
                        isLength(definition.crucibleHeight, { (millimeter) : [0.1, 50, 5000] } as LengthBoundSpec);

                        annotation { "Name" : "Crucible bottom thickness" }
                        isLength(definition.crucibleBottom, { (millimeter) : [0.1, 5, 500] } as LengthBoundSpec);

                        annotation { "Name" : "Crucible material" }
                        definition.crucibleMaterial is CoilCrucibleMaterial;

                        if (definition.crucibleMaterial == CoilCrucibleMaterial.CUSTOM)
                        {
                            annotation { "Name" : "Crucible resistivity (uOhm-cm)" }
                            isReal(definition.crucibleRho, { (unitless) : [0.1, 1000, 1e9] } as RealBoundSpec);
                        }

                        annotation { "Name" : "Charge material" }
                        definition.chargeMaterial is CoilMaterial;

                        if (definition.chargeMaterial == CoilMaterial.CUSTOM)
                        {
                            annotation { "Name" : "Charge resistivity (uOhm-cm)" }
                            isReal(definition.chargeRho, { (unitless) : [0.1, 17, 1e9] } as RealBoundSpec);

                            annotation { "Name" : "Charge relative permeability (ur)" }
                            isReal(definition.chargeMu, { (unitless) : [1, 1, 100000] } as RealBoundSpec);
                        }

                        annotation { "Name" : "Charge fill height" }
                        isLength(definition.chargeFill, { (millimeter) : [0, 25, 5000] } as LengthBoundSpec);
                    }
                    else
                    {
                        annotation { "Name" : "Sample material" }
                        definition.sampleMaterial is CoilMaterial;

                        if (definition.sampleMaterial == CoilMaterial.CUSTOM)
                        {
                            annotation { "Name" : "Sample resistivity (uOhm-cm)" }
                            isReal(definition.sampleRho, { (unitless) : [0.1, 17, 1e9] } as RealBoundSpec);

                            annotation { "Name" : "Sample relative permeability (ur)" }
                            isReal(definition.sampleMu, { (unitless) : [1, 1, 100000] } as RealBoundSpec);
                        }
                    }

                    // Heating uniformity settings
                    annotation { "Name" : "Uniformity band (%)" }
                    isReal(definition.uniformBand, { (unitless) : [0.1, 10, 100] } as RealBoundSpec);

                    annotation { "Name" : "Target zone length (centered)" }
                    isLength(definition.targetZone, { (millimeter) : [0.1, 10, 5000] } as LengthBoundSpec);
                }
            }

            // --- Cooling ---
            annotation { "Group Name" : "Cooling water", "Collapsed By Default" : false }
            {
                if (definition.profileType == CoilProfileType.ROUND)
                {
                    annotation { "Name" : "Tube inner diameter (ID)" }
                    isLength(definition.tubeBoreId, { (millimeter) : [0.01, 3, 200] } as LengthBoundSpec);
                }
                else
                {
                    annotation { "Name" : "Tube wall thickness" }
                    isLength(definition.tubeWall, { (millimeter) : [0.01, 0.8, 100] } as LengthBoundSpec);
                }

                annotation { "Name" : "Model tube bore" }
                definition.modelBore is boolean;

                annotation { "Name" : "Flow rate (L/min)" }
                isReal(definition.flowRate, { (unitless) : [0.01, 2, 500] } as RealBoundSpec);
            }

            // --- Tank circuit & power ---
            annotation { "Group Name" : "Tank circuit & power", "Collapsed By Default" : false }
            {
                annotation { "Name" : "Capacitance (uF)" }
                isReal(definition.tankCap, { (unitless) : [0.001, 1, 100000] } as RealBoundSpec);

                annotation { "Name" : "Extra lead length to capacitor" }
                isLength(definition.tankLeadLength, { (millimeter) : [0, 100, 10000] } as LengthBoundSpec);

                annotation { "Name" : "Extra lead spacing (center to center)" }
                isLength(definition.tankLeadSpacing, { (millimeter) : [0.1, 10, 1000] } as LengthBoundSpec);

                annotation { "Name" : "Supply power (kW)" }
                isReal(definition.supplyPower, { (unitless) : [0.01, 3, 100000] } as RealBoundSpec);
            }
        }
    }
    {
        // 1. Guard against empty target selection
        if (isQueryEmpty(context, definition.target))
        {
            throw regenError("Please select a vertex or edge target axis.", ["target"]);
        }

        // 2. Resolve Target and Establish Coordinate System
        var origin;
        var zAxis;
        var xAxis;

        var isVertex = !isQueryEmpty(context, qEntityFilter(definition.target, EntityType.VERTEX));
        var isEdge = !isQueryEmpty(context, qEntityFilter(definition.target, EntityType.EDGE));

        if (isVertex) {
            origin = evVertexPoint(context, { "vertex" : definition.target });
            zAxis = vector(0, 0, 1);
            xAxis = vector(1, 0, 0);
        } else if (isEdge) {
            var tangent = evEdgeTangentLine(context, { "edge" : definition.target, "parameter" : 0.5 });
            origin = tangent.origin;
            zAxis = tangent.direction;
            xAxis = perpendicularVector(zAxis);
        } else {
            throw regenError("Please select a vertex or an edge.", ["target"]);
        }

        var yAxis = cross(zAxis, xAxis);

        // 3. Fallback Type Guards
        var coilId = (definition.coilId is ValueWithUnits) ? definition.coilId : 25 * millimeter;
        var turns = (definition.turns is number) ? definition.turns : 5;
        var useGap = (definition.useGap == true);
        var gap = (definition.gap is ValueWithUnits) ? definition.gap : 3 * millimeter;
        var coilLength = (definition.coilLength is ValueWithUnits) ? definition.coilLength : 50 * millimeter;
        var leadLength = (definition.leadLength is ValueWithUnits) ? definition.leadLength : 25 * millimeter;
        var reverseDirection = (definition.reverseDirection == true);

        var bendRadius = (definition.bendRadius is ValueWithUnits && definition.bendRadius > 0*meter) ? definition.bendRadius : 5 * millimeter;
        if (bendRadius > leadLength * 0.8) { bendRadius = leadLength * 0.8; }

        var isRectangular = (definition.profileType == CoilProfileType.RECTANGULAR);
        var tubeOd = 5 * millimeter;
        var tubeL = 5 * millimeter;
        var tubeW = 5 * millimeter;

        if (!isRectangular) {
            if (definition.tubeOd is ValueWithUnits) { tubeOd = definition.tubeOd; }
            tubeL = tubeOd;
            tubeW = tubeOd;
        } else {
            if (definition.tubeL is ValueWithUnits) { tubeL = definition.tubeL; }
            if (definition.tubeW is ValueWithUnits) { tubeW = definition.tubeW; }
            tubeOd = tubeW;
        }

        // Sleeve Guards
        var addSleeve = (definition.addSleeve == true);
        var sleeveThickness = (definition.sleeveThickness is ValueWithUnits) ? definition.sleeveThickness : 1 * millimeter;

        // Default sleeve lead length to the main lead length if undefined, and ensure it doesn't break the bend curve
        var sleeveLeadLength = (definition.sleeveLeadLength is ValueWithUnits) ? definition.sleeveLeadLength : leadLength;
        if (sleeveLeadLength < bendRadius) { sleeveLeadLength = bendRadius; }

        // Estimated-values Guards
        var showEstimates = (definition.showEstimates == true);
        var addSample = showEstimates && (definition.addSample == true);
        var modelBore = showEstimates && (definition.modelBore == true);
        var estimateDisplay = (definition.estimateDisplay is CoilEstimateDisplay) ? definition.estimateDisplay : CoilEstimateDisplay.INDUCTANCE;

        var boreId = tubeOd * 0.6;
        var tubeWall = 0.8 * millimeter;
        if (showEstimates)
        {
            if (!isRectangular)
            {
                if (definition.tubeBoreId is ValueWithUnits) { boreId = definition.tubeBoreId; }
                if (boreId >= tubeOd)
                {
                    throw regenError("Tube ID must be smaller than the tube OD.", ["tubeBoreId"]);
                }
            }
            else
            {
                if (definition.tubeWall is ValueWithUnits) { tubeWall = definition.tubeWall; }
                if (2 * tubeWall >= min(tubeL, tubeW))
                {
                    throw regenError("Wall thickness is too large for this tube size.", ["tubeWall"]);
                }
            }
        }

        var flowLpm = (definition.flowRate is number) ? definition.flowRate : 2;
        var tankCapUF = (definition.tankCap is number) ? definition.tankCap : 1;
        var tankLeadLength = (definition.tankLeadLength is ValueWithUnits) ? definition.tankLeadLength : 100 * millimeter;
        var tankLeadSpacing = (definition.tankLeadSpacing is ValueWithUnits) ? definition.tankLeadSpacing : 10 * millimeter;
        var supplyKW = (definition.supplyPower is number) ? definition.supplyPower : 3;
        var uniformBandPct = (definition.uniformBand is number) ? definition.uniformBand : 10;
        var targetZone = (definition.targetZone is ValueWithUnits) ? definition.targetZone : 10 * millimeter;

        // =========================================================================
        // COORDINATE SYSTEM PIVOT (Automatically counters the 180 degree flip)
        // =========================================================================
        var rotationAngle = (definition.rotationAngle is ValueWithUnits) ? definition.rotationAngle : 0 * degree;
        var finalRotation = rotationAngle;

        // If CCW is selected, apply a 180-degree correction to keep the leads aligned
        if (reverseDirection) {
            finalRotation += 180 * degree;
        }

        if (finalRotation != 0 * degree) {
            var rotTransform = rotationAround(line(vector(0, 0, 0) * meter, zAxis), finalRotation);
            var xLine = line(vector(0, 0, 0) * meter, xAxis);
            var yLine = line(vector(0, 0, 0) * meter, yAxis);

            xLine = rotTransform * xLine;
            yLine = rotTransform * yLine;

            xAxis = xLine.direction;
            yAxis = yLine.direction;
        }

        var pathRadius = (coilId / 2) + (tubeOd / 2);
        var revs = turns + 0.5;

        // =========================================================================
        // PITCH CALCULATION (GAP MODE vs. LENGTH MODE)
        // Gap mode:    pitch = tubeL + gap
        // Length mode: Coil Length = overall axial extent, outer edge of first
        //              turn to outer edge of last turn = revs * pitch + tubeL.
        //              The gap between winds is whatever is left: pitch - tubeL.
        // =========================================================================
        var pitch;
        if (useGap)
        {
            pitch = tubeL + gap;
        }
        else
        {
            pitch = (coilLength - tubeL) / revs;
            if (pitch < tubeL * 0.9999)
            {
                var minLength = (revs + 1) * tubeL;
                throw regenError("Coil length too short: the turns would overlap. Minimum length for this tube and turn count is " ~ roundToPrecision(minLength / millimeter, 1) ~ " mm.", ["coilLength"]);
            }
        }

        // Calculate total coil height to establish centering offset
        var totalHeight = revs * pitch;
        var centeredAxisStart = origin - zAxis * (totalHeight / 2.0);

        // Cubic Bezier math engine
        var sampleBezier = function(p0, p1, p2, p3, t) {
            var mt = 1.0 - t;
            return p0 * (mt * mt * mt) + p1 * (3.0 * mt * mt * t) + p2 * (3.0 * mt * t * t) + p3 * (t * t * t);
        };

        // =========================================================================
        // 4. CENTERED ROTATED HELIX GENERATION
        // =========================================================================
        var helixId = id + "helix";
        opHelix(context, helixId, {
            "direction" : zAxis,
            "axisStart" : centeredAxisStart,
            "startPoint" : centeredAxisStart + (xAxis * pathRadius),
            "interval" : [0, revs],
            "clockwise" : !reverseDirection,
            "helicalPitch" : pitch,
            "spiralPitch" : 0 * meter
        });
        var helixQuery = qCreatedBy(helixId, EntityType.EDGE);

        var startTarget = evEdgeTangentLine(context, { "edge" : helixQuery, "parameter" : 0.0 });
        var endTarget = evEdgeTangentLine(context, { "edge" : helixQuery, "parameter" : 1.0 });

        // Isolate horizontal tracing directions to make paths completely direction-agnostic
        var startHorizontalDir = normalize(startTarget.direction - zAxis * dot(zAxis, startTarget.direction));
        var endHorizontalDir = normalize(endTarget.direction - zAxis * dot(zAxis, endTarget.direction));

        var inletSlope = dot(zAxis, startTarget.direction);
        var outletSlope = dot(zAxis, endTarget.direction);

        // =========================================================================
        // 5. INLET SIDE SMOOTH RAMP
        // =========================================================================
        var ip0 = startTarget.origin;
        var ip1 = ip0 - startTarget.direction * (bendRadius * 0.5);
        var ip3 = ip0 - startHorizontalDir * bendRadius - zAxis * (inletSlope * bendRadius * 0.5);
        var ip2 = ip3 + startHorizontalDir * (bendRadius * 0.5);

        var inletCurvePts = [];
        for (var i = 0; i <= 5; i += 1) {
            inletCurvePts = append(inletCurvePts, sampleBezier(ip0, ip1, ip2, ip3, i / 5.0));
        }

        var inletCurveId = id + "inletCurve";
        opFitSpline(context, inletCurveId, { "points" : inletCurvePts });
        var inletCurveQuery = qCreatedBy(inletCurveId, EntityType.EDGE);

        var inletLineId = id + "inletLine";
        var inletStartPoint = ip3 - startHorizontalDir * (leadLength - bendRadius);
        opFitSpline(context, inletLineId, { "points" : [inletStartPoint, ip3] });
        var inletLineQuery = qCreatedBy(inletLineId, EntityType.EDGE);

        // =========================================================================
        // 6. OUTLET SIDE SMOOTH RAMP
        // =========================================================================
        var op0 = endTarget.origin;
        var op1 = op0 + endTarget.direction * (bendRadius * 0.5);
        var op3 = op0 + endHorizontalDir * bendRadius + zAxis * (outletSlope * bendRadius * 0.5);
        var op2 = op3 - endHorizontalDir * (bendRadius * 0.5);

        var outletCurvePts = [];
        for (var i = 0; i <= 5; i += 1) {
            outletCurvePts = append(outletCurvePts, sampleBezier(op0, op1, op2, op3, i / 5.0));
        }

        var outletCurveId = id + "outletCurve";
        opFitSpline(context, outletCurveId, { "points" : outletCurvePts });
        var outletCurveQuery = qCreatedBy(outletCurveId, EntityType.EDGE);

        var outletLineId = id + "outletLine";
        var outletEndPoint = op3 + endHorizontalDir * (leadLength - bendRadius);
        opFitSpline(context, outletLineId, { "points" : [op3, outletEndPoint] });
        var outletLineQuery = qCreatedBy(outletLineId, EntityType.EDGE);

        // =========================================================================
        // 7. SKETCH PROFILE & COIL SWEEP
        // =========================================================================
        var inletPlane = plane(inletStartPoint, startHorizontalDir, xAxis);
        var mainCoilSketchId = id + "mainCoilSketch";
        var sk = newSketchOnPlane(context, mainCoilSketchId, { "sketchPlane" : inletPlane });

        if (!isRectangular) {
            skCircle(sk, "c", { "center" : vector(0,0)*meter, "radius" : tubeOd / 2 });
        } else {
            skRectangle(sk, "r", { "firstCorner" : vector(-tubeW/2, -tubeL/2), "secondCorner" : vector(tubeW/2, tubeL/2) });
        }
        skSolve(sk);

        var unifiedPath = qUnion([inletLineQuery, inletCurveQuery, helixQuery, outletCurveQuery, outletLineQuery]);

        opSweep(context, id + "seamlessSweep", {
            "profiles" : qSketchRegion(mainCoilSketchId),
            "path" : unifiedPath
        });

        // Measure path lengths now (wire geometry is deleted in cleanup)
        var pathLenTotal = evLength(context, { "entities" : unifiedPath }) / meter;   // m
        var pathLenHelix = evLength(context, { "entities" : helixQuery }) / meter;    // m

        // =========================================================================
        // 8. OUTER SLEEVE GENERATION (WITH INDEPENDENT LEAD LENGTHS)
        // =========================================================================
        var cleanupEntities = [
            unifiedPath,
            qCreatedBy(mainCoilSketchId, EntityType.BODY)
        ];

        if (addSleeve)
        {
            // Build separate shorter straight segments for the sleeve
            var sleeveInletStartPoint = ip3 - startHorizontalDir * (sleeveLeadLength - bendRadius);
            var sleeveInletLineId = id + "sleeveInletLine";
            opFitSpline(context, sleeveInletLineId, { "points" : [sleeveInletStartPoint, ip3] });
            var sleeveInletLineQuery = qCreatedBy(sleeveInletLineId, EntityType.EDGE);

            var sleeveOutletEndPoint = op3 + endHorizontalDir * (sleeveLeadLength - bendRadius);
            var sleeveOutletLineId = id + "sleeveOutletLine";
            opFitSpline(context, sleeveOutletLineId, { "points" : [op3, sleeveOutletEndPoint] });
            var sleeveOutletLineQuery = qCreatedBy(sleeveOutletLineId, EntityType.EDGE);

            // Create a custom sweep path just for the sleeve
            var sleevePath = qUnion([sleeveInletLineQuery, inletCurveQuery, helixQuery, outletCurveQuery, sleeveOutletLineQuery]);

            // Create a new sketch plane positioned at the tip of the shorter sleeve lead
            var sleeveInletPlane = plane(sleeveInletStartPoint, startHorizontalDir, xAxis);
            var sleeveSketchId = id + "sleeveSketch";
            var skSleeve = newSketchOnPlane(context, sleeveSketchId, { "sketchPlane" : sleeveInletPlane });

            if (!isRectangular) {
                skCircle(skSleeve, "outer", { "center" : vector(0,0)*meter, "radius" : (tubeOd / 2) + sleeveThickness });
            } else {
                skRectangle(skSleeve, "outer", {
                    "firstCorner" : vector(-tubeW/2 - sleeveThickness, -tubeL/2 - sleeveThickness),
                    "secondCorner" : vector(tubeW/2 + sleeveThickness, tubeL/2 + sleeveThickness)
                });
            }
            skSolve(skSleeve);

            // Sweep the solid oversized profile
            opSweep(context, id + "sleeveSweep", {
                "profiles" : qSketchRegion(sleeveSketchId),
                "path" : sleevePath
            });

            // Hollow it out
            opBoolean(context, id + "hollowSleeve", {
                "tools" : qCreatedBy(id + "seamlessSweep", EntityType.BODY),
                "targets" : qCreatedBy(id + "sleeveSweep", EntityType.BODY),
                "operationType" : BooleanOperationType.SUBTRACTION,
                "keepTools" : true
            });

            // Name and color the sleeve body
            var sleeveBody = qCreatedBy(id + "sleeveSweep", EntityType.BODY);
            setProperty(context, {
                "entities" : sleeveBody,
                "propertyType" : PropertyType.NAME,
                "value" : "Sleeve"
            });
            setProperty(context, {
                "entities" : sleeveBody,
                "propertyType" : PropertyType.APPEARANCE,
                "value" : color(230 / 255, 230 / 255, 230 / 255)  // #E6E6E6
            });

            // Add new sleeve wireframes to the cleanup routine
            cleanupEntities = append(cleanupEntities, sleeveInletLineQuery);
            cleanupEntities = append(cleanupEntities, sleeveOutletLineQuery);
            cleanupEntities = append(cleanupEntities, qCreatedBy(sleeveSketchId, EntityType.BODY));
        }

        // =========================================================================
        // 8b. OPTIONAL TUBE BORE (done after the sleeve so the sleeve stays solid-walled)
        // =========================================================================
        if (modelBore)
        {
            var boreSketchId = id + "boreSketch";
            var skBore = newSketchOnPlane(context, boreSketchId, { "sketchPlane" : inletPlane });

            if (!isRectangular) {
                skCircle(skBore, "bore", { "center" : vector(0,0)*meter, "radius" : boreId / 2 });
            } else {
                skRectangle(skBore, "bore", {
                    "firstCorner" : vector(-tubeW/2 + tubeWall, -tubeL/2 + tubeWall),
                    "secondCorner" : vector(tubeW/2 - tubeWall, tubeL/2 - tubeWall)
                });
            }
            skSolve(skBore);

            opSweep(context, id + "boreSweep", {
                "profiles" : qSketchRegion(boreSketchId),
                "path" : unifiedPath
            });

            opBoolean(context, id + "hollowCoil", {
                "tools" : qCreatedBy(id + "boreSweep", EntityType.BODY),
                "targets" : qCreatedBy(id + "seamlessSweep", EntityType.BODY),
                "operationType" : BooleanOperationType.SUBTRACTION
            });

            cleanupEntities = append(cleanupEntities, qCreatedBy(boreSketchId, EntityType.BODY));
        }

        // =========================================================================
        // 8c. DUMMY SAMPLE / CRUCIBLE
        // Each "segment" is an axial slice of the load described as:
        //   outer radius Ro, optional conductive wall (thickness wallT), and an
        //   optional solid conductive core (radius coreR) inside it.
        // All radii/z positions in meters, z measured from the coil center.
        // =========================================================================
        var sampleSegments = [];
        var isCrucible = false;
        var sampleQuery = qNothing();
        var crucibleQuery = qNothing();
        var chargeQuery = qNothing();
        var crucibleColor = color(58 / 255, 58 / 255, 58 / 255);   // graphite #3A3A3A
        var aInnerM = (coilId / 2) / meter;

        // Heated region for the uniformity estimate (m, from the coil center)
        var heatZmin = 0;
        var heatZmax = 0;
        var heatR = 0;            // load surface radius (m)

        // Resistivity (ohm*m), relative permeability, conductive?
        var materialProps = function(mat, rhoCustom, muCustom) {
            if (mat == CoilMaterial.STEEL_COLD) { return { "rho" : 1.7e-7, "mu" : 100, "cond" : true }; }
            if (mat == CoilMaterial.STEEL_HOT)  { return { "rho" : 1.15e-6, "mu" : 1, "cond" : true }; }
            if (mat == CoilMaterial.STAINLESS)  { return { "rho" : 7.2e-7, "mu" : 1, "cond" : true }; }
            if (mat == CoilMaterial.COPPER)     { return { "rho" : 1.7e-8, "mu" : 1, "cond" : true }; }
            if (mat == CoilMaterial.ALUMINUM)   { return { "rho" : 2.8e-8, "mu" : 1, "cond" : true }; }
            if (mat == CoilMaterial.BRASS)      { return { "rho" : 6.5e-8, "mu" : 1, "cond" : true }; }
            if (mat == CoilMaterial.TITANIUM)   { return { "rho" : 4.2e-7, "mu" : 1, "cond" : true }; }
            if (mat == CoilMaterial.GRAPHITE)   { return { "rho" : 1.0e-5, "mu" : 1, "cond" : true }; }
            var rc = (rhoCustom is number) ? rhoCustom : 17;
            var mc = (muCustom is number) ? muCustom : 1;
            return { "rho" : rc * 1e-8, "mu" : mc, "cond" : true };
        };

        var crucibleProps = function(mat, rhoCustom) {
            if (mat == CoilCrucibleMaterial.CERAMIC) { return { "rho" : 1, "mu" : 1, "cond" : false }; }
            if (mat == CoilCrucibleMaterial.CUSTOM)
            {
                var rc = (rhoCustom is number) ? rhoCustom : 1000;
                return { "rho" : rc * 1e-8, "mu" : 1, "cond" : true };
            }
            return { "rho" : 1.0e-5, "mu" : 1, "cond" : true };   // graphite
        };

        var airProps = { "rho" : 1, "mu" : 1, "cond" : false };

        var makeSeg = function(z0, z1, ro, wallT, wallP, coreR, coreP, isCharge) {
            return {
                "zMin" : z0, "zMax" : z1, "Ro" : ro,
                "wallT" : wallT, "wallRho" : wallP.rho, "wallCond" : wallP.cond,
                "coreR" : coreR, "coreRho" : coreP.rho, "coreMu" : coreP.mu, "coreCond" : coreP.cond,
                "isCharge" : isCharge
            };
        };

        if (addSample)
        {
            if (definition.sampleType == CoilSampleType.CRUCIBLE)
            {
                isCrucible = true;
                var cOd = (definition.crucibleOd is ValueWithUnits) ? definition.crucibleOd : 20 * millimeter;
                var cWall = (definition.crucibleWall is ValueWithUnits) ? definition.crucibleWall : 3 * millimeter;
                var cH = (definition.crucibleHeight is ValueWithUnits) ? definition.crucibleHeight : 50 * millimeter;
                var cBot = (definition.crucibleBottom is ValueWithUnits) ? definition.crucibleBottom : 5 * millimeter;
                var cFill = (definition.chargeFill is ValueWithUnits) ? definition.chargeFill : 25 * millimeter;

                if (cOd >= coilId) { throw regenError("Crucible OD must be smaller than the coil ID.", ["crucibleOd"]); }
                if (2 * cWall >= cOd) { throw regenError("Crucible wall is too thick for its OD.", ["crucibleWall"]); }
                if (cBot >= cH) { throw regenError("Crucible bottom must be thinner than the crucible height.", ["crucibleBottom"]); }
                if (cFill > cH - cBot) { throw regenError("Charge fill height is taller than the crucible cavity.", ["chargeFill"]); }

                var zBot = -cH / 2;
                var cInnerR = cOd / 2 - cWall;

                // Crucible = outer cylinder minus bore from the top
                fCylinder(context, id + "crucibleOuter", {
                    "bottomCenter" : origin + zAxis * zBot,
                    "topCenter" : origin + zAxis * (cH / 2),
                    "radius" : cOd / 2
                });
                fCylinder(context, id + "crucibleBore", {
                    "bottomCenter" : origin + zAxis * (zBot + cBot),
                    "topCenter" : origin + zAxis * (cH / 2 + 1 * millimeter),
                    "radius" : cInnerR
                });
                opBoolean(context, id + "crucibleHollow", {
                    "tools" : qCreatedBy(id + "crucibleBore", EntityType.BODY),
                    "targets" : qCreatedBy(id + "crucibleOuter", EntityType.BODY),
                    "operationType" : BooleanOperationType.SUBTRACTION
                });
                crucibleQuery = qCreatedBy(id + "crucibleOuter", EntityType.BODY);

                // Charge sits on the crucible floor
                if (cFill > 0 * meter)
                {
                    fCylinder(context, id + "charge", {
                        "bottomCenter" : origin + zAxis * (zBot + cBot),
                        "topCenter" : origin + zAxis * (zBot + cBot + cFill),
                        "radius" : cInnerR
                    });
                    chargeQuery = qCreatedBy(id + "charge", EntityType.BODY);
                }

                var crucP = crucibleProps(definition.crucibleMaterial, definition.crucibleRho);
                var chargeP = materialProps(definition.chargeMaterial, definition.chargeRho, definition.chargeMu);
                if (definition.crucibleMaterial == CoilCrucibleMaterial.CERAMIC)
                {
                    crucibleColor = color(232 / 255, 220 / 255, 200 / 255);  // ceramic beige
                }

                var roM = (cOd / 2) / meter;
                var tM = cWall / meter;
                var aiM = roM - tM;
                var z0 = zBot / meter;
                var z1 = (zBot + cBot) / meter;
                var z2 = (zBot + cBot + cFill) / meter;
                var z3 = (cH / 2) / meter;

                // Uniformity is reported over the charge zone, at the charge surface
                heatZmin = z1;
                heatZmax = z2;
                heatR = aiM;

                // Bottom: solid crucible-material disc
                sampleSegments = append(sampleSegments, makeSeg(z0, z1, roM, 0, airProps, roM, crucP, false));
                // Charge zone: crucible wall + metal core
                if (z2 > z1)
                {
                    sampleSegments = append(sampleSegments, makeSeg(z1, z2, roM, tM, crucP, aiM, chargeP, true));
                }
                // Empty zone above the charge: crucible wall only
                if (z3 > z2)
                {
                    sampleSegments = append(sampleSegments, makeSeg(z2, z3, roM, tM, crucP, 0, airProps, false));
                }
            }
            else
            {
                var sampleP = materialProps(definition.sampleMaterial, definition.sampleRho, definition.sampleMu);
                var sampleB = 0;
                var sampleZmin = 0;
                var sampleZmax = 0;

                if (definition.sampleType == CoilSampleType.CYLINDER)
                {
                    var sDia = (definition.sampleDiameter is ValueWithUnits) ? definition.sampleDiameter : 15 * millimeter;
                    var sLen = (definition.sampleCylLength is ValueWithUnits) ? definition.sampleCylLength : 100 * millimeter;
                    if (sDia >= coilId)
                    {
                        throw regenError("Sample diameter must be smaller than the coil ID.", ["sampleDiameter"]);
                    }

                    fCylinder(context, id + "sampleCyl", {
                        "bottomCenter" : origin - zAxis * (sLen / 2),
                        "topCenter" : origin + zAxis * (sLen / 2),
                        "radius" : sDia / 2
                    });
                    sampleQuery = qCreatedBy(id + "sampleCyl", EntityType.BODY);

                    sampleB = (sDia / 2) / meter;
                    sampleZmin = -(sLen / 2) / meter;
                    sampleZmax = (sLen / 2) / meter;
                }
                else if (definition.sampleType == CoilSampleType.PRISM)
                {
                    var sW = (definition.samplePrismWidth is ValueWithUnits) ? definition.samplePrismWidth : 10 * millimeter;
                    var sD = (definition.samplePrismDepth is ValueWithUnits) ? definition.samplePrismDepth : 10 * millimeter;
                    var sL = (definition.samplePrismLength is ValueWithUnits) ? definition.samplePrismLength : 55 * millimeter;
                    if (sqrt(sW * sW + sD * sD) >= coilId)
                    {
                        throw regenError("Sample diagonal must be smaller than the coil ID.", ["samplePrismWidth"]);
                    }

                    var samplePlane = plane(origin - zAxis * (sL / 2), zAxis, xAxis);
                    var sampleSketchId = id + "sampleSketch";
                    var skSample = newSketchOnPlane(context, sampleSketchId, { "sketchPlane" : samplePlane });
                    skRectangle(skSample, "s", {
                        "firstCorner" : vector(-sW / 2, -sD / 2),
                        "secondCorner" : vector(sW / 2, sD / 2)
                    });
                    skSolve(skSample);

                    opExtrude(context, id + "samplePrism", {
                        "entities" : qSketchRegion(sampleSketchId),
                        "direction" : zAxis,
                        "endBound" : BoundingType.BLIND,
                        "endDepth" : sL
                    });
                    sampleQuery = qCreatedBy(id + "samplePrism", EntityType.BODY);
                    cleanupEntities = append(cleanupEntities, qCreatedBy(sampleSketchId, EntityType.BODY));

                    sampleB = sqrt((sW / meter) * (sD / meter) / PI);
                    sampleZmin = -(sL / 2) / meter;
                    sampleZmax = (sL / 2) / meter;
                }
                else
                {
                    if (!(definition.samplePart is Query) || isQueryEmpty(context, definition.samplePart))
                    {
                        throw regenError("Select a sample part.", ["samplePart"]);
                    }

                    // Axial extent in the coil's own coordinate system
                    var coilCsys = coordSystem(origin, xAxis, zAxis);
                    var sampleBox = evBox3d(context, { "topology" : definition.samplePart, "cSys" : coilCsys, "tight" : true });
                    sampleZmin = sampleBox.minCorner[2] / meter;
                    sampleZmax = sampleBox.maxCorner[2] / meter;

                    // Average cross-section area = volume / axial span
                    var sVol = evVolume(context, { "entities" : definition.samplePart }) / (meter ^ 3);
                    var sSpan = sampleZmax - sampleZmin;
                    if (sSpan <= 0)
                    {
                        throw regenError("Could not measure the sample part along the coil axis.", ["samplePart"]);
                    }
                    sampleB = sqrt((sVol / sSpan) / PI);
                }

                var bClamped = min(sampleB, aInnerM * 0.999);
                sampleSegments = [makeSeg(sampleZmin, sampleZmax, bClamped, 0, airProps, bClamped, sampleP, true)];

                heatZmin = sampleZmin;
                heatZmax = sampleZmax;
                heatR = bClamped;
            }
        }

        // =========================================================================
        // 9. CLEANUP CONSTRUCTION GEOMETRY
        // =========================================================================
        opDeleteBodies(context, id + "cleanupCurves", {
            "entities" : qUnion(cleanupEntities)
        });

        // =========================================================================
        // 10. ESTIMATED INDUCTANCE (EMPTY COIL)
        // Base: thin-sheet single-layer coil (Nagaoka, Lundin approx.)
        //   L = mu0 * pi * a^2 * N^2 * K / l
        // LF: a = tube centerline radius, minus conductor-shape correction
        //     L = L_sheet - mu0 * a * N * (kS + kM)
        //     kS = 1.5 - ln(pitch / GMD_conductor)   (self term, any cross-section)
        //     kM = Rosa's N-turn mutual term
        //     GMD_conductor: round = d*e^(-1/4)/2 ; rectangle ~ 0.2235*(W+L)
        // HF: a = coil ID / 2 (current crowds onto inner face of the turns)
        // =========================================================================
        var mu0 = 4 * PI * 1e-7;                     // H/m
        var nTurns = revs;                           // actual helix revolutions (turns + 0.5)
        var windLen = totalHeight / meter;           // m (revs * pitch)

        var f1 = function(x) { return (1 + 0.383901 * x + 0.017108 * x * x) / (1 + 0.258952 * x); };
        var f2 = function(x) { return 0.093842 * x + 0.002029 * x * x - 0.000801 * x * x * x; };

        // Thin-sheet inductance (H) for mean radius a (m)
        var sheetL = function(a) {
            var d = 2 * a;
            var k;
            if (windLen >= d) {
                var dOverL = d / windLen;
                k = f1(dOverL * dOverL) - (4 * dOverL) / (3 * PI);
            } else {
                var lOverD = windLen / d;
                k = (2 / PI) * lOverD * ((log(4 / lOverD) - 0.5) * f1(lOverD * lOverD) + f2(lOverD * lOverD));
            }
            return mu0 * PI * a * a * nTurns * nTurns * k / windLen;
        };

        // --- LF estimate: centerline radius + conductor-shape correction ---
        var aMean = pathRadius / meter;
        var gmdCond;                                 // conductor self-GMD (m)
        if (isRectangular)
        {
            gmdCond = 0.2235 * (tubeW + tubeL) / meter;
        }
        else
        {
            gmdCond = 0.5 * exp(-0.25) * tubeOd / meter;
        }
        var kS = 1.5 - log((pitch / meter) / gmdCond);
        var kM = log(2 * PI) - 1.5 - log(nTurns) / (6 * nTurns) - 0.33084 / (nTurns * nTurns)
                 - 1 / (120 * nTurns * nTurns * nTurns);
        var lfH = sheetL(aMean) - mu0 * aMean * nTurns * (kS + kM);

        // --- HF estimate: current on the inner face of the turns ---
        var aInner = aInnerM;
        var hfH = sheetL(aInner);

        var lfUH = lfH * 1e6;
        var hfUH = hfH * 1e6;

        setVariable(context, "coilInductanceLF_uH", lfUH);
        setVariable(context, "coilInductanceHF_uH", hfUH);

        var inductanceText = "Inductance " ~ roundToPrecision(hfUH, 3) ~ " µH (HF) – " ~ roundToPrecision(lfUH, 3) ~ " µH (LF)";
        var msg = inductanceText ~ " · " ~ roundToPrecision(nTurns, 2) ~ " turns · no workpiece";

        // =========================================================================
        // 11. ESTIMATED VALUES
        // =========================================================================
        if (showEstimates)
        {
            // ---------------------------------------------------------------------
            // 11a. Lead inductance (two parallel conductors, opposite currents)
            //   L = (mu0 * len / pi) * acosh(s / (2r))
            // ---------------------------------------------------------------------
            var rCond = isRectangular ? ((tubeW + tubeL) / PI) / meter : (tubeOd / 2) / meter;
            var acoshFn = function(x) { return log(x + sqrt(x * x - 1)); };
            var twoWireL = function(len, s) {
                var x = s / (2 * rCond);
                if (x < 1.0001) { x = 1.0001; }
                return (mu0 * len / PI) * acoshFn(x);
            };

            var leadDir = -startHorizontalDir;
            var sepVec = op3 - ip3;
            var coilLeadSpacing = norm(sepVec - leadDir * dot(sepVec, leadDir)) / meter;

            if (tankLeadLength > 0 * meter && tankLeadSpacing <= 2 * rCond * meter)
            {
                throw regenError("Extra lead spacing must be larger than the tube size.", ["tankLeadSpacing"]);
            }

            var coilLeadsH = twoWireL(leadLength / meter, coilLeadSpacing);
            var tankLeadsH = twoWireL(tankLeadLength / meter, tankLeadSpacing / meter);
            var leadsH = coilLeadsH + tankLeadsH;

            // ---------------------------------------------------------------------
            // 11b. Complex math helpers ([re, im])
            // ---------------------------------------------------------------------
            var cMul = function(a, b) { return [a[0] * b[0] - a[1] * b[1], a[0] * b[1] + a[1] * b[0]]; };
            var cDiv = function(a, b) {
                var den = b[0] * b[0] + b[1] * b[1];
                return [(a[0] * b[0] + a[1] * b[1]) / den, (a[1] * b[0] - a[0] * b[1]) / den];
            };
            var cSub = function(a, b) { return [a[0] - b[0], a[1] - b[1]]; };
            var cScale = function(a, s) { return [a[0] * s, a[1] * s]; };

            // ---------------------------------------------------------------------
            // 11c. Solid conducting cylinder in a uniform axial AC field
            // Normalized flux F = mu_r * 2*J1(kr) / (kr*J0(kr)),  k = (1-j)/delta
            // With u = r^2 / (2 delta^2):
            //   J0 -> S0 = sum (j u)^n / (n!)^2
            //   2*J1/(kr) -> S1 = sum (j u)^n / (n!(n+1)!)
            // Re(F) = flux kept (inductance), -Im(F) = power absorbed (resistance)
            // For r/delta > 30 use the thick-body limit (1 - j) * delta / r.
            // ---------------------------------------------------------------------
            var kelvinRatio = function(rOverDelta) {
                if (rOverDelta > 30)
                {
                    return [1 / rOverDelta, -1 / rOverDelta];
                }
                var u = rOverDelta * rOverDelta / 2;
                var s0r = 0; var s0i = 0; var s1r = 0; var s1i = 0;
                var t0 = 1; var t1 = 1;
                var ph = 0;
                var nMax = floor(4 * sqrt(u)) + 25;
                for (var n = 0; n <= nMax; n += 1)
                {
                    if (n > 0)
                    {
                        t0 = t0 * u / (n * n);
                        t1 = t1 * u / (n * (n + 1));
                    }
                    if (ph == 0) { s0r += t0; s1r += t1; }
                    else if (ph == 1) { s0i += t0; s1i += t1; }
                    else if (ph == 2) { s0r -= t0; s1r -= t1; }
                    else { s0i -= t0; s1i -= t1; }
                    ph += 1;
                    if (ph == 4) { ph = 0; }
                }
                return cDiv([s1r, s1i], [s0r, s0i]);
            };

            // ---------------------------------------------------------------------
            // 11d. Layered load (crucible wall + charge), normalized flux F
            // Core -> air gap -> conductive wall (planar slab transfer) -> outside.
            // Returns F (normalized to pi*Ro^2) and the fraction of absorbed
            // power that lands in the core when the core is the "charge".
            // ---------------------------------------------------------------------
            var layeredF = function(seg, omega) {
                var ro = seg.Ro;
                var ai = ro - seg.wallT;
                var rc = seg.coreR;

                var fCore = [1, 0];
                if (rc > 0 && seg.coreCond)
                {
                    var dCore = sqrt(2 * seg.coreRho / (omega * mu0 * seg.coreMu));
                    fCore = cScale(kelvinRatio(rc / dCore), seg.coreMu);
                }

                // Effective flux area inside the wall (complex, m^2)
                var aIn = [PI * (ai * ai - rc * rc) + PI * rc * rc * fCore[0], PI * rc * rc * fCore[1]];
                var aOut;
                var hGain2 = 1;   // |H at core / H outside|^2

                if (seg.wallCond && seg.wallT > 0)
                {
                    // Surface impedance at the inner wall face: Z = -j*w*mu0*A / (2*pi*a)
                    var zIn = cScale(cMul([0, -1], aIn), omega * mu0 / (2 * PI * ai));
                    var dW = sqrt(2 * seg.wallRho / (omega * mu0));
                    var x = min(seg.wallT / dW, 30);
                    var ex = exp(x);
                    var emx = exp(-x);
                    var chx = (ex + emx) / 2;
                    var shx = (ex - emx) / 2;
                    var cx = cos(x * radian);
                    var sx = sin(x * radian);
                    var coshG = [chx * cx, shx * sx];     // cosh((1+j)x)
                    var sinhG = [shx * cx, chx * sx];     // sinh((1+j)x)
                    var eta = [seg.wallRho / dW, seg.wallRho / dW];

                    var hT = cSub(coshG, cMul(cDiv(zIn, eta), sinhG));   // H_out / H_in
                    var eT = cSub(cMul(zIn, coshG), cMul(eta, sinhG));   // E_out / H_in
                    var zOut = cDiv(eT, hT);

                    aOut = cScale(cMul([0, 1], zOut), 2 * PI * ro / (omega * mu0));
                    hGain2 = 1 / (hT[0] * hT[0] + hT[1] * hT[1]);
                }
                else
                {
                    aOut = [aIn[0] + PI * (ro * ro - ai * ai), aIn[1]];
                }

                var coreLoss = (rc > 0 && seg.coreCond) ? -PI * rc * rc * fCore[1] : 0;
                var totalLoss = -aOut[1];
                var frac = (totalLoss > 0 && seg.isCharge) ? min(hGain2 * coreLoss / totalLoss, 1) : 0;

                return { "F" : [aOut[0] / (PI * ro * ro), aOut[1] / (PI * ro * ro)], "chargeFrac" : frac };
            };

            // Coupling weight of each segment: G = (overlap / coil length) * L_sheet(Ro)
            var segG = [];
            for (var s = 0; s < size(sampleSegments); s += 1)
            {
                var sg = sampleSegments[s];
                var lo = max(sg.zMin, -windLen / 2);
                var hi = min(sg.zMax, windLen / 2);
                var ov = max(hi - lo, 0);
                segG = append(segG, (ov > 0) ? (ov / windLen) * sheetL(sg.Ro) : 0);
            }

            // Load seen by the coil: dL (H), R_load (ohm), R into charge (ohm)
            var computeLoad = function(omega) {
                var dL = 0;
                var rLoad = 0;
                var rCharge = 0;
                for (var s = 0; s < size(sampleSegments); s += 1)
                {
                    if (segG[s] > 0)
                    {
                        var res = layeredF(sampleSegments[s], omega);
                        dL += segG[s] * (res.F[0] - 1);
                        var rSeg = -omega * segG[s] * res.F[1];
                        rLoad += rSeg;
                        rCharge += rSeg * res.chargeFrac;
                    }
                }
                return { "dL" : dL, "R" : rLoad, "Rc" : rCharge };
            };

            // ---------------------------------------------------------------------
            // 11e. Tank frequency (iterate: frequency <-> loaded inductance)
            // ---------------------------------------------------------------------
            var capF = tankCapUF * 1e-6;
            var fEmpty = 1 / (2 * PI * sqrt((hfH + leadsH) * capF));
            var omega = 2 * PI * fEmpty;
            var load = computeLoad(omega);
            for (var it = 0; it < 8; it += 1)
            {
                var lCoil = max(hfH + load.dL, hfH * 0.05);
                omega = 1 / sqrt((lCoil + leadsH) * capF);
                load = computeLoad(omega);
            }
            var hfLoadedH = max(hfH + load.dL, hfH * 0.05);
            var fLoaded = omega / (2 * PI);

            // ---------------------------------------------------------------------
            // 11f. Copper losses & efficiency
            // Coil: current on inner face, R = rho * N * 2*pi*a / (delta * w)
            // Leads: current on conductor surface, R = rho * len / (delta * perimeter)
            // Copper at ~40 °C (water-cooled): rho = 1.8e-8 ohm*m
            // ---------------------------------------------------------------------
            var rhoCu = 1.8e-8;
            var deltaCu = sqrt(2 * rhoCu / (omega * mu0));
            var wEff = tubeL / meter;
            var rCoil = rhoCu * nTurns * 2 * PI * (aInner + deltaCu / 2) / (deltaCu * wEff);
            var leadLenTotal = 2 * (leadLength + tankLeadLength) / meter;
            var rLeads = rhoCu * leadLenTotal / (deltaCu * 2 * PI * rCond);
            var rCopper = rCoil + rLeads;

            var eff = (load.R > 0) ? load.R / (load.R + rCopper) : 0;
            var effCharge = (load.R > 0) ? load.Rc / (load.R + rCopper) : 0;
            var qFactor = omega * (hfLoadedH + leadsH) / (load.R + rCopper);

            // ---------------------------------------------------------------------
            // 11g. Cooling water pressure drop (Darcy-Weisbach)
            // Water at 20 °C. Straight sections: laminar 64/Re, turbulent Haaland
            // (drawn copper, eps = 1.5 µm). Helix: curved-pipe correlations
            // (Mishra & Gupta) with Schmidt critical Re. Leads + bends treated
            // as straight. Fittings / hoses / manifolds NOT included.
            // ---------------------------------------------------------------------
            var flowQ = flowLpm / 60000;             // m^3/s
            var rhoW = 998.2;                        // kg/m^3
            var muW = 1.002e-3;                      // Pa*s

            var boreArea;
            var dH;
            if (!isRectangular)
            {
                var dB = boreId / meter;
                boreArea = PI * dB * dB / 4;
                dH = dB;
            }
            else
            {
                var li = (tubeL - 2 * tubeWall) / meter;
                var wi = (tubeW - 2 * tubeWall) / meter;
                boreArea = li * wi;
                dH = 2 * li * wi / (li + wi);
            }

            var vel = flowQ / boreArea;
            var reyn = rhoW * vel * dH / muW;
            var log10f = function(x) { return log(x) / log(10); };

            var fStraight;
            if (reyn < 2300)
            {
                fStraight = 64 / reyn;
            }
            else
            {
                var eps = 1.5e-6;
                var inv = -1.8 * log10f((eps / dH / 3.7) ^ 1.11 + 6.9 / reyn);
                fStraight = 1 / (inv * inv);
            }

            var curv = dH / (2 * pathRadius / meter);          // tube bore / coil diameter
            var reCrit = 2300 * (1 + 8.6 * (curv ^ 0.45));
            var fHelix;
            var regime;
            if (reyn < reCrit)
            {
                var dean = max(reyn * sqrt(curv), 1);
                var lgDe = log10f(dean);
                fHelix = (64 / reyn) * (1 + 0.033 * lgDe * lgDe * lgDe * lgDe);
                regime = "laminar";
            }
            else
            {
                fHelix = 0.3164 * (reyn ^ -0.25) + 0.03 * sqrt(curv);
                regime = "turbulent";
            }

            var dynP = 0.5 * rhoW * vel * vel;
            var lenStraight = max(pathLenTotal - pathLenHelix, 0);
            var dP = (fStraight * lenStraight / dH + fHelix * pathLenHelix / dH) * dynP;   // Pa

            // ---------------------------------------------------------------------
            // 11h. Power split & water temperature rise
            // ---------------------------------------------------------------------
            var pIn = supplyKW * 1000;
            var pWork = eff * pIn;
            var pCopper = pIn - pWork;
            var waterRise = pCopper / (flowQ * rhoW * 4180);

            // ---------------------------------------------------------------------
            // 11i. Heating uniformity along the load (axial power profile)
            // Each helix revolution = one circular loop at its mean height (the
            // last half revolution = a half-weight loop). Each loop is split into
            // 3 filaments across the tube's inner face so the current is not a
            // single line. Axial field Hz at the load surface radius from the
            // off-axis loop formula (complete elliptic integrals K, E via AGM).
            // Heating per unit length ~ Hz^2 (thick body, r/delta >> 1).
            // Smoothed profile = average over exactly one pitch (turn ripple removed).
            // Empty-coil field: the load's own end effects and end-face heating
            // are NOT included. This is heat generated, not temperature.
            // ---------------------------------------------------------------------
            var uniformZoneMm = 0;
            var endRatioPct = 0;
            var zoneMinMaxPct = 0;
            var ripplePct = 0;
            var uniformWhole = false;
            var heatSpan = heatZmax - heatZmin;
            var hasUniformity = addSample && heatSpan > 0 && heatR > 0;

            if (hasUniformity)
            {
                // Complete elliptic integrals K(m), E(m), m = k^2 (AGM method)
                var ellipKE = function(m) {
                    var aG = 1;
                    var bG = sqrt(max(1 - m, 0));
                    var cSum = m / 2;             // sum of 2^(n-1) * c_n^2, n = 0 term
                    var w2 = 0.5;
                    for (var n = 0; n < 10; n += 1)
                    {
                        var cG = (aG - bG) / 2;
                        var aNext = (aG + bG) / 2;
                        bG = sqrt(aG * bG);
                        aG = aNext;
                        w2 = w2 * 2;
                        cSum += w2 * cG * cG;
                    }
                    var kK = PI / (2 * aG);
                    return { "K" : kK, "E" : kK * (1 - cSum) };
                };

                // Axial field of a 1 A loop of radius aL at radius r, axial offset z
                var loopHz = function(aL, r, z) {
                    var sP = (aL + r) * (aL + r) + z * z;
                    var dM = (aL - r) * (aL - r) + z * z;
                    var ke = ellipKE(4 * aL * r / sP);
                    return (ke.K + (aL * aL - r * r - z * z) / dM * ke.E) / (2 * PI * sqrt(sP));
                };

                var pitchM = pitch / meter;
                var halfH = windLen / 2;
                var spread = isRectangular ? 0.25 * tubeL / meter : 0.15 * tubeOd / meter;

                var loopZ = [];
                var loopW = [];
                var nFull = floor(revs);
                var fracRev = revs - nFull;
                for (var k = 0; k < nFull; k += 1)
                {
                    loopZ = append(loopZ, -halfH + (k + 0.5) * pitchM);
                    loopW = append(loopW, 1);
                }
                if (fracRev > 1e-6)
                {
                    loopZ = append(loopZ, -halfH + (nFull + fracRev / 2) * pitchM);
                    loopW = append(loopW, fracRev);
                }

                var hzAt = function(z) {
                    var hSum = 0;
                    for (var k = 0; k < size(loopZ); k += 1)
                    {
                        for (var fl = -1; fl <= 1; fl += 1)
                        {
                            hSum += (loopW[k] / 3) * loopHz(aInnerM, heatR, z - loopZ[k] - fl * spread);
                        }
                    }
                    return hSum;
                };

                // Sample the profile (about 8 points per pitch, limited for big coils)
                var nPts = ceil(heatSpan / (pitchM / 8)) + 1;
                nPts = min(max(nPts, 41), 401);
                nPts = min(nPts, max(41, floor(60000 / (27 * size(loopZ)))));
                var dzS = heatSpan / (nPts - 1);

                // pRaw = local heating; pSm = average over exactly one pitch
                // (8 midpoint samples, only those inside the load)
                var zs = [];
                var pRaw = [];
                var pSm = [];
                for (var i = 0; i < nPts; i += 1)
                {
                    var zP = heatZmin + dzS * i;
                    var hP = hzAt(zP);
                    zs = append(zs, zP);
                    pRaw = append(pRaw, hP * hP);

                    var sumP = 0;
                    var cntP = 0;
                    for (var q = 0; q < 8; q += 1)
                    {
                        var zQ = zP + ((q + 0.5) / 8 - 0.5) * pitchM;
                        if (zQ >= heatZmin - 1e-12 && zQ <= heatZmax + 1e-12)
                        {
                            var hQ = hzAt(zQ);
                            sumP += hQ * hQ;
                            cntP += 1;
                        }
                    }
                    pSm = append(pSm, (cntP > 0) ? sumP / cntP : hP * hP);
                }

                // Reference = coil center (or the nearest point of the load)
                var zRef = min(max(0, heatZmin), heatZmax);
                var ic = 0;
                for (var i = 1; i < nPts; i += 1)
                {
                    if (abs(zs[i] - zRef) < abs(zs[ic] - zRef)) { ic = i; }
                }
                var pC = pSm[ic];
                var band = uniformBandPct / 100;

                // Uniform zone: walk out from the center while within +/- band
                var iLo = ic;
                while (iLo > 0 && abs(pSm[iLo - 1] / pC - 1) <= band) { iLo -= 1; }
                var iHi = ic;
                while (iHi < nPts - 1 && abs(pSm[iHi + 1] / pC - 1) <= band) { iHi += 1; }
                uniformZoneMm = (zs[iHi] - zs[iLo]) * 1000;
                uniformWhole = (iLo == 0 && iHi == nPts - 1);

                // Ends of the load vs center (the colder end)
                endRatioPct = min(pSm[0], pSm[nPts - 1]) / pC * 100;

                // Target zone (centered on zRef): weakest / strongest
                var tHalf = (targetZone / meter) / 2;
                var zoneMin = -1;
                var zoneMax = 0;
                for (var i = 0; i < nPts; i += 1)
                {
                    if (abs(zs[i] - zRef) <= tHalf + 1e-12)
                    {
                        if (zoneMin < 0 || pSm[i] < zoneMin) { zoneMin = pSm[i]; }
                        if (pSm[i] > zoneMax) { zoneMax = pSm[i]; }
                    }
                }
                zoneMinMaxPct = (zoneMax > 0) ? zoneMin / zoneMax * 100 : 100;

                // Turn ripple: raw profile within one pitch of the center
                var rMin = -1;
                var rMax = 0;
                for (var i = 0; i < nPts; i += 1)
                {
                    if (abs(zs[i] - zRef) <= pitchM / 2 + 1e-12)
                    {
                        if (rMin < 0 || pRaw[i] < rMin) { rMin = pRaw[i]; }
                        if (pRaw[i] > rMax) { rMax = pRaw[i]; }
                    }
                }
                ripplePct = (rMax > 0) ? (rMax - rMin) / rMax * 100 : 0;
            }

            // ---------------------------------------------------------------------
            // 11j. Variables (always available in the variable table)
            // ---------------------------------------------------------------------
            setVariable(context, "coilInductanceLoaded_uH", hfLoadedH * 1e6);
            setVariable(context, "leadInductance_uH", leadsH * 1e6);
            setVariable(context, "tankFreqEmpty_kHz", fEmpty / 1000);
            setVariable(context, "tankFreqLoaded_kHz", fLoaded / 1000);
            setVariable(context, "efficiency_pct", eff * 100);
            setVariable(context, "efficiencyCharge_pct", effCharge * 100);
            setVariable(context, "workPower_kW", pWork / 1000);
            setVariable(context, "copperLoss_kW", pCopper / 1000);
            setVariable(context, "waterTempRise_C", waterRise);
            setVariable(context, "coilQ", qFactor);
            setVariable(context, "coolantDeltaP_kPa", dP / 1000);
            setVariable(context, "coolantVelocity_mps", vel);
            setVariable(context, "tubeLength_mm", pathLenTotal * 1000);
            setVariable(context, "heatUniformZone_mm", uniformZoneMm);
            setVariable(context, "heatZoneMinMax_pct", zoneMinMaxPct);
            setVariable(context, "heatEndRatio_pct", endRatioPct);
            setVariable(context, "heatRipple_pct", ripplePct);

            // ---------------------------------------------------------------------
            // 11k. Message text (pick one topic, or All)
            // ---------------------------------------------------------------------
            var sampleText = addSample ? " · with load " ~ roundToPrecision(hfLoadedH * 1e6, 3) ~ " µH" : "";
            var leadsText = " · leads " ~ roundToPrecision(leadsH * 1e6, 3) ~ " µH";

            var tankText = "Tank @ " ~ roundToPrecision(tankCapUF, 3) ~ " µF: " ~ roundToPrecision(fEmpty / 1000, 1) ~ " kHz";
            if (addSample)
            {
                tankText = tankText ~ " empty → " ~ roundToPrecision(fLoaded / 1000, 1) ~ " kHz loaded";
            }

            var effText;
            if (addSample)
            {
                effText = "Efficiency " ~ roundToPrecision(eff * 100, 1) ~ "% @ " ~ roundToPrecision(fLoaded / 1000, 1) ~ " kHz";
                if (isCrucible)
                {
                    effText = effText ~ " (charge " ~ roundToPrecision(effCharge * 100, 1) ~ "%, crucible " ~
                              roundToPrecision((eff - effCharge) * 100, 1) ~ "%)";
                }
                effText = effText ~ " · " ~ roundToPrecision(supplyKW, 2) ~ " kW in → " ~ roundToPrecision(pWork / 1000, 2) ~
                          " kW to work, " ~ roundToPrecision(pCopper / 1000, 2) ~ " kW copper loss (water +" ~
                          roundToPrecision(waterRise, 1) ~ " °C) · Q " ~ roundToPrecision(qFactor, 1);
            }
            else
            {
                effText = "Efficiency: add a dummy sample to estimate";
            }

            var velNote = (vel > 3) ? " (high – erosion risk)" : "";
            var coolText = "Cooling @ " ~ roundToPrecision(flowLpm, 2) ~ " L/min: ΔP " ~ roundToPrecision(dP / 1000, 1) ~
                           " kPa (" ~ roundToPrecision(dP / 6894.76, 1) ~ " psi) · " ~ roundToPrecision(vel, 2) ~ " m/s" ~ velNote ~
                           " · Re " ~ roundToPrecision(reyn, 0) ~ " " ~ regime ~
                           " · tube " ~ roundToPrecision(pathLenTotal * 1000, 0) ~ " mm";

            var uniText;
            if (hasUniformity)
            {
                var loadName = isCrucible ? "charge" : "sample";
                var zoneText = uniformWhole ? "whole " ~ loadName ~ " (" ~ roundToPrecision(uniformZoneMm, 1) ~ " mm)"
                                            : roundToPrecision(uniformZoneMm, 1) ~ " mm";
                uniText = "Heating uniformity: within ±" ~ roundToPrecision(uniformBandPct, 1) ~ "% over " ~ zoneText ~
                          " · " ~ roundToPrecision(targetZone / millimeter, 1) ~ " mm target zone min/max " ~
                          roundToPrecision(zoneMinMaxPct, 1) ~ "% · " ~ loadName ~ " ends " ~ roundToPrecision(endRatioPct, 1) ~
                          "% of center · turn ripple " ~ roundToPrecision(ripplePct, 1) ~ "% (heat generated, not temperature)";
            }
            else if (addSample)
            {
                uniText = "Heating uniformity: no charge in the crucible";
            }
            else
            {
                uniText = "Heating uniformity: add a dummy sample to estimate";
            }

            if (estimateDisplay == CoilEstimateDisplay.INDUCTANCE)
            {
                msg = inductanceText ~ sampleText ~ leadsText ~ " · " ~ roundToPrecision(nTurns, 2) ~ " turns";
            }
            else if (estimateDisplay == CoilEstimateDisplay.UNIFORMITY)
            {
                msg = uniText;
            }
            else if (estimateDisplay == CoilEstimateDisplay.TANK)
            {
                msg = tankText ~ " · total L " ~ roundToPrecision((hfLoadedH + leadsH) * 1e6, 3) ~ " µH";
            }
            else if (estimateDisplay == CoilEstimateDisplay.EFFICIENCY)
            {
                msg = effText;
            }
            else if (estimateDisplay == CoilEstimateDisplay.COOLING)
            {
                msg = coolText;
            }
            else
            {
                msg = inductanceText ~ sampleText ~ leadsText ~ "   |   " ~ tankText ~ "   |   " ~ effText ~ "   |   " ~ coolText ~ "   |   " ~ uniText;
            }
        }

        reportFeatureInfo(context, id, msg);

        // =========================================================================
        // 12. NAMES & COLORS (done last so later operations can't reset them)
        // =========================================================================
        var coilBody = qCreatedBy(id + "seamlessSweep", EntityType.BODY);
        setProperty(context, {
            "entities" : coilBody,
            "propertyType" : PropertyType.NAME,
            "value" : "Coil"
        });
        setProperty(context, {
            "entities" : coilBody,
            "propertyType" : PropertyType.APPEARANCE,
            "value" : color(184 / 255, 115 / 255, 51 / 255)      // #B87333
        });

        var steelGray = color(168 / 255, 169 / 255, 173 / 255);   // #A8A9AD

        if (!isQueryEmpty(context, sampleQuery))
        {
            setProperty(context, { "entities" : sampleQuery, "propertyType" : PropertyType.NAME, "value" : "Sample" });
            setProperty(context, { "entities" : sampleQuery, "propertyType" : PropertyType.APPEARANCE, "value" : steelGray });
        }
        if (!isQueryEmpty(context, crucibleQuery))
        {
            setProperty(context, { "entities" : crucibleQuery, "propertyType" : PropertyType.NAME, "value" : "Crucible" });
            setProperty(context, { "entities" : crucibleQuery, "propertyType" : PropertyType.APPEARANCE, "value" : crucibleColor });
        }
        if (!isQueryEmpty(context, chargeQuery))
        {
            setProperty(context, { "entities" : chargeQuery, "propertyType" : PropertyType.NAME, "value" : "Charge" });
            setProperty(context, { "entities" : chargeQuery, "propertyType" : PropertyType.APPEARANCE, "value" : steelGray });
        }
    });
```
