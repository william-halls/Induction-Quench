# GRUNT — Cheap Model Vault Executor

**GRUNT** is a cheaper, faster Claude model (Haiku) for executing vault edits based on specs from your thinking chat.

## When to use GRUNT

✅ **Good fits:**
- Mechanical file operations (create notes from template, move files, rename entries)
- Bulk edits with clear rules ("replace X with Y in these 5 notes")
- Formatting fixes, re-indexing, media organization
- Any task with a crystal-clear spec where ambiguity is zero

❌ **Don't use GRUNT for:**
- Decisions about vault structure or design
- Writing rationale or nuanced design notes
- Anything requiring understanding of your project's intent
- Tasks where GRUNT needs to interpret ambiguity

## Workflow

1. **In your thinking chat** (this one):
   - Analyze, plan, decide
   - Create a **spec** (see template below)

2. **Copy the spec**

3. **Open GRUNT** (new chat tab, ask Claude Code to spawn Haiku agent, or similar)
   - Paste the spec
   - GRUNT executes
   - You review and pull/push as needed

## Spec Template

Use this format when handing off to GRUNT. The clearer this is, the fewer mistakes.

```
# [Task Name]

## What to do
[1-3 sentence description of the action]

## Exact changes required

### File: [path/to/note.md]
- [Exact line/section to change]
- Replace: `[old text]`
- With: `[new text]`

### File: [path/to/note2.md]
- [Exact line/section to change]
- Replace: `[old text]`
- With: `[new text]`

[Repeat for each file]

## Files to create (if any)
- Path: `[path/to/new-note.md]`
- Content:
  ```
  [exact content or template to use]
  ```

## Do NOT
- [List anything ambiguous or requiring decision-making]
- [Any cleanup or refactoring not in the spec]
- [Any structural changes beyond what's listed]

## After GRUNT is done
- Review changes: `git diff`
- If correct: `git add -A && git commit -m "[message]" && git push`
```

## Example: A good GRUNT spec

```
# Add Ceramic Mount retaining slot refs to Charpy Head

## What to do
Update Design/SUBSYSTEMS/Charpy Sample Head.md to add cross-references to the new T-slot ceramic mount design.

## Exact changes required

### File: Design/SUBSYSTEMS/Charpy Sample Head.md
- Find the section "### Mounting System"
- Add this line after "Current design: button head cap screw mounted directly to ceramic"
- Add: `- See [[Design/SUBSYSTEMS/Ceramic Mount]] for retaining slot matching this T-slot pattern`

## Do NOT
- Rewrite any existing sections
- Add new subsections
- Change anything else in the file
```

## Example: A bad GRUNT spec ❌

```
# Update the coil references

## What to do
Fix all the old coil info and update it

[Way too vague — GRUNT can't know what "old coil info" means or what the new info should be]
```

## Tips for writing GRUNT specs

- **Be literal.** Quote exact strings you want replaced.
- **List every file.** Don't say "and similar files" — name them all.
- **Include context.** Paste the surrounding lines so GRUNT can find the right place to edit.
- **No interpretation.** If GRUNT has to guess, the spec isn't ready.
- **Test mentally.** Imagine GRUNT executing it step-by-step. Does it work?

## Model choice

GRUNT uses **Claude Haiku** because it:
- Executes clear specs reliably (~95% accuracy on mechanical edits)
- Is 10–20x cheaper than Opus/Sonnet
- Responds in seconds
- Struggles with ambiguity (so your spec must be crystal-clear)

If a task is borderline or your spec feels complex, use your thinking chat instead — it's worth the cost.

---

**Created 2026-08-17** — After testing this workflow, add notes here about what works and what doesn't.
