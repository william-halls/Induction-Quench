# Push Log

Record of what changed with each push to GitHub.

<!-- Newest entries at the top -->

## 2026-09-06 - HOTSHOT calibration curve, PID architecture, heat curve/quench software design
**Commit**: `99c1cc6`
- [[Design/Wiring/NI-DAQ Control Architecture.md]]: added the bench-measured HOTSHOT Setpoint Calibration Curve (Control From set to Rear Panel, NI-9269 → CTB1:1-2) — confirmed ~1.10V turn-on threshold, linear region `Amps ≈ 137×V − 151` from ~1.10V–5.1V, hard saturation ceiling at 550A from ~5.1V to 10V (likely the Icap/tap-cap limit); resolves the long-open "exact scaling/linearity still TBD" item. Also logged the PID software architecture decision: PID output runs in Amps (`PID Advanced.vi` Output Range 0–550 for correct anti-windup), Amps→Volts conversion done as a separate step after the PID block, and anything ≤50A treated as "off" (no meaningful heating below that regardless of electrical turn-on, so the non-smooth knee near 8-13A doesn't need to be modeled)
- [[Design/Wiring/Electrical System.md]]: updated the CTB1 pinout table and the 2026-09-05 "Resolved" note to point to the measured calibration curve instead of the prior "assume linear" placeholder
- New [[Design/Wiring/Heat Curve Profile Software.md]]: designed a multi-segment ramp/hold GUI + execution engine for programming anneal-style heat curves on top of the PID loop — Rate segments (ramp to a target temp at a set rate), Hold segments (maintain within a tolerance band for a duration, with dwell-timer **reset** on any out-of-band excursion — decided this way because the process is annealing and needs genuinely continuous time-at-temperature), and a terminal **Quench action** that cuts heating and fires the existing ST-PMC1 RUN trigger (USB-6009 P0.0 → LCD4075DD3 SSR, per [[Design/Wiring/Ball Screw Motor Control.md]]) to hand off to the ball screw's own lower/submerge quench sequence
- Commit also included pre-existing modified files from prior uncommitted sessions not part of this specific conversation (Ball Screw Motor Control, Ceramic Mount, Quenching Methods, several INDEX files, README, Obsidian workspace config, etc.) — see `git log`/diff for that commit if exact prior-session content needs auditing
- Files changed: 3 intentional (2 modified, 1 new) + pre-existing carryover from prior sessions

## 2026-09-05 - Ball screw MF/STOP wiring decisions
**Commit**: `82a5b80`
- [[Design/Wiring/Ball Screw Motor Control.md]]: decided TB6600 MF+/− will be DAQ-controlled via the spare LCD4075DD3-type SSR (motor-free/coil-disable for manual repositioning); decided ST-PMC1 STOP will be permanently jumpered closed rather than wired as a safety E-stop or SSR-controlled, since the ball screw stage doesn't present the hazard level that motivated the hardwired-E-stop approach elsewhere (HOTSHOT RF, vacuum chamber) — "stop everything" now relies on the existing AC power-release interlock instead
- Updated SSR inventory table (added MF row, added explicit STOP-not-SSR-controlled row) and USB-6009 digital line budget (MF added, STOP line freed up) to match
- Commit also included pre-existing untracked files from prior sessions not part of this change: Design/Wiring/Ambrell Contact - HOTSHOT 103927.md, Design/Wiring/HOTSHOT Manual - Full Transcription.md, Design/Wiring/TODO - HOTSHOT Docs & E-Stop-Wiring.md, and 2 media/wiring files
- 19 files changed total (1 intentional, 18 pre-existing untracked)

## 2026-08-17 - Add thermocouple pass-through part ID
**Commit**: `1c50812`
- [[Design/Plumbing/Thermal Couple Pass-through.md]]: added part ID PFT2NPT-1K to the current plan
- 1 file changed

## 2026-08-17 - Acrylic vacuum lid design, shaft/seal part numbers
**Commit**: `e257a4f`
- New [[Design/Vacuum Chamber/Acrylic Vacuum Lid.md]]: material/stock (11/16" cast acrylic), 3-bracket mounting with aluminum backing plates, hole pattern/edge-distance guidelines, acrylic drilling notes
- [[Design/Plumbing/Vertical Sliding Shaft Seal.md]]: added McMaster part numbers — 5154T48 lip seal, 8934K31 304 SS shaft (tolerance, finish, hardness, cost); noted 316/316L upgrade path if quenchant chemistry changes
- Cross-linked new lid note from Vacuum Enclosure.md and Vacuum Chamber INDEX.md
- 4 files changed

## 2026-08-17 - Coil pass-through redesign, T-slot sample geometry, CLAUDE.md bootstrap
**Commit**: `8b42bf6`
- Coil lead pass-throughs: bronze/brass flared fittings -> Omega SSLK-14-14 compression fittings (fixes excess resistive power loss); noted one-piece-lead trade-off; archived rejected design in Design/Archive/INDEX.md
- Modified charpy sample head: button head -> T-slot, for water-jet mass manufacturability; updated Ceramic Mount retaining slot to match
- Added CLAUDE.md at vault root (auto-read each new session) pointing to Claude/ folder conventions
- Added Claude/AGENT_PREFERENCES.md: auto-save helpful uploaded pictures into the vault
- Note: local git repo had to be re-initialized this session (`.git` was missing despite PUSHING_TO_GITHUB.md describing it as set up) and reconnected to the existing GitHub history — no data was lost
- 9 files changed

## 2026-08-07 - Add NI-DAQ control architecture and polish vault navigation/media
**Commit**: `3fb56e7`
- Added Design/Wiring/NI-DAQ Control Architecture.md, cross-linked from 7 subsystem notes
- Linked the orphaned Design/SUBSYSTEMS.md map into the main hub and README
- Moved loose pasted screenshots into media/mechanisms/ with descriptive names
- Fixed README file-structure tree, stale file count, and Media/INDEX embed guidance
- Added missing Ball Screw.md entry to Mechanisms INDEX
- 31 files changed

## 2026-08-07 - Update Obsidian workspace config and Design documentation
**Commit**: `43213f2`
- Updated Obsidian workspace layout and plugin configuration
- Refreshed Design documentation across multiple subsystems
- 11 files changed (Coil Geometry, Mechanisms, Plumbing, Sample Quenching, Vacuum Chamber)
