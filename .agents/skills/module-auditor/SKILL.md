---
name: module-auditor
description: "Independent, clean-context verification of module-build work against the frozen requirements. TRIGGER: at two gates — (1) after reference-app-builder produces a green reference app, and (2) after module-increment-loop finishes and before module-wrap-up. Spawns a fresh-context reviewer (Intent's create_sub_agent or the host harness's own sub-agent) that grades the actual artifacts against acceptance-spec.md, distrusting the trackers and the builder's self-report. The primary quality gate in autonomous runs. Never let the agent that built the work certify it."
---

# Module Auditor

## Purpose

The agent that built the work is the worst judge of it — it knows what it *intended*, so it reads a diff to
confirm intent rather than to catch divergence. This skill replaces that self-certification with an
**independent, clean-context reviewer** that grades the real output against the **frozen requirements**. In an
autonomous run it is the primary quality gate; in a gated run it runs *before* the developer checkpoint, so the
developer only ever reviews work that already survived an independent audit.

> **Governing rule (from `AGENTS.md`): never grade your own homework.** The reviewer must not inherit the
> implementer's context, rationalisations, or trackers. It sees the frozen spec and the ground truth — nothing
> the builder wrote *about* its own work.

## When to load — the two gates

| Gate | When | Question it answers |
|---|---|---|
| **Gate 1 — Reference architecture** | After `reference-app-builder` is green, before `module-ecosystem-analyst` | *Does the reference architecture actually match what the developer described?* Catches "we're about to automate the wrong thing." |
| **Gate 2 — Built module** | After `module-increment-loop`, before `module-wrap-up` | *Do the generated code, the designer model, and the running behaviour match the spec — with nothing out of scope missing or leaking?* |

For a sliced build, run Gate 2 **per slice** (against that slice's acceptance check) as each slice closes, and
once more across the whole system before wrap-up.

## Inputs (what the reviewer is given — and only this)

1. **The frozen `acceptance-spec.md`** — the requirements pinned at kickoff (slice acceptance checks +
   negative / out-of-scope checks). This is the rubric.
2. **Ground truth** — paths to the generated code, the running app (or how to run it), and access to the
   designer model (MCP) where reachable.
3. **The gate-specific brief** (below).

**Explicitly withheld:** `WORKING.md`, the Progress Tracker, the Decision Log, and any builder narrative about
what "should" be there. Those are self-reports; the retros show them asserting "done" over stubs and drift.

## Musts

1. **Run in a fresh context.** Spawn the reviewer via the available sub-agent mechanism — Intent's
   `create_sub_agent` **or** the host harness's own sub-agent (`Agent`) — per the harness-agnostic principle.
   Assume it works; if no sub-agent mechanism is available at all, run the audit in a deliberately reset frame
   and **log the limitation** to `RETROSPECTIVE.md` as an Intent/harness gap.
2. **Grade against the frozen spec, not the trackers.** Every finding cites a spec line and the observed ground
   truth. A tracker saying "✅ complete" is not evidence and must be ignored.
3. **Be adversarial.** The brief asks the reviewer to *find where the output diverges*, not to confirm it. Assume
   something is wrong until the artifact proves otherwise.
4. **Check the negatives too.** Assert that out-of-scope items are **absent** (a shipped-but-unasked feature is a
   deviation — the "scope leak"). Omission from a checklist is not the same as verified absence.
5. **Cover every reachable dimension** in one pass: generated **code** vs spec, designer **model** vs spec,
   **runtime** behaviour vs the acceptance check. If a dimension is unreachable (e.g. the sub-agent has no MCP
   access to the model, or cannot run the app), audit the rest and **record the un-audited dimension as an
   explicit gap** — never silently skip it.
6. **Write findings to `.module-builder/audit-findings.md`** in the format below, then hand back to the
   implementer. This is the durable channel — surviving compaction — not a chat message.
7. **Verify against the *current* spec.** If `acceptance-spec.md` changed, the audit is against the new version.
   A stale rubric produces a false pass.

## Must Nots

1. **Never let the implementing agent perform its own audit.** A "review pass" by the same context is not this
   skill — it is the failure mode this skill exists to remove.
2. **Never trust green compilation as conformance.** `dotnet build` exit 0 proves syntax, not that the output
   matches the spec (the retros found DI collisions, swallowed validation, and stub bodies all behind a green
   build). Read the diff and exercise the behaviour.
3. **Never resolve a spec ambiguity by fiat.** If the spec is unclear (not merely unmet), that is an
   **escalation**, not a finding to adjudicate — see the loop bounds.
4. **Never loop unbounded.** Respect the revise-and-re-audit cap.
5. **Never rewrite the code.** The auditor reports; the implementer revises. (Read-only reviewer persona.)

## The audit brief (template handed to the reviewer)

```
You are an independent reviewer with no prior context on this build. Your job is to find where the
delivered work does NOT match the requirements — assume divergence until proven otherwise.

RUBRIC (the only source of "correct"): <path to acceptance-spec.md>
GROUND TRUTH to inspect:
  - Generated code: <paths>
  - Running app: <how to run / endpoints to exercise>   (skip only if you cannot run it — then say so)
  - Designer model: <MCP access notes>                   (skip only if unreachable — then say so)
GATE: <Gate 1 reference-architecture | Gate 2 built-module>  → focus: <gate-specific focus below>

DO NOT read or trust WORKING.md, progress trackers, decision logs, or any note the builder wrote about
its own work. Those are self-reports.

Check, in order:
  1. Every requirement / slice acceptance check in the rubric — met, partially met, or unmet? Cite the
     ground truth (file:line, endpoint response, model element) for each.
  2. Negative checks — are all out-of-scope items ABSENT? A present-but-unasked feature is a deviation.
  3. Wiring — does it actually run end-to-end (not just compile)? Exercise it.
  4. Any dimension you could not reach — name it as a gap.

Return findings ONLY in this shape (no prose preamble):
```

### Gate-specific focus

- **Gate 1 (reference architecture):** Does the hand-crafted reference app exercise the behaviour the spec
  describes, with the correct output shapes? Is anything the spec requires missing from the reference? Is the
  reference proving something the developer did *not* ask for? (We are about to automate this — it must be right.)
- **Gate 2 (built module):** Does the generated code reproduce the reference app's proven shapes *and* generate
  everything correctly for a from-scratch app? Is every slice acceptance check met at runtime? Is the designer
  model coherent? Any scope leak, stub, `NotImplementedException`, DI collision, or dropped merge-region?

## Output — `.module-builder/audit-findings.md`

Append a dated block per audit round. One row per finding:

```markdown
## [Date] — Gate [1|2] audit, round [n]

| # | Severity | Spec ref | Expected (spec) | Observed (ground truth) | Dimension | Status |
|---|---|---|---|---|---|---|
| 1 | blocker | §3.2 | Publish emits event to broker X | Handler stubbed, no publish call — Foo.cs:41 | code+runtime | open |
| 2 | scope-leak | §out-of-scope | request/reply NOT in scope | FooResponder.cs generated | code | open |
| 3 | gap | — | (runtime behaviour) | could not run app — no Docker in sub-agent | runtime | un-audited |

Verdict: [PASS | FAIL (n blockers) | INCONCLUSIVE (n un-audited dimensions)]
```

Severities: **blocker** (spec unmet / scope leak — must fix), **minor** (works but diverges from intent),
**gap** (a dimension the reviewer could not verify), **spec-ambiguity** (the rubric is unclear — escalate).

## The hand-back loop (bounds)

```
audit → findings written → implementer revises against audit-findings.md → re-audit
```

- **Cap: 2 revise-and-re-audit rounds.** If blockers remain after the second round, **escalate to the developer**
  with the outstanding findings (a Level 2+ pivot per the `build-module` Pivot Scale).
- **A `spec-ambiguity` finding escalates immediately** — do not loop. The reviewer and implementer disagreeing
  because the rubric is unclear is a signal to ask the developer, not to iterate.
- **Full-auto does not mean never surface.** Autonomy means the audit self-clears when it *passes*; it still
  escalates on the cap or on ambiguity. That is the whole point of the gate.
- On a clean **PASS**, mark the gate complete in `WORKING.md` and continue the chain (Gate 1 → ecosystem
  analysis; Gate 2 → wrap-up).

## Source of truth

- `acceptance-spec.md` (frozen at kickoff) — the rubric
- `.module-builder/audit-findings.md` — the findings ledger + hand-back channel
- `build-module` agent — Pivot Scale (escalation levels) and where the two gates sit in the chain
- `module-wrap-up` — will not declare done until Gate 2 returns PASS
