---
description: Record requirement → model element / code file traceability as you implement a spec, against the catalog's source-native requirement ids. The single trace contract shared by /sdd-implement-wave, /sdd-heal, /sdd-trace-reconcile, and any run driven by another SDD framework's own command (Spec Kit, Kiro, BMAD, OpenSpec).
short-description: Record requirement to model element & code file traceability as you implement.
intent-updates: automatic
requiredTools:
  - read_spec
  - record_spec_traceability
  - complete_spec_task
group: Spec-driven development
---

# SDD — Record traceability

Recording **which requirement each change realizes** is what turns a pile of edits into a spec that can be verified and healed. Every other SDD framework's traceability stops at _requirement → task → a file path written in prose_. Yours goes to the **model element** and the **generated/hand-written file**, and it is checkable: `/sdd-verify` judges against it and `/sdd-heal` repairs what it finds.

This skill is the contract for doing that. It is the **only** copy — `/sdd-implement-wave`, `/sdd-heal`, `/sdd-trace-reconcile` and framework-driven runs all reference it rather than restating it, so there is one definition to keep right.

**It applies to work in ANY framework.** If the spec's requirements came from Spec Kit, Kiro, BMAD or OpenSpec — or the implementation is being driven by that framework's own command (`/speckit.implement` and friends) rather than by Intent's wave orchestrator — nothing here changes. The catalog is uniform; only the id spelling differs.

## The ids you record against

Record against the **catalog's own requirement ids, verbatim** — `read_spec(slug, artifact: "coverage")` lists them. They are the framework's native ids, copied unchanged on import precisely so links survive a re-import:

| Source                    | Ids you will see                                              |
| ------------------------- | ------------------------------------------------------------- |
| Intent Architect (native) | `R1`, `R2`, `R3.1`                                            |
| Spec Kit                  | `FR-001`, `NFR-001`, `SC-001`, `US1`, `US1.2`                 |
| Kiro                      | `R1`, `R2`                                                    |
| BMAD (PRD lane)           | `FR1` or `FR-1` — **whichever the PRD wrote** — `FR-1.2`, `NFR1`, `Story 1.1` |
| BMAD (lean SPEC lane)     | `CAP-1`, `CAP-1.2`                                            |
| OpenSpec                  | `REQ-<8 hex>`, `REQ-<8 hex>.1` — minted from the requirement's NAME |

Never invent, renumber, normalize or "tidy" an id — a link recorded against an id the catalog doesn't hold is an orphan the cockpit reports and nothing resolves. A dotted id (`FR-1.2`, `US1.2`, `CAP-1.2`) is an **acceptance criterion of** its parent; recording against the criterion is right and better — the panel rolls it up onto the parent's row automatically, and the link keeps the criterion, so when that target changes only that criterion's tests have to prove it still holds. Name the criteria a target realises when it realises only those; name the requirement itself when it realises all of it or you are unsure. A whole-requirement link is never wrong, only noisier.

Two spellings need care, because in both cases the id you see quoted in prose is **not** the id you record against:

- **BMAD does not normalize its own ids.** One feature's PRD writes `FR1` while its epics document's coverage map writes `FR-1`, and a sibling feature writes the hyphenated form throughout. The catalog copies the PRD's spelling verbatim; record THAT, not the one the epics file or a story spec happened to use.
- **OpenSpec ids are minted, not native.** OpenSpec names requirements (`### Requirement: Save a product to a wishlist`) but never numbers them, so import mints `REQ-<hash of the name>`. Renaming the heading mints a new id and orphans the old links — visible in the panel's orphaned bucket, and the deliberate trade for OpenSpec being importable at all. Never guess a `REQ-` id; read it from the catalog.

## The loop, per task

In **this order**, for each task you are about to tick:

1. **`record_spec_traceability(slug, taskId, requirementIds, targets, designerId)`** — link the task's requirements to what actually realizes them.
2. **Read the result's `failures`.** Fix them and re-record until the call comes back clean.
3. **`complete_spec_task(slug, taskId)`** — only then.

**`taskId` is the key a re-record replaces and `complete_spec_task` checks**, nothing more. **A Capability has no tasks**, and neither does a heal or sync outside a wave: record each requirement under its own id as the `taskId` (`taskId: "R3"`), and tick nothing.

**A failed record call means the link does NOT exist.** A target that doesn't resolve — a guessed element id, a path to a file nobody wrote — is **rejected, not stored**, and the call comes back as a failure naming it. There is no "recorded but broken" state to move on from: until you re-record with a target that resolves, that requirement has no link. If every target was rejected, nothing at all was written (including the usual replace of the task's previous links, so a fumbled re-record can't wipe good ones).

**Rewording a criterion makes its requirement `stale`** until verify re-judges that criterion's tests or judgement against the new wording. The links themselves don't change, so don't re-record them for a reword. Re-record with full `targets` only when the targets themselves changed.

The one exception is an **orphaned requirement id** — an id the catalog doesn't hold. That link IS stored (against the id you gave) and reported, because the target itself is fine and a lagging catalog is healed by re-importing the source rather than by re-recording. Fix the id, or re-import, but don't re-record a different target for it.

**Never tick a task whose traceability isn't recorded with zero `failures`** — and `complete_spec_task` enforces it two ways:

- It re-checks that task's own links and **refuses while any is broken**. That catches the case the record call can't, where a *later* task renamed or deleted an element an earlier task legitimately linked.
- It **refuses a `[model]`/`[code]` task with nothing recorded at all**. Zero links means nothing to find broken, so this is the one gap the check above cannot see — and it is permanent, because nothing revisits a ticked task.

A refusal means "record (or correct) that task, then tick" — never "tick something else". Both gates apply to a still-open task; re-confirming an already-ticked one stays a no-op.

### Affirming a task with nothing to record

Occasionally a `[model]`/`[code]` task genuinely produces no element and no hand-written file — it turned out to be pure investigation, or the work was already covered by a sibling task's link. Say so explicitly rather than working around the gate:

```
record_spec_traceability(slug, taskId, requirementIds: [], targets: [], clear: true,
                         note: "<why there is nothing to record>")
```

That clears the task's links, records the reason in `traceability.json`, and lets the tick through. Use it when it is **true**: it is the difference between a gap that is written down and one that isn't.

## What counts as a target

- **Model elements / associations** — `{ id, name }`. Take the `id` and `name` **exactly** as `run_designer_script` returned them in its `changes`, whose entries are already this shape. This is the link that makes the cockpit's change tree, drift detection and broken-link checking work.
- **Code files** — `{ kind: "file", relativePath }` plus the `applicationId` the file belongs to. **Only files carrying hand-written code** (`custom`, or `deterministic + custom`). Purely `deterministic` generated boilerplate and contracts trace to the **model element** that produced them, not to a file: a file link there would break on every regeneration and say nothing the element link doesn't.
- A target that carries its own `requirementIds` links only to those; one that doesn't falls back to the task's full list. Use it when a task satisfies several requirements but a member serves only some.
- A task that produces **no** designed element and no hand-written file (installing a module, say) has nothing to record. A `[module]`/`[app]`-tagged task can simply be ticked; a `[model]`/`[code]` one **must say so explicitly** — see "Affirming a task with nothing to record" above.

### How each link is classified

A target says **what** was touched; the `created` / `updated` / `deleted` recorded against it is read from **git**.

The range is the spec's own **pinned baseline** → working tree. That baseline is captured on the spec's **first** recorded traceability and never moves afterwards, so work committed mid-wave stays in range and the operations go on describing what *this spec* did however much lands later. It pins itself — there is nothing to set up.

What follows from that:

- **A deletion is recorded by deleting the file.** Link the removed path as an ordinary `{ kind: "file", relativePath }` target; git showing that path deleted in range is what makes the link legitimate, and it then reads as an intentional removal rather than a broken one. A path git says nothing about is **rejected** as not found — so a typo cannot pass itself off as a deletion.
- **A target git says didn't change is recorded without a classification** and shows uncoloured, like a manual link. That's honest, not a fault — you linked something real that this work didn't touch.
- **In a workspace with no repository** nothing can be classified: the links still record, the feedback says so, and a link to a file that doesn't exist is refused. Read the feedback rather than re-recording.
- **Read the feedback for the two range caveats.** *"Only uncommitted work was in range"* means a link recorded without a classification might have changed in an earlier commit rather than not at all — the baseline had collapsed onto the branch tip when it was pinned. *"Pinned baseline no longer exists"* means it was rebased or squash-merged away and the live range was substituted. Neither is your fault and neither is fixed by re-recording; both are worth surfacing to the user if they ask why the coverage panel is uncoloured.

### Spelling a file path

**Don't guess a base — any honest spelling is accepted.** `record_spec_traceability` probes the path you pass against the application output you named, the workspace root, every other application's output, the solution directory, and finally each immediate child directory of the workspace root; it then **rewrites the hit to one canonical form** and reports the rewrite. So all three of these land on the same stored link:

| You are standing in…                                        | Spell it                                                 |
| ----------------------------------------------------------- | -------------------------------------------------------- |
| The application you passed as `applicationId`               | `Api/Program.cs` (output-relative)                       |
| Anywhere, naming a file in another project                   | `eShop.Storefront/src/cart.tsx` (workspace-relative)     |
| Inside a sibling project, with its root as your mental cwd   | `src/cart.tsx` (resolved against each workspace child)   |

A `../` escape out of an application output (`../eShop.Storefront/src/cart.tsx`) is accepted too, and canonicalized the same way. **The rule is: spell it the way you actually know it, and let the tool decide the base.** Prefer the workspace-relative form when a file is outside the application you were dispatched with — it is unambiguous, whereas a bare `src/cart.tsx` is refused if two sibling projects both have that file (the refusal names both, and you re-record with the workspace-relative one).

If a path is refused as not found, **read the refusal**: it lists the concrete absolute directories that were probed. Either the file isn't written yet, or the casing is wrong — it is not a base you need to guess again.

**Code outside every application is a smell, not a path problem.** If a file genuinely lives in no application's output — a hand-written front-end, say — the link is stored workspace-relative with no application, which resolves but joins to nothing in the Changes Review, and `write_file` / `patch_file` can't touch that file either. Its real fix is upstream: that codebase area should have had an application (see the design phase). Record the link so the work is traceable, and say plainly in your report that the area has no application.

`record_spec_traceability` saves the designer before recording, and refuses a `[model]`/`[code]` target before the model has been generated to disk — a refusal means "run the Software Factory, then record", not "record something else".

## Replacing vs preserving links

Passing `targets` **replaces** that task's links; passing none **preserves** them. So:

- Re-recording with the corrected target is how you fix a wrong or broken link. **Never hand-edit `traceability.json`.**
- If you only edited files that were already linked and introduced no new element or file, there is nothing to record.

## Before you finish: sweep for broken links

The per-task `failures` check only sees one task's recording. A later task can rename or delete an element an earlier, already-ticked task linked. Before you report done:

- `read_spec(slug, artifact: "coverage")` — check the `broken` rollup.
- `read_spec(slug, artifact: "coverage", status: "broken")` lists just the broken requirements; `read_spec(slug, artifact: "coverage", requirementId: "<id>")` shows one requirement's full targets.
- Re-record the affected task with the corrected target (or clear its stale link) so you leave no broken traceability behind.

## If you are NOT the one holding the model

When the implementation is driven by another framework's command, or by a sub-agent without designer tools, the rule is unchanged — but the ids come from wherever the change was actually made:

- A `coding` sub-agent records and ticks **its own** `[code]` tasks as each one's build/test passes, using file targets. Don't record those on its behalf.
- **Test files are never linked.** A test cites the criteria it asserts in its title, `[spec:<slug> R2.1]`, and `read_spec(slug, artifact: "tests")` finds it from that; the change review never counts a test file as untraced. Cite numbered criteria (`R2.1`), never a bare requirement (`R2`), and only on the test itself, not on a `describe` block or test class: anything else matches no criterion and is reported as an orphan citation.
- If you changed the model but can't see a `changes` list (you didn't run the script yourself), **don't guess an element id.** Resolve the element with `searchElements()` / `getElementDetails()` and record the id it reports, or leave the link unrecorded and say so — an invented id is worse than a gap, because it reads as covered.

## When nothing recorded it live

A run with no Intent connection at all — a framework command executed from a plain terminal — records nothing as it goes. That is what **`/sdd-trace-reconcile`** is for: it maps the change set back onto the catalog afterwards and marks those links as inferred rather than recorded-live. It starts from `read_spec(slug, artifact: "trace-candidates")`, which reads the requirement→file hypotheses the framework left in its own prose. Use it after the fact; use this skill during.
