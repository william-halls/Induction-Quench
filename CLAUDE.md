# CLAUDE.md

This file is auto-loaded at the start of every new chat session in this vault. Read it first, then follow the instruction below before doing anything else.

## Startup Instruction

At the start of every new chat session, read every file in the **`Claude/`** folder (not `.claude/` — that's local app state, gitignored, unrelated) before responding to the user's first message:

- **[[Claude/AGENT_PREFERENCES.md]]** — standing behavioral preferences (e.g. auto-saving helpful uploaded pictures into the vault)
- **[[Claude/PUSHING_TO_GITHUB.md]]** — how/when to push vault changes to GitHub
- **[[Claude/PULLING_FROM_GITHUB.md]]** — how/when to pull vault changes from GitHub
- **[[Claude/PUSH_LOG.md]]** — history of what changed in each push, newest first

If new files are added to `Claude/` in the future, read those too — treat the whole folder as required session-start context, not just the files listed above.

## Why

This project accumulates working conventions and preferences over time (see `Claude/AGENT_PREFERENCES.md`). Reading the folder fresh each session keeps behavior consistent without relying on conversation memory carrying over between chats.
