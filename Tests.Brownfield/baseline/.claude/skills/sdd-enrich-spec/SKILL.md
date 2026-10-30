---
description: Write an Intent Architect companion document for a spec whose artifacts another framework owns (Spec Kit, Kiro, BMAD, OpenSpec) — model diagrams, live element links and the traceability story — WITHOUT touching the framework's own markdown.
short-description: Add an Intent companion document (diagrams, links, traceability) to a spec another framework owns.
intent-updates: automatic
requiredTools:
  - read_spec
  - write_spec
  - use_skill
group: Spec-driven development
---

# SDD — Enrich a spec with an Intent Architect companion

A spec whose documents belong to another framework is rendered, catalogued and traced by Intent already. What it does **not** have is the rich, live layer: model diagrams that redraw as the model changes, `ModelRef` chips that navigate into the designer, a `DataModel` view of what the design actually produced, the change set that realized it.

This skill adds that as a **companion** — a separate, Intent-owned `.mdx` that **cites** the framework's documents. It never rewrites them.

## The one hard rule

**Do not edit the framework's own files.** Not `spec.md`, not `plan.md`, not `tasks.md`, not the PRD. Spec Kit's templates regenerate those, other agents read them, and a rich block written into one is either destroyed on the next run or breaks the tool that owns it. `write_spec` will refuse a framework-owned artifact anyway — that refusal is the guardrail, not the plan.

The companion is additive. If the framework's document changes, the companion is re-authored; the framework's document is never reconciled backwards.

## What to do

1. **Resolve the slug — never guess it.** With no exact slug in hand, call `read_spec` with no `slug`: it lists every spec with its `slug`, `phase` and progress. If nothing plausibly matches, ask rather than invent.
2. **Read what is already true.** Do not restate it — cite it.
   - `read_spec(slug, artifact: "requirements")` — for a framework-owned spec this returns the framework's OWN documents (marked `externallyOwned`), plus any governance documents it declares.
   - `read_spec(slug, artifact: "design")` — likewise, the framework's plan and its supporting artifacts where it owns design.
   - `read_spec(slug, artifact: "coverage")` — which requirements are realized and by what.
   - `read_spec(slug, artifact: "traceability")` — the element/file links behind that coverage.
   - `read_spec(slug, artifact: "tasks")` — the framework's work list. For BMAD this is its epics document, and the spec's stages are its stories; naming the story a section belongs to is how a reader joins your companion to the card.
3. **Load the block reference.** `use_skill("authoring-rich-documents")`. Every rule about which blocks exist and what props they take lives there — this skill does not repeat them, and a block invented here will fail the write-time validator.
4. **Write the companion** with `write_spec(slug, artifact: "design", format: "mdx", ...)` when Intent owns design, or as a plain companion document alongside the spec when it does not. The write is validated as it lands and the verdict comes back in the feedback — fix what it reports.

## What belongs in it

Value the framework structurally cannot provide. If a section would just paraphrase `spec.md`, cut it.

- **A model diagram** of the aggregate / services the spec realizes, and a `DataModel` view of the entities behind it.
- **`ModelRef` chips** for each significant element, so a reader walks from a requirement into the designer.
- **The traceability story**: requirement → model element → generated code, with the gaps named honestly. This is the part no other tool has.
- **`ModelChanges`** for what the implementation actually changed, when the spec has been implemented.
- **Realization notes** the framework's document has no place for: which module supplies a stereotype, why an operation is hand-written rather than generated, where the generated surface stops.

### When the framework's design is an architecture spine (BMAD)

BMAD's design phase is its `ARCHITECTURE-SPINE.md`: numbered decisions (`AD-1`, `AD-2`), each with a `**Binds:**` line naming the FRs it serves. That is a good decision record and a poor realization plan — it says *what was decided*, never *which model elements express it*. The companion's job is precisely that missing half:

- **Decision → model.** One section per `AD-n`, citing the spine's own wording, then the elements that realize it as `ModelRef` chips. This is the join BMAD structurally cannot write: its story tasks say "Domain designer: add a `Payment` class" in a checkbox, with no field for an element.
- **Keep the spine's ids.** Cite `AD-1` verbatim rather than renumbering; the story specs' `-- realizes AD-n` suffixes and the trace candidates seeded from them are keyed on that spelling.
- **Say where the spine is silent.** A decision with no `Binds:` line, or an FR no decision claims, is a real hole worth naming — it is what a later `/sdd-verify` will find anyway.

Do **not** re-derive the decisions, re-argue the trade-offs, or restate the `Binds:` map as a table. Cite, then add the model layer.

## What does NOT belong in it

- A restatement of the requirements. They have a home and a catalog; a second copy drifts and gives one behaviour two defensible trace targets.
- A task list. `tasks.md` is the progress signal the cockpit ticks; a `Checklist` block beside it is a second, dead one.
- Anything you would have to keep in step by hand with a file the framework regenerates.

## Then stop

Report what you wrote and what you deliberately left to the framework's own documents. Do not advance the phase and do not tick anything.
