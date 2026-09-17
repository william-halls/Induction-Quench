---
subsystem: wiring
tags: [wiring, control-system, NI-DAQ, software, PID, heat-curve, annealing, safety]
related: [[Design/Wiring/NI-DAQ Control Architecture.md]]
status: in progress — LabVIEW GUI build underway (2026-09-09)
---

# Heat Curve Profile Software

GUI + execution design for programming multi-segment heat curves (ramp/hold sequences) on top of the [[Design/Wiring/NI-DAQ Control Architecture|NI-DAQ PID control loop]]. Purpose: anneal samples by running a defined temperature-vs-time profile (heat/cool at a controlled rate, then hold within a tolerance band for a set dwell time) rather than a single fixed setpoint.

## 1. Data Model

A heat curve is an ordered list of **segments/actions**, of three types:

**Rate segment** — ramps to a target temperature; behavior differs by direction, confirmed 2026-09-07, **direction input method revised 2026-09-09**:
- `target_temp` (°C) — where this segment ends
- `rate` (°C/min or °C/s) — ramp speed
- **Direction (heat vs. cool) is signed on `rate` itself (revised 2026-09-09)**: a positive `rate` means heating, a negative `rate` means cooling. This supersedes the original 2026-09-07 decision ("direction inferred from whether `target_temp` is above/below the previous segment's end temp, `rate` always a positive magnitude") — the GUI build surfaced a real case for letting the user type a negative value directly to mean "cool." **The magnitude (`Abs(rate)`) is what's actually used in all duration/setpoint math** — the sign is now purely an input convention for the user, not something the math branches on by itself. (Whether the software should also cross-check that a negative-rate row's `target_temp` is actually below the running temp — catching a contradictory entry like a negative rate paired with a temp that's higher than current — is still open, not yet built.)

**⚠️ Cooling is not actively controllable — the system can only turn the coil on or off.** There is no forced-cooling mechanism (no chiller/fan/quench-on-demand mid-curve — the only active cooling event is the terminal Quench action). This means:
- A **heating** Rate segment is closed-loop: the PID tracks a ramped setpoint toward `target_temp` at `rate`, same as originally designed.
- A **cooling** Rate segment is **open-loop**: output is simply commanded to 0 (coil off), and the segment waits for the actual thermocouple reading to fall to `target_temp` on its own via natural radiative/convective loss. The `rate` field is not enforceable for cooling — actual cooling speed is whatever physics gives you, and there is no way to speed it up. Don't use `rate` to gate segment advancement for cooling; gate on `actual_TC_temp <= target_temp` instead.
- No explicit duration field for either case — for heating, duration is derived (`time = |target_temp − start_temp| / rate`); for cooling, duration is whatever the natural cooldown takes (unknown in advance, will vary run to run).

**Hold segment** — maintains a temperature within a tolerance band for a set dwell time
- `hold_temp` (°C) — target to sit at
- `tolerance` (±°C) — how tight the thermocouple must stay to count as "in band". **Revised 2026-09-09: this is now a single global control on the GUI (one value applies to every Hold segment in the curve), not a per-row field** — see GUI section below.
- `duration` (seconds) — how long it must stay continuously in-band

**Quench action — no longer a segment type (revised 2026-09-09).** The original design (below, kept for history) had Quench as a terminal, list-ending entry the user manually added via an "Add Quench" button. **This was dropped.** Quench is now purely behavioral, not data:
- It **fires automatically** the instant the last user-defined Rate/Hold segment in the curve finishes — no explicit "Quench row" is stored in the array at all.
- A separate, standalone **"Quench Now"** button also exists (sibling to Start/Pause/Stop) letting the operator trigger the same sequence immediately, abandoning whatever segment is currently active — see "Quench Action" section below for the updated behavior and rationale.
- Execution is unchanged from the original design: (1) stop the PID loop / write 0V to the NI-9269 (heating off), (2) pulse **USB-6009 line P0.0** — this fires the **LCD4075DD3 SSR → ST-PMC1 RUN** trigger per [[Design/Wiring/Ball Screw Motor Control#Component Details|Ball Screw Motor Control]], which runs the ST-PMC1's own onboard program: lower the sample into the pre-filled water bath (the lowering motion *is* the quench — no valve/release involved) and hold submerged for its own programmed duration. The heat-curve software does not need to manage submerge timing itself — that already lives in the ST-PMC1 program (see "Control Sequence Example: Sample Lift & Quench" in that note).

**LabVIEW representation (as actually built, 2026-09-09)**: an array of a typedef'd cluster, `Segment.ctl`:

```
Segment.ctl (Type Def cluster):
  - Type            (Ring: 0 = "-- Select --", 1 = Rate, 2 = Hold)
  - Temp             (double)  -- target_temp for Rate, hold_temp for Hold
  - RateOrDuration   (double)  -- rate (signed, °C/min) for Rate; duration for Hold
  - EstStartTime     (double, indicator-only field — see GUI section)
```

`Tolerance` is **not** in this cluster — it's a single global numeric control on the front panel, read once by the execution engine and applied to every Hold row. `Type = 0` ("-- Select --") represents an incomplete/placeholder row the user hasn't finished filling in yet — rows sitting on this value are treated as inactive (skipped by the preview-curve math, and filtered out entirely by the "Clean" button — see GUI section).

The array of these clusters *is* the recipe — flattens cleanly to XML/JSON for saving/loading named profiles (still open, see Open Items).

<details>
<summary>Original data model (2026-09-07, superseded 2026-09-09 — kept for history)</summary>

Original `Rate` segment had direction *inferred* from comparing `target_temp` to the previous segment's end temp, with `rate` always a positive magnitude. Original `Quench` was a stored, list-terminal segment type added via a dedicated "Add Quench" button:
- No parameters. Not a temperature segment at all.
- **Only valid as the last item in the list** — GUI disabled "Add Segment" once a Quench action was placed.
- After firing, curve was complete, no further segments executed.

Original cluster shape:
```
Segment Cluster:
  - Type       (enum: Rate | Hold | Quench)
  - TargetTemp (double)
  - Rate       (double)   -- used only if Type=Rate
  - Tolerance  (double)   -- used only if Type=Hold
  - Duration   (double)   -- used only if Type=Hold
                           -- Quench uses none of these fields
```
</details>

## 2. GUI (as actually built, 2026-09-09)

The original "array-of-clusters with per-row property-node hide/show" plan turned out not to be how LabVIEW arrays actually work — an Array control can't show a different layout per row (Property Nodes restyle the whole element template, not one row independently). The layout below is what was actually built instead, and it sidesteps that limitation by giving every row the *same* 4 controls regardless of Type, just reinterpreted per row:

**Segment table** — Array of `Segment.ctl` (see Data Model), 4 columns per row, uniform layout:

| Col | Control | Rate row means | Hold row means |
|---|---|---|---|
| 1 | `Type` — Ring dropdown (`-- Select --` / `Rate` / `Hold`) | `Rate` | `Hold` |
| 2 | `Temp` — editable numeric | `target_temp` | `hold_temp` |
| 3 | `RateOrDuration` — editable numeric | `rate` (signed — see Data Model) | `duration` |
| 4 | `EstStartTime` — **display-only**, computed | when this segment begins | when this segment begins |

- **`EstStartTime` is locked read-only** via a Property Node (`Disabled` property, set to `2` = "Disabled and Grayed") targeting that specific field inside the array's element template, wired once outside any loop at VI start. (Converting the field to an "Indicator" inside the `Segment.ctl` typedef was tried first and doesn't reliably hold once nested inside an array — the Property Node approach is the one that actually works.)
- **Global `Tolerance (±°C)`** numeric control sits above the table, applies to every Hold row (not per-row — see Data Model).
- **Buttons**: `Add Segment`, `Clean`, `Move Up`, `Move Down` — no "Add Quench" button (Quench is no longer a stored segment, see Data Model).
  - **`Add Segment`** appends one blank row (`Type = 0`, `Temp = 0`, `RateOrDuration = 0`) to the end of the array.
  - **Auto-add-row**: after any edit, if the *last* row in the table now has `Type != 0` AND both numeric fields filled in (non-zero), a new blank row is automatically appended — so the table keeps growing as the user finishes each row, without needing to click `Add Segment` every time.
  - **`Clean`** (renamed from an earlier "Remove by selected index" design) — filters the whole table, stripping out every row still sitting on `Type = 0` (`-- Select --`, i.e. an unfinished placeholder), keeping all rows that have actually been assigned a real Type. Implemented as a For Loop over the array with a Conditional output tunnel (`Type != 0` as the keep-condition).
  - **`Move Up` / `Move Down`** — swap the row at a `Selected Row #` index with its neighbor. (Not yet re-verified against the Local-Variable-based read/write pattern used for Add/Clean — see Open Items.)
- **Live preview graph**: before running, walk the segment list (no hardware involved) via a dedicated subVI, `Generate Preview Curve.vi`, to generate a planned temp-vs-time XY array and plot it. See "Preview Curve Generation" section below — this subVI is built and validated.
- **During a run**: overlay the actual thermocouple trace on the same graph against the planned curve, auto-focusing/scrolling to track the live point as the run progresses (since actual Hold duration can run longer than the planned estimate while the PID settles into tolerance, or reset entirely on an excursion) — **not yet built**, needs the execution engine (Loop 2) to exist first, see Open Items.
- **Run controls**: `Start` / `Pause` / `Stop` / `Quench Now` — four independent buttons, not nested under the table. See "Pause & Stop Behavior" and "Quench Action" below for exact semantics.
- **`Home`** button — unrelated to the heat curve table; sends the ball-screw sample-lift mechanism to its reference position via the IN1+IN2 polled combination per [[Design/Wiring/Ball Screw Motor Control#On-Demand Position Control: GOTO Instruction + Finalized I/O Mapping (2026-09-01)|Ball Screw Motor Control]]. Lives in Loop 2 (control loop), not the table-editing loop.
- **Status readout**: current segment #, time remaining/elapsed in segment, current setpoint, in-band status (for Hold segments) — not yet built.

### LabVIEW loop architecture (decided 2026-09-09)

Two separate, parallel **While Loops**, each with its own **Event Structure** — not one shared loop/structure (an earlier idea to combine both into a single Event Structure was rejected: two Event Structures inside *one* loop can deadlock each other, since a loop iteration won't complete until every Event Structure in it has processed an event).

```
Loop 1 (GUI/table loop)              Loop 2 (control/heat-curve loop)
- Event Structure                    - Event Structure
  - Add Segment: Value Change          - Timeout (~100ms) — drives the
  - Clean: Value Change                  Rate/Hold/Quench execution-engine
  - Move Up/Down: Value Change           tick (see Section 3)
  - Stop: Value Change (exits Loop 1)  - Start: Value Change (sets RunActive)
                                        - Quench Now: Value Change
                                        - Home: Value Change
                                        - Stop: Value Change (exits Loop 2)
```

- **Cross-loop data sharing uses Local Variables**, not shift registers or a Global Variable — e.g. Loop 2 reads `Segment Table` via a `Segment Table` Local Variable (read mode) each tick; Loop 1's own edits (Add/Clean/Move) each read-modify-write `Segment Table` the same way, independently, inside their own event case (a fresh Local Variable pair per case — they can't share one instance across cases). A Global Variable was considered and rejected: unnecessary, since both loops live in the same VI and Local Variables already cover this.
- **Shift registers are still used, but only for state that's local to one loop** — e.g. Loop 2's own execution state (`current_segment_index`, `segment_start_time`, etc., bundled as `Exec State`) doesn't need to be read from Loop 1, so it stays a shift register on Loop 2 alone.
- **Timeout event case**: an Event Structure's built-in `Timeout` case (fires automatically after N ms of no other event) is what gives Loop 2 a steady periodic tick without needing a plain polling loop — the Timeout terminal on the structure's edge is wired to a constant (currently `100` ms, TBD once real DAQ timing is known).
- **`Stop`'s Mechanical Action had to be changed from Latch to `Switch Until Released`** — LabVIEW explicitly disallows creating a Local Variable of a Latch-action Boolean (`Boolean 'STOP': Boolean Latch Action is incompatible with Local Variables`), since Latch's auto-reset behavior only works through the control's own terminal/Event Structure, not through a Local Variable read from a second loop.
- **Editing during a run — decided against live editing (2026-09-09)**: earlier idea was to allow editing rows after the currently-executing segment while a run is active (blocking edits at/before it). **Simplified instead**: the curve is built entirely while idle; once a run starts there is no live-editing support at all — to change anything, stop the program and restart it. Matches the broader decision (below) that `Stop` is a full program exit, not a resumable pause.

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

## Quench Action (confirmed 2026-09-06, revised 2026-09-09)

**Revised 2026-09-09**: Quench is no longer a stored, list-terminal segment the user manually adds — see Data Model. It's now purely behavioral, triggered two ways:

1. **Automatically**, the instant the last user-defined Rate/Hold segment in the curve finishes (`current_segment_index` advances past the end of the array).
2. **Manually**, via a standalone **`Quench Now`** button (sibling to `Start`/`Pause`/`Stop`) — available any time a curve is running (including while Paused), fires the same sequence immediately regardless of which segment is currently active, abandoning the rest of the curve.

Both paths call the same underlying sequence — no duplicated logic:

1. Cuts heating (writes 0V to the NI-9269, bypassing the PID/calibration conversion entirely).
2. Pulses **USB-6009 digital line P0.0**, which fires the **LCD4075DD3 SSR → ST-PMC1 RUN** input (per [[Design/Wiring/Ball Screw Motor Control#Component Details|Ball Screw Motor Control]]).
3. Hands off entirely to the **ST-PMC1's own onboard program** — lowering the sample into the pre-filled water bath (the lowering motion *is* the quench; no valve or release mechanism exists) and holding it submerged for whatever duration is programmed into that controller (see that note's "Control Sequence Example: Sample Lift & Quench").

The heat-curve software's job stops at firing the trigger — submerge duration, motion speed, and the raise/lower sequence itself are owned by the ST-PMC1, not duplicated here.

Distinct from **Stop**: Stop opens the STOP SSR and zeroes output but does *not* trigger quench — `Quench Now` is the one control that both stops heating *and* fires the physical quench sequence on demand.

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

**Revised 2026-09-09 — Stop = full program exit, not a resumable hold.** An "E-stop gate" design (loops keep running, all work pauses while Stop is held, un-latching returns to an idle/ready state requiring a fresh `Start` press to resume) was considered and explicitly **rejected** as unnecessary complexity for now — simpler to require: releasing/un-latching Stop does nothing on its own, and the operator manually stops and restarts the whole LabVIEW program to run again. `Stop`'s Mechanical Action is `Switch Until Released` (not Latch — see loop architecture notes above for why Latch is incompatible with the Local Variable Loop 2 needs to poll it), and it feeds both loops' conditional terminals (via the real terminal in Loop 1, via a Local Variable read in Loop 2), ending both loops and the VI's execution entirely.

## Preview Curve Generation — `Generate Preview Curve.vi` (built & validated 2026-09-09)

A standalone subVI that computes the planned temp-vs-time preview curve from the Segment Table, with no hardware involved — called from the main VI's `Add Segment`/`Clean`/`Move Up`/`Move Down` event cases after each edit, feeding an XY Graph (`Heat Curve Preview`) on the front panel.

**Interface**: `Curve Data` (Array of `Segment.ctl`, input) → `Times` (Array of Double, output), `Temps` (Array of Double, output).

**Logic**: a single For Loop, auto-indexing over `Curve Data`, with four shift registers carrying state across iterations — `running_time`, `running_temp` (both Double), and `Times`/`Temps` (both growing Arrays of Double, appended via `Insert Into Array` each iteration rather than `Build Array` + Concatenate Inputs, which turned out to be all-or-nothing per-node in this LabVIEW version rather than settable per-input as expected). A Case Structure on `Type` handles the three possibilities:

- **`0` / Default** (`-- Select --` or unexpected) — straight pass-through, no points added, running values unchanged.
- **`1` (Rate)** — `seg_duration = Abs(Temp − running_temp) / Abs(RateOrDuration)` (the `Abs` on the rate is what makes the 2026-09-09 signed-rate convention safe — without it, a negative rate produces a negative duration and makes `running_time` run backward). `new_time = running_time + seg_duration`, `new_temp = Temp`. One point appended: `(new_time, new_temp)`.
- **`2` (Hold)** — temperature doesn't move. Two points appended: `(running_time, running_temp)` (start of the flat segment) and `(running_time + RateOrDuration, running_temp)` (end of it) — this is what actually draws the horizontal line on the graph.

**Validated** against hand-calculated multi-row test cases (mixed Rate/Hold, including a negative-rate cooling segment) — output matched expected values exactly once two wiring bugs were found and fixed (both were "New Element" inputs on `Temps`' `Insert Into Array` nodes accidentally wired from a time-value instead of a temp-value — one in the Rate case, one in the Hold case's pair of appends).

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

**Done (2026-09-09):**
- [x] Segment table GUI — Array of `Segment.ctl`, 4-column layout (Type/Temp/RateOrDuration/EstStartTime), global Tolerance control
- [x] `Add Segment` (append blank row) + auto-add-row when last row fills in
- [x] `Clean` (filter out `Type = 0` placeholder rows)
- [x] `EstStartTime` locked read-only via Property Node
- [x] `Generate Preview Curve.vi` — planned curve math, built and validated against hand-calculated test cases
- [x] Two-loop architecture decided and shell built (Loop 1 GUI/table, Loop 2 control/heat-curve with Timeout-driven tick)
- [x] Cross-loop data sharing via Local Variables; `Stop` mechanical-action fix (Latch → Switch Until Released) for Local Variable compatibility
- [x] Quench redesigned as automatic-at-completion + manual `Quench Now` override, no longer a stored segment
- [x] Cooling direction redesigned as signed `rate` (negative = cooling) instead of inferred-from-temp-comparison
- [x] Decided against live table-editing during a run — full stop/restart instead
- [x] Decided against E-stop-gate Stop behavior — Stop is a full, non-resumable program exit

**Still open:**
- [ ] Re-verify `Move Up`/`Move Down` against the Local-Variable read/write pattern (built before the shift-register-staleness bug was found and fixed in Add/Clean — may have the same bug)
- [ ] Wire `Generate Preview Curve.vi` output (`Times`/`Temps`) into an actual `Heat Curve Preview` XY Graph on the front panel (subVI itself is done, graph wiring is not)
- [ ] Build the Loop 2 execution-engine `Timeout` case itself (state machine pseudocode in Section 3 exists; not yet implemented in LabVIEW — currently just the empty loop/Event Structure shell)
- [ ] Implement actual-vs-planned overlay graph during a run, with auto-focus/scroll on the live point (needs Loop 2's execution engine to exist first, to have real data to plot)
- [ ] Build `Start`/`Pause`/`Quench Now`/`Home` event cases in Loop 2 (currently just Timeout is planned in detail)
- [ ] Wire the Quench action's P0.0 pulse into the same digital-output mechanism already used elsewhere for ST-PMC1 RUN triggering
- [ ] Wire `Home`'s IN1+IN2 output per the finalized Ball Screw I/O mapping — exact bit-combination for "Home" vs. Center/Top/Bottom not yet pulled from that doc into this implementation
- [ ] Decide/tune `tolerance` values empirically once the PID loop's real-world hold stability is characterized
- [ ] Decide save/load format for named profiles (suggest flattened XML or JSON)
- [ ] Implement PID output slew-rate limiter; determine max Amps/sec empirically (fast enough for intended Rate segments, slow enough to smooth startup transients)
- [ ] Implement run-out trip on thermocouple dT/dt; calibrate threshold from observed peak dT/dt during slew-limited test runs
- [ ] Watch for detached-TC failure modes that present as noise/erratic readings rather than fast sustained rate changes — may need a plausibility check in addition to the dT/dt trip if this turns out to matter in practice
- [ ] Implement open-loop cooling behavior for cooling-direction Rate segments in the actual execution engine (coil off, gate advancement on actual temp reaching target, not on a ramped setpoint) — `Generate Preview Curve.vi` handles this for the *preview* math only, the real-time execution engine still needs it
- [ ] Implement Pause (freeze timers, hold setpoint at actual temp at time of pause) per the confirmed semantics above — deferred until Loop 2's execution engine exists to pause against
- [ ] Implement data logging (timestamp + TC temp, one row per loop tick, per-run file)
- [ ] Wire STOP (CTB1:4-5) through the fail-safe SSR per [[Design/Wiring/NI-DAQ Control Architecture]] — this is now required (not optional) since the Stop control depends on it
- [ ] Decide whether to cross-check a negative-`rate` row's `Temp` is actually below the running temp (catch a contradictory sign/temp combination) — not yet built

## See also
- [[Design/Wiring/NI-DAQ Control Architecture]] — PID loop, calibration curve, Amps→Volts conversion this feeds into
- [[Design/Wiring/Electrical System]] — HOTSHOT CTB1 pinout and hardware context
- [[Design/Wiring/Ball Screw Motor Control]] — ST-PMC1 RUN trigger (USB-6009 P0.0 → LCD4075DD3 SSR) and the onboard lower/submerge quench sequence the Quench action hands off to
