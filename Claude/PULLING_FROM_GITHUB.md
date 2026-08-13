# Pulling This Vault on Another PC/Laptop

This vault is a git repo connected to:
`https://github.com/william-halls/Induction-Quench-`

## First time on a new PC/laptop

1. Install [Git](https://git-scm.com/downloads) if it isn't already.
2. Open a terminal in your `Obsidian Vault` folder (or wherever you keep vaults) and run:
   ```
   git clone https://github.com/william-halls/Induction-Quench-.git "Induction Quench"
   ```
3. Open the resulting `Induction Quench` folder as a vault in Obsidian.
4. Install the **Claudian** community plugin in Obsidian (Settings → Community plugins) if it isn't bundled — the plugin files live in `.obsidian/plugins/claudian` and should already be there from the clone, but you may need to enable it under Community Plugins.

## Every time you sit down to work (on any machine)

Before making changes, pull the latest:
```
git pull
```

This brings in anything pushed from your other PC/laptop, so you never work from a stale copy.

## Every time you want to save/sync changes

```
git add -A
git commit -m "short description of what changed"
git push
```

## If you forget to pull first and get a conflict

- `git status` — see what's conflicting
- Obsidian markdown files conflict rarely if you're not editing the same note on two machines at once
- If you do get a merge conflict, open the flagged file(s) — git marks the conflicting sections with `<<<<<<<`, `=======`, `>>>>>>>` — resolve manually, then `git add` the file and `git commit`

## Notes

- `.gitignore` excludes `.claude/` and `.claudian/` (local app state/session data) — these are NOT synced between machines and will be recreated locally as needed.
- If you rename/move the vault folder, the git repo moves with it.
