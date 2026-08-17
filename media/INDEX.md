---
tags: [index, media, images, assets]
---

# Media Assets

Organized repository of all images, diagrams, and visual references for the Induction Quench project.

## Folder Structure

```
media/
├── coil/                      # Induction coil designs & geometry
├── mechanisms/                # Sample mounting & actuation
├── vacuum-chamber/            # Chamber assembly & overview
└── plumbing/                  # Fluid systems, samples, components
```

---

## Coil Designs

| Image | File | Reference |
|-------|------|-----------|
| **Round Coil Geometry** | `round-coil-geometry.png` | [[Design/Coil Geometry/Round Coil\|Round Coil.md]] |

---

## Mechanisms & Sample Mounts

| Image | File | Reference |
|-------|------|-----------|
| **Bottom Lift Design** | `bottom-lift-design.png` | [[Design/Mechanisms/Bottom Lift\|Bottom Lift.md]] |
| **Titanium Claw V1** | `titanium-claw-v1.png` | [[Design/Mechanisms/Titanium Claw\|Titanium Claw.md]] |
| **Titanium Claw V2** | `titanium-claw-v2.png` | [[Design/Mechanisms/Titanium Claw\|Titanium Claw.md]] |
| **Titanium Claw V3 (Final)** | `titanium-claw-v3-final.png` | [[Design/Mechanisms/Titanium Claw\|Titanium Claw.md]] |
| **Ceramic Mount Assembly** | `ceramic-mount-assembly.png` | [[Design/Mechanisms/Ceramic Mount\|Ceramic Mount.md]] |
| **Ceramic Mount Detail** | `ceramic-mount-detail.png` | [[Design/Mechanisms/Ceramic Mount\|Ceramic Mount.md]] |
| **Ceramic Mount Rendering** | `ceramic-mount-rendering.png` | [[Design/Mechanisms/Ceramic Mount\|Ceramic Mount.md]] |
| **Ball Screw Stepper Assembly** | `ball-screw-stepper-assembly.png` | [[Design/Mechanisms/Ball Screw\|Ball Screw.md]] |
| **Shaft Clamp Mount** | `shaft-clamp-mount.png` | [[Design/Mechanisms/Ball Screw\|Ball Screw.md]] |
| **Ball Screw Full Assembly** | `ball-screw-full-assembly.png` | [[Design/Mechanisms/Ball Screw\|Ball Screw.md]] |

---

## Vacuum Chamber

| Image | File | Reference |
|-------|------|-----------|
| **Chamber CAD Overview** | `chamber-cad-overview.png` | [[Design/Vacuum Chamber/Vacuum Chamber CAD\|Vacuum Chamber CAD.md]] |
| **Chamber Assembly** | `chamber-assembly.png` | [[Design/Vacuum Chamber/Vacuum Chamber CAD\|Vacuum Chamber CAD.md]] |
| **Used Vacuum Chamber** | `used-vacuum-chamber.png` | [[Design/Vacuum Chamber/Used Vacuum Chamber\|Used Vacuum Chamber.md]] |

---

## Plumbing & Components

| Image | File | Reference |
|-------|------|-----------|
| **Charpy Standard Sample** | `charpy-standard-sample.png` | [[Design/Sample Quenching/Charpy\|Charpy.md]] |
| **Charpy Standard Reference** | `charpy-standard-reference.png` | [[Design/Sample Quenching/Charpy\|Charpy.md]] |
| **Charpy Modified Sample (button head, superseded)** | `charpy-modified-sample.png` | [[Design/Sample Quenching/Modified Charpy\|Modified Charpy.md]] |
| **Charpy Modified Sample (T-slot head, current)** | `charpy-modified-tslot.png` | [[Design/Sample Quenching/Modified Charpy\|Modified Charpy.md]] |
| **Air Control Assembly** | `air-control-assembly.png` | [[Design/Plumbing/Air System Control Assembly\|Air System Control Assembly.md]] |
| **Coil Lead Pass-Through 1** | `coil-lead-passthrough-1.png` | [[Design/Plumbing/Coil Lead Pass-Throughs\|Coil Lead Pass-Throughs.md]] |
| **Coil Lead Pass-Through 2** | `coil-lead-passthrough-2.png` | [[Design/Plumbing/Coil Lead Pass-Throughs\|Coil Lead Pass-Throughs.md]] |

---

## Image Naming Convention

All images follow a descriptive naming pattern:

- **Lowercase** with hyphens (not underscores or spaces)
- **Descriptive** — Indicates content at a glance
- **Version-aware** — V1, V2, V3, etc. for iterations
- **Searchable** — Keywords for finding related images

**Examples:**
- ✓ `round-coil-geometry.png`
- ✓ `titanium-claw-v2.png`
- ✓ `chamber-cad-overview.png`

---

## How to Add New Images

1. **Determine category** — coil, mechanisms, vacuum-chamber, or plumbing
2. **Rename descriptively** — e.g., `new-feature-diagram.png`
3. **Place in appropriate folder** — e.g., `media/mechanisms/new-feature-diagram.png`
4. **Update markdown** — Reference as `![[image-name.png]]`
5. **Update this INDEX** — Add entry in appropriate table

---

## Image Usage in Markdown

**Example:** From a file in `Design/Mechanisms/Example.md`:

```markdown
![[ceramic-mount-assembly.png]]
```

Obsidian resolves embeds by filename (unique across the vault), so a bare filename works from any markdown file — no relative path needed.

---

*All images organized for easy discovery and consistent referencing across the vault.*
