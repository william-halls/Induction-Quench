---
subsystem: wiring
tags: [wiring, control-system, NI-DAQ, software, PID, heat-curve, annealing, safety]
related: [[Design/Wiring/NI-DAQ Control Architecture.md]]
status: in progress — execution engine v2 (time-based lookup) built and bench-debugged in LabVIEW, full end-to-end test with real TC/HOTSHOT still pending (2026-10-10)
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

## Execution Engine v2 — Time-Based Lookup Architecture (built & bench-debugged 2026-10-09/10, supersedes the tick-accumulator approach and the pseudocode above)

**Built in LabVIEW, not just designed.** The Section 3 pseudocode above was the original plan; what actually got built went through two full iterations before landing here — first a tick-counted accumulator (`setpoint += rate × 0.1` per tick), which turned out to be loop-speed-dependent (wrong if Loop 2 doesn't actually tick at a true 100ms), then this time-based lookup redesign, which is immune to loop speed entirely. **Tolerance-based Hold is gone** — see the superseded decision below.

### Core idea: both `setpoint` and `current_segment_index` are now direct lookups against precomputed curve data, keyed on real elapsed time — not incremented/accumulated tick by tick.

**Precomputed once per `Update` button press** (in Loop 1, alongside the existing `Generate Preview Curve.vi` call), three parallel arrays, each read into Loop 2 via Local Variable:
- `PlannedTimes` / `PlannedTemps` — the existing XY preview-graph data (1 point per Rate segment, 2 points per Hold segment — doesn't map 1:1 to table rows).
- `SegmentEndTimes` — **new**, one value per Segment Table row, the cumulative planned completion time for that row (same per-row duration math as the preview curve, just accumulated differently). Used purely for the segment-index lookup, since `PlannedTimes` can't be indexed by row.

**Loop 2 shift registers (9 total):** `last_tick_ms`, `elapsed_run_time`, `prev_start`, `RunActive`, `current_setpoint`, `current_segment_index`, `TC_time`, `TC_temp`, `quench_fired`.

**Each tick:**
1. Measure **real** per-tick delta (`dt_real = (Tick Count(ms) − last_tick_ms) / 1000`), not an assumed `0.1` — this is what makes everything downstream loop-speed-independent.
2. `start_pressed` (Run button, edge-detected: `start_now AND Not(prev_start)`) resets all 9 state values to zero/empty and sets `RunActive = True`.
3. `elapsed_run_time` accumulates `dt_real` only when `RunActive` and not `Paused` (Pause is a **toggle**, Mechanical Action = Switch When Pressed, read directly — no edge detection needed). This single freeze point is what makes Pause hold the setpoint, freeze the wide-view graph, *and* stop `TC_time`/`TC_temp` logging all at once.
4. `current_segment_index` ← `Ceiling(Threshold 1D Array(SegmentEndTimes, elapsed_run_time))` — a direct time lookup, not an increment. No more `advance` boolean, no more per-segment Rate/Hold state machine.
5. `setpoint`, per `Type` of the active segment:
   - **Rate** (heating *or* cooling — no direction branch needed, see below): `Interpolate 1D Array(PlannedTemps, Threshold 1D Array(PlannedTimes, elapsed_run_time))`.
   - **Hold**: `setpoint = Temp` directly, no ramp.
   - **Placeholder (Type=0)**: `setpoint = 0` (defensive fallback, shouldn't occur post-`Clean`).
6. `TC_time`/`TC_temp` — append `(elapsed_run_time, actual TC reading)` each tick, same Pause-freeze gating as step 3.
7. PID Advanced.vi: SP = `current_setpoint`, PV = live TC reading, **dt = `dt_real`** (not a hardcoded constant — same loop-speed-independence reasoning), Output Range [8, 550] (not [0,550] — matches the conversion subVI's real floor), `reinitialize? = start_pressed`.

### Heating/cooling unification (2026-10-10) — no direction branch needed

Originally planned to branch Case 1 (Rate) on `Sign(rate)` — heating via lookup, cooling hardcoded to `setpoint = 0`. **Dropped**: since the coil is heating-only (no active cooling actuator), a persistently-low setpoint during a cooling segment makes the PID naturally saturate near the Output Range floor anyway (8A does negligible real heating per the bench calibration curve) — same practical result as hardcoding 0, with no special case needed. `Generate Preview Curve.vi`'s planned-curve data already slopes downward for a cooling segment (negative rate), so the *same* lookup produces correct behavior for both directions. **Known tradeoff accepted**: cooling is physics-limited/not enforceable, so if real cooldown outpaces the planned curve, the looked-up setpoint could briefly sit above actual temp and cause the coil to kick back on mid-cooldown — judged acceptable since it's the same category of approximation already accepted for Hold (see below).

### Hold — tolerance dropped entirely (2026-10-10, supersedes the Hold-band-excursion decision below)

No more tolerance check, no in-band flag, no reset-on-excursion. `Hold`'s `advance` (now subsumed into the time-based index lookup above, not a separate boolean) is purely `elapsed_run_time ≥` that row's `SegmentEndTimes` entry — a fixed duration, full stop. This removed a whole class of bug (the original `in_band_elapsed`/tolerance machinery) and the `Tolerance` global control is no longer used by the execution engine.

### Quench/End — index-overflow check doesn't work; must compare against final planned time

**Bug found and fixed 2026-10-10**: originally planned `past_end = current_segment_index ≥ Array Size(Segment Table)`. This never fires — `Threshold 1D Array` clamps to the array's last valid index once the threshold exceeds every entry, so the index can never actually reach `Array Size`. **Fix**: compare `elapsed_run_time` directly against the **last value in `SegmentEndTimes`** instead (`Index Array` at `Array Size(SegmentEndTimes) − 1`), latched through a `quench_fired` shift register so the actual Quench action (0V direct write, bypassing the Amps→Volts conversion subVI, plus a `quench_trigger`-driven write into the `Quench` button's Local Variable to fire the same sequence Loop 3/motion already handles) only fires once.

### Bugs found during bring-up (2026-10-10) — worth remembering the patterns, not just the fixes

Several real bugs this session, all variations on **"a Case structure branch passes the stale/old value through instead of the freshly computed one"**:
- `current_setpoint`'s advance-logic False branch was wired from the old shift-register value instead of the Type-case-structure's fresh `setpoint` output — stuck at 0 until found.
- `past_end`'s True/False branches were inverted (the real segment-lookup logic was effectively being skipped).
- `TC_time`/`TC_temp` indicator terminals were placed **outside** the While Loop border — a classic LabVIEW trap: an indicator outside a loop only updates once, after the loop exits, so it looked permanently stuck empty even though Highlight Execution showed the underlying computation working correctly the whole time. Fix: move indicator terminals inside the loop.

**Takeaway for future debugging on this VI**: when a value looks "stuck," check (in order) — is the Case structure branch wired to the fresh computation or a stale pass-through; is the shift register's right terminal actually reached (not dead-ending at just an indicator); is the indicator terminal itself inside the loop border.

## Decision: Hold-band excursion behavior — RESET (confirmed 2026-09-06, superseded 2026-10-10 — tolerance-based Hold dropped entirely, see "Execution Engine v2" above)

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

## Pause & Stop Behavior (confirmed 2026-09-07, Pause semantics revised 2026-10-10, Stop software half built 2026-10-12)

Two distinct controls, not to be confused:

**Pause** — holds the run in place without aborting it.
- Freezes segment progression (`current_segment_index`, all elapsed/dwell timers stop advancing).
- **Revised 2026-10-10, as actually built in Execution Engine v2**: setpoint is held at **whatever `current_setpoint` already was at the moment of pause** (frozen in place, since `elapsed_run_time` itself stops advancing while paused — see "Execution Engine v2" above) — **not** re-snapped to the live actual thermocouple reading as originally specified below. The distinction matters if the sample is drifting away from the setpoint right as Pause is pressed: the original spec would have the PID immediately accept wherever the TC happens to be as the new target (zero error at the instant of pause); what's actually built keeps chasing the pre-pause target instead. Functionally similar in most cases (pause is usually pressed near steady-state), but worth knowing this is the real behavior if a pause is ever triggered mid-transient.
- Resume continues from where it left off (`elapsed_run_time` resumes accumulating, curve lookup picks back up naturally).

**Stop** — a real abort, not a pause.
- **Software half confirmed built (2026-10-12)**: forces the PID's output (Amps, pre-conversion) to literal `0`. The existing Amps→Volts conversion subVI has its own dedicated branch for an input of exactly `0` that outputs genuine 0V directly (distinct from its general 8-550A clamping behavior for nonzero inputs, which floors anything below 8A up to the 8A-equivalent voltage — confirmed this does *not* apply to a literal 0 input). Not yet bench-verified with a multimeter on the actual 9269 output terminal — recommended before trusting this live.
- **Hardware half still open**: per the original design, Stop should *also* open the **STOP SSR (CTB1:4-5)** per [[Design/Wiring/NI-DAQ Control Architecture#START / STOP (CTB1:3-5) — still applies regardless of AO source|NI-DAQ Control Architecture]] at the same time as the software zeroing — deliberate belt-and-suspenders redundancy, so either mechanism alone halts heating if the other fails. **This SSR wiring is not yet connected to the new Stop button** — see Open Items.
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

**Done (2026-10-09/10):**
- [x] Loop architecture revised to 4 loops: Loop 1 (GUI/table + `Update` button), Loop 2 (PID/execution engine, plain polling While Loop — no Event Structure, uses edge-detection instead), Loop 3 (motion, separate due to DAQmx-task-recreation slowness discovered when sharing a loop with PID), Loop 4 (reads `PlannedTimes`/`PlannedTemps`/`TC_time`/`TC_temp` via Local Variable, feeds the wide-view graph)
- [x] Execution engine v2 built — time-based lookup architecture, see "Execution Engine v2" section above (supersedes the Section 3 pseudocode and the original Timeout/Event-Structure plan)
- [x] `current_segment_index` and `setpoint` both now pure functions of `elapsed_run_time` (via `Threshold 1D Array` + `Interpolate 1D Array`/`Ceiling`), immune to loop-speed variation
- [x] Tolerance-based Hold dropped entirely — pure duration-based now
- [x] Heating/cooling direction unified — no Sign/branch needed, same curve lookup for both
- [x] Run (edge-detected, full state reset) and Pause (toggle, Switch When Pressed, freezes setpoint/elapsed-time/TC-logging/wide-view-graph while leaving the live TC chart and PID's own tick-to-tick math running) built and bench-tested
- [x] Quench/End trigger fixed — compares `elapsed_run_time` against `SegmentEndTimes`' last entry, not an index-vs-Array-Size check (which can't fire, `Threshold 1D Array` clamps at the array's last index)
- [x] AD8495 CJC amp moved off the 6009's shared +5V onto a dedicated 12V supply (see [[Design/Wiring/NI-DAQ Control Architecture]])
- [x] Cubic correction polynomial for AD8495/9229 thermocouple reading, loaded directly into the 9229 channel's DAQmx Custom Scale (composite cubic in terms of raw volts) — see [[Design/Wiring/NI-DAQ Control Architecture]]

**Still open:**
- [ ] Re-verify `Move Up`/`Move Down` against the Local-Variable read/write pattern (built before the shift-register-staleness bug was found and fixed in Add/Clean — may have the same bug)
- [ ] Wire the Quench action's P0.0 pulse / `Quench` control Local-Variable write into whatever Loop 3 already does for ST-PMC1 RUN triggering — mechanism decided (write `quench_trigger` into the `Quench` button's Local Variable, simulating a manual press) but not yet confirmed Loop 3 handles it correctly end-to-end
- [ ] Wire `Home`'s IN1+IN2 output per the finalized Ball Screw I/O mapping — exact bit-combination for "Home" vs. Center/Top/Bottom not yet pulled from that doc into this implementation
- [ ] Decide save/load format for named profiles (suggest flattened XML or JSON)
- [ ] Implement PID output slew-rate limiter; determine max Amps/sec empirically (fast enough for intended Rate segments, slow enough to smooth startup transients)
- [ ] Implement run-out trip on thermocouple dT/dt; calibrate threshold from observed peak dT/dt during slew-limited test runs
- [ ] Watch for detached-TC failure modes that present as noise/erratic readings rather than fast sustained rate changes — may need a plausibility check in addition to the dT/dt trip if this turns out to matter in practice
- [ ] Implement data logging (timestamp + TC temp, one row per loop tick, per-run file) — note `TC_time`/`TC_temp` arrays already capture this live in-memory; still need to actually write them to a per-run file on disk
- [ ] Wire STOP (CTB1:4-5) through the fail-safe SSR per [[Design/Wiring/NI-DAQ Control Architecture]] — this is now required (not optional) since the Stop control depends on it
- [ ] Decide whether to cross-check a negative-`rate` row's `Temp` is actually below the running temp (catch a contradictory sign/temp combination) — not yet built
- [ ] Full end-to-end bench test with real TC + HOTSHOT now that the setpoint/index/Quench bugs are fixed — only partially smoke-tested so far

## Chamber Side-View Infographic: Tank + Slide Overlay (confirmed 2026-09-19)

A composite front-panel visualization showing a side-view cutaway of the vacuum chamber — water level, atmosphere type, and Charpy sample position all at a glance — built by layering two LabVIEW indicators rather than using separate gauges. This is distinct from the Segment Table/preview-graph GUI above; it's a standalone status display, likely living on the same front panel or a dedicated monitoring tab.

**Shared scale (both controls): 0–11in**, matching the vacuum chamber's actual internal height (`~11" diameter × 11" height`, confirmed in [[Design/Vacuum Chamber/Vacuum Enclosure|Vacuum Enclosure]]) — chosen deliberately so the infographic is geometrically accurate to the real hardware, not an arbitrary gauge range. Both controls must also be set to the same pixel height on the front panel — matching the numeric `Minimum`/`Maximum` alone isn't sufficient, since a height mismatch would make the two controls' markers drift apart visually even when their underlying values agree.

**Layer 1 — Tank (background: vessel + water body + atmosphere)**
- Fill value = estimated/current water level.
- Fill color driven by a Property Node off **`IsInert`** — a **boolean**, not an enum (revised 2026-09-19: atmosphere state simplified to "inert or not," since the only two states that matter are air vs. an inert gas — matches the vault's own atmosphere spec, "vacuum/argon/nitrogen," where any inert gas reads the same on this display). `False` (air) → light gray/white; `True` (inert) → blue/purple. Implemented as a Select or two-case Case Structure keyed on the boolean rather than an enum-keyed Case Structure.
- **Target water level (confirmed 2026-09-19): a horizontal line** overlaid on the Tank — a thin Decoration (flat rectangle) spanning the Tank's width, in a color that contrasts with the fill (e.g. a bright/high-contrast line color, not matching either the air or argon fill colors, so it stays visible against both). Its vertical position is recomputed each tick from `target_level` using the same 0–11in scale — the Tank control itself only natively fills to one value, so target-vs-actual requires this second element layered on top.
- **Numbers/scale labels live here** — the Tank is the only one of the two controls showing a visible scale.

**Layer 2 — Slide (foreground overlay: water-level line + Charpy position)**
- **A single Slide control with two sliders** (right-click the Slide → **Add Slider**), not two separate overlapping Slide controls — since both sliders live on the same control, they automatically share one scale and one exact pixel geometry, with no separate alignment step needed between them. (The whole control is still positioned in front of the Tank and aligned to it, per the z-order/scale-matching notes above — that alignment is between the Slide control and the Tank, not between two Slides.)
- Positioned **in front of** the Tank (front panel z-order), same 0–11in scale, same pixel height/position as the Tank.
- Track and background set fully transparent so the Tank's fill/color shows through underneath — **no visible numbers or scale on the Slide itself** (confirmed 2026-09-19); only the two slider thumbs are opaque.
- Two sliders sharing the one scale: Slider 0 marks current water level (redundant with the Tank's fill line by design — this is what makes the two controls read as one picture instead of two separate gauges, this slider acting as the visible "water surface line" drawn over the Tank's water body), Slider 1 marks Charpy sample position.
- On the block diagram, the control's terminal is a **cluster of the slider values** (not a single double) — Bundle/Unbundle by name to read or write each slider independently.
- Thumb color for the water-level and Rod/Charpy sliders also driven off the same `IsInert` boolean, from the same source variable as the Tank's fill color (not a separately-maintained copy) so nothing can visually disagree.
- Distinct thumb/fill shapes recommended so markers stay legible if they visually converge — notably **at the moment of quench**, when the Charpy segment and water-level marker meet.

**Rod + Charpy sliders — drawing the sample with real length (confirmed 2026-09-19)**

Rather than a single-point marker for the Charpy sample, **two additional sliders on the same Slide control** draw the sample as an actual filled segment with physical length, using LabVIEW's per-slider Fill styles:

- **Rod slider (Slide 3)**: Fill = *Fill to Max* — its filled bar spans from **its own value up to the top of the scale (11in)**. This represents the mechanical shaft/rod hanging down from the mechanism above the chamber. **Rod's value = the sample's top edge** — i.e. this slider is driven by the same Heat/Quench-state value previously defined as HeatPos/QuenchPos (9in fixed when heating, `TargetWaterLevel ÷ 2` when quenching — see below for the resulting adjustment to Quench's meaning).
- **Charpy slider**: Fill = *fill to the Rod slider's value* — its filled bar spans from **its own value up to wherever the Rod slider currently is**, drawing the physical body of the sample as a distinctly-colored segment directly below the rod's endpoint. **Charpy's value = the sample's bottom edge**, computed as:
  ```
  CharpySliderValue = RodSliderValue − SampleLength
  ```
  where `SampleLength` = the Charpy specimen's actual physical length, **55mm ≈ 2.17in** (`10mm × 10mm × 55mm`, per [[Design/Sample Quenching/Charpy|Charpy]]), assuming the specimen is held vertically along its long axis — consistent with the vertical raise/lower motion.
- Together, the two sliders draw one continuous column from the top of the scale down through the rod, then a visually distinct block for the sample itself — so both Heat Position (top-referenced, `RodValue = 9in`) and Quench Position (previously center-referenced, now reinterpreted as `RodValue = TargetWaterLevel ÷ 2 + SampleLength/2` so the sample's actual center still lands at the mid-water-column point) are represented as real, correctly-sized geometry rather than a single point.

**Connecting the Charpy marker to Heat Curve execution state (confirmed 2026-09-19, revised 2026-09-19):**

The ball screw runs standalone on the ST-PMC1 (Option A, per [[Design/Wiring/NI-DAQ Control Architecture|NI-DAQ Control Architecture]]) and is open-loop — there's no encoder or live position feedback into the DAQ/Heat Curve software to drive a continuously-tracked real height. Instead, the Charpy marker is driven by **two named states, tied to the Heat Curve execution engine's own state (Section 3), not to real position sensing**. The two states use **different reference points on the sample and different bases** — this asymmetry is intentional, not an inconsistency, so it's spelled out explicitly below:

- **Quench Position = TargetWaterLevel ÷ 2**, referencing the **center** of the Charpy sample. Uses the **target/set** water level (the configured fill setpoint shown by the Tank's target-level marker), not the Tank's estimated/actual-current water level — the sample is centered against where the water is *supposed to* end up, not wherever the current reading happens to be mid-fill. Example: at TargetWaterLevel = 4in, Quench Position (sample center) = 2in. Set the instant Quench fires (`curve_complete = True` / the Quench Action sequence in Section 3).
- **Heat Position = ChamberTop − 2in**, referencing the **top edge** of the Charpy sample — a **fixed constant anchored to the chamber's top**, independent of water level entirely (does not move if WaterLevel changes). On the 0–11in scale, this is `11 − 2 = 9in` for the sample's top edge. Active whenever a Rate/Hold segment is running (sample is up in the coil).
- The marker **toggles** between these two states in sync with execution state (heating/holding vs. Quench-fired) — it does not interpolate smoothly between them, since there's no real position data to animate against; the ~20ms actual raise/lower motion (per [[Design/Mechanisms/Ball Screw Motor Control|Ball Screw Motor Control]]'s "Control Sequence Example: Sample Lift & Quench") is fast enough that a snap-transition is an accurate representation, not a simplification.

**Open / worth watching once built:**
- Because Heat Position references the sample's **top** and Quench Position references its **center**, the Charpy marker needs to be drawn with its actual physical length on screen (not a single point) so both reference points place it correctly — a single-point thumb can't honor both conventions at once.

## See also
- [[Design/Wiring/NI-DAQ Control Architecture]] — PID loop, calibration curve, Amps→Volts conversion this feeds into
- [[Design/Wiring/Electrical System]] — HOTSHOT CTB1 pinout and hardware context
- [[Design/Wiring/Ball Screw Motor Control]] — ST-PMC1 RUN trigger (USB-6009 P0.0 → LCD4075DD3 SSR) and the onboard lower/submerge quench sequence the Quench action hands off to
- [[Design/Vacuum Chamber/Vacuum Enclosure]] — chamber envelope dimensions (11" × 11") used as the shared scale for the Tank+Slide infographic above
