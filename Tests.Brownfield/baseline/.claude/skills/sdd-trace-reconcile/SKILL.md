---
description: Reconcile traceability after the fact for work implemented outside Intent — map the change set back onto the spec's requirement catalog and record the links nothing recorded live. The safety net for a framework command run from a plain terminal, or any agent with no Intent connection.
short-description: Reconcile traceability after the fact for work implemented outside Intent.
intent-updates: automatic
requiredTools:
  - read_spec
  - get_change_review
  - record_spec_traceability
  - run_designer_query
  - ask_user_question
group: Spec-driven development
---

# SDD — Reconcile traceability

Work implemented **with** Intent connected records its links as it goes (`/sdd-record-traceability`). Work implemented **without** it — a `/speckit.implement` run from a plain terminal, a teammate's own agent, a hand-written change — records nothing. This skill closes that gap afterwards, so there is no untraceable dead zone.

It is **post-hoc inference, and it says so**: links you record here are marked as inferred rather than recorded-live, because you are reading a change set and reasoning about intent, not observing the change being made. Never present an inferred link as though it were witnessed.

## What to do

1. **Resolve the slug — never guess it.** With no exact slug, `read_spec` with no `slug` lists every spec with its `slug`, `phase` and progress. If nothing matches what the user named, `ask_user_question` with the closest candidates.

2. **Load the catalog and what is already linked.**
   - `read_spec(slug, artifact: "coverage")` — every requirement, its source-native id, and its current status. The **uncovered** rows are your work list.
   - `read_spec(slug, artifact: "traceability")` — what is already recorded. **Never re-record a requirement that is already covered** by a live-recorded link; you would replace evidence with inference.
   - `read_spec(slug, artifact: "requirements")` — the actual statements, so you can judge what realizes what. For a framework-owned spec this returns the framework's own documents.
   - `read_spec(slug, artifact: "tasks")` when a `tasks.md` exists — its ticked tasks and any `satisfies` / `[US#]` / `_Requirements:_` tags are the strongest evidence available of which requirement a change belonged to. Use them before you reason from the diff.

3. **Take the framework's own breadcrumbs as your starting work list.** `read_spec(slug, artifact: "trace-candidates")`.

   No framework persists requirement→code, but they all leave evidence in passing, and this reads it back: BMAD's story `Code Map` and its `-- realizes AD-n` task suffixes joined to the spine's `**Binds:**` lines, Kiro's `_Requirements: 1.1_` trailers beside the files a task names, Spec Kit's `[US1]` tags and task-embedded paths. Each row gives you a requirement id, a target, the document it came from, and a one-line `evidence` clause — plus a `confidence` of `strong` (the framework tied that target to that requirement explicitly) or `weak` (the tie is by association, e.g. the file was touched by a story whose epic covers the requirement).

   A row's `targetKind` is `file` (a `relativePath`) or `element` (an `elementName`, never an id). **Prefer the element rows.** They come from a task line that named a designer — BMAD records a model change as a sentence in a checkbox because it has no field for one — and an element link survives regeneration where a link to generated output does not. Resolve the name with `searchElements()` and record the id it reports; **never invent an id** from the name.

   Three rules:
   - **They are hypotheses, not links.** Nothing is recorded until you have checked it. Work `strong` before `weak`.
   - **Verify each one** against the model (`searchElements()`) and the change set before recording it — a Code Map is frozen at write time, so a file it names may have been renamed, deleted, or never written, and a named element may have been renamed since.
   - **An empty result is normal**, not a failure: it means the framework left no breadcrumbs, or every requirement it named is already linked. Reconcile from the change set instead.

4. **Read the change set.** `get_change_review` — the model elements and code files that changed in range. This is the same untraced-change signal `/sdd-verify` consumes, so reconciling here is what makes a later verify meaningful. It is also how you catch what the breadcrumbs missed: a candidate list is only as complete as the prose that seeded it.

5. **Map changes to requirements — conservatively.**
   - Prefer evidence over inference, in this order: a verified `strong` trace candidate → an explicit `satisfies` tag on a completed task → a task title that names the requirement → an element/file whose purpose is unambiguous from the requirement statement.
   - **A change you cannot confidently attribute is left unlinked.** A wrong link is worse than a missing one: it reports a requirement as covered, so `/sdd-verify` stops looking and a genuine gap ships. Say what you left unattributed and why.
   - Apply the same target rules as `/sdd-record-traceability` — element targets from the designer's real ids (resolve with `searchElements()`; **never invent an id**), file targets only for files carrying hand-written code.

6. **Record.** `record_spec_traceability(slug, taskId, requirementIds, targets, designerId)` per task, reading the result's `failures` and re-recording until clean. Where there is no task to hang a link on (a framework whose tasks.md Intent doesn't own, or none at all), record against the requirement's closest task id or the framework's own task id verbatim — whatever the change set genuinely came from. For BMAD that is the story key the work belonged to (`1.1`), which is what the panel's stages are keyed on.

7. **Re-check coverage.** `read_spec(slug, artifact: "coverage")` — confirm no broken links, and see what is still uncovered.

## Then stop

Report, plainly:

- how many requirements you newly linked, and against what;
- which requirements are **still uncovered**, and whether that is because nothing implements them (a real gap for `/sdd-verify` and `/sdd-heal`) or because you could not attribute the change with confidence (a reconciliation limit);
- anything in the change set you could not attribute at all;
- any trace candidate you **rejected**, and why (the file no longer exists, the evidence didn't hold up). A candidate silently dropped looks the same as one that was never offered.

Do **not** tick tasks, record a verdict, or advance the phase. Reconciliation records what happened; it does not judge whether it was right — that is `/sdd-verify`, which is worth running next.
