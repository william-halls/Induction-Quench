---
tags: [coil-design, cad, parametric, feature-script, automation]
---

# Coil Feature Script

Parametric OnShape FeatureScript for rapid generation of induction coil geometries. Supports both circular and rectangular tube profiles with configurable dimensions.

## Quick Access

🔗 **OnShape Document**: https://cad.onshape.com/documents/f8c5aa176fe5d3504340f7c7/v/e52646392fe2bef23f6bfba3/e/91393e0f3187f0dfdba36f16

## Features

✓ **Coil Types** — Circular or rectangular tube profiles  
✓ **Parametric** — Adjust ID, turns, gap, lead length dynamically  
✓ **Cooling** — Configurable water jacket sleeve  
✓ **Orientation** — Automatic centering and rotation  
✓ **Smart Leads** — Bezier-smooth inlet/outlet bends  

## Key Parameters

| Parameter           | Range      | Purpose                  |
| ------------------- | ---------- | ------------------------ |
| Coil Inner Diameter | 1-2000 mm  | Center hole radius       |
| Number of Turns     | 0.01-500   | Winding count            |
| Gap Between Winds   | 0-500 mm   | Pitch spacing            |
| Tube OD             | 0.1-200 mm | Wire/tube size           |
| Lead Length         | 0-2000 mm  | Supply line length       |
| Bend Radius         | 0.1-500 mm | Smooth entry/exit curves |
| Sleeve              | Optional   | Gf Sleeve VIsulization   |

## Usage

1. Open the OnShape document
2. Set desired parameters in the FeatureScript feature panel
3. Geometry auto-generates with smooth Bezier transition curves
4. Export STL for 3D printing molds or STEP for manufacturing

## Implementation Highlights

- **Centered Helix** — Auto-calculates total height and centers coil vertically
- **Bezier Transitions** — Smooth inlet/outlet curves with configurable bend radius
- **Dual-Lead Support** — Separate lead lengths for main coil and optional sleeve
- **Cleanup Routine** — Removes construction geometry for clean final model

---

## Full Source Code

FeatureScript 2960;
import(path : "onshape/std/common.fs", version : "2960.0");
import(path : "onshape/std/geometry.fs", version : "2960.0");

annotation { "Feature Type Name" : "Induction Coil" }
export const inductionCoil = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
        // =========================================================================
        // 1. TARGET & ORIENTATION SETUP
        // =========================================================================
        annotation { "Name" : "Center Target (Point or Line)", "Filter" : EntityType.VERTEX || EntityType.EDGE, "MaxNumberOfPicks" : 1 }
        definition.target is Query;

        annotation { "Name" : "Reverse Winding (CCW)" }
        definition.reverseDirection is boolean;

        annotation { "Name" : "Rotation Angle" }
        isAngle(definition.rotationAngle, { (degree) : [-360, 0, 360] } as AngleBoundSpec);

        // =========================================================================
        // 2. COIL CORE DIMENSIONS
        // =========================================================================
        annotation { "Name" : "Coil Inner Diameter (ID)" }
        isLength(definition.coilId, { (millimeter) : [0.1, 25, 2000] } as LengthBoundSpec);

        annotation { "Name" : "Number of Turns" }
        isReal(definition.turns, { (unitless) : [0.01, 5, 500] } as RealBoundSpec);

        annotation { "Name" : "Gap Between Winds" }
        isLength(definition.gap, { (millimeter) : [0, 3, 500] } as LengthBoundSpec);

        // =========================================================================
        // 3. PROFILE TYPE & GEOMETRY
        // =========================================================================
        annotation { "Name" : "Rectangular Profile Type" }
        definition.isRectangular is boolean;

        if (!definition.isRectangular)
        {
            annotation { "Name" : "Tube Outer Diameter (OD)" }
            isLength(definition.tubeOd, { (millimeter) : [0.1, 5, 200] } as LengthBoundSpec);
        }
        else
        {
            annotation { "Name" : "Tube Axial Length (L)" }
            isLength(definition.tubeL, { (millimeter) : [0.1, 5, 200] } as LengthBoundSpec);

            annotation { "Name" : "Tube Radial Width (W)" }
            isLength(definition.tubeW, { (millimeter) : [0.1, 5, 200] } as LengthBoundSpec);
        }

        // =========================================================================
        // 4. SUPPLY LEADS CONFIGURATION
        // =========================================================================
        annotation { "Name" : "Lead Length" }
        isLength(definition.leadLength, { (millimeter) : [0, 25, 2000] } as LengthBoundSpec);

        annotation { "Name" : "Bend Radius" }
        isLength(definition.bendRadius, { (millimeter) : [0.1, 5, 500] } as LengthBoundSpec);
        
        // =========================================================================
        // 5. SLEEVE CONFIGURATION
        // =========================================================================
        annotation { "Name" : "Add Outer Sleeve" }
        definition.addSleeve is boolean;

        if (definition.addSleeve)
        {
            annotation { "Name" : "Sleeve Thickness" }
            isLength(definition.sleeveThickness, { (millimeter) : [0.1, 1, 200] } as LengthBoundSpec);
            
            annotation { "Name" : "Sleeve Lead Length" }
            isLength(definition.sleeveLeadLength, { (millimeter) : [0, 25, 2000] } as LengthBoundSpec);
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
        var gap = (definition.gap is ValueWithUnits) ? definition.gap : 3 * millimeter;
        var leadLength = (definition.leadLength is ValueWithUnits) ? definition.leadLength : 25 * millimeter;
        var reverseDirection = (definition.reverseDirection == true);
        
        var bendRadius = (definition.bendRadius is ValueWithUnits && definition.bendRadius > 0*meter) ? definition.bendRadius : 5 * millimeter;
        if (bendRadius > leadLength * 0.8) { bendRadius = leadLength * 0.8; } 

        var isRectangular = (definition.isRectangular == true);
        var tubeOd = 5 * millimeter;
        var tubeL = 5 * millimeter;
        var tubeW = 5 * millimeter;

        if (!isRectangular) {
            if (definition.tubeOd is ValueWithUnits) { tubeOd = definition.tubeOd; }
            tubeL = tubeOd;
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

        var pitch = tubeL + gap;
        var pathRadius = (coilId / 2) + (tubeOd / 2);
        var revs = turns + 0.5; 

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

            // Add new sleeve wireframes to the cleanup routine
            cleanupEntities = append(cleanupEntities, sleeveInletLineQuery);
            cleanupEntities = append(cleanupEntities, sleeveOutletLineQuery);
            cleanupEntities = append(cleanupEntities, qCreatedBy(sleeveSketchId, EntityType.BODY));
        }

        // =========================================================================
        // 9. CLEANUP CONSTRUCTION GEOMETRY
        // =========================================================================
        opDeleteBodies(context, id + "cleanupCurves", {
            "entities" : qUnion(cleanupEntities)
        });
    });