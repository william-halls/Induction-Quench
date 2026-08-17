---
tags: [index, navigation, project-overview]
---

# Induction Quench Vault

**Welcome to the Induction Quench Research Project documentation hub.**

This vault contains the complete design, engineering notes, and development history for an induction quenching research instrument collaboration with **Dr. Buchely**.

---

## 🎯 Quick Start

Start here: **[[Induction Quench Research Instrument|Project Overview]]**

This central hub links to all major subsystems and provides a complete project map. For the full cross-subsystem architecture diagram and dependency map, see **[[Design/SUBSYSTEMS|Subsystems Interconnection Map]]**.

---

## 📁 Vault Organization

The vault is organized by **engineering subsystems**, each with its own design notes, iterations, and CAD references.

### Thermal & Containment Systems

| Subsystem | Purpose | Status |
|-----------|---------|--------|
| **[[Design/Coil Geometry/Induction Coil\|Coil Geometry]]** | Induction heating coil optimization | 🟢 Active |
| **[[Design/Vacuum Chamber/Vacuum Enclosure\|Vacuum Chamber]]** | Inert atmosphere enclosure | 🟢 Active |

### Delivery & Control Systems

| Subsystem | Purpose | Status |
|-----------|---------|--------|
| **[[Design/Plumbing/Fluid Systems\|Plumbing & Fluid Systems]]** | Gas, vacuum, and quench medium distribution | 🟡 In Progress |
| **[[Design/Wiring/Electrical System\|Wiring & Electrical]]** | Power delivery and instrumentation | 🟡 In Progress |

### Functional Integration

| Subsystem | Purpose | Status |
|-----------|---------|--------|
| **[[Design/Sample Quenching/Quenching Methods\|Sample Quenching Routes]]** | Rapid cooling strategies | 🟡 In Progress |
| **[[Design/Mechanisms/Control System\|Mechanisms & Automation]]** | Actuation, control, and safety systems | 🟡 In Progress |

### Reference & History

| Subsystem | Purpose |
|-----------|---------|
| **[[Design/Archive/Design History\|Design Archive]]** | Historical designs, rejected concepts, lessons learned |

---

## 🗂️ File Structure

```
Induction Quench Vault/
├── CLAUDE.md (auto-loaded session startup instructions)
├── README.md (this file)
├── Induction Quench Research Instrument.md (main project hub)
│
├── Claude/
│   ├── AGENT_PREFERENCES.md (agent behavioral conventions)
│   ├── GRUNT.md (specs for cheap model execution)
│   ├── PUSHING_TO_GITHUB.md (push workflow)
│   ├── PULLING_FROM_GITHUB.md (pull workflow)
│   └── PUSH_LOG.md (commit history)
│
├── Design/
│   ├── SUBSYSTEMS.md (interconnection map & architecture diagram)
│   │
│   ├── Coil Geometry/ (+ INDEX.md)
│   │   ├── Induction Coil.md (overview & integration)
│   │   ├── Round Coil.md (active design)
│   │   ├── Square Coil.md (archived)
│   │   └── Coil Feature Script.md (parametric tools)
│   │
│   ├── Vacuum Chamber/ (+ INDEX.md)
│   │   ├── Vacuum Enclosure.md (overview)
│   │   ├── Used Vacuum Chamber.md (current design)
│   │   ├── Vacuum Chamber CAD.md (OnShape model)
│   │   └── Quartz Glass Tube.md (archived)
│   │
│   ├── Mechanisms/ (+ INDEX.md)
│   │   ├── Control System.md (overview & integration)
│   │   ├── Ceramic Mount.md (active design)
│   │   ├── Ball Screw.md (vertical actuation)
│   │   ├── Bottom Lift.md (rejected)
│   │   ├── Titanium Claw.md (rejected)
│   │   └── Trapdoor.md (deferred)
│   │
│   ├── Plumbing/ (+ INDEX.md)
│   │   ├── Fluid Systems.md (overview)
│   │   ├── Vacuum Lid Systems.md (overview & planning)
│   │   ├── Air System Control Assembly.md
│   │   ├── Coil Lead Pass-Throughs.md
│   │   ├── Thermal Couple Pass-through.md
│   │   ├── Vertical Sliding Shaft Seal.md
│   │   └── Random Holes.md
│   │
│   ├── Sample Quenching/ (+ INDEX.md)
│   │   ├── Quenching Methods.md (overview)
│   │   ├── Charpy.md (standard reference geometry)
│   │   └── Modified Charpy.md (test variant)
│   │
│   ├── Wiring/ (+ INDEX.md)
│   │   ├── Electrical System.md (power & instrumentation overview)
│   │   └── NI-DAQ Control Architecture.md (automated control system)
│   │
│   └── Archive/ (+ INDEX.md)
│       └── Design History.md (project evolution)
│
└── Media/
    ├── INDEX.md (image asset catalog)
    ├── coil/ (induction heating elements)
    ├── mechanisms/ (sample mounts, actuation, ceramic mount)
    ├── samples/ (test sample geometries - Charpy variants)
    ├── plumbing/ (fluid systems, pass-throughs)
    └── vacuum-chamber/ (chamber assembly, enclosure)
```

---

## 📋 How to Use This Vault

### Finding Information

1. **Project Overview** — Start at [[Induction Quench Research Instrument|the main hub]]
2. **By Subsystem** — Browse tables above or use the Design folder structure
3. **By Status** — Look for 🟢 active, 🟡 in-progress, or 🔴 archived items
4. **Search** — Use Obsidian's search (Ctrl+F) to find topics

### Navigation

- **Wikilinks** — Click `[[links]]` to jump between related notes
- **Breadcrumbs** — Files include "Integration Points" showing connections
- **Backlinks** — Obsidian's backlink panel shows which notes reference each file

### Contributing

- Add metadata to new files using the **YAML frontmatter** format (see example below)
- Link related notes using root-relative wikilinks: `[[Design/Subsystem/Note Name]]`
- Update the project hub when adding major new sections
- Archive rejected designs in the Design/Archive folder

---

## 🏷️ Metadata & Tags

All files include YAML frontmatter with tags for categorization:

```yaml
---
tags: [design, coil, induction-heating, electromagnetic]
---
```

**Common tags:**
- `design` — Design documentation
- `active`, `archived`, `rejected` — Design status
- `cad` — CAD models and references
- `mechanism`, `mechanism-*` — Mechanical subsystems
- `current-design` — Primary active design
- `notes` — Engineering notes, considerations

Use tags to organize and filter your vault queries.

---

## 📊 Key Specifications

| Spec | Target |
|------|--------|
| **Max Temperature** | 1000°C |
| **Sample Geometry** | Charpy-shaped (10×10×55 mm) |
| **Atmosphere** | Inert (vacuum/argon/nitrogen) |
| **Heating Method** | Induction coil (~1 MHz) |
| **Quench Method** | TBD (oil/water/gas options) |
| **Data Recording** | Thermocouple thermal profiles |

---

## 🔗 External References

- **OnShape CAD Hub**: https://cad.onshape.com/documents/c9bb59bfc991b1182ce6c971/
- **Collaborator**: Dr. Buchely

---

## ✅ Vault Maintenance

This vault was last **reorganized on 2026-08-17** with:

✓ Reorganized media folder — moved ceramic mount images to `media/mechanisms/`  
✓ Created dedicated `media/samples/` folder for Charpy test geometries  
✓ Updated media/INDEX.md with correct folder structure and all assets  
✓ Added CLAUDE.md bootstrap file with startup instructions  
✓ Added Claude/ folder with agent preferences and workflow documentation  
✓ Updated file structure tree and documentation  

**Previous maintenance (2026-08-07):**
✓ Flattened nested folder structure  
✓ Added frontmatter to all files  
✓ Standardized wikilinks to root-relative format  
✓ Fixed naming inconsistencies (Seel → Seal)  
✓ Linked the orphaned Subsystems Interconnection Map into the main hub & README  
✓ Moved loose pasted screenshots into `Media/mechanisms/` with descriptive names  
✓ Corrected file-structure references (e.g. `Charpy.md`) and stale file counts  

---

**Last Updated:** 2026-08-17  
**Status:** 🟢 Organized & Ready  
**Files:** 44 total (3 root + 5 Claude/ + 35 Design/ + 1 media/)
