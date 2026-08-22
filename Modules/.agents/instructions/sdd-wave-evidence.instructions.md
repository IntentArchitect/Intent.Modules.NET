---
applyTo: '**'
description: "What a wave of an Intent Architect SDD spec must produce as evidence before any of its tasks may be reported complete."
keywords: [sdd, wave, evidence, traceability, completion, verification]
template-id: Intent.ModuleBuilder.AI.SDD.RootPrinciples.SddWaveEvidenceMd
contentHash: 3BF7E2402B146D85A5F2CA76C5DBFC9584284CEA865622555D356600D0CB39EC
---
# SDD Wave Evidence Contract

Scope — read the next sentence and stop if it does not apply. This applies only when you are
implementing, or reporting on, a wave of a Spec-Driven Development spec in this solution. For any
other work, ignore it.

## Why This Exists

An SDD orchestrator dispatches one sub-agent per wave and treats what comes back as authoritative —
it does not re-do the wave's work to check it. That trust is what makes wave orchestration
affordable, and it is only safe if a completion report is backed by evidence that could not exist
unless the work happened.

Two failures make this concrete, and both have happened:

- A wave reported every task complete on "both builds green". The build was green. The application

  never invoked the framework the wave existed to wire up. Green proved the syntax, and nothing else.

- A sub-agent returned a status placeholder describing what it *would* do, phrased as a report of

  what it *had* done. Three consecutive dispatches did the same. Each looked like progress.

Neither was caught by the report's tone; both would have been caught by its evidence.

## The Contract

For **every task you tick**, the report must carry:

- **The files** created or modified, by path, relative to the application root.
- **The model changes**, by element name and type, for anything modelled rather than written.
- **The command and its result** — the exact build or test command run, and the tail of its output.

  Name what was proven, not that something passed.

- **Traceability confirmation** — that the traceability record was accepted with zero failures.

A report that omits any of these, for any ticked task, **is not a completion statement**. It is an
unfinished wave, and the honest thing to return.

## Rules That Follow From It

- **Never report a task done that you did not do.** If you are blocked, say so and stop, or ask.

  A placeholder describing intended work, phrased as completed work, is the single most expensive
  thing you can return — the next wave builds on it.

- **A green build is not a working feature.** Compilation proves syntax. If a task claims behaviour,

  something must have executed that behaviour — through the application's real startup path, not a
  harness assembled inside the test that bypasses it.

- **Inspect what regeneration produced; never edit generated output to make it look right.** That

  inverts the test: it stops checking whether the template is correct and starts checking whether
  the disk matches itself.

- **An unapplied destructive diff is a defect, not a chore.** If a regeneration would strip

  hand-written code, resolve it inside the wave that created it — protect the code with a
  code-management directive, or model it properly. Leaving it for later leaves a live fault in the
  tree, and the loss lands on whoever runs the generator next.

- **Verification that matters gets its own task and its own artifact.** Where a wave is a gate,

  make its check a task whose deliverable is a file on disk — a parity or evidence report — so
  verification is durable and reviewable rather than a sentence in a transcript.

## For The Orchestrator

Classify each wave report before acting on it. A report that violates this contract, or whose prose
disagrees with the spec's own recorded task state, is **not a usable report** — re-dispatch the wave
and name the specific deficiency. Trust in this order:

1. tool-verified state (recorded task completion and traceability)
2. evidence links (paths, commands, output)
3. prose

Reading the spec's recorded state to make that judgement is not re-verifying the wave's work; it is
checking that a report is a report.
