---
description: Turn requirements into intended Intent Architect MODEL changes plus a per-requirement realization plan (design.md) — step 2 of the built-in SDD flow.
short-description: Turn requirements into model changes plus a realization plan — SDD step 2.
intent-updates: automatic
requiredTools:
  - read_spec
  - write_spec
  - ask_user_question
  - advance_spec_phase
group: Spec-driven development
---

# SDD — Design

You are running the **design** step. IA is **model-driven** — the designer model is the source of truth and the Software Factory generates code from it. So `design.md` describes **model changes**, never code the generator owns.

## References — read only what you need

`read_file` these on demand (paths are relative to this skill's directory), never all of them up front.

| When | Reference |
| --- | --- |
| Step 2: the full inspection detail — module landscape, application ownership, designer rules, stereotypes, feasibility | `references/inspection.md` |
| Step 4: the section table, the `<ModelRef>` / `<FileRef>` conventions and the document self-review — **read before drafting** | `references/document-shape.md` |
| Loaded by `/sdd-extract`, or `/sdd-run` handed you a Capability | `references/as-built.md` — its deltas override the rules below |

## Honour the solution's governance

When the spec's framework declares governance documents (e.g. Spec Kit's `.specify/memory/constitution.md`), `read_spec` returns them as `governanceDocuments`. **Read them first and treat their rules as binding** — they are the user's standing constraints, not background reading. If a requirement and a governance rule genuinely conflict, say so and ask rather than silently picking one.

## As-built mode (extraction)

When `/sdd-extract` loads this skill, or `/sdd-run` hands you a Capability, the code is the truth. Read `references/as-built.md` before step 1; its deltas override the steps below, including the inspection.

## What to do

**First, resolve the slug — never guess it.** If you weren't handed an exact slug, call **`read_spec` with no `slug`**: it lists every spec with its `slug`, `phase` and progress; match what the user named against it. Specs live under `intent/.specs/` and are reachable **only** through `read_spec` — never glob or grep for spec artifacts. If nothing plausibly matches, `ask_user_question` offering the closest specs rather than inventing a slug.

1. **`read_spec(slug, artifact: "requirements")`** and read the whole document — its framing sections are **design inputs**; ignoring them is how a design ends up correct against the criteria and wrong against the feature:
   - **Glossary** — candidate Domain elements and multiplicities. **Name modeled elements after the Glossary terms**, no synonyms; if a term already exists in the model under another name, say so rather than model it twice.
   - **Non-Goals** / **MVP Scope → Out of Scope** — a hard boundary: never model capability listed there, however cheap; if one is unavoidable, put it to the user.
   - **Key User Journeys (`UJ-N`)** — every beat needs a modeled operation (command / query / UI interaction); a journey with no path through the design is a hole, even when every criterion is covered.
   - An imported spec (e.g. a BMAD PRD) carries the same material under its own headings; read it the same way.
   - **Externally-owned requirements are read-only.** For an imported spec `read_spec` returns the framework's documents with `externallyOwned: true` — that IS the requirements artifact. Never write or "normalize" a `requirements.md` for it (`write_spec` refuses; a copy would drift). If they are wrong, say so and stop — they are fixed in the owning tool and re-imported.
2. **Inspect before you design — strictly read-only**: no designer scripts, module installs, Software Factory runs or code changes. **Read `references/inspection.md`** — it holds the why and the edge cases. In short:
   - Load **exploring-the-model** and **changing-the-model**; propose only changes **expressible through that mutation flow**.
   - `listInstalledModules()` — fit its patterns. For each **cross-cutting** requirement (soft delete, auditing, concurrency, security…) use an installed module, else `searchAvailableModules()`; **prefer a module over hand-modelling**. **Install nothing** — record a prerequisite and confirm it with the user.
   - `get_applications` — a codebase area with code but **no** owning application is a gap: `ask_user_question` to add one (a prerequisite) or knowingly leave it outside Intent.
   - Per designer you will mutate: at minimum **`getDesignerSchema()`** + **`getDesignerModelStructure()`**; design only what the schema permits.
   - **Name the exact stereotype and property values** that carry each requirement's bounds into generated code.

   **Resolve every genuine design trade-off with the user before you write — this step may not finish with open questions**, including every module and architecture decision. Use `ask_user_question` when there's a small set of plausible answers, plain chat only for genuinely open-ended ones; ask in focused rounds and do **not** start writing `design.md` while a question is open. If the user won't decide a point, pick a sensible default and record it as an assumption.

   **The same gate covers feasibility.** A novel realization, one that fights an installed pattern, or one you doubt the model can express cleanly must be **stress-tested against the actual mutation flow**, not asserted; if it fails, re-approach it or put alternatives to the user. **Design may not finish with an unresolved feasibility risk** — record the resolution (or assumption) in `design.md`.
3. Write `design.md` via **`write_spec(slug, "design", <content>)`** containing:
   - **Application prerequisites** — every application the work lands in; for an uncovered area, **"add application \<name\> (\<architecture\>) for \<area\>"** (the tasks step schedules it before any work there) or the user's decision to leave it outside Intent, recorded as a choice. If all are covered, say so.
   - **Module & architecture prerequisites** — up front: id + concrete version from `searchAvailableModules()`, the requirement(s) it satisfies, and why over hand-modelling. The tasks step installs each (and re-runs the Software Factory) **before** the model work depending on its stereotypes. If none, say so.
   - **Model changes** — designers / elements / associations / stereotypes to add or change, **naming the stereotypes and property values each element/member carries**.
   - A **per-requirement Realization plan**. **Every requirement names a concrete realization mechanism**, exactly one of: **(a)** an installed / prerequisite **module capability or stereotype** (preferred), **(b)** a **modeled element type the schema defines** (name it), or **(c)** **hand-written code behind a code-management _ignore_ directive** for logic the model can't own. Decide the model-vs-code boundary **here** — logic with no modeled operation otherwise has no sanctioned home. Classify the work into the task-type tags:
     - **`[model]`** — designer-model changes; **most of the work**. The Software Factory generates its code (entities, handlers, DTOs, DbContext, repositories, controllers, DI) — never hand-written, never its own task.
     - **`[code]`** — the _only_ hand-written code: business logic filling modeled operation bodies, its tests, and mechanism (c) logic protected by an _ignore_ directive. Say which files/regions.

     Cross-cutting requirements are **module-first** (name the module's stereotype/setting on the owning element); only with **no module**, hand-model members on the entity another requirement owns and **name them** (soft delete → an `IsDeleted` attribute + a `SoftDelete()` operation). This member-level intent lets the tasks step build a per-requirement member map, so traceability links just what realizes a requirement, not the whole entity.
   - **Cross-requirement dependencies** — only genuine ones (e.g. R3's handler calls the domain method R1 adds). Facts, not a schedule: sequencing and waves belong to the tasks step.
   - Keep structural scaffolding out of `[code]` — if the Software Factory generates it, it isn't a task.
4. **Write it as MDX** (the `design` default). **`use_skill` authoring-rich-documents BEFORE you draft** — never MDX from memory; a bare `<` or `{` in prose (`List<Order>`) fails the whole document. `format: "md"` only if the user asked for plain markdown. Read `references/document-shape.md` for the section table. **Write the sections in the table's order, so the design reads from the high level down to the detail, and lead every section that has a shape with its diagram** — a `` ```mermaid `` fence (or `<Wireframe>` for UI) first, prose and tables after; fall back to prose or a table only where a diagram would genuinely communicate worse. The per-entity `<DataModel>` and per-command `<ApiEndpoint>` detail blocks stay mandatory: they are the member-level intent `sdd-tasks` and traceability read. Every existing element is a `<ModelRef>` carrying `application`, every named file a `<FileRef>`. **Link a related spec by its slug**, `[[other-spec]]` or `[[other-spec/design#heading]]`, never by a path.

## Before you call write_spec — self-review

Fix anything that fails, then run the document self-review in `references/document-shape.md`:

- [ ] Every material decision (module / architecture included) is resolved with the user; **no design question is left open.**
- [ ] **Glossary** terms verbatim, **Non-Goals / out-of-MVP** respected, every **`UJ-N`** has a complete path.
- [ ] Each cross-cutting requirement checked against installed and available modules; module preferred, hand-modelled equivalent dropped.
- [ ] Every module to install first is a prerequisite with id + concrete version and its requirement(s).
- [ ] Every change fits the designer's rules (types, containment, cardinality, mapping) and names its stereotypes with values; **every requirement names its realization mechanism; every risky realization was stress-tested and resolved, not asserted.**
- [ ] Each requirement has a `[model]` / `[code]` plan and its cross-requirement dependencies.

## Dispatched by /sdd-run

As a `/sdd-run` sub-agent, stop once `design.md` is written: the orchestrator raises the gate, so skip the section below and end with the report your instructions ask for, naming any assumptions and module or architecture decisions.

## Then raise the phase gate

Tell the user the design is ready to review — a **brief summary** calling out any assumptions and any **module prerequisites / architecture decisions** you settled, so they can correct them. Never paste `design.md` into the chat; every surface reads the artifact off disk.

Then, with the artifact written and no question open, call **`advance_spec_phase(slug, artifact: "design", toPhase: "tasks")`** — the approval gate on every surface. In-app it blocks on an approval card; under `gates: auto` it advances immediately. On **reject**, revise `design.md` with `write_spec` and call the gate again. Phases advance **one gate at a time**, so this moves the spec to **tasks** only — don't write tasks yourself.
