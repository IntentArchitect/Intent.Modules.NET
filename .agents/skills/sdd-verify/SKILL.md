---
description: Verify an implemented spec against its acceptance criteria — model validation, designed elements present, traceability captured correctly with no broken links, code matches requirements — the review step of the built-in SDD flow.
short-description: Verify an implemented spec against its acceptance criteria — SDD review step.
intent-updates: automatic
requiredTools:
  - read_spec
  - record_spec_verdict
  - create_sub_agent
  - get_change_review
group: Spec-driven development
---

# SDD — Verify

You are running the **verify** step — the review pass after `/sdd-implement`. Check that what was built actually satisfies the spec and **record a per-requirement verdict** the flow can act on. This is **review, not repair**: your only write is the verdict — you do not change the model or code, and you raise no gate. When gaps remain, `/sdd-heal` fixes each failing requirement **in place**, one gap at a time, and never re-does the work that already passed; you just judge. Your gap descriptions **are** the healer's brief, so make them precise and actionable.

**Dispatched with only a slug** (as `/sdd-run` does): judge from the spec, the model and the code alone. Treat every earlier claim — a wave report, a heal summary, the previous verdict — as unverified until you have checked it yourself. Your final reply goes to the dispatcher, so end with the chat report from **Record the verdict, then report**.

**Honour the solution's governance.** When `read_spec` returns `governanceDocuments` (Spec Kit's `.specify/memory/constitution.md`, say), read them first and treat their rules as binding on what you produce — they are the user's standing constraints. If a requirement and a governance rule genuinely conflict, say so and ask rather than silently picking one.

## References — read only what you need

`read_file` these from this skill's base directory when their trigger applies, never all of them:

| Read it when | Reference |
| --- | --- |
| Before judging any criterion's tests: the run commands, what each test status means, orphan citations, the sceptic dispatch and its brief, and how to record a disagreement | `references/judging-tests.md` |
| You have a candidate behavioural gap to record (challenge it first) | `references/challenge.md` |
| You were told to re-verify only some ids (as `/sdd-run` does after a heal) — its deltas override the steps below | `references/re-verify.md` |
| `/sdd-extract` loaded this skill (as-built mode) | `references/as-built-mode.md` |
| The change review shows rolled-up rows, omitted rows, or a range line that fell back to HEAD, is reversed, or says "(last verify)"; or you need what a `broken` / `uncovered` requirement usually means | `references/change-review.md` |
| `record_spec_verdict` warns about untraced changes or a narrowed range, or `verdict.json` / `test-judgements.tsv` need merging | `references/recording-the-verdict.md` |

## What to do

**First, resolve the slug — never guess it.** If you weren't handed an exact slug, call **`read_spec` with no `slug`**: it lists every spec with its `slug`, `phase` and progress; match what the user named against it. Specs live under `intent/.specs/` and are reachable **only** through `read_spec` — never glob or grep for spec artifacts. If nothing plausibly matches, `ask_user_question` offering the closest specs as options rather than inventing a slug.

**Questions that cite requirements.** Pass the slug as `spec` on every `ask_user_question` that names a requirement id, so the card links each id, and say in plain words what each cited requirement requires: the user may not remember what `R3.14` is.

1. Load the spec:
   - `read_spec(slug, artifact: "requirements")` — the EARS acceptance criteria (R1, R2, …) **and the framing above them**: the **Key User Journeys** (`UJ-N`) are what the criteria should add up to, the **Glossary** is the vocabulary the model should speak, and **Non-Goals / out-of-MVP** is a boundary the implementation can violate as easily as it can fall short of a criterion. An imported spec (e.g. a BMAD PRD) comes back as the framework's own documents with `externallyOwned: true`; they are **read-only** — never write or normalize a `requirements.md` for it. If the requirements themselves are wrong, say so and stop rather than fixing them in place.
   - `read_spec(slug, artifact: "design")` — the intended model changes + realization plan.
   - `read_spec(slug, artifact: "traceability")` — which tasks/elements/files were meant to satisfy each requirement; compact summaries by default, one requirement's full targets with `requirementId: "R3"`.
   - `read_spec(slug, artifact: "coverage")` — the covered/uncovered/stale/broken rollup: one compact row per requirement, **no** per-target arrays. Narrow with `status: "gap"` (or `uncovered`/`stale`/`broken`/`covered`); drill in with `requirementId: "R3"`.
   - `read_spec(slug, artifact: "tests")` — the `testFiles` citing the spec, then each criterion not yet `tested` with its **test status** (`partly tested`, `stale`, `unconfirmed`, `mismatch`, `untested`, `judged` or `untestable`) and how many tests cite it, and any orphan citations. A criterion's numbered clauses and citing tests (`file:line title`, each marked with its degree or `unconfirmed`) come only with `requirementId`, for each criterion not yet tested (a tested one gives just its test count, as its tests are run, not re-read; a criterion id gives its full detail): read them only for the requirements you or a sceptic judge (comma-separated in a targeted run, one at a time in a full verify), never for the whole spec. A test cites criteria in its title as `[spec:<slug> R1.4, R1.5]`.
   - `get_change_review(section: "traceability", specSlug: "<this spec>")` — the **change side**, and the one input that is computed rather than judged. `coverage` walks from each requirement to its links, so it is blind to a designer element or hand-written file changed for this feature that **no requirement points at**. This review (merge-base → working tree, so committed waves and the worktree are both in it) lists those under `untraced elements` / `untraced custom files`, after the traced map. Read it before you judge anything. **Always pass `specSlug`**, or no spec's requirements are expanded. A row ending `(+N child change(s))` is N+1 changes, resolved as a group. If rows were omitted, page on with the `offset` the overflow line names. If the range line says it fell back to HEAD, or calls the range reversed or diverged, fix the range (`references/change-review.md`) before concluding anything is untraced or missing.
   Read each artifact once and reuse what came back. Read it again only after something changed it: a sceptic's bindings change `tests`.
2. Verify, per requirement:
   - **Model**: `getDesignerValidationErrors()` is clean; the designed elements/associations exist (`searchElements()`, `getElementDetails()`).
   - **Coverage**: every requirement has ≥1 trace target, spanning **both** what realized it — every model change it drove **and** every custom-code file written for it. Flag `uncovered`, `stale` (a criterion was reworded since its tests were judged) and `broken` (a linked element/file no longer resolves against the saved model / disk; `coverage` computes this, matching the Specs panel). A `broken` or `uncovered` requirement — including one whose model is linked but its custom code isn't, or vice versa — is a **`gap`** whose `actual` names the missing/unresolved target, so `/sdd-heal` re-records the corrected link.
   - **Untraced changes — the deterministic gate.** Every row under `untraced elements` / `untraced custom files` is a change that nothing claims. **Resolve each one, and say which way** — leaving them unmentioned is the failure this step exists to prevent. Exactly two outcomes: it **realizes one of this spec's requirements** (the link was never recorded) — record a **`gap`** on that requirement, naming the element/file in `actual` and `location`, attributing it via `design.md` / `tasks.md` — or it **belongs to other work** (another spec, a manual fix, an unrelated refactor on the branch) — record no gap and name it in your chat report as out-of-spec work. The range is solution-wide, so the second outcome is normal, not a failure. When the report warns that a designer's element diff failed, the element list is **incomplete** — say so rather than reading it as clean.
   - **Tests — the durable proof.** Judge each criterion by its test status, following `references/judging-tests.md`. In short: a **tested** criterion's citing tests are run, and green is a pass, red a **disagreement**; **stale** and **unconfirmed** ones get their tests run and judged by sceptic sub-agents; an **untested** one is reported, not a gap (a full verify audits its requirement by reading the code). A disagreement — a failing citing test, or a sceptic's `mismatch` — is a `gap` the user settles, recorded as that reference says.
   - For a large area, you may dispatch `discovery` sub-agents (`create_sub_agent`) to map it read-only before judging, all in a **single response**, each with a slice of the requirements. Use **at most four** in all, because each one re-reads the same spec and code. Have each return, per requirement, its verdict and the `file:line` it rests on.
3. **Then verify the journeys, not only the criteria.** A feature can satisfy every criterion and still have no working path through `UJ-2`. Walk each `UJ-N` beat by beat against what was built — entry state, each step, the climax, the resolution, and its edge case — and check a real operation / endpoint / interaction stands behind each beat. A missing or dead beat is a **`gap`** against the requirement(s) naming that journey under `**Realizes:**` (or the nearest owning requirement when none does), with the broken beat in `actual`. Anything built that lands in **Non-Goals** or out-of-MVP scope is a gap too, and the healer's brief is to remove it, not to add more.
4. **Challenge every behavioural gap** before you record it (`references/challenge.md`). Traceability gaps and disagreements skip the challenge.

## As-built mode (extraction)

When `/sdd-extract` loads this skill, the code is the truth — a user decision. Read `references/as-built-mode.md` before step 1 and follow its deltas wherever they differ from the steps above.

## Record the verdict, then report

1. **Record the machine-readable verdict.** `record_spec_verdict` is available in **every** session type — panel-fired, in-app chat, or an external ACP harness — so recording it is always this step's required output. Call `record_spec_verdict(slug, results)` with one result per requirement (in re-verify mode, one per listed id with `merge: true`). A **pass** is `{ requirementId, verdict: "pass" }`, plus a `reason` naming the `file:line` that satisfies its untested criteria when it has any: the tool records that reason as their `judged` binding, which holds until their wording or code moves. The sceptics already recorded the test bindings, so pass none. A **gap** carries the healer's brief — `{ requirementId, verdict: "gap", reason, expected, actual, location }`:
   - `reason` — a one-line summary of what's missing or wrong.
   - `expected` — what the acceptance criterion requires (the behaviour the fix must produce), starting with the ids of the criteria it fails (`R1.4, R1.10 — …`): heal writes tests only for those.
   - `actual` — what is there now: the specific deficiency (missing, wrong, or a broken/absent element).
   - `location` — where to fix it: the file(s) and/or model element(s)/operation(s) involved.

   Mark `gap` whenever the behaviour the criteria describe isn't genuinely there **and the challenge didn't refute it**; when unsure, prefer `gap`. `/sdd-heal` reads these fields straight from `verdict.json` and turns each gap into one repair task, so make them precise and actionable — a weak or blank brief leaves the healer guessing.

   `record_spec_verdict` re-runs the change review over the range you last reviewed and warns when untraced changes remain: treat that as a prompt to correct the verdict, not as noise. If its feedback says the range was narrowed, widen it with `refA` and record again rather than reporting clean. When `verdict.json` conflicts on merge, re-run verify rather than merging it by hand.

2. **Report in chat too.** Summarise the per-requirement verdict, each citing its evidence (an element id, a file, a test, or the missing piece); give the test status counts (tested, partly tested, mismatch, untested, judged, manual check); list the **disagreements** apart from the other gaps; note how many candidate gaps the challenge refuted; and list the concrete gaps, with traceability problems (uncovered, broken links, stale or mis-linked traces) alongside the behavioural ones.

   State the **change review** result explicitly, since the user cannot reconstruct it from the verdict rows: the range reviewed, how many changed elements and code files it covered, how many requirements they realized, and — one line each — every untraced change with how you resolved it (which requirement's gap, or why it is out-of-spec work). If nothing was untraced, say so plainly; that is a real result.
3. **Stop.** The recorded verdict is the phase decision, and the lifecycle applies it: a clean verdict moves a spec at verification to **done**, whatever the gate cadence, and a verdict with gaps leaves a done spec at **done**, showing its gaps. `record_spec_verdict`'s feedback says when the phase moved. Never call `advance_spec_phase`, and change nothing else.
