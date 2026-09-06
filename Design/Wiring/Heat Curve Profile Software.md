---
subsystem: wiring
tags: [wiring, control-system, NI-DAQ, software, PID, heat-curve, annealing]
related: [[Design/Wiring/NI-DAQ Control Architecture.md]]
status: designed, not yet implemented
---

# Heat Curve Profile Software

GUI + execution design for programming multi-segment heat curves (ramp/hold sequences) on top of the [[Design/Wiring/NI-DAQ Control Architecture|NI-DAQ PID control loop]]. Purpose: anneal samples by running a defined temperature-vs-time profile (heat/cool at a controlled rate, then hold within a tolerance band for a set dwell time) rather than a single fixed setpoint.

## 1. Data Model

A heat curve is an ordered list of **segments/actions**, of three types:

**Rate segment** — ramps to a target temperature at a controlled rate
- `target_temp` (°C) — where this segment ends
- `rate` (°C/min or °C/s) — ramp speed
- Direction (heat vs. cool) is **inferred**, not stored separately: based on whether `target_temp` is above or below wherever the previous segment left off
- No explicit duration field — duration is derived: `time = |target_temp − start_temp| / rate`

**Hold segment** — maintains a temperature within a tolerance band for a set dwell time
- `hold_temp` (°C) — target to sit at
- `tolerance` (±°C) — how tight the thermocouple must stay to count as "in band"
- `duration` (seconds) — how long it must stay continuously in-band

**Quench action** — terminal action; ends the heat curve by triggering the physical quench
- No parameters. Not a temperature segment at all — it stops the PID loop and fires the ball screw's quench sequence.
- **Only valid as the last item in the list.** The GUI should enforce this (disable "Add Segment" after a Quench action is placed, or grey out Rate/Hold once one exists).
- Execution: (1) stop the PID loop / write 0V to the NI-9269 (heating off), (2) pulse **USB-6009 line P0.0** — this fires the **LCD4075DD3 SSR → ST-PMC1 RUN** trigger per [[Design/Wiring/Ball Screw Motor Control#Component Details|Ball Screw Motor Control]], which runs the ST-PMC1's own onboard program: lower the sample into the pre-filled water bath (the lowering motion *is* the quench — no valve/release involved) and hold submerged for its own programmed duration. The heat-curve software does not need to manage submerge timing itself — that already lives in the ST-PMC1 program (see "Control Sequence Example: Sample Lift & Quench" in that note).
- After firing the trigger, the curve is complete — no further segments execute.

**LabVIEW representation**: an array of clusters, e.g.:

```
Segment Cluster:
  - Type       (enum: Rate | Hold | Quench)
  - TargetTemp (double)
  - Rate       (double)   -- used only if Type=Rate
  - Tolerance  (double)   -- used only if Type=Hold
  - Duration   (double)   -- used only if Type=Hold
                           -- Quench uses none of these fields
```

The array of these clusters *is* the recipe — flattens cleanly to XML/JSON for saving/loading named profiles.

## 2. GUI

- **Segment table**: array-of-clusters control, one row per segment. Grey out/hide irrelevant fields per row based on that row's Type (e.g. hide Rate when Type=Hold; hide all parameter fields when Type=Quench) via property node keyed off the Type enum.
- **Add / Remove / Move Up / Move Down** buttons to build and reorder the sequence.
- **"Add Quench" is a distinct action from adding a Rate/Hold segment**, and can only ever be the last item in the list — disable/grey out "Add Segment" (Rate/Hold) once a Quench action exists, and disable "Add Quench" if one is already present. This keeps the recipe from ending in anything other than the physical quench trigger.
- **Live preview graph**: before running, walk the segment list once (no hardware involved) to generate a planned temp-vs-time XY array and plot it.
- **During a run**: overlay the actual thermocouple trace on the same graph against the planned curve — useful for tuning and for visually catching a stuck/lagging segment.
- **Start / Pause / Abort** controls.
- **Status readout**: current segment #, time remaining/elapsed in segment, current setpoint, in-band status (for Hold segments).

## 3. Execution Engine

Runs inside the control loop tick, alongside the [[Design/Wiring/NI-DAQ Control Architecture#HOTSHOT Setpoint Calibration Curve (confirmed 2026-09-06)|PID loop]] (PID output in Amps, converted to Volts via the confirmed calibration formula before writing to the NI-9269).

State kept: `current_segment_index`, `segment_start_time`, `segment_start_temp` (captured when a segment becomes active), `in_band_elapsed` (Hold segments only).

**Each loop tick:**

```
segment = curve[current_segment_index]
elapsed = now - segment_start_time

if segment.Type == Rate:
    direction = sign(segment.target_temp - segment_start_temp)
    setpoint = segment_start_temp + direction * segment.rate * elapsed
    setpoint = clamp_toward(setpoint, segment_start_temp, segment.target_temp)  # don't overshoot past target
    if setpoint == segment.target_temp:
        advance_to_next_segment()

elif segment.Type == Hold:
    setpoint = segment.hold_temp
    in_band = abs(actual_TC_temp - segment.hold_temp) <= segment.tolerance
    if in_band:
        in_band_elapsed += loop_dt
    else:
        in_band_elapsed = 0   # RESET on excursion — see decision below
    if in_band_elapsed >= segment.duration:
        advance_to_next_segment()

elif segment.Type == Quench:
    setpoint = 0                          # heating off — write 0V to NI-9269 directly, bypass PID/Amps-Volts conversion
    pulse_digital_line(USB6009_P0_0)      # fires LCD4075DD3 SSR -> ST-PMC1 RUN
    # ST-PMC1 runs its own onboard program from here: lower sample into bath, hold submerged
    # for its own programmed duration (see Ball Screw Motor Control "Control Sequence Example").
    # This software does not manage submerge timing — that lives on the ST-PMC1.
    curve_complete = True                 # no further segments; sequence ends here

PID_setpoint = setpoint
```

## Decision: Hold-band excursion behavior — RESET (confirmed 2026-09-06)

Three options were considered for what happens to the Hold segment's dwell timer when the thermocouple drifts outside the tolerance band mid-hold:

1. **Reset the timer to 0** — strictest; requires a genuinely continuous stable hold for the full duration.
2. **Pause the timer** (stop accumulating, don't reset) — only accumulates "good" time; a brief excursion costs just that time.
3. **Ignore the band for timing** — run duration as a pure wall-clock timer regardless of in-band status, flag excursions as warnings only.

**Decision: Option 1 (reset).** Rationale: this process is being used to **anneal samples** — the metallurgical requirement is continuous, sustained time-at-temperature. Accumulated-but-interrupted dwell time is not physically equivalent to a genuinely continuous hold for annealing purposes, so a reset-on-excursion policy is the correct match for the actual process, even though it's the least forgiving option computationally.

**Implication**: a noisy thermocouple reading or a momentary PID hunt near the band edge can restart an entire hold segment from zero. Consider:
- Choosing `tolerance` wide enough to not trigger on ordinary loop noise/PID ripple (informed by however tightly the loop actually holds once tuned)
- Watching the live overlay graph during early runs to check how often resets are actually happening in practice

## Quench Action (confirmed 2026-09-06)

The heat curve always ends in a terminal **Quench** action rather than a Rate/Hold segment — see the Data Model above. It doesn't carry temperature parameters; it's a trigger, not a setpoint. On execution it:

1. Cuts heating (writes 0V to the NI-9269, bypassing the PID/calibration conversion entirely).
2. Pulses **USB-6009 digital line P0.0**, which fires the **LCD4075DD3 SSR → ST-PMC1 RUN** input (per [[Design/Wiring/Ball Screw Motor Control#Component Details|Ball Screw Motor Control]]).
3. Hands off entirely to the **ST-PMC1's own onboard program** — lowering the sample into the pre-filled water bath (the lowering motion *is* the quench; no valve or release mechanism exists) and holding it submerged for whatever duration is programmed into that controller (see that note's "Control Sequence Example: Sample Lift & Quench").

The heat-curve software's job stops at firing the trigger — submerge duration, motion speed, and the raise/lower sequence itself are owned by the ST-PMC1, not duplicated here.

## Open Items

- [ ] Implement segment table GUI (LabVIEW array-of-clusters control + Add/Remove/Move buttons)
- [ ] Implement "Add Quench" as a distinct, list-terminal action in the GUI
- [ ] Implement live preview graph (pre-run planned curve)
- [ ] Implement execution state machine (above) wired into the existing PID loop
- [ ] Implement actual-vs-planned overlay graph during a run
- [ ] Wire the Quench action's P0.0 pulse into the same digital-output mechanism already used elsewhere for ST-PMC1 RUN triggering
- [ ] Decide/tune `tolerance` values empirically once the PID loop's real-world hold stability is characterized
- [ ] Decide save/load format for named profiles (suggest flattened XML or JSON)

## See also
- [[Design/Wiring/NI-DAQ Control Architecture]] — PID loop, calibration curve, Amps→Volts conversion this feeds into
- [[Design/Wiring/Electrical System]] — HOTSHOT CTB1 pinout and hardware context
- [[Design/Wiring/Ball Screw Motor Control]] — ST-PMC1 RUN trigger (USB-6009 P0.0 → LCD4075DD3 SSR) and the onboard lower/submerge quench sequence the Quench action hands off to
