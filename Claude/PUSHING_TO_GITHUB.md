# Pushing This Vault to GitHub

This vault is a git repo connected to:
`https://github.com/william-halls/Induction-Quench-`

## One-time setup (already done)

- `git init` — made this folder a git repo
- `git config user.name` / `user.email` — set to identify commits
- `.gitignore` — excludes `.claude/`, `.claudian/` (local app state/session data, not vault content)
- `git remote add origin https://github.com/william-halls/Induction-Quench-.git` — linked to GitHub

## Every time you want to push new changes

Open a terminal in this folder (`Induction Quench/`) and run:

```
git add -A
git commit -m "short description of what changed"
git push
```

- `git add -A` stages all new/changed/deleted files (respecting `.gitignore`)
- `git commit -m "..."` saves a snapshot with a message describing the change
- `git push` uploads it to GitHub

## Useful checks

- `git status` — see what's changed since your last commit
- `git log --oneline` — see commit history
- `git diff` — see exact line changes before committing

## Notes

- Anything in `.gitignore` (currently `.claude/` and `.claudian/`) will never be pushed, even with `git add -A`.
- If you rename/move the vault folder, the git repo moves with it — no need to redo setup.
- To change the GitHub repo's visibility (public/private) or delete it, do that from the repo's **Settings** page on github.com — not from git itself.
