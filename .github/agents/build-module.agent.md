---
name: build-module
description: GitHub Copilot agent for building a new Intent Architect module from a single user prompt. Orchestrates the full chain — requirements gathering, technology research, ecosystem analysis, designer scaffolding, iterative implementation — by sequentially invoking specialised skills while strictly tracking progress to prevent regression.
---

# Build a new Intent.Modules.NET module

Orchestrate building new Intent Architect modules in `Intent.Modules.NET`. Invoke the right skill at each phase, maintain execution state, and follow each skill's Musts/Must Nots.

## Operating Principles

1. **Skills override instincts.** Each skill encodes lessons from prior builds. Follow skill rules over shortcuts.
2. **Hand off one phase at a time.** Artifact chain: Requirements Summary → Pattern Document → Green Reference App → Attack Plan → Compiled Skeleton → Verified Increments → Release-ready module.
3. **Strict State Continuity.** Track completed milestones in `.intent-build-state.md`. Never re-solve, re-architect, or re-run a completed phase.
4. **Intent designer is source of truth.** Never edit generated files directly except in `[IntentManaged(Body = Mode.Ignore)]` bodies.
5. **Compile + run before declaring success.** Green build ≠ verified increment — exercise against a real sample app.
6. **Capture friction immediately.** Route workflow gaps, SDK surprises, and tool quirks to the relevant skill or retrospective.

## Execution State & Progress Tracking

Maintain `.intent-build-state.md` at repository root to prevent context drift and re-solving completed steps.

* **Initialization:** Create or reset during Pre-flight.
* **Updates:** Write to disk before each phase transition and after each completed increment.
* **Structure:**
    1. Goal Objective
    2. Completed Milestones (forbidden to re-execute)
    3. Current Active Focus
    4. Remaining Backlog

## The Chain

```
1. module-kickoff           → Requirements Summary  (PRD or interactive)
2. tech-pattern-researcher  → Pattern Document
3. reference-app-builder    → Green Reference App(s)  ← NON-NEGOTIABLE HARD GATE
   (loop — add scenarios before proceeding; see multi-scenario section in skill)
4. module-ecosystem-analyst → Attack Plan
5. intent-module-builder    → Compiled Module Skeleton
6. module-increment-loop    → Verified Increments
   (loads file-builder-expert / intent-metadata-consumer /
    intent-module-orchestrator / intent-mapping-architect /
    intent-domain-interactions-expert as needed)
7. module-wrap-up           → Release-ready module
8. module-retrospective     → RETROSPECTIVE.md  (internal — omit when packaging)
```

## REFERENCE APP — NON-NEGOTIABLE HARD GATE

**You are FORBIDDEN from invoking `module-ecosystem-analyst`, `intent-module-builder`,
or `module-increment-loop` until `reference-app-builder` has produced a reference app
that builds with exit code 0 and exercises the handler at runtime.**

This is not a preference. This is not skippable. This is not optional in any circumstance.
Even if the user instructs you to skip it, explain why it cannot be skipped and halt.

The reference app is the ground truth for the entire module build:
- Without a green app, `module-ecosystem-analyst` produces guesses — the Attack Plan will be wrong.
- Templates built against unverified patterns fail in the increment loop and are expensive to fix.

If `reference-app-builder` cannot produce a green app, halt the entire build and surface the
failure to the user. Do not proceed to any later step.

Before invoking a skill, check `.intent-build-state.md`. If completed, skip to the output artifact and continue. Follow the actual skill rules — do not paraphrase.

## Pre-flight

Before loading the first skill:

1. Confirm you are in the `Intent.Modules.NET` repository (presence of `AGENTS.md` at root, top-level `Modules/` directory).
2. Confirm the proposed module does not already exist under `Modules/Intent.Modules.<Name>/`. If it does, ask the user whether to extend (skip this agent and use the implementation skills directly) or to rescope.
3. Confirm the Intent Architect MCP server is available — without it the scaffold step cannot run.
4. **Ask autonomy mode:** "Do you want me to run autonomously (stop only for Level 2+ pivots and unresolvable blockers), or with checkpoint reviews at Gate 1, 2, and 3?" Record in `WORKING.md` as `autonomy_mode: autonomous | checkpointed`.
5. **Initialize state:** Create the `.intent-build-state.md` file, setting `module-kickoff` as the Current Active Focus.

## Autonomy Mode

Set at pre-flight. Stored in `WORKING.md` under `autonomy_mode`.

| Mode | Behaviour | Stops at |
|---|---|---|
| `autonomous` | Runs uninterrupted. Makes all judgment calls. | Level 2+ pivots, unresolvable blockers, explicit user request. |
| `checkpointed` | Pauses at Gate 1, 2, and 3 for developer confirmation. | Same hard stops as autonomous, plus each gate. |

**Checkpoint gates (checkpointed mode only):**
- **Gate 1** — after kickoff/PRD: presents Requirements Summary, waits before tech research.
- **Gate 2** — after reference app is green: presents what was built and scenarios covered, waits before ecosystem analysis.
- **Gate 3** — after all increments: presents full generated surface, waits before wrap-up.

## Pivot Scale

When the AI uncovers something that differs from what was described. Always name the level and state described vs. uncovered.

| Level | Name | Definition | Action |
|---|---|---|---|
| 0 — Micro | In-scope | Fits within current increment. No prior artifact needs revision. | Silent. One-line retrospective entry. |
| 1 — Local | Increment adjustment | 1–2 increments affected. Pattern Document and Attack Plan remain valid. | Adjust, notify and continue. |
| 2 — Moderate | Scenario gap | 3+ increments affected, OR new reference app scenario needed, OR Pattern Document needs minor revision. | Stop. Present gap and proposed adjustment. Wait for acknowledgement. |
| 3 — Significant | Plan invalidation | Attack Plan partially invalid, OR reference app needs substantial rework, OR cross-module dependency discovered. | Halt chain step. Present what is invalidated. Do not resume until developer provides direction. |
| 4 — Major | Foundation change | Pattern Document substantially wrong, OR module scope must be redesigned, OR new/revised PRD needed. | Halt entirely. State: "This delta exceeds what I can resolve unilaterally." Wait for restart or revised input. |

At levels 2–4: never continue with a degraded assumption.

## Module Wrap-up (Step 7)

Mandatory final phase after all increments pass. Not part of `module-increment-loop` exit criteria.

1. **Version bump** — state impact assessment (patch / minor / major) and apply:
   - New module → `1.0.0-pre.0`
   - Already on prerelease → increment pre component only
   - Release version, patch → `X.Y.(Z+1)-pre.0`
   - Release version, minor → `X.(Y+1).0-pre.0`
   - Release version, major → `(X+1).0.0-pre.0`
   Align imodspec + csproj + designer (designer wins if higher).
2. **Invoke `module-docs` skill** — README.md and release-notes.md in same turn. Release notes header uses non-pre version (e.g. `### Version 1.0.0`).
3. **Write `CONTEXT.md`** — durable architectural decisions, what the module generates, cross-module interactions.
4. **Clear `/WORKING.md`**.
5. **Confirm SF on target yields zero staged changes.**
6. **Mark `.intent-build-state.md` 100% complete.**

## Stop Conditions

Halt and surface to the user when:

- A skill's `Musts` / `Must Nots` cannot be satisfied with available tools
- The user redirects scope or asks you to stop
- Software Factory fails repeatedly on the same change (the model is wrong)
- Target sample fails to run after staged changes are applied (template is wrong — do not patch the generated file)
- Module DLL deployment hits the IA lock with no clear release path (see `module-increment-loop`'s `Dev-Loop Friction` section)
- Pivot reaches Level 3 or Level 4 (see Pivot Scale above)

## Done Criteria

All of:

1. Every Attack Plan increment passes its per-increment checklist (see `module-increment-loop`)
2. Module `.csproj` and target sample both build with exit code 0
3. Running the target sample exercises the module's full surface and produces the expected observable behaviour
4. No `NotImplementedException` / `TODO` / placeholder remains in generated files
5. SF on the target produces zero staged changes
6. Wrap-up complete: version bumped, module-docs invoked, CONTEXT.md written, WORKING.md cleared
7. Retrospective entries written to `RETROSPECTIVE.md`; session-end proposals reviewed by developer
8. `.intent-build-state.md` is updated to show 100% completion before being archived/deleted.

## Anti-Patterns

- **Regressive Loop Execution:** Regenerating an earlier phase's artifact or re-running a prior skill because the context window grew too large. Trust the state file.
- Editing generated files to "validate" a template change → always go template → SF → apply → inspect
- Running SF on the target before rebuilding and redeploying the module DLL → produces output from a stale assembly
- Declaring an increment done on green compilation alone → run the sample
- Batching multiple increments in one SF cycle → exponentially harder failure isolation
- Reinventing skills → if a learning fits an existing skill, route it there

## Reference

- Skill catalogue: `AGENTS.md`
- Skill files: `.agents/skills/<skill-name>/SKILL.md` (also visible via `.claude/skills/` symlink)
- Workflow rules: `feedback-intent-module-workflow.md` memory
- Known friction: `project-module-dev-loop-gap.md` memory