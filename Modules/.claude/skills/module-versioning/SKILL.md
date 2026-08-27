---
name: module-versioning
description: "Set an Intent Architect module's version correctly via the Module Builder model property (never by hand-editing .imodspec), then propagate it to dependents (architecture templates, other modules). USE ONLY WHEN asked to set, release, publish, or bump a module to a specific, already-decided version string. DO NOT USE FOR deciding whether or when a task should bump a version, or which component to bump (see module-version-increment) — this skill only executes a version already supplied. REQUIRES the target version string supplied by the caller; it does not decide or validate what it should be."
argument-hint: "[new version, e.g. 1.3.0 or 1.3.0-pre.1]"
keywords: [version, versioning, release, imodspec, module settings]
template-id: Intent.ModuleBuilder.AI.Skills.Skills.ModuleVersioning_SkillMd_Agents
contentHash: EEB88287536B99E4A584FB3D841DC6E3CBE53BE37FA555BA7D1DF1646072CB42
---
# Skill: module-versioning

## The Core Trap

`.imodspec`'s `<version>` and `<dependencies>` are generated FROM the model on every Software
Factory run. Hand-editing them directly is silently reverted on the next run — or worse, left
subtly wrong in the meantime: a hand-edited `<dependencies>` block has been found tab-indented,
out of alphabetical order, and missing an entry that a proper regeneration restores. Treat any
`.imodspec` that looks hand-touched (odd indentation, entries out of order) as suspect and
re-derive it with a real regeneration rather than trusting what's on disk.

`<summary>`/`<description>` are model-driven too, but from a different source than you'd guess:
the *application's* own `description` setting (`get_application_settings` /
`update_application_settings`), not a Module Settings stereotype property. Set it there and
regenerate — never hand-edit these either.

`<tags>` and `<authors>` are the exceptions: they have **no modelled source at all** — no designer
field, no stereotype property, nothing in `get_application_settings`. An existing value for either
survives every regeneration untouched, and a module with a placeholder (e.g. a stale scaffold
`<authors>` value) keeps it no matter how many times it's regenerated for something else. These
are the only two fields in `.imodspec` that are safe — and necessary — to hand-edit directly (see
`module-docs-chore` for tag format/content and for copying the correct author from a sibling
module rather than inventing one).

## How to Set It

1. On the module's package, in the Module Builder designer:

   
   `pkg.ensureStereotype("Module Settings").setProperty("Version", "<supplied version>")`
   (or the designer UI) — never the `.imodspec` file. Use the version exactly as supplied.

2. Run the Software Factory to regenerate `.imodspec`'s `<version>`.
3. Confirm via `get_file_diffs` that only the version line changed. If the diff touches more than
   that (e.g. the `<dependencies>` block shifts), the file was very likely hand-edited out of sync
   with the model at some point — trust the regenerated result over whatever was previously on
   disk, don't try to preserve the old shape.

### The Downgrade Guard

The Software Factory silently refuses to regenerate `.imodspec` if the version you set compares as
*lower*, by semver precedence, than the `<version>` already on disk — `run_software_factory`
reports zero changes, no error or warning. A `-pre.#` suffix sorts *below* the same `X.Y.Z` with no
suffix. If a version-set script + regeneration reports nothing staged when you expected a diff,
suspect this before anything else. Workaround: temporarily hand-edit *only* the `<version>` line
down to a safe value (confirm via `git diff`/`git status` first that the file is uncommitted), then
reapply your intended version through the designer and regenerate forward.

## Propagating the Change

- **Architecture Templates** referencing this module — update the `Component Module`'s

  `Version` in `metadata.iatspec`, only once the new version is actually published (confirm via
  `search_available_modules`, never guess).

- **Other modules depending on it** — update their `<dependency id="..." version="...">` entry.
- **NuGet package alignment** — keep `.csproj` package versions in step with the module version

to avoid `NU1605` (see known-build-gotchas).

## After Setting the Version

Run `module-docs` to keep `release-notes.md` and other version-dependent documentation in sync —
this skill sets the version, it doesn't own the documentation that mirrors it.

## Verification Checklist

- [ ] Set via `Module Settings → Version`, never by hand-editing `.imodspec`
- [ ] Software Factory run; `.imodspec` confirmed to match, no stray diffs
- [ ] Dependents checked/updated if they pin this module's version
- [ ] `module-docs` run afterward to keep documentation in sync
