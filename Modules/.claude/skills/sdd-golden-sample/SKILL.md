---
name: sdd-golden-sample
description: "Gate an Intent module's golden reference sample — verify it against tiered pass/fail criteria, record the Golden Sample Dossier, and only then allow module-building SDD to proceed. USE ONLY WHEN a reference sample exists on disk (an existing codebase, or an executed scaffolding plan) and needs gating before /sdd-requirements runs, or when a previously cleared criterion has regressed and must be re-checked. DO NOT USE FOR building or scaffolding the sample itself — that is plan work the Module Building SDD agent drives via write_plan/implement_plan — or for the module's own metamodel/template implementation. REQUIRES the sample codebase on disk and the ability to run the Software Factory and a build/test command."
keywords: [golden-sample, gate, reference-solution, dossier, baseline, sdd]
template-id: Intent.ModuleBuilder.AI.SDD.Skills.SddGoldenSample_SkillMd_Agents
contentHash: 208747E081E99241A558E8BFC2CC9F7185FCCF59E8454E2CB38B0279BD1FC7B1
---
# Skill: sdd-golden-sample

The gate between a reference sample and module-building SDD. Its product is a verdict, a
committed and test-proven sample, and a **dossier** describing it — the document
`/sdd-requirements` derives from. This skill does not build the sample; it decides whether what
exists is fit to be ground truth.

## Why The Gate Exists

A module generates code, so a spec for a module asserts things about code — which framework API
to call, what the generated file looks like, which names collide. Asserted from theory, those
claims are guesses, and guesses do not fail at design time; they fail on first contact with the
compiler, after the spec is already approved. A green build is no defence: a sample can compile
perfectly while never wiring up the framework it exists to demonstrate. The gate is what stands
between "it builds" and "it is ground truth".

Never theorize code generation in the abstract. The verified sample is ground truth, and every
metamodel, stereotype, and template exists to reproduce it.

===

## Where The Sample Comes From

This skill assumes a sample already exists on disk. Two legitimate origins:

- **An existing codebase** the developer named as the reference. Gate it as-is; the gaps found

  here become work on the sample, never reasons to lower the gate.

- **An executed scaffolding plan.** When no reference existed, the Module Building SDD agent

  authored a plan (`write_plan`), raised the approval card (`implement_plan`), and executed it
  with the developer. A completed plan is **not** a cleared gate — it has earned exactly this
  check, nothing more.

If neither exists yet, stop: scaffolding is plan work, not gate work. Hand back to the agent to
author the plan first.

===

## Classify The Tier — it sets the gate's depth

| Tier | Trigger | Gate depth |
|---|---|---|
| **S — small delta** | Existing module and test app, output-affecting fix or minor enhancement | Essentials only (marked in the table below). The fixed, committed test app is the sample. |
| **M — one pattern family** | New module or major feature over one coherent generated-output pattern | The full gate. Variants stay compile-only unless one diverges structurally, which promotes it to full depth. |
| **L — many pattern families** | A broad or custom architecture; one sample cannot cover the surface | Do not gate one mega-sample. Split into subsystems along generated-file pattern families; each runs its own complete loop (plan → sample → gate → spec → implement → verify → publish), and each finished module is installed into the next subsystem's scaffold so later samples shrink. |

Any one of these tripwires forces the L-split — raise it with the developer rather than pressing
on: more than ~3 distinct generated-file pattern families; custom elements or stereotypes in more
than ~2 designers; more than ~12 hand-written sample files, or a third application beyond a pair;
more than ~10 open unknowns; an honest estimate putting the gate more than ~2 focused days away.
One spec equals one gate-sized sample.

A designer-only change with no generated-code impact skips this skill entirely.

===

## The Gate

Every criterion is pass or fail with named evidence. Compiling satisfies exactly one of them.
Tier S runs only the rows marked **S**; tiers M and L run every row.

| # | S | Criterion | Evidence |
|---|---|---|---|
| 1 | S | Every application and test project builds clean | The build command and its exit code |
| 2 | S | **Runtime-proven through the real host** — at least one integration test per capability boots the sample's actual startup path. A test that constructs its own host counts for nothing | Test names, and the entry point each boots |
| 3 |   | Planned test list complete and green — every test named in the plan exists and passes, none skipped; tests the build forced into existence are retro-added to the plan with their reason | Test-run summary, diffed against the plan |
| 4 |   | **Citable surface** — every capability the design will cite verbatim exists somewhere committed: a full variant, a compile-only variant, or a committed probe. The sample need not cover the module's whole shipped surface; it must cover what the design quotes. Anything uncovered is a recorded descope, and the spec cannot then assert it | File or probe path per cited capability |
| 5a | S | **Pre-module delta inventory** — run the Software Factory and record, line by line, what it would strip or rewrite. Before the module exists nothing owns the hand-written wiring, so the Software Factory strips it by definition: that diff is the enumeration of what the module must generate, and it is a required output, not a defect | Staged-diff summary, itemised |
| 5b | S | **Sample survives regeneration** — every hand-written line the inventory names is protected by a code-management directive, so a regeneration cannot silently delete it. Not because the line is precious — because a baseline that self-destructs on the next Software Factory run cannot be parity-checked against. Verify by re-running until the diff is clean; no stale lock left behind | The directive per protected line, plus a clean re-run |
| 6 | S | Baseline captured and **committed** — sample paths clean in version control, tagged `golden/<slug>`, probes committed | Tag name and commit id |
| 7 | S | **Every unknown closed with an artifact** — a compiling call site, a passing test, or committed probe output — or descoped with recorded sign-off. "Investigated" is not closure; a file path is | The list of unknowns, all rows closed |
| 8 |   | Naming and namespace check — no application root namespace shares a root segment with a referenced package's namespace; every alias the design will rely on verified to compile; convention-derived names verified against the real runtime target, not against document prose | The completed checklist |
| 9 |   | Licence inventory — built from the actually-resolved package list, licence per pinned version, no licence-gated package present unnoticed | The inventory |
| 10 |  | Plan coverage — every capability in the plan is full, explicitly downgraded, or recorded as descoped; improvised artifacts retro-added to the plan | Plan against the tree |
| 11 | S | Developer approval of the gate report, quoted | The quoted answer |

When a criterion fails, the sample is not ready — so fix the sample. Do not open the gate by
weakening a criterion, and do not proceed to requirements "while that last test is written".
Criterion 2 is the one a green build fakes most often: prove the real host runs.

### The pre-module delta is the point, not the problem

Expect the Software Factory to strip the sample's hand-written wiring. Before the module exists no
template owns those lines, so regeneration removing them is correct behaviour — and the list of
what it removes is the most valuable artifact this phase produces: it enumerates, line by line,
exactly what the module will have to generate. That list becomes Scope A of the requirements.

So read the diff twice, for two different purposes:

- as **inventory** (5a) — what must the module generate? Record every stripped or rewritten line.
- as **durability** (5b) — will the sample still be here after the next regeneration? Protect each

  of those lines with a code-management directive so it cannot vanish silently.

These are complementary, not alternatives, and the sample usually already demonstrates the
protected form somewhere — a hand-edit inside a merge-managed member survives, an additive
settings registration survives, an unprotected line in a fully-managed file does not. Protection
here is not a claim that the line is permanent; it keeps the baseline stable long enough to be
compared against. Once the module generates that line, the template owns it and the directive goes.

#### Prefer merge-style protection, and mark every directive you add

The protection you choose decides how a missed cleanup fails later. Prefer a **merge-style**
directive: the hand-written line survives regeneration *and* the template's output can still land
once it exists, so a forgotten cleanup surfaces as a visible duplicate rather than silently
suppressed output. Fall back to an ignore-style directive only where merge is unavailable, and
never take an ignore mode over a whole file — that suppresses everything the template will
eventually own, and the parity check then passes while proving nothing.

Whichever you use, leave a marker beside it carrying the stable token `GOLDEN-SAMPLE:`, the reason,
and the template that will take the line over:

```csharp
// GOLDEN-SAMPLE: pre-module delta — remove this and the directive above once
// <ModuleName>'s <TemplateId> generates this line.
builder.Host.UseWolverine(opts => EventingConfiguration.Configure(opts, builder.Configuration));
```

Three rules make the marker worth having:

- **It must sit inside the protected region.** A comment outside a merge-managed member is stripped

  on the next regeneration — a marker that evaporates is worse than none, because its absence reads
  as "already cleaned up".

- **Name the owning template.** It doubles as the traceability link: the parity wave knows exactly

  which template must reproduce the line.

- **It is removed with the directive, in the same step**, and it must never reach a real consumer.

  The template does not emit the marker, so a leftover comment shows up as a parity diff.

Because the token is stable, the cleanup is a sweep rather than a memory: `grep -rn "GOLDEN-SAMPLE:"`
over the sample must return nothing before the spec can be called done. Record each marker in the
dossier's pre-module delta row so the obligation survives into a session that never ran this gate.

### Descoping is allowed; silently inheriting its consequence is not

A capability can be descoped from the sample with the developer's recorded sign-off — a
rough-shape sample is a legitimate first increment. What does not survive the descope is the
spec's right to assert what the sample never showed:

- No runtime proof (2, 3 descoped) → acceptance criteria may describe generated **shape** only.

  Nothing in the spec may claim a message is delivered, retried, or handled.

- No variant and no probe for a capability (4) → the design cannot cite its APIs, so that

  capability leaves the spec's scope until a variant or probe exists.

Record each descope with its consequence next to it. A descope whose consequence is not written
down comes back as an approved requirement nobody can satisfy.

While fixing the sample, hold the execution disciplines the plan carries: protect hand-edits
inside Intent-managed files the moment they are made; close unknowns with artifacts; never edit
generated output to make the sample look right; re-run the Software Factory and read the staged
diff until it proposes nothing destructive.

===

## The Golden Sample Dossier

The gate's durable output, written at the sample's root (for example `GOLDEN-SAMPLE.md`),
committed at the tag. Its sections exist because a later phase consumes each one:

| Section | Contents | Consumed by |
|---|---|---|
| **Topology** | A diagram of what the sample proves — participants, the path between them, the stores and failure paths involved | The requirements narrative; the developer reading the gate |
| **Per-file inventory** | Every sample file: path, classification (generated by released modules / hand-written / hand-modified), what it demonstrates, and the module artifact that will own it | The design's traceability matrix — **transcribed from here, never invented** |
| **Pre-module delta** | Every line the Software Factory strips or rewrites, the code-management directive now protecting it, and the `GOLDEN-SAMPLE:` marker beside it | Scope A of the requirements, and the parity wave's cleanup checklist — one row, one directive to remove |
| **API citation index** | Every framework API the sample exercises, cited by file and line at the tag, with the pinned package version | The design's verbatim-API rule |
| **Closed unknowns** | Each unknown from the plan with its resolution and evidence artifact, including research sources | Proof that requirements can carry zero technical assumptions |
| **Test evidence** | Test name, capability proven, host booted, run summary | Acceptance criteria cite these as their oracle |
| **Decisions and deltas** | Retro-added artifacts, approved downgrades, descopes, research findings | Requirements non-goals and scope; design rationale |
| **Gate scorecard** | The criteria with evidence links, the approval quote, the tag | The permanent gate record; re-checked at verification |

Once a spec slug exists, copy the tagged sample and this dossier into the spec's own baseline
folder. That copy is the oracle every later parity check measures against.

===

## Hand Over To The Spec

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

   shipped usage, or a committed probe that exercised it. This is what stops an approved criterion
   from turning out to be unimplementable.

6. **Phrase output criteria as parity** against the baseline, so each criterion carries its own

   oracle.

Handover is explicit, never automatic. Sample work often spans sessions, and a spec already in
implementation cannot rewind. When a cleared dossier is newer than the spec's requirements, offer
the revision as the next step and wait for a clear yes — then revise requirements, take approval,
amend the design (which no longer builds a sample; it scopes to test applications and module
changes), and regenerate the task waves from the amended design.

===

## Reopening The Gate

When implementation reveals that the sample itself is wrong or incomplete — a missing
registration, a runtime dependency that was never exercised — that is a **reference gap**, and the
correction runs toward the sample, never away from it:

1. Pause module work.
2. Fix the sample; re-check the affected criteria (essentials depth).
3. Re-tag and refresh the baseline copy.
4. Resume.

Never adjust a template to match a sample known to be wrong, and never edit the baseline to match
what the templates happen to produce. The direction of correction is what makes the baseline worth
having.
