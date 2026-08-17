# Graph View Color Scheme

Your vault files are now color-coded in Obsidian's graph view by subsystem. This makes it easy to see your project structure at a glance.

## Color Mapping

| Color | Subsystem | Files | Hex Code |
|-------|-----------|-------|----------|
| 🔴 Red | Coil Geometry | 5 files | `#FF5050` |
| 🟠 Orange | Sample Quenching | 4 files | `#FFA550` |
| 🟡 Yellow | Mechanisms | 8 files | `#FFC850` |
| 🟢 Green | Plumbing | 8 files | `#78C878` |
| 🔵 Blue | Wiring | 3 files | `#6496FF` |
| 🟣 Purple | Vacuum Chamber | 5 files | `#C878FF` |
| 🟦 Cyan | Claude (project docs) | 5 files | `#64C8DC` |
| ⚪ Gray | Design hub | Hub files | `#B4B4B4` |

## How It Works

- **Graph View**: Open the graph view (Obsidian → Graph View). Files now appear with their subsystem colors.
- **Frontmatter**: Each file has a `subsystem:` field in its frontmatter that enables the coloring.
- **CSS Enhancement**: A custom CSS snippet (`graph-colors.css`) enhances node styling with shadows and text visibility.

## Usage

1. **Open Graph View** in Obsidian (Cmd/Ctrl + Shift + G)
2. **See the colors**: Nodes are grouped by subsystem and color-coded
3. **Click to navigate**: Click any node to jump to that file

## File Updates

All Design files and Claude folder files have been updated with the `subsystem:` frontmatter field. This enables the graph color grouping to work automatically.

---

**Updated**: 2026-08-17  
**Created by**: Claudian (Obsidian AI assistant)
