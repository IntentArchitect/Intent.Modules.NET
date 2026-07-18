---
name: module-wrap-up
description: >
  Final mandatory phase of a module build, after all increments pass. Bumps and aligns the
  version (imodspec + csproj + designer), applies the supportedClientVersions two-step rule,
  invokes module-docs, writes CONTEXT.md into the module project folder, clears the
  .module-builder build state, and confirms the Software Factory is clean. TRIGGER: every
  increment in the Attack Plan is verified and the module is ready to release.
keywords: [wrap-up, version-bump, supportedClientVersions, context, release, imodspec]
---

# Skill: module-wrap-up

## When to Use
Load this as the **final phase** once `module-increment-loop` reports every increment verified **and `module-auditor`'s Gate 2 has returned PASS** against the frozen `acceptance-spec.md`. Do not begin wrap-up on self-certified completion — the independent audit is the entry condition. Nothing here is optional — a module is not "done" until this phase completes.

The chain that leads here:
```
… → module-increment-loop → module-wrap-up
```

---

## The Wrap-up Sequence (in order)

**Precondition — a *fresh* `module-auditor` Gate 2 run returns PASS.** Wrap-up does not start until a **new** independent audit run passes against the frozen `acceptance-spec.md` — **never** a stored or hand-edited `PASS` in `audit-findings.md`. If the latest verdict isn't a clean PASS from a fresh run, open findings go back through `module-increment-loop` (fix the module → re-invoke the auditor), not into wrap-up. The implementer never authors this verdict.

1. **Version bump** — assess impact and apply the rule (below); align imodspec + csproj + designer, reconciling against the `WORKING.md` version ledger.
2. **`supportedClientVersions`** — apply the two-step rule (below) and confirm it survives any rename.
3. **Docs** — invoke **`module-docs`** for `release-notes.md`, `docs/README.md`, and the `.imodspec` metadata fields, in the same turn.
4. **`CONTEXT.md`** — write/update it **inside the module project folder** (never at repo root).
5. **Clear build state** — delete/clear `.module-builder/WORKING.md`, `acceptance-spec.md`, `audit-findings.md`, and the per-module `.module-builder/<ModuleName>/` transitory files once the task is complete — **except `RETROSPECTIVE.md`** (append-only; the developer harvests it). Verify the folder is clean so the next task starts fresh.
6. **Confirm SF clean** — a final Software Factory run yields **zero staged changes**, verified *after* the regeneration baseline was reset (not a hand-craft echo).

---

## Version Bump

State the impact (patch / minor / major) and reasoning, then apply. The canonical bump table lives in **`module-docs`** → *Version Numbering*; the short form:

| Situation | Rule |
|---|---|
| New module | `1.0.0-pre.0` |
| Already on a prerelease | increment the `pre` component only |
| Released version → change | bump per impact, then add `-pre.0` |

**Alignment (non-negotiable):** the version in the **`.imodspec`**, the **`.csproj`**, and the designer **Module Settings** stereotype must all match. If they differ, the **designer value wins when it is higher** — bring the other two up to it.

**Final vs per-iteration.** The *per-iteration* local pre-version bumping used while testing an already-published module is owned by `module-increment-loop` (see its *Module Version Management* section). Wrap-up owns the **final** version: confirm the released number is aligned and is **ahead of whatever is already published** — check with the module-search tool (`search_available_modules`) so the release will actually carry your changes.

---

## `supportedClientVersions` — Two-Step Rule

**Symptom:** a freshly scaffolded module keeps the wizard default (e.g. `[4.4.0-a,6.0.0)`) and SF fails:
> "The `<supportedClientVersions/>` element value does not support one or more referenced SDK NuGet package versions. Resolved `Intent.SoftwareFactory.SDK` version: 3.14.0. Minimum required Intent Architect version: 5.0.0-a."

**Two-step rule:**
1. **SDK floor (hard minimum):** the SF error states it directly (e.g. `Minimum required Intent Architect version: 5.0.0-a`). This is the lowest the lower bound may be.
2. **Dependency floor (may be higher):** check the module's `modules.config` lockfile — the highest lower-bound across all installed modules' `supportedClientVersions`. (E.g. SDK needs `4.5.18-a`, but `Intent.Common 3.11.2` requires `[5.0.0-a, 6.0.0-a)` → use `5.0.0-a`.)
3. **Final lower bound = max(SDK floor, highest dependency floor).** Set `[floor, ceiling)`; cross-check a neighbouring module — the values should align.

Re-check after any SF run that bumps SDK or dependency versions.

> ⚠️ **Survives a rename?** If the module package is renamed, the `.imodspec` must be renamed *and* stay in sync with the package name — otherwise IA can misresolve the minimum client version and the range silently reverts to the wizard default. After any package rename, re-verify `supportedClientVersions` before release.

---

## `CONTEXT.md` — Durable Knowledge, in the Module Folder

`CONTEXT.md` is the **only durable** build artifact and lives **inside the module project folder** (e.g. `Modules/Intent.Modules.X/CONTEXT.md`) — **never** at the repo root and **never** under `.module-builder/`. Consolidate from `PATTERN-DOCUMENT.md` / `ATTACK-PLAN.md`:
- architectural decisions, invariants, technology constraints, accepted patterns
- which other modules this module affects and how they interact
- design decisions taken during implementation

This is what a future session reads to understand the *why* once the transitory build state is gone.

---

## Gate — Do Not Declare Done Until

- [ ] `module-auditor` Gate 2 returned **PASS** against the frozen `acceptance-spec.md` (not self-certified).
- [ ] Version bumped and **aligned** across imodspec + csproj + designer, and ahead of the published version.
- [ ] `supportedClientVersions` set per the two-step rule (and re-verified after any rename).
- [ ] `module-docs` completed: `release-notes.md` (version heading matches imodspec, ≥1 bullet), `docs/README.md`, imodspec metadata fields (no scaffold defaults).
- [ ] `CONTEXT.md` written **in the module project folder**.
- [ ] `.module-builder/WORKING.md`, `acceptance-spec.md`, `audit-findings.md`, and per-module transitory files cleared (`RETROSPECTIVE.md` kept).
- [ ] Final SF run = **0 staged changes**, after the regeneration baseline was reset.
