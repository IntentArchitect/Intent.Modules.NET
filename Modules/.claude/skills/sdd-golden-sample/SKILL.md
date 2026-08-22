---
name: sdd-golden-sample
description: "Build, verify, and baseline a golden reference sample before an Intent module spec is written, then record it as a dossier that the requirements and design phases derive from. USE ONLY WHEN starting or resuming module-building SDD work whose generated output has no committed, test-proven reference sample yet, or when a cleared Gate G0 criterion has regressed and the sample must be reopened. DO NOT USE FOR authoring the module's own metamodel or templates — that is implementation work that only starts after this gate clears — or for a designer-only change with no generated-code impact. REQUIRES a solution in which the reference sample can be scaffolded or already exists, and the ability to run the Software Factory and a build/test command."
keywords: [golden-sample, phase-0, gate, reference-solution, dossier, baseline, sdd]
template-id: Intent.ModuleBuilder.AI.SDD.Skills.SddGoldenSample_SkillMd_Agents
contentHash: D3F5A3E03371107C32635BCCC0EE9E05DCC034B49BD5CA6690A37474DDC0A204
---
# Skill: sdd-golden-sample

Phase 0 of module-building SDD. It runs **before `/sdd-requirements`**, and its product is a
committed, test-proven reference sample plus a **dossier** describing it. The spec phases that
follow describe that sample; they never predict it.

## Why This Runs First

A module generates code. A spec for a module therefore asserts things about code that does not
exist yet — which framework API to call, what the generated file looks like, which names collide.
Written from theory, those assertions are guesses, and the guesses do not fail at design time.
They fail on first contact with the compiler, after the spec has already been approved.

Putting the sample **after** the spec does not prevent this — it only delays the collision until
the expensive end. Requirements, design, and tasks are all explicitly read-only toward the model
("mutation starts at implementation"), so the sample cannot live inside them. It has to come
first, before a spec exists at all — which is exactly where this skill sits.

The rule that follows: never theorize code generation in the abstract. The verified sample is
ground truth, and every metamodel, stereotype, and template exists to reproduce it.

===

## Step 1 — Classify The Tier

| Tier | Trigger | What Phase 0 means here |
|---|---|---|
| **S — small delta** | Existing module, existing test app, output-affecting bug fix or minor enhancement | Reproduce the defect in the existing app, hand-fix it there, prove it with one real-host test, **commit** — that commit is the sample. Charter collapses to a short note; gate to items 1, 2, 3, 5, 6. |
| **M — one pattern family** | New module or major feature over one coherent generated-output pattern | The full flow below. Variants stay compile-only unless one diverges structurally, which promotes it to full depth. |
| **L — many pattern families** | A broad or custom architecture; one sample cannot cover the surface | **Do not** write one mega-spec. Split into subsystems along generated-file pattern families; each runs its own complete loop (Phase 0 → spec → implement → verify → publish), and each finished module is installed into the next subsystem's Phase 0 scaffold so later samples shrink. |

Any one of these tripwires is sufficient to force an L-split. Raise it with the developer rather
than pressing on:

- more than ~3 distinct generated-file pattern families in scope
- custom elements or stereotypes in more than ~2 designers
- more than ~12 hand-written sample files, or a third application beyond a pair
- an assumption ledger that opens with more than ~10 rows
- an honest estimate putting Gate G0 more than ~2 focused days away

One spec equals one gate-sized sample. If the sample cannot clear the gate in days, the spec is
too large — not the gate too strict.

A **designer-only change with no generated-code impact** skips this skill entirely.

===

## Step 2 — Is There Already A Sample?

Ask this before offering to build anything, and resolve it with the developer rather than
assuming:

1. **Does a reference sample already exist?** Look for an existing test or sample application that

   generates the output family in question. If one exists, do not build a second — verify it
   against Gate G0 and treat the gaps as the work.

2. **If none exists, should one be built?** Get an explicit answer. A "no" ends module work, not

   just this skill — there is no sanctioned path from theory straight to templates.

3. **If yes, how should it be built?** Two legitimate answers, and the developer picks:
  - **Scaffold with Intent Architect** — create or reuse an application, install the released

     modules that already generate the surrounding architecture, run the Software Factory, then
     hand-write only the target pattern on top. Preferred for anything the existing module
     ecosystem already covers: it makes the sample's floor identical to what a real consumer gets.

  - **Hand-write everything** — no scaffold, plain projects. Appropriate when no module generates

     the surrounding shape yet, or when the scaffold's own behaviour is what is in question.

Record the answers in the charter. They are design decisions, not incidental setup.

===

## Step 3 — Research Before Assuming

Do this **before** writing the charter's test list, not after a build fails.

For every framework, package, or platform capability the sample will exercise, consult current
sources — official documentation, the package's own repository, release notes, and any
documentation-lookup or web-search tooling available in the session. Establish, and record with
the source:

- the **actual** API surface for the operation in question, including exact method signatures
- the **current** package versions, and which of them the sample will pin
- whether the capability exists at all in the version being pinned

Reflection over installed assemblies is a legitimate last resort when documentation is silent, but
it is evidence-gathering, not a substitute for looking first. Committed output from such a probe is
a valid ledger artifact.

An API named in a charter or design with no source behind it is an assumption, and it belongs in
the ledger as one.

===

## Step 4 — Write The Sample Charter

A short document at the sample's own root (for example `SAMPLE-CHARTER.md`), approved by the
developer before building. It contains:

- **Capabilities** the sample must demonstrate — each one the reason a later requirement will exist.
- **Topology** — the applications, their roles, and how they are built (Step 2's answer).
- **Test list** — every test named up front, each with the capability it proves and the fact that it

  boots the sample's real host. A test invented later to fit what got built proves nothing.

- **Variants** — which get a full sample, which are compile-only, and the explicit reason for each

  downgrade.

- **Assumption ledger, opened** — one row per unknown, each with the probe that will close it.

  Opening the ledger is the point: an unknown that is never written down is never discharged.

- **Naming and namespace pre-check** — Gate G0 item 8, answered before the names are baked in.

Approval of the charter is a real gate. Get it before building.

===

## Step 5 — Discovery Build

Work the charter in probe loops. Legal here and nowhere later in the flow: installing modules,
running the Software Factory, mutating the model, writing code by hand.

- **Model first, then hand-write.** Model the contracts (messages, entities, services) and generate,

  so the sample's floor is what a real consumer would actually receive. Hand-write only the target
  pattern on top.

- **Close every ledger row with an artifact** — a compiling call site, a passing test, or committed

  probe output. "Investigated" is not closure; a file path is.

- **Protect hand-edits the moment you make them.** A hand-written line inside an Intent-managed file

  is destroyed by the next Software Factory run unless it carries a code-management directive. Add
  the directive in the same edit, never "later" — later is after the loss.

- **Never edit generated output to make the sample look right.** That inverts the test.
- **Re-run the Software Factory and read the staged diff** until it proposes nothing destructive.

  An unapplied destructive diff left in the tree is a live defect, not a deferred chore.

- **Retro-charter anything the build forced into existence.** If the compiler or the generator

  demanded an artifact the charter never planned, add it to the charter with its reason. An
  improvised artifact left undocumented becomes an unexplained requirement later.

===

## Step 6 — Gate G0

Eleven criteria. Each is pass or fail with named evidence, and a build that merely compiles
satisfies exactly one of them.

| # | Criterion | Evidence |
|---|---|---|
| 1 | Every application and test project builds clean | The build command and its exit code |
| 2 | **Runtime-proven through the real host** — at least one integration test per charter capability boots the sample's actual startup path. A test that constructs its own host counts for nothing | Test names, and the entry point each boots |
| 3 | Charter test list complete and green — every named test exists and passes, none skipped, executed list diffed against the charter | Test-run summary plus that diff |
| 4 | **Shipped-shape fidelity** — tests exercise the classes the module will generate, not stand-ins, and every charter default has its own variant (including the module's default, not only the interesting non-default) | File path per capability |
| 5 | Software Factory parity — the staged diff is empty or touches only an explicit ignore list; nothing would strip a hand-written line; no stale lock left behind | Staged-diff summary |
| 6 | Baseline captured and **committed** — sample paths clean in version control, tagged, probes committed | Tag name and commit id |
| 7 | **Assumption ledger empty** — every row closed with an artifact link, or descoped with recorded sign-off | The ledger, all rows closed |
| 8 | Naming and namespace check — no application root namespace shares a root segment with a referenced package's namespace; every alias the design will rely on verified to compile; convention-derived names verified against the real runtime target, not against spec prose | The completed checklist |
| 9 | Licence inventory — built from the actually-resolved package list, licence per pinned version, no licence-gated package present unnoticed | The inventory |
| 10 | Charter coverage — every capability is full, explicitly downgraded, or recorded as descoped; improvised artifacts retro-chartered | Charter against the tree |
| 11 | Developer approval of the gate report, quoted | The quoted answer |

When a criterion fails, the sample is not ready — so fix the sample. Do not open the gate by
weakening the criterion, and do not proceed to requirements "while that last test is written".
Item 2 is the one most often faked by a green build: prove the real host runs.

===

## Step 7 — The Golden Sample Dossier

The gate's durable output, written at the sample's root (for example `GOLDEN-SAMPLE.md`),
committed at the tag. Its sections exist because a later phase consumes each one:

| Section | Contents | Consumed by |
|---|---|---|
| **Topology** | A diagram of what the sample proves — participants, the path between them, the stores and failure paths involved | The requirements narrative; the developer reading the gate |
| **Per-file inventory** | Every sample file: path, classification (generated by released modules / hand-written / hand-modified), what it demonstrates, and the module artifact that will own it | The design's traceability matrix — **transcribed from here, never invented** |
| **API citation index** | Every framework API the sample exercises, cited by file and line at the tag, with the pinned package version | The design's verbatim-API rule |
| **Closed ledger** | Each charter assumption with its resolution and evidence artifact, including research sources | Proof that requirements can carry zero technical assumptions |
| **Test evidence** | Test name, capability proven, host booted, run summary | Acceptance criteria cite these as their oracle |
| **Decisions and deltas** | Retro-chartered artifacts, approved downgrades, descopes, research findings | Requirements non-goals and scope; design rationale |
| **G0 scorecard** | The eleven criteria with evidence links, the approval quote, the tag | The permanent gate record; re-checked at verification |

Once a spec slug exists, copy the tagged sample and this dossier into the spec's own baseline
folder. That copy is the oracle every later parity check measures against.

===

## Step 8 — Hand Over To The Spec

Only now invoke `/sdd-requirements`, and derive rather than interview:

1. **Read the dossier first.** Ask the developer only what it cannot answer — scope, preference,

   developer experience. The questions that otherwise cause requirements to bounce repeatedly
   through design are technical unknowns, and the dossier has already answered them.

2. **Per-file inventory becomes Scope A.** Each hand-touched file maps to exactly one future module

   artifact.

3. **Zero technical assumptions.** Only developer-preference defaults may remain. Every other

   unknown was closed at the gate or descoped with sign-off.

4. **Every API the design will emit must be citable** from a committed sample file at the tag. An

   API with no citation does not go in the design.

5. **Any criterion needing a platform capability cites a precedent or a probe** — an existing

   shipped usage, or a Phase 0 probe that exercised it. This is what stops an approved criterion
   from turning out to be unimplementable.

6. **Phrase output criteria as parity** against the baseline, so each criterion carries its own

   oracle.

Handover is explicit, never automatic. Sample work often spans sessions, and a spec already
in implementation cannot rewind. When a cleared dossier is newer than the spec's requirements,
offer the revision as the next step and wait for a clear yes — then revise requirements, take
approval, amend the design (which no longer builds a sample; it scopes to test applications and
module changes), and regenerate the task waves from the amended design.

===

## Reopening Phase 0

When implementation reveals that the sample itself is wrong or incomplete — a missing registration,
a runtime dependency that was never exercised — that is a **reference gap**, and the correction runs
toward the sample, never away from it:

1. Pause module work.
2. Fix the sample; re-clear the affected G0 criteria.
3. Re-tag and refresh the baseline copy.
4. Resume.

Never adjust a template to match a sample known to be wrong, and never edit the baseline to match
what the templates happen to produce. The direction of correction is what makes the baseline worth
having.
