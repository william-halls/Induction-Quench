# Agent Preferences

Standing preferences for Claudian when working in this vault. Check this file when in doubt about default behavior.

<!-- Newest entries at the top -->

## Route vault edits to GRUNT when appropriate (2026-08-17)

When a vault edit task is identified:

- **Evaluate:** Does it require reasoning, design judgment, or interpretation? 
  - **YES** → Handle it yourself (expensive model). You think, plan, write, decide.
  - **NO** → Write a GRUNT spec and spawn a Haiku agent to execute it (see [[Claude/GRUNT.md]]).

- **GRUNT candidates:** Mechanical file operations, bulk edits with clear rules, formatting, re-indexing, file moves/renames, adding entries to indexes, creating notes from exact templates.

- **Keep for yourself:** Decisions about where things belong, writing design rationale, understanding cross-link implications, nuanced changes to existing content.

- **Spec format:** Use the template in [[Claude/GRUNT.md]] — be literal (quote exact strings), list every file, include context, no interpretation required.

- **After GRUNT:** Review the changes (`git diff`), verify correctness, then `git commit && git push` as needed.

---

## Save helpful uploaded pictures into the vault (2026-08-17)

When the user pastes/uploads a picture in chat (not already a vault file) and it's relevant to a design note being discussed or edited:

- Save the image into the appropriate `media/<subsystem>/` subfolder using a descriptive filename (matching the existing naming convention, e.g. `charpy-modified-tslot.png`, `coil-lead-passthrough-3.png`).
- Embed it in the relevant note(s) with `![[filename.png]]`, replacing or supplementing any stale/outdated picture already there.
- Do this proactively — don't wait to be asked each time — whenever an uploaded image would help document a design, decision, or change.
- If a note currently flags a picture as stale/outdated (e.g. "update once a new render is added"), check whether a newly uploaded image resolves that flag and update the note accordingly.
