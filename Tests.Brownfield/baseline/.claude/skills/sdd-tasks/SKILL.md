---
description: Translate the design's realization plan into a checkpointed task list with an explicit wave dependency graph, linked to requirements and model elements (tasks.md) — step 3 of the built-in SDD flow.
short-description: Translate the design into a checkpointed, wave-based task list — SDD step 3.
intent-updates: automatic
requiredTools:
  - read_spec
  - write_spec
  - advance_spec_phase
group: Spec-driven development
---

# SDD — Tasks

You are running the **tasks** step. Translate the design's Realization plan into `tasks.md` — a faithful translation of what the design already decided, not a fresh decomposition. Produce **granular, requirement-traced tasks** plus an **explicit wave dependency graph** so the cockpit can drive implementation autonomously.

## Honour the solution's governance

When the spec's framework declares governance documents — Spec Kit's `.specify/memory/constitution.md`, say — `read_spec` returns them as `governanceDocuments` and the Specs panel lists them. **Read them first and treat their rules as binding on what you produce.** They are the user's standing constraints, not background reading. If a requirement and a governance rule genuinely conflict, say so and ask rather than silently picking one.

## What to do

**First, resolve the slug — never guess it.** If you weren't handed an exact slug, call **`read_spec` with no `slug`**: it lists every spec in the solution with its `slug`, `phase` and progress. Match what the user named against that list. Specs live under `intent/.specs/` and are reachable **only** through `read_spec` — never glob or grep the workspace hunting for spec artifacts. If nothing there plausibly matches what they named, `ask_user_question` offering the closest specs as options rather than inventing a slug.

Tasks belong to a **Change** only. When the listing shows the spec's kind is `capability`, write nothing: say a Capability has no tasks, and that new work on it starts as a Change (`/sdd-run` with a description of the work).

1. `read_spec(slug, artifact: "design")` and `read_spec(slug, artifact: "requirements")`. Like requirements and design, this step is **read-only toward the model** — no designer scripts, no module installs, no Software Factory runs, no code changes; the tasks you write are what implementation will mutate.
2. **Write the task list** (`write_spec(slug, "tasks", <content>)`) — GitHub-style checkboxes grouped under **coherent work-item milestones** (typically a **Domain aggregate**, so the milestones line up with the aggregate-slice waves below — or a shared element several aggregates need). A milestone holds **both** the `[model]` and `[code]` tasks for its aggregate. Each **leaf** task:
   - carries a **type tag** — `[app]` / `[module]` / `[model]` / `[code]` — routing how it's implemented;
   - carries a **`(satisfies: …)`** list of requirement ids (`R3`, or a criterion `R3.4`) — the requirement → task traceability the cockpit reads;
   - has a **stable id** (`T1.1`, `T3.2`) — never renumbered; leave a gap if one is removed;
   - is followed by **indented detail bullets** spelling out the specifics: the exact designer + element + properties / associations for `[model]`, the operation body + behaviour (or the cases it must cover) for `[code]`.
   - when a `[model]` task **satisfies more than one requirement** and different members realize different ones, **name the realizing member(s) per requirement** in the detail bullets (e.g. `R5 (soft delete):
     IsDeleted attribute + SoftDelete() operation`). This per-requirement member map is what lets implementation record traceability **surgically** — linking each requirement to just the members that realize it, not the whole entity.

   **Module prerequisites are `[module]` tasks.** If the design's *Module & architecture prerequisites* section names any module to install, emit a `[module]` task for each — detail bullets carrying the **module id + the concrete version** the design recorded, and a `(satisfies: …)` for the cross-cutting requirement(s) the module realizes (soft delete, auditing, etc.). Placement is a **hard dependency**: put each `[module]` task in the **earliest wave whose `[model]` work applies that module's stereotype/capability** — usually the first wave — and **never later** than any task that relies on it, because a `[model]` task can only apply a stereotype the module has already installed. Installing a module is a **real, checkpointed action** — unlike generation and build (below), which are automatic — so it earns a checkbox; the implementing agent installs it before that wave's model work and ticks it.

   **An application prerequisite is an `[app]` task in the FIRST wave.** If the design's *Application prerequisites* names a codebase area with no application, emit one `[app]` task per area — detail bullets carrying the application **name and architecture** the design recorded, and the area it covers. It has to land before any task that writes into that area, because the application's output is what makes `write_file` work there, what a traceability file link is relative to, and what the Changes Review joins those files onto — so an area's `[code]` tasks are only reachable once it exists. Creating an application is a real, checkpointed action, so it earns a checkbox. Where the design instead records the user's decision to leave an area outside Intent, emit **no** task and don't second-guess it.

   **Never** write code-scaffolding tasks for what the Software Factory produces (classes, handlers, DTOs, DI) — reserve `[code]` for the hand-written operation bodies the design named, and the tests that cover them. Generation and the build/test pass are never tasks either: running the Software Factory is automatic once a wave's `[model]` tasks are done, and running the build/tests is automatic once a wave's `[code]` tasks are done — the implementing agent fixes and re-runs on failure before finishing the wave, with no checkbox for either step.

   **The Software Factory generates structure only — never business logic.** It produces the class / handler / DTO / DbContext / repository / controller scaffolding and the DI wiring, and stops there: it will **never** fill an operation body, a domain method body, a command / query handler body, a domain event handler, or any UI behaviour. **Never assume generation will implement logic in the Domain, Services or UI.** Every piece of behaviour the design calls for — each modeled operation body, domain method, handler, and UI interaction — MUST have its own explicit `[code]` task (plus the tests that cover it). If the design named behaviour and you can't point to a `[code]` task that implements it, the task list is incomplete — add the task.

   **Do** write explicit `[code]` tasks for authoring tests — a sibling task in the same wave as the logic it covers (e.g. `T3.2 [code] (satisfies: R2.1) — Write unit tests for GetProductsQueryHandler`), not a late catch-all. Test-writing is ordinary implementation work, not a final checkpoint. Say in the task that each test **cites the criteria it asserts in its title**, `[spec:<slug> R2.1]`, so it becomes the criterion's durable proof; prefer `(satisfies: R2.1)` criterion ids on test tasks for that reason. A test cites only the criteria its **assertions** prove: a test that merely checks how a collaborator or host is called proves no behaviour criterion, so give it no `[spec:…]` citation rather than borrowing one.

   **Keep each `[code]` task readable, and the wave's code volume workable.** An **implementation point** is one distinct piece of hand-written behaviour — an operation / handler / domain-method body, a UI interaction, or one cohesive group of tests (roughly one detail bullet). Split a task into **sibling `[code]` tasks** in the same wave whenever that gives cleaner requirement traceability or a more legible checklist — sibling tasks are cheap and encouraged. The **binding** size cap, though, is on the **wave's total** `[code]` volume (see the size rule under the wave graph below), because implementation dispatches **one** coding agent for the whole wave, not one per task.
3. **Write the wave dependency graph**

   Related modelling and coding for an aggregate belong **together in the same wave** — **never** split an aggregate's own modelling and coding into a model-only wave and a separate code-only wave, and never defer code to a late catch-all wave. Group and order with two rules, **dependency first**:
   - **Dependency (hard constraint).** Take requirement → requirement dependencies from the design's Realization-plan Depends-on column. No wave may reference a model element / type that a **later** wave creates. Bundling dependent aggregates into the **same** wave is safe and even removes this risk — a wave's `[model]` tasks are applied in dependency order (Domain → Services → UI) before its single generation, so one aggregate can reference another modelled alongside it. The constraint bites only when the size cap **forces** a split: then the depended-on aggregate — and any **shared / foundational** element several aggregates need — must land in the **same or an earlier** wave, never a later one.
   - **Size (bias correctly-sized waves).** A wave's `[code]` tasks must total no more than about **5–8 implementation points**, because implementation dispatches **one** coding agent for the **whole wave**. That total — not an aggregate count — is what decides where a wave ends: coalesce aggregates into the current wave while its combined code volume stays under the cap, and start a **new** wave once it would exceed it (or when a dependency split, above, forces one). Truly independent small aggregates with little code should **share** a wave, not each get their own; a single aggregate with a lot of hand-written behaviour may need a wave to itself.

   **Wave self-sufficiency — no forward references (this is non-negotiable, verify it explicitly).** Every task must be **fulfillable at the moment its wave runs**: it may reference only model elements / types / modules that already exist by then — created or installed in an **earlier** wave, or in the **same** wave (a wave's `[module]` installs run, then its `[model]` tasks, then generation, before its `[code]`). A task must **never** reference something that only a **later** wave creates. Concretely, this covers:
   - a `[model]` task that **applies a module-provided stereotype / capability** (soft delete, audit, security, …) — the `[module]` task that installs it must be in the **same or an earlier** wave;
   - a `[model]` element whose **type reference** points at another aggregate's element (a typed property, operation parameter/return, or DTO field) — that target element must be modelled in this wave or earlier;
   - an **association** whose other end is another aggregate's element;
   - a `[code]` body that **calls a domain method, handler, or operation** another task adds;
   - a mapping whose source/target lives in another aggregate.

   Walk each task and resolve every element / type / member it names against the wave order. If any reference is produced by a later wave, the graph is mis-ordered — move the depended-on aggregate to an earlier wave (or the dependent task to a later one) until **every** reference resolves within its own wave or a prior one. Prefer pulling a shared / foundational element into its own early wave over splitting an aggregate.

   **The classic violation — a cross-cutting concern applied to elements other waves create.** Applying a stereotype / setting / member to an element (e.g. `Secured` on each admin `Command` / `Query`, a validation, an audit or soft-delete stereotype) is only possible **once that element exists**. So do **not** gather that per-element application into an early "defaults" / "security" / "cross-cutting" wave whose task lists endpoints created in later waves — that is exactly the forward reference this rule forbids. Instead split the concern:
   - **Early foundational wave — standalone only.** Put in it only what references nothing a later wave creates: a new element that stands alone (e.g. an `Admin` `Role`), or a stereotype applied to an element that **already exists** (e.g. `Secured` on the Services **package** — the package is already there).
   - **Fold the per-element application into the wave that creates that element.** Securing `CreateProductCommand` is part of the Product wave's `[model]` task that adds it (name the stereotype + roles in that task's detail bullets), **not** a separate earlier task. Likewise applying an audit/soft-delete stereotype to an entity happens in the task that adds the entity.

   A cross-cutting requirement therefore normally **spans several waves** — one small foundational task plus a line inside each aggregate's own task — and its `satisfies` id appears on each of those tasks. It is **never** a single early wave that reaches forward into endpoints that don't exist yet.

   Every leaf task id must appear in exactly one wave (milestone parent rows are not waved).

   **State what each wave really depends on (optional `dependsOn`).** A wave may carry `"dependsOn": [<wave ids>]`, naming the **earlier** waves whose model elements, code or conventions it actually needs. Leave it **out** and the wave depends on every earlier wave, which is the safe default and how waves have always run. Add it only where you can say a wave is independent of some earlier ones — e.g. a small aggregate that references nothing from the waves before it. Never list a wave that is not earlier, and never omit a real dependency: a wrong `dependsOn` is worse than none.

   **Give UI work an early look-point.** When the requirements describe something the user will **see** (a screen, panel, layout, visual state), don't leave all the UI to the last wave, where a wrong layout is only found after everything else is built. Put the UI shell in an **early** wave built on **stub or sample data** — it may only reference what exists by then, so the real data wiring is a later wave's `[code]` task that replaces the stub. Mark that wave `"checkpoint": "<what the user should look at, in a few words>"`; the orchestrator pauses after it so the user can see the result and redirect before the rest is built. Use it only for UI the user will judge by eye — never for a back-end-only change, and at most one or two checkpoints per spec.
4. **Verify the wave graph before you finish** — and fix anything that fails:
   - Every catalogued requirement is covered by at least one task's `satisfies`.
   - Every module named in the design's *Module & architecture prerequisites* has a `[module]` task, placed no later than any `[model]` task that applies its stereotype/capability.
   - Every codebase area the design's *Application prerequisites* flags as having no application has an `[app]` task in the first wave — or the design records the user's decision to leave it outside Intent, in which case there is deliberately no task.
   - Every leaf task id appears in exactly one wave.
   - Any `dependsOn` names only earlier wave ids, and any wave that references another's elements lists that wave (or leaves `dependsOn` out).
   - **No task references an element / type / association target / member / operation — or a module-provided stereotype — that a later wave creates or installs** — every reference resolves within the task's own wave or an earlier one (the self-sufficiency rule above).
   - **Size tripwire (one per wave).** No wave's `[code]` tasks total more than ~5–8 implementation points — sum them per wave and split any wave that exceeds it (respecting the dependency rule), since one coding agent implements the whole wave. A wave with **no** `[code]`/test task is a smell (probably half a slice) — fold it into the wave that exercises it unless it is a genuinely standalone model change.

### Shape

````markdown
# Implementation Plan — <title>

## Tasks

- [ ] T0. Prerequisites
  - [ ] T0.1 [module] (satisfies: R5) — Install the soft-delete module
    - `Intent.Modules.SoftDelete` v1.2.3 (from the design's prerequisites); realizes R5, applied as a stereotype in T1.1
- [ ] T1. Catalogue aggregate
  - [ ] T1.1 [model] (satisfies: R1, R5) — Add `Product` entity (Domain)
    - R1: `Name: string` (+ `Max Length` 100 stereotype), `Price: decimal`, `Sku: string`; root of the `Catalogue` aggregate
    - R5 (soft delete): apply the module's `[Soft Delete]` stereotype to `Product` (module-first — no hand-modelled `IsDeleted`)
  - [ ] T1.2 [model] (satisfies: R1, R2) — Add `GetProducts` query (Services), maps to `Product`
    - Returns `ProductDTO[]`; advanced-map the DTO fields to the entity
  - [ ] T1.3 [code] (satisfies: R2.1) — Fill `GetProductsQueryHandler.Handle`
    - Query the repository, project to `ProductDTO`, apply the paging in criterion R2.1
  - [ ] T1.4 [code] (satisfies: R2.1) — Write unit tests for `GetProductsQueryHandler`
    - Cover the paging behaviour in criterion R2.1 and the empty-catalogue case
- [ ] T2. Category aggregate
  - [ ] T2.1 [model] (satisfies: R4) — Add `Category` entity (Domain), independent of the catalogue

## Task Dependency Graph

```json
{
  "waves": [
  { "id": 1, "tasks": ["T0.1", "T1.1", "T1.2", "T1.3", "T1.4", "T2.1"], "label": "Catalogue & Category" }
  ]
}
```
````

- `tasks.md` is rendered as a LIVE progress document: the wave graph becomes a progress banner, each
  `(satisfies: …)` id becomes a chip carrying that requirement's coverage, and the checkboxes are real —
  ticking one in the rendered document writes back to this file. That only works while the conventions hold:
  keep every task a `- [ ]` checkbox with its id first, keep `(satisfies: …)` on the task line itself
  (not only in the sub-bullets), and keep the graph's task ids identical to the checkbox ids.
- **`tasks.md` is ALWAYS plain `.md` — never MDX.** Those checkboxes *are* the wave-progress engine: the
  Specs panel ticks them in place, the implementation cockpit derives wave progress from them, and a Tasks
  reset clears them. An MDX `<Checklist>` would leave the cockpit showing a spec with zero tasks and no way
  to advance, so `write_spec` refuses `format: "mdx"` here. Rich blocks belong in `design`, not here.
- A wave is a vertical slice over **one or more** aggregates. Here the **Catalogue** and the independent **Category** aggregates are **coalesced into a single wave** (bias: fewer waves — both are small and neither references the other): the soft-delete module is installed first (`T0.1`) so its stereotype is available, then `Product`, `GetProducts` and `Category` are modelled (Domain before Services) and generated automatically, then the handler body and its tests (`T1.3`, `T1.4`) are written by a **single** coding agent, and the wave ends with an automatic build/test pass. Its `[code]` volume is ~2–3 implementation points, comfortably inside the ~5–8 cap. A **second** wave would appear only once this one's `[code]` tasks approached that cap, or once a later aggregate depended on an element an earlier wave creates. `label`, `dependsOn` and `checkpoint` are optional (`label` is the wave's name in the cockpit); task ids in the graph **must match** the checkbox ids exactly.

## Dispatched by /sdd-run

As a `/sdd-run` sub-agent, stop once `tasks.md` is written: the orchestrator raises the gate and runs the waves, so skip the section below and end with the report your instructions ask for, giving the wave count and shape.

## Then raise the phase gate

Tell the user the tasks are ready and that **`/sdd-implement`** will execute them wave by wave. Keep this to a **brief summary** — the wave count and shape, not the list itself; never paste `tasks.md` into the chat, because every surface reads the artifact off disk.

Then call **`advance_spec_phase(slug, artifact: "tasks", toPhase: "implementation")`** — this is the approval gate, and it works from every surface. In-app it raises an approval card and blocks until the user decides; under a spec whose manifest carries `gates: auto` it advances immediately. On **reject**, revise `tasks.md` per the user's feedback with `write_spec` and call the gate again. Phases advance **one gate at a time** (the tool refuses a jump), so this call moves the spec to **implementation** and nothing further — don't start implementing yourself.
