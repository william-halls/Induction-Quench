---
subsystem: mechanisms
tags: [mechanisms, sample-mounting, thermal-management, concept]
---

# Hybrid Ceramic Holder (BN + Alumina Silicate "Lava")

> **Status (2026-09-21): concept only.** Not committed — not quoted, machined, or tested. Geometry below reflects the current CAD concept model; thermal margin figures are projections pending measurement.

Parent note: [[Design/Mechanisms/Ceramic Mount|Ceramic Mount]] (current Version 3 all-BN build).
Thermal derivation: [[Design/Plumbing/Seal Thermal Margin Analysis.md]].

**Problem this solves**: real-world thermocouple testing showed the shaft seal **already exceeding** its 210°F rated limit (~120°C / 248°F measured with sample held at 850°C, confirmed 2026-09-18). Swapping part of the holder to a much lower-conductivity ceramic cuts the heat reaching the seal.

## Concept

Keep the **orange wedge and gold ring in boron nitride** (proven thermal-shock tolerance, right at the direct sample-contact interface), and switch **teal main body + blue (formerly "purple") cap to alumina silicate ceramic ("Lava"), McMaster 8479K69** (3/4" dia rod, $79.36). The teal/blue boundary becomes a ceramic-to-ceramic interface. **The original steel screw is retained** — the titanium screw / mica washer heat-break is a separate deferred option (see below), not part of this concept.

- **k = 1.265 W/(m·°C) at 20°C** (datasheet-confirmed) for Lava vs. BN's ~60-90 W/m·K cross-plane / 150-250 in-plane (corrected range, see [[Design/Plumbing/Material Properties Reference.md]])
- **Why split at gold→teal instead of switching the whole holder**: the orange wedge is in direct clamped contact with the sample. When the sample is quenched, it cools from ~850°C toward ambient in seconds — since the orange wedge touches it, heat conducts *backward* out of the wedge into the rapidly-cooling sample, a real cooling-driven thermal-shock event at that interface, independent of whether water/steam ever touches the ceramic directly. Lava's thermal-shock tolerance for this specific conducted-shock scenario is unverified, while BN has established service history in exactly this kind of repeated-cycling role (previously used for hot-rolling mill guides). The orange+gold sections contribute under 1% of the holder's total thermal resistance anyway (0.19 + 0.47 K/W out of ~95 K/W), so keeping them BN costs almost nothing thermally while removing the risk entirely from the one interface where it actually matters.
- **Fabrication note**: sold "Unfired," rated to 2,010°F even unfired per this datasheet — may not require firing at all for this application (peak ~1,562°F/850°C), which would avoid the ~2% fire-expansion dimensioning issue and kiln dependency generally associated with this material class. **Supporting evidence added 2026-09-25**: McMaster's own product-page description for this exact part (8479K69) says it's "often used in applications that require high temperature resistance and low thermal expansion, such as standoffs and welding jigs" — with no firing step mentioned as part of normal use. This is a real materials family often called "lava stone" or "wonderstone," typically sold and used *permanently unfired*; firing is an optional hardening step some users skip entirely. This raises confidence that the as-sold unfired spec sheet (all values above) already describes the material's working state, not a pre-fire intermediate — but still worth a direct confirmation call with McMaster/supplier before final commitment, since the product page doesn't explicitly rule out firing being needed for a 2,010°F duty cycle specifically.
- **Mechanical properties are modest** — tensile strength only 2,500 psi (compressive 25,000 psi, flexural 10,000 psi) — weaker than BN; fine for this application's light loads but avoid tension/bending-loaded features.

## Joint Design: Pinned Clevis (Fork + Tongue)

*Corrected 2026-09-21, read from CAD mass-property renders.*

A plain flat butt joint was ruled out — the screw only clamps cap-to-shaft (it doesn't run through the full stack), so nothing preloads the BN/Lava interface in compression, and a flat face has zero tensile capacity anyway, so it can't resist the pull-apart load this joint carries.

> **Superseded concepts**: two intermediate designs (spigot/socket register + central dowel; dovetail key + slots) were written up during the 2026-09-21 discussion and are both **incorrect** — they came from a misreading of the section view. Disregard them; the actual geometry is the cross-pinned clevis below.

| Part | Volume | Geometry |
|---|---|---|
| **AS Ceramic Holder** (Lava cap) | 7052.261 mm³ | Cylinder; tapped hole in top face for the 304 rod screw; **straight slot milled up from the bottom** forming a two-eared fork/clevis; **cross-hole through both ears** |
| **Ceramic Holder** (BN body) | 4098.363 mm³ | Cylinder with a flat **tongue/blade** standing off its top face and a **cross-hole through the tongue**; sample-retention T-slot at its bottom end (pre-existing feature, not part of this joint) |
| **AS Pin** | 353.429 mm³ | Plain round rod (~3mm dia × ~50mm), dropped through the aligned fork/tongue holes |

- **Tension is carried by the pin in double shear** — this is what resists pull-apart where a butt face couldn't.
- **Pin material: Alumina Silicate (AS/Lava), not metal** — so the only material contacting the BN body at this joint is ceramic. Keeps the standing rule intact (**metal — the screw/304 rod — never touches BN; it only ever contacts the Lava cap**) and avoids a metal thermal bridge across the very interface Lava was added to insulate.
- **No undercut/reentrant geometry anywhere** — open-ended slot, external tongue, round drilled holes, plain rod.
- **Pin retention: the quartz tube, not a separate feature.** The pin isn't captured by an e-clip, cotter, adhesive, or shoulder — it's a plain rod dropped through a through-hole, which on its own could migrate/slide out sideways. It stays put because the assembly sits inside the quartz tube in service: the tube's ID is close enough to the holder's ⌀20mm OD that there's no radial clearance for the pin to escape once the assembly is loaded in. This means **pin captivity is only guaranteed while the holder is inside the tube** — worth keeping in mind for out-of-tube handling/assembly steps (bench work, transport between stations) where nothing is holding the pin in besides friction/gravity.

## Joint Load Check (2026-09-21)

The only load on the clevis joint is the **sample weight** (confirmed — the holder is not otherwise loaded in tension).

| Quantity | Value |
|---|---|
| Sample geometry | 10 × 10 × 61mm (per [[Design/Sample Quenching/Modified Charpy.md]]) ≈ 6.1 cm³ |
| Sample material | Steel, ~7.85 g/cm³ |
| Sample mass | **~47.9 g** |
| Tension load | **~0.47 N (0.1 lbf)** |
| Pin cross-section | ~7 mm² (353.429 mm³ over ~50mm length, ≈3mm dia) |
| Pin stress | **~0.067 MPa ≈ 9.7 psi** |
| Lava tensile rating | 2,500 psi → **~250× margin** |

**Verdict**: raw strength is a non-issue at this load — not a close call. Remaining caveats the margin number does *not* cover:
- **Shock/impact loading** from the trapdoor / lift / positioning mechanisms could be 10-100× static weight, and brittle ceramic has no yield to absorb it. Worth sanity-checking how gentle those mechanisms actually are.
- Stress concentration at any sharp arris is a separate failure mode from nominal stress — see machinability notes.

**CAD mass values — resolved (2026-09-21)**: materials are now assigned in Onshape, giving real readouts:

| Part | Volume | Mass | Implied density |
|---|---|---|---|
| Ceramic Holder (BN body) | 4098.363 mm³ | **8.607 g** | 2.10 g/cm³ |
| AS Ceramic Holder (Lava cap) | 7052.261 mm³ | **16.925 g** | 2.40 g/cm³ |
| AS Pin | 353.429 mm³ | **0.848 g** | 2.40 g/cm³ |

These land right in the estimated range from the earlier hand calc (BN ~8.2g, Lava ~16.2-17.6g, pin ~0.81-0.88g) — CAD density confirms the assumption rather than contradicting it.

**Resolved 2026-09-25**: the McMaster 8479K69 datasheet lists density as **0.09 lb/in³ = 2.49 g/cm³**. Onshape's assigned 2.40 g/cm³ is close but **~3.7% lower** than the real datasheet value — confirms it's not wildly off (not a wrong-material library default), but it's also not an exact match, so it was likely a generic "alumina silicate" catalog default rather than this specific part's number. Worth updating the Onshape material assignment to 2.49 g/cm³ for accuracy, though at ~4% this doesn't change any load-check conclusion (the joint runs at ~250x margin regardless).

## Machinability Assessment (2026-09-21)

**Verdict: easy to machine.** Every feature is in the friendliest category for brittle ceramic — no undercuts, no reentrant corners, no form tooling.

| Part | Operations |
|---|---|
| Lava cap | Lathe: turn OD, face, drill + tap top hole. Mill: straight slot up from bottom (slitting saw or end mill, open-ended cut — no blind pocket). Drill: one cross-hole through both ears. |
| BN body | Lathe: turn OD, face. Mill: two flats to leave the tongue standing (**external** material removal — gentlest op for brittle material). Drill: one cross-hole through the tongue. (Sample T-slot at the bottom is a pre-existing requirement, not chargeable to this joint.) |
| AS pin | Cut to length from stock rod — alumina is available as stock precision rod, so possibly not a machined part at all. |

Easier than either superseded concept (the dovetail needed form tooling and fillet-blending passes; the spigot/socket needed a concentric boss/bore register held to a fit tolerance).

**Safety note added 2026-09-25**: this material's datasheet composition is **~59% silicon dioxide** — machining it (turning, milling, drilling) generates fine crystalline silica dust, a silicosis inhalation hazard. This wasn't previously flagged anywhere in this note despite the detailed machining operations listed below. Use dust collection/vacuum extraction at the tool and a respirator rated for crystalline silica for any shop work on the Lava cap or pin, same precautions as any silica-bearing stone/ceramic.

**Details worth specifying to the shop:**
1. **Match-drill the cross-hole** with fork and tongue assembled — eliminates the alignment tolerance stack entirely. Standard clevis practice.
2. **Drill exit breakout** — brittle ceramic chips where the bit exits. Peck drill, back the exit face, or drill from both sides to meet in the middle.
3. **Chamfer the slot mouth and hole edges** — cheap insurance against chip initiation.
4. **Fork ear thickness** — the ears are the weakest element (brittle, loaded in bending by the pin). Non-issue at 0.47 N, but don't thin them unnecessarily.
5. **Tongue root radius** — the one spot that still needs a called-out radius. The cylinder-to-tongue transition is where bending stress lands under any side load. External fillet, trivially machined; just don't leave it a sharp step.
6. **Tongue-to-slot clearance** — needs to accommodate BN vs. AS differential expansion without binding. Flat parallel faces make this far easier to control than a wedge profile would have been.

## Heat Transfer — Margin Table Predates This Geometry

The margin percentages below were derived assuming a **single flat face-to-face butt interface**. The clevis joint is a different thermal path, and every geometric change points the same direction — *more* resistance, not less:

- **More interfaces in series**: BN tongue → fork inner faces, plus tongue → pin → ear bores. Every solid-solid contact carries its own contact resistance, even same-material.
- **Reduced contact area** vs. a full flat face — the slot removes material that would otherwise conduct.
- **Expansion clearance is an air gap**, and air is a far better insulator than ceramic contact.

Material choice is not a concern here: the pin being AS (k = 1.265 W/(m·°C)) rather than metal means **no low-resistance bypass** was introduced across the interface Lava exists to insulate.

**Net**: directionally favorable for seal margin (more resistance = less heat reaching the shaft seal), but the specific figures **no longer strictly describe this geometry**. Consistent with this project's established preference for measurement over modeling (the pure conduction model already proved too pessimistic vs. the real 120°C reading), this should be **re-verified with a thermocouple reading at the seal once built** rather than re-modeled by hand.

## Margin Projection (calibrated to the real 120°C measurement, not the pure modeled sweep)

Rather than trust the pure series-conduction model (which we know runs too pessimistic — it predicted 328-554°F for the current hardware when the real measurement showed 120°C/248°F), the ambient heat-loss resistance implied by the real measurement was backed out and used to project this hybrid configuration forward. Full derivation: [[Design/Plumbing/Seal Thermal Margin Analysis.md]].

| Configuration | Margin (calibrated projection) |
|---|---|
| All-BN (current, no change) | **-19% to -27%** (already exceeds rating) |
| Orange+gold+teal BN / blue Lava only | +51.0% |
| **Orange+gold BN / teal+blue Lava (concept as written)** | **+80.6%** |
| Full Lava holder (all 4 zones) | +85.7% |

The concept captures nearly all of the available margin (+80.6% vs. full-Lava's +85.7%) while keeping BN at the one interface where the quench-adjacent thermal shock actually occurs.

### Unresolved (2026-09-21) — which configuration is actually being modeled?

The clevis CAD renders show only **two ceramic bodies**: one BN body running all the way down to the sample, and one Lava cap. That reads as the **"orange+gold+teal BN / blue Lava only"** row (**+51.0%**), not the +80.6% concept, which requires part of the teal body to be Lava. Either the CAD is a simplified concept model, or the design has effectively reverted to the Lava-cap-only configuration. **This needs confirming before the +80.6% figure is quoted anywhere downstream** — it's a ~30 percentage point difference. (Both rows are still positive, so this is a question of how much headroom exists, not whether the approach works.)

### Teal split point — open item (2026-09-19)

*Applies only if the 4-zone / split-teal configuration is the one being built — see unresolved item above.*

The BN-to-Lava split point was planned to fall *within* the teal body (not at a zone boundary) — two faces of the same ⌀20mm diameter, rather than across a step change in geometry. Exact split location (how much of teal's 26.65mm length stays BN vs. becomes Lava) is pending discussion with the machine shop on what's practical to hold/cut. Margin stays comfortably positive across a wide range of split points — a conservative 75% BN / 25% Lava split still projects **+64.5%**; a 25% BN / 75% Lava split projects **+77.1%**. Manufacturability, not thermal performance, is the deciding factor.

Split orientation is fixed as **BN on the gold-ring side, Lava on the cap side** — not the reverse — so the screw lands entirely in Lava. Each segment can be **any length**; the screw's bore is confined to the cap and doesn't extend into teal regardless of the split.

**Standing rule, valid in either configuration**: every metal-to-ceramic interface (screw-to-cap, shaft-to-cap) touches **Lava only**. No metal ever crosses the BN-to-Lava boundary. The clevis design satisfies this, since the pin is AS ceramic.

## Thermal Shock Risk — reassessed and narrowed (2026-09-17)

Initial concern was unverified thermal shock resistance under repeated rapid heat/quench cycling. Reassessed given the actual quench process — **the holder itself is never submerged; only the sample is quenched in water**:

- Ceramic thermal-shock cracking is driven by rapid *cooling* putting the surface in tension (ceramics, including this one, are much weaker in tension — 2,500 psi — than compression — 25,000 psi). Full immersion in cold water is the severe version of this. Since the holder never contacts the quench bath directly, it never experiences that event — it only sees the gradual, conduction-limited transient already modeled (τ≈89 min for holder+joint), the same duty cycle McMaster's own use-case examples describe ("standoffs and welding jigs" — repeatedly heated by proximity, cooled between cycles, never submerged).
- Even incidental water/steam contact (splash, condensation creeping up the shaft) would be **preheated by the sample's own heat dump into the bath**, not cold bulk-bath-temperature water — a much smaller ΔT than the worst case, and if it's condensing steam specifically, that's a *heating* event (safe compressive stress), not a cooling one.
- **Remaining open item, narrowed**: the **orange wedge** (direct clamped sample contact) still sees the fastest/steepest gradient in the assembly — conducted, not immersion-driven, but worth a validation test specifically on that geometry/contact condition before committing the full holder, rather than testing the whole assembly against full immersion.
- **Fallback if that test doesn't hold up**: machine the holder from **Grade 5 Titanium** instead — no ceramic brittle-fracture risk at all, still gives **+49% margin** (vs. Lava's +75%), fully ductile/well-characterized, ordinary shop-machinable. Comfortable margin either way; Lava is the higher-upside option worth trying first given the risk is now well-bounded.

## Firing Schedule (only if firing is done)

Published schedule for this material family (Foundry Service & Supplies Grade A aluminum silicate) — not needed if the as-machined unfired rating holds up:

- **Heating ramp**: 111-139°C/hour for standard sections (max 167°C/hour). For thick sections (≥13mm cross-section — applies to the teal main body at ⌀20mm) slow to **28-83°C/hour**, and consider stress-relief holes to reduce cracking risk during the firing itself.
- **Maturing (soak) temperature**: **1,010-1,093°C** — do not exceed 1,093°C (causes crystallization, distortion, shrinkage, loss of properties).
- **Soak time**: 30 min for sections up to ~6mm thick, 45 min for sections ≥13mm thick — the teal body needs the longer soak.
- **Cooling**: no mandated rate published; pull parts once below 93°C. Given the thermal-shock discussion above, cool passively (furnace-closed) rather than open-air, even though the manufacturer doesn't require it.
- **Atmosphere**: ordinary air, nothing special.
- **Dimensional change**: expands ~2% during firing (not shrinkage) — build green-state machining dimensions around growth, not shrinkage.
- **Open issue**: this part has widely varying cross-sections (thin orange/gold wedges vs. the thick teal body) — the whole firing schedule needs to be built around the thickest section's slower ramp rate, which may over-stress the thinner sections. Worth discussing with whoever does the firing.

## Thermal Break at the Screw Joint — deferred, not part of this concept

**Status (2026-09-19)**: the holder material change alone already projects +80.6% margin using the original steel screw — comfortable enough that this joint heat-break is **not currently planned**. Kept on record as a future option if more margin is ever needed (e.g. if the orange-wedge validation test comes back marginal, or if real testing shows less benefit than projected).

The concept would add a titanium screw + mica washer at the rod-to-ceramic-holder joint:

- **Screw: Grade 5 Titanium (Ti-6Al-4V), 1/4-20 × 3/4"L — McMaster 94081A112.** k≈6.7 W/m·K vs. steel's 16-45 W/m·K (confirmed this needs to be Grade 5 specifically — Grade 2/CP titanium runs k≈17-22 W/m·K, essentially no better than steel; a ceramic-alumina screw option considered along the way was ruled out for the same reason, alumina's k≈20-30 W/m·K isn't actually low). Nonmagnetic, corrosion-resistant, 130,000 psi tensile. Original screw was 0.4"L; confirm the assembly has clearance for the longer 3/4"L part (or trim to length).
- **Washer: Muscovite mica, cut from tube stock — McMaster 5067K56** (1/2" OD × 1/4" ID × 1/8" wall tube; saw thin cross-sectional rings off the end). Datasheet-confirmed k=0.3 W/(m·°C) — blocks the parallel direct-contact heat path between rod end and cap face (not just the screw itself). Target thickness ~0.01-0.025" (stack multiple thin slices if needed) adds a meaningful 25-100% to the joint's thermal resistance. Ream the as-cut 1/4" ID slightly for screw clearance (it's an exact nominal fit, not a clearance hole) — mica files/sands easily, just avoid forcing an undersized screw through it. OD (0.5") deliberately matches the rod's own OD so the washer fully covers the rod's end face with no gap or wasted overhang. Chosen over phlogopite mica (marginally higher temp ceiling; k is essentially the same between the two — a 2026-09-18 double-check found the real difference is negligible and, if anything, points the other way from what was first assumed — muscovite was picked for cost and because 499°C already has huge margin over anything this joint sees, not a real k advantage), Macor, Vespel, and PEEK. Light clamp torque at this joint suits mica's main weakness (delamination under shear/point load isn't triggered by pure axial compression).

## Open Items Summary

| Item | Status |
|---|---|
| Which configuration is actually being built (+51.0% vs +80.6%) | **Unresolved** — CAD shows 2 bodies, concept text describes split teal |
| Lava density (for real mass values) | **Resolved 2026-09-25** — datasheet confirms 2.49 g/cm³; Onshape's 2.40 g/cm³ is ~3.7% low, worth correcting but doesn't change any conclusion |
| Materials assigned in Onshape | **Done (2026-09-21)** — real masses: BN 8.607g, Lava cap 16.925g, AS pin 0.848g (recompute after density correction above if precision matters) |
| Unfired rating sufficient (skip firing?) | Still needs direct confirmation with McMaster/supplier, but McMaster's own product description (added 2026-09-25) supports this material typically being used permanently unfired for this exact standoff/jig use case — raises confidence, doesn't fully resolve it |
| Silica dust precautions during machining | **Added 2026-09-25** — not previously documented; material is ~59% SiO₂ |
| Orange wedge thermal-shock validation test | Not yet run |
| Teal split location (if split config) | Pending machine shop input |
| Tongue root radius spec | Needs calling out on the drawing |
| Seal temperature re-measurement after build | Pending build |

## Related

- [[Design/Mechanisms/Ceramic Mount|Ceramic Mount]] — parent note, current Version 3 all-BN build
- [[Design/Plumbing/Seal Thermal Margin Analysis.md]] — full thermal derivation
- [[Design/Plumbing/Material Properties Reference.md]] — material k values
- [[Design/Plumbing/Vertical Sliding Shaft Seal|Vertical Sliding Shaft Seal]] — the component this concept protects
- [[Design/Sample Quenching/Modified Charpy|Modified Charpy]] — sample geometry driving the load case
