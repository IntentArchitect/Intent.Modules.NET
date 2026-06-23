---
name: build-module
description: Build a new Intent.Modules.NET module by orchestrating the full skill chain from kickoff to verified increments.
model: sonnet
---

# Build a new Intent.Modules.NET module

Orchestrate building new Intent Architect modules in `Intent.Modules.NET`. Invoke the right skill at each phase, track state strictly, and follow each skill's Musts and Must Nots.

## Operating Principles

1. Skills override instincts.
2. Hand off one phase at a time.
3. Maintain strict state continuity.
4. Intent designer is source of truth.
5. Compile and run before declaring success.
6. Capture friction immediately in memory or relevant skills.

## Execution State

Maintain `.intent-build-state.md` at repository root. Create or reset during Pre-flight. Update before every phase transition and after every completed increment.

Structure:
1. Goal Objective
2. Completed Milestones (do not re-execute)
3. Current Active Focus
4. Remaining Backlog

## Chain

1. module-kickoff -> Requirements Summary  (PRD or interactive)
2. tech-pattern-researcher -> Pattern Document
3. reference-app-builder -> Green Reference App(s)  ← NON-NEGOTIABLE HARD GATE
   (loop — add scenarios before proceeding; see multi-scenario section in skill)
4. module-ecosystem-analyst -> Attack Plan
5. intent-module-builder -> Compiled Module Skeleton
6. module-increment-loop -> Verified Increments
7. module-wrap-up -> Release-ready module
8. module-retrospective -> RETROSPECTIVE.md  (internal — omit when packaging)

Use implementation skills inside the increment loop as needed:
- file-builder-expert, intent-metadata-consumer, intent-module-orchestrator
- intent-mapping-architect, intent-domain-interactions-expert

Before invoking a chain skill, check `.intent-build-state.md`. If completed, skip to its artifact and continue.

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

## Pre-flight

Before loading the first skill:

1. Confirm repository identity (`AGENTS.md` exists, top-level `Modules/` exists).
2. Confirm target module does not already exist under `Modules/Intent.Modules.<Name>/`.
3. Confirm Intent Architect MCP tooling is available.
4. **Ask autonomy mode:** "Do you want me to run autonomously (stop only for Level 2+ pivots and unresolvable blockers), or with checkpoint reviews at Gate 1, 2, and 3?" Record in `WORKING.md` as `autonomy_mode: autonomous | checkpointed`.
5. Initialize `.intent-build-state.md` with Current Active Focus set to `module-kickoff`.

## Autonomy Mode

Set at pre-flight. Stored in `WORKING.md` under `autonomy_mode`.

| Mode | Behaviour | Stops at |
|---|---|---|
| `autonomous` | Runs uninterrupted. Makes all judgment calls. | Level 2+ pivots, unresolvable blockers, explicit user request. |
| `checkpointed` | Pauses at Gate 1, 2, and 3 for developer confirmation. | Same hard stops as autonomous, plus each gate. |

**Checkpoint gates (checkpointed mode only):**
- **Gate 1** — after kickoff/PRD analysis: presents Requirements Summary, waits before tech research.
- **Gate 2** — after reference app is green: presents what was built and scenarios covered, waits before ecosystem analysis.
- **Gate 3** — after all increments pass: presents full generated surface, waits before wrap-up.

## Pivot Scale

When the AI uncovers something that differs from what was described, it classifies the delta and acts accordingly. Always name the level and state what was described vs. uncovered.

| Level | Name | Definition | Action |
|---|---|---|---|
| 0 — Micro | In-scope | Fits within current increment. No prior artifact needs revision. | Silent. One-line retrospective entry. |
| 1 — Local | Increment adjustment | 1–2 increments affected. Pattern Document and Attack Plan remain valid. | Adjust plan, notify: "Adjusting increment N due to [finding]. Continuing." |
| 2 — Moderate | Scenario gap | 3+ increments affected, OR new reference app scenario needed, OR Pattern Document needs minor revision. | Stop. Present gap and proposed adjustment. Wait for acknowledgement before continuing. |
| 3 — Significant | Plan invalidation | Attack Plan partially invalid, OR reference app needs substantial rework, OR cross-module dependency discovered. | Halt chain step. Present what is invalidated and what input is needed. Do not resume until developer provides direction. |
| 4 — Major | Foundation change | Pattern Document substantially wrong, OR module scope must be redesigned, OR new/revised PRD needed. | Halt entirely. State: "This delta exceeds what I can resolve unilaterally." Wait for developer to restart or revise. |

At levels 2–4: never continue with a degraded assumption.

## Module Wrap-up (Step 7)

Mandatory final phase after all increments pass. Not part of `module-increment-loop` exit criteria.

1. **Version bump** — state impact assessment (patch / minor / major) and apply:

| Situation | Rule |
|---|---|
| New module | `1.0.0-pre.0` |
| Already on a prerelease | Increment pre only: `1.0.0-pre.4` → `1.0.0-pre.5` |
| Release version, patch change | `X.Y.(Z+1)-pre.0` |
| Release version, minor change | `X.(Y+1).0-pre.0` |
| Release version, major change | `(X+1).0.0-pre.0` |

Align imodspec + csproj + designer (designer version wins if higher).

2. **Invoke `module-docs` skill** — README.md and release-notes.md in the same turn. Release notes header uses the non-pre version (e.g. `### Version 1.0.0`, not `1.0.0-pre.5`).
3. **Write `CONTEXT.md`** — durable architectural decisions, what the module generates, cross-module interactions.
4. **Clear `/WORKING.md`**.
5. **Confirm SF on target yields zero staged changes.**
6. **Mark `.intent-build-state.md` 100% complete.**

## Stop Conditions

Stop and surface to the user when:
- A skill's Musts or Must Nots cannot be satisfied.
- The user redirects scope or asks to stop.
- Software Factory fails repeatedly on the same change.
- Target sample fails to run after staged changes are applied.
- Module DLL deployment remains blocked by an IA lock.
- Pivot reaches Level 3 or Level 4 (see Pivot Scale above).

## Done Criteria

All must be true:
1. Every Attack Plan increment passes its checklist.
2. Module `.csproj` and target sample both build with exit code 0.
3. Target sample run verifies expected behavior.
4. No placeholders remain (`NotImplementedException`, `TODO`, etc.).
5. SF on target yields zero staged changes.
6. Wrap-up complete: version bumped, module-docs invoked, CONTEXT.md written, WORKING.md cleared.
7. Retrospective entries written to `RETROSPECTIVE.md`; session-end proposals reviewed.
8. `.intent-build-state.md` shows 100 percent completion before archive or deletion.

## Anti-patterns

- Re-running completed phases instead of trusting state.
- Editing generated files directly for validation.
- Running SF on target before rebuilding and redeploying module DLL.
- Declaring success from compile only without runtime verification.
- Batching multiple increments in one SF cycle.
- Reinventing skill logic instead of invoking the right skill.

## References

- `AGENTS.md`
- `.agents/skills/<skill-name>/SKILL.md`
- workflow memory: `feedback-intent-module-workflow.md`
- friction memory: `project-module-dev-loop-gap.md`
