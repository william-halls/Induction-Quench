---
subsystem: wiring
tags: [wiring, control-system, NI-DAQ, software, PID, heat-curve, annealing, safety]
related: [[Design/Wiring/NI-DAQ Control Architecture.md]]
status: designed, not yet implemented
---

# Heat Curve Profile Software

GUI + execution design for programming multi-segment heat curves (ramp/hold sequences) on top of the [[Design/Wiring/NI-DAQ Control Architecture|NI-DAQ PID control loop]]. Purpose: anneal samples by running a defined temperature-vs-time profile (heat/cool at a controlled rate, then hold within a tolerance band for a set dwell time) rather than a single fixed setpoint.

## 1. Data Model

A heat curve is an ordered list of **segments/actions**, of three types:

**Rate segment** — ramps to a target temperature; behavior differs by direction, confirmed 2026-09-07:
- `target_temp` (°C) — where this segment ends
- `rate` (°C/min or °C/s) — ramp speed — **only meaningful when heating**
- Direction (heat vs. cool) is **inferred**, not stored separately: based on whether `target_temp` is above or below wherever the previous segment left off

**⚠️ Cooling is not actively controllable — the system can only turn the coil on or off.** There is no forced-cooling mechanism (no chiller/fan/quench-on-demand mid-curve — the only active cooling event is the terminal Quench action). This means:
- A **heating** Rate segment is closed-loop: the PID tracks a ramped setpoint toward `target_temp` at `rate`, same as originally designed.
- A **cooling** Rate segment is **open-loop**: output is simply commanded to 0 (coil off), and the segment waits for the actual thermocouple reading to fall to `target_temp` on its own via natural radiative/convective loss. The `rate` field is not enforceable for cooling — actual cooling speed is whatever physics gives you, and there is no way to speed it up. Don't use `rate` to gate segment advancement for cooling; gate on `actual_TC_temp <= target_temp` instead.
- No explicit duration field for either case — for heating, duration is derived (`time = |target_temp − start_temp| / rate`); for cooling, duration is whatever the natural cooldown takes (unknown in advance, will vary run to run).

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
- **Start / Pause / Stop** controls — see "Pause & Stop Behavior" below for exact semantics.
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

    if direction > 0:   # HEATING — closed-loop, rate is controllable
        setpoint = segment_start_temp + segment.rate * elapsed
        setpoint = clamp_toward(setpoint, segment_start_temp, segment.target_temp)
        if setpoint == segment.target_temp:
            advance_to_next_segment()

    else:               # COOLING — open-loop, coil off, physics-limited
        setpoint = 0    # coil off; segment.rate is not used/enforceable here
        if actual_TC_temp <= segment.target_temp:
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

## Pause & Stop Behavior (confirmed 2026-09-07)

Two distinct controls, not to be confused:

**Pause** — holds the run in place without aborting it.
- Freezes segment progression (`current_segment_index`, all elapsed/dwell timers stop advancing).
- Setpoint is held at **whatever the actual thermocouple temperature was at the moment of pause** — not the segment's original target/hold value. This effectively turns the paused state into an implicit hold at the current temperature, regardless of what the active segment was doing.
- Resume continues the segment from where it left off (timers pick back up, Rate segments resume ramping from the current point).

**Stop** — a real abort, not a pause.
- Commands the HOTSHOT to genuinely stop: opens the **STOP SSR (CTB1:4-5)** per [[Design/Wiring/NI-DAQ Control Architecture#START / STOP (CTB1:3-5) — still applies regardless of AO source|NI-DAQ Control Architecture]] (de-energizes the fail-safe SSR, which was confirmed to route through the DAQ specifically to support this) **and** writes 0A/0V to the NI-9269 at the same time — both mechanisms fire together rather than relying on just one.
- Does **not** trigger the Quench action — Stop is a plain heating abort, not a request to lower the sample into the bath. If the sample needs to come out of the coil after a Stop, that's a separate/manual action.
- Ends curve execution; there is no "resume" from Stop the way there is from Pause.

## Data Logging (confirmed 2026-09-07)

Log **thermocouple temperature and timestamp** for every run, at the control loop's own update rate. Minimum viable format: two columns (`timestamp`, `TC_temp`), one row per loop tick, written to a file per run (named/timestamped so multiple runs don't overwrite each other).

**Not yet decided / open for later**: whether to also log commanded Amps/Volts, active segment index, and Pause/Stop events in the same file — worth adding if post-run analysis needs to reconstruct *why* the temperature did what it did, not just what it did. Starting with the minimum (temp + time) per the current requirement; extend later if needed.

## Startup & Run-Out Safety (confirmed 2026-09-07)

Two related but independent safety/control mechanisms, plus one deliberately deferred feature. Easy to conflate since both of the first two involve a "dx/dt" — they measure different signals and serve different purposes.

### 1. PID output slew-rate limit

**What it measures/controls**: d(commanded Amps)/dt — how fast the PID's own output is allowed to change, applied *before* the Amps→Volts conversion and the NI-9269 write.

**Why**: without this, a large startup error (temperature far below setpoint) drives the PID's integral term to wind up through the known-ineffective low-current zone (see the HOTSHOT calibration curve — nothing meaningful happens below ~50A for the current sample) and then slam output upward once it crosses into effective territory. That produces a real, physical jump in heating rate — not a sensor fault, but indistinguishable from one to a naive rate-of-change check on the temperature signal (see #2 below).

**Implementation**: cap `|Amps(t) − Amps(t−1)| / dt` to a maximum value (e.g. X Amps/sec — TBD empirically, needs to stay fast enough to support the fastest intended Rate segment while still meaningfully smoothing the startup transient). Apply this clamp to the PID's output on every loop tick, before conversion to volts.

**Explicitly not used for**: detecting a faulty thermocouple. This only constrains the controller's own behavior — it has no visibility into whether the sensor reading is trustworthy.

### 2. Run-out safety trip (thermocouple fault detection)

**What it measures**: d(measured °C)/dt — the thermocouple's own reading over time, completely independent of what the controller is commanding.

**Why**: a detached/loose thermocouple (assessed as high-likelihood for this setup) will report a physically implausible rate of change — either falling toward ambient faster than any real sample's thermal mass would allow, or (per the noise concerns already flagged for the NI-9229 raw TC signal near the ~1MHz coil — see [[Design/Wiring/NI-DAQ Control Architecture#Thermocouple Signal Chain — NI-9229 + Analog CJC Amp (confirmed 2026-09-01)|Thermocouple Signal Chain]]) picking up EMI and reading erratic garbage. Either failure mode should trip an abort.

**Threshold calibration depends on #1**: with the output slew-rate limit in place, the *legitimate* maximum real dT/dt during startup becomes bounded and predictable (further smoothed by the sample's own thermal lag). Run a few normal slew-limited heating cycles, observe the actual peak measured dT/dt, and set the run-out trip threshold comfortably above that — giving clean separation between "fast but real heating" and "sensor fault." Without #1 in place first, this threshold would be much harder to set reliably (legitimate startup spikes and real faults would look too similar).

**Open, not yet resolved**: a detached TC reading garbage/noise near the coil might not always present as *fast* — it could also just look noisy without a large sustained rate change. If real-world failures turn out to look this way, the dT/dt trip alone may need to be paired with a plausibility/noise-floor check later. Not a blocker for v1 — revisit if/when an actual failure is observed that a pure dT/dt trip misses.

### 3. Effective minimum power auto-tune — deferred, not building for v1

Originally considered: a startup routine that ramps commanded current up from the HOTSHOT's electrical turn-on point (~7.7-8.8A) in small steps, watching for the first detectable thermal effect on the sample, and using that measured value as the PID's Output Range Low bound.

**Decision: not building this now.** Its two original justifications are both superseded:
- *PID tuning efficiency*: unnecessary — ordinary PID integral action climbs through the ineffective zone on its own without needing this pre-characterized.
- *Run-out grace-window boundary*: unnecessary — the output slew-rate limit (#1 above) solves the false-trip problem directly and works regardless of where the ineffective zone actually ends, without needing a dedicated measurement.

Also worth noting: the ~50A "ineffective" estimate this would have refined is itself a rough, **sample-specific** value (different mass/material/geometry would shift it) — not a fixed system constant — so building a whole auto-tune feature around it is more complexity than currently justified. Revisit only if real-world testing shows the plain PID + slew limiter approach isn't good enough on its own.

### Considered and dismissed: Hold-timeout safeguard and HOTSHOT Fault/Limit monitoring (2026-09-07)

Two guardrails were raised and deliberately **not** added, rather than left as oversights:

- **Hold segment stuck in an indefinite reset loop** if `hold_temp` turns out physically unreachable — no max-retry/timeout was added to catch this. **Decision: not a real risk worth guarding against** — if a hold temperature is genuinely unreachable, there are bigger underlying problems (HOTSHOT fault, coil/coupling issue, wrong sample, etc.) that would need direct attention anyway; a software timeout wouldn't fix the actual cause, and the run being stuck/visible (via the live overlay graph and status readout) is a sufficient signal that something's wrong without needing dedicated logic for it.
- **Monitoring HOTSHOT Fault/Limit status (CTB1:8-13) mid-run** to catch the HOTSHOT throttling/faulting independently of the PID's commands — not being built. **Decision: same reasoning** — if the HOTSHOT is faulting or hitting its own protection limits, that's a bigger problem than this software is responsible for catching; not worth the added wiring/complexity to guard against it here.

## Open Items

- [ ] Implement segment table GUI (LabVIEW array-of-clusters control + Add/Remove/Move buttons)
- [ ] Implement "Add Quench" as a distinct, list-terminal action in the GUI
- [ ] Implement live preview graph (pre-run planned curve)
- [ ] Implement execution state machine (above) wired into the existing PID loop
- [ ] Implement actual-vs-planned overlay graph during a run
- [ ] Wire the Quench action's P0.0 pulse into the same digital-output mechanism already used elsewhere for ST-PMC1 RUN triggering
- [ ] Decide/tune `tolerance` values empirically once the PID loop's real-world hold stability is characterized
- [ ] Decide save/load format for named profiles (suggest flattened XML or JSON)
- [ ] Implement PID output slew-rate limiter; determine max Amps/sec empirically (fast enough for intended Rate segments, slow enough to smooth startup transients)
- [ ] Implement run-out trip on thermocouple dT/dt; calibrate threshold from observed peak dT/dt during slew-limited test runs
- [ ] Watch for detached-TC failure modes that present as noise/erratic readings rather than fast sustained rate changes — may need a plausibility check in addition to the dT/dt trip if this turns out to matter in practice
- [ ] Implement open-loop cooling behavior for cooling-direction Rate segments (coil off, gate advancement on actual temp reaching target, not on a ramped setpoint)
- [ ] Implement Pause (freeze timers, hold setpoint at actual temp at time of pause) and Stop (open STOP SSR + zero NI-9269 output, no quench) per the confirmed semantics above
- [ ] Implement data logging (timestamp + TC temp, one row per loop tick, per-run file)
- [ ] Wire STOP (CTB1:4-5) through the fail-safe SSR per [[Design/Wiring/NI-DAQ Control Architecture]] — this is now required (not optional) since the Stop control depends on it

## See also
- [[Design/Wiring/NI-DAQ Control Architecture]] — PID loop, calibration curve, Amps→Volts conversion this feeds into
- [[Design/Wiring/Electrical System]] — HOTSHOT CTB1 pinout and hardware context
- [[Design/Wiring/Ball Screw Motor Control]] — ST-PMC1 RUN trigger (USB-6009 P0.0 → LCD4075DD3 SSR) and the onboard lower/submerge quench sequence the Quench action hands off to
