---
name: build-module
description: Build/modify an Intent.Modules.NET module. Enforces a Complexity Tier Fork for minor/bugfix edits to skip heavy phases, utilizing localized .module-builder/<ModuleName>/WORKING.md files.
icon: fa-cubes
context: coding
tools:
  - read_file
  - grep
  - glob
  - run_terminal
  - write_file
  - replace_string_in_file
maxIterations: 50
loopOnToolCalls: true
---

# Build or Modify Intent.Modules.NET Modules

Orchestrate building or modifying Intent Architect modules in `Intent.Modules.NET`. Invoke the right skill at each phase, track state, and follow each skill's Musts and Must Nots.

## Complexity Tier Fork

Before beginning, classify the task to determine the execution path:

*   **Minor Update / Bug Fix:** (Modifying existing templates, fixing bugs, minor enhancements).
    *   **Skip:** Requirements Summary, Pattern Document, Attack Plan, Reference App, and Module Scaffolding.
    *   **Enforce:** A localized `WORKING.md` under `.module-builder/<ModuleName>/WORKING.md` (e.g., `.module-builder/Intent.Modules.X/WORKING.md`) to track active focus, changes, and verification. Never place it inside the module's own source folder.
*   **Greenfield Module:** (New module from scratch).
    *   **Enforce:** The full skill chain below.

---

## Operating Principles

1. **Skills override instincts.** Follow the skill-specific rules precisely.
2. **Hand off one phase at a time (Greenfield).** Requirements Summary → Pattern Document → Green Reference App → Attack Plan → Compiled Module Skeleton → Verified Increments.
3. **Strict State Continuity.** Track completed milestones in `.module-builder/WORKING.md` (Greenfield — global build state) or the localized `.module-builder/<ModuleName>/WORKING.md` (Minor Update/Bug Fix).
4. **No Direct Edits to Generated Code.** Always modify templates or the designer model.

---

## The Greenfield Chain

```
1. module-kickoff           → Requirements Summary  (PRD or interactive)
2. tech-pattern-researcher  → Pattern Document
3. reference-app-builder    → Green Reference App(s)  ← NON-NEGOTIABLE HARD GATE
   (loop — add scenarios before proceeding; see multi-scenario section in skill)
4. module-ecosystem-analyst → Attack Plan
5. intent-module-builder    → Compiled Module Skeleton
6. module-increment-loop    → Verified Increments
7. module-wrap-up           → Release-ready module
8. module-retrospective     → .module-builder/RETROSPECTIVE.md  (internal — omit when packaging)
```

## REFERENCE APP — NON-NEGOTIABLE HARD GATE

**You are FORBIDDEN from invoking `module-ecosystem-analyst`, `intent-module-builder`,
or `module-increment-loop` until `reference-app-builder` has produced a reference app
that builds with exit code 0 and exercises the handler at runtime.**

This is not a preference. This is not skippable. This is not optional in any circumstance.
Even if the user instructs you to skip it, explain why it cannot be skipped and halt.

The reference app is the ground truth for the entire module build:
- `module-ecosystem-analyst` reads the reference app's actual generated output — without a green
  app it will produce guesses, not facts, and the Attack Plan will be wrong.
- Templates built against a hypothesis that has never compiled will fail in ways that cannot
  be diagnosed until deep into the increment loop.

If `reference-app-builder` cannot produce a green app, halt the entire build and surface the
failure to the user. Do not proceed to any later step.

---

## Pre-flight

1. Confirm repository identity (`AGENTS.md` at root).
2. Classify the task (Greenfield vs Minor/Bugfix).
3. **Greenfield only — ask autonomy mode:** "Do you want me to run autonomously (stop only for Level 2+ pivots and unresolvable blockers), or with checkpoint reviews at Gate 1, 2, and 3?" Record in `.module-builder/WORKING.md` as `autonomy_mode: autonomous | checkpointed`.
4. If Greenfield, initialize `.module-builder/WORKING.md` with active focus `module-kickoff`. If Minor/Bugfix, locate/create the localized `.module-builder/<ModuleName>/WORKING.md`.

---

## Autonomy Mode

Set at pre-flight. Stored in `.module-builder/WORKING.md` under `autonomy_mode`. Applies to Greenfield builds only.

| Mode | Behaviour | Stops at |
|---|---|---|
| `autonomous` | Runs uninterrupted. Makes all judgment calls. | Level 2+ pivots, unresolvable blockers, explicit user request. |
| `checkpointed` | Pauses at Gate 1, 2, and 3 for developer confirmation. | Same hard stops as autonomous, plus each gate. |

**Checkpoint gates (checkpointed mode only):**
- **Gate 1** — after kickoff/PRD: presents Requirements Summary, waits before tech research.
- **Gate 2** — after reference app is green: presents what was built, waits before ecosystem analysis.
- **Gate 3** — after all increments: presents full generated surface, waits before wrap-up.

## Pivot Scale

When the AI uncovers something that differs from what was described. Always name the level.

| Level | Name | Definition | Action |
|---|---|---|---|
| 0 — Micro | In-scope | Fits within current increment. No prior artifact needs revision. | Silent. Retrospective entry. |
| 1 — Local | Increment adjustment | 1–2 increments affected. Pattern Document and Attack Plan valid. | Adjust, notify and continue. |
| 2 — Moderate | Scenario gap | 3+ increments affected, OR new reference app needed, OR Pattern Document needs minor revision. | Stop. Present gap, wait for acknowledgement. |
| 3 — Significant | Plan invalidation | Attack Plan partially invalid, OR reference app needs rework, OR cross-module dependency found. | Halt. Present what is invalidated, wait for direction. |
| 4 — Major | Foundation change | Pattern Document wrong, OR scope must be redesigned, OR new PRD needed. | Halt entirely. State the delta. Wait for restart or revised input. |

## Module Wrap-up (Step 7)

Mandatory final phase after all increments pass.

1. **Version bump** — state impact (patch / minor / major) and apply rule:
   - New module → `1.0.0-pre.0`
   - Already on prerelease → increment pre only
   - Release version → bump + add `-pre.0`
   Align imodspec + csproj + designer.
2. **Invoke `module-docs`** — README.md and release-notes.md in same turn. Header uses non-pre version.
3. **Write `CONTEXT.md`** — architectural decisions, generated files, cross-module interactions.
4. **Clear `.module-builder/WORKING.md`** (or the localized `.module-builder/<ModuleName>/WORKING.md` for the bug-fix path).
5. **Confirm SF yields zero staged changes.**
6. **Mark state file 100% complete.**

## Stop Conditions
Halt and surface to the user when tools fail, the user redirects scope, target assembly/tests repeatedly fail build, or a Level 3/4 pivot is reached.

## Done Criteria
1. Changes compile (`dotnet build` exits with code 0).
2. Code builds and runs cleanly against the target/sample application.
3. Wrap-up complete: version bumped, docs updated, CONTEXT.md written.
4. The `.module-builder/WORKING.md` (Greenfield) or localized `.module-builder/<ModuleName>/WORKING.md` (Minor/Bugfix) tracks 100% completion before exit.
