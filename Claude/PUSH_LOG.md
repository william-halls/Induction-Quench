# Push Log

Record of what changed with each push to GitHub.

<!-- Newest entries at the top -->

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
