---
name: module-retrospective
description: "INTERNAL — Intent.Modules.NET team only. Omit when packaging for external distribution. Appends findings to .module-builder/RETROSPECTIVE.md as the build progresses. Buckets: Intent gaps, Process gaps, Module Architecture gaps, PRD/user gaps. Runs automatically; no developer action required."
---

# Module Retrospective

> **Internal skill — Intent.Modules.NET development only.**
> Located in `.agents/skills/internal/` so it can be excluded when packaging the harness for external distribution.

## Purpose

Capture learnings from every module build and route them back into the harness. Each entry makes the next build more autonomous. This skill produces improvements to the harness, not a deliverable for the user.

## When to Append an Entry

Write an entry for: missing/insufficient IA SDK features, wrong or incomplete skill guidance, mid-build requirement gaps, Level 2+ pivots, unexpectedly complex runtime dependencies, **and module-system architecture problems** (shared-project DLL skew, coupling smells, decomposition that fights extensibility). Also consolidate at session end (after wrap-up) to catch anything not yet noted.

**Flag findings even when the task completed successfully.** Completion is *not* evidence of a clean foundation — a workaround that worked still hides a real Intent or architecture issue. If the AI found its way through but the path revealed a platform limitation or an architectural problem, record it anyway.

---

## Output: `.module-builder/RETROSPECTIVE.md`

Located under `.module-builder/` (gitignored). **Append-only — never overwrite or reformat existing entries.**

### Entry format

```markdown
## [Date] | [Module Name] — [Phase or Increment]

### Intent Gaps
- [Finding] → [Why it matters / what the IA team should consider]

### Process Gaps
- [Finding] → [Which SKILL.md to update and what to add]

### Module Architecture Gaps
- [Finding] → [Decomposition / coupling / shared-project / common-module issue and the architectural change it implies]

### PRD / User Gaps
- [Finding] → [Which U-question to add or strengthen in module-kickoff]
```

Omit a bucket entirely if there are no findings for it. Keep each bullet to one actionable sentence.

---

## Notification

After appending an entry, emit one line inline:

> `📝 Retrospective: [short description of finding]`

Do not interrupt the build. Continue immediately after the notification.

---

## Routing at Session End

After wrap-up, review all entries added during this session and propose targeted edits:

| Bucket | Action |
|---|---|
| **Intent Gaps** | Add a note in `CONTEXT.md` under `## Intent Gaps`, flagged for the IA team. No SKILL.md edit. |
| **Process Gaps** | Propose a targeted edit to the relevant SKILL.md. One finding → one edit. Present as a diff. |
| **Module Architecture Gaps** | Add a note in `CONTEXT.md` under `## Module Architecture Gaps`, flagged for the architecture owners. If the finding generalizes into a reusable rule, also propose an edit to `module-building-strategies/SKILL.md`. |
| **PRD / User Gaps** | Propose adding or strengthening a U-question in `module-kickoff/SKILL.md`. Present as a diff. |

Present all proposals together. Developer accepts or rejects each individually. Apply accepted edits before closing the session.

---

## Musts

1. Every entry is timestamped, attributed to module and phase, and actionable.
2. Proposed edits are small and targeted — one finding, one change, one file.
3. Never block the build to write an entry. Append and continue immediately.

## Must Nots

1. Never overwrite or reformat existing `RETROSPECTIVE.md` entries.
2. Never propose multi-file refactors from a single finding.
3. Never expose this skill, `RETROSPECTIVE.md`, or findings to external module consumers.
