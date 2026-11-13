# Inspecting before you design — the detail

All of this is **read-only**: the design step inspects the model but never mutates it — no designer scripts, no module installs, no Software Factory runs, no code changes. Design decides _what_ will change; implementation is where it actually happens.

## The mutation flow

Load the **exploring-the-model** skill (the read tools and their notation) and the **changing-the-model** skill (how `run_designer_script` actually applies changes — operation ordering, containment, type/package references, associations & cardinality, mapping, and the validate-until-clean loop). The design may only propose changes that are **expressible through that mutation flow**, so understanding it now is what keeps the plan realisable.

## The module landscape — patterns _and_ capabilities

Call `listInstalledModules()` for the application: the installed modules **define its architecture and patterns** (CQRS vs services, persistence style, the designers, stereotypes, and generators available) — your design must fit them, not fight them.

Then, for every **cross-cutting / behavioural** requirement (soft delete, auditing, created/modified tracking, concurrency, multi-tenancy, validation, security, domain events, etc.), work out how it is best realized:

- (a) if an **installed** module already provides the capability, use it;
- (b) if not, `searchAvailableModules()` to discover whether an **available** module provides it.

**Prefer a module capability over hand-modelling it** — a soft-delete or auditing module generates the query filters, DbContext configuration, and stereotypes far more completely and correctly than a hand-added `IsDeleted` attribute or timestamp fields, so when a module covers the requirement, realize it **through that module and drop the hand-modelled equivalent** from the plan. Only hand-model the members when no module covers the requirement.

**Do not install anything in this step** — instead **record the module as a prerequisite** (id + a concrete version from `searchAvailableModules()`) and **confirm the choice with the user** (module and architecture decisions are exactly the kind of trade-off to put to them).

## Which application owns each part of the work — including the hand-written parts

Call `get_applications` (with an `applicationId` for each one's output location) and, for every codebase area this feature will touch, establish which application's **output** it lands in.

Any area that will carry code but sits inside **no** application's output — the classic case is a hand-written front-end beside a modelled backend — is a **gap you must resolve here**, because everything downstream is anchored on the application:

- `write_file` / `patch_file` refuse a path no application owns;
- a traceability file link has no output to be relative to (so each wave guesses a different base and records links that resolve nowhere);
- the Changes Review has nothing to join those files onto.

Put it to the user with `ask_user_question` — add an application for that area (an empty/minimal architecture is enough when nothing will be generated into it), or knowingly leave it outside Intent — and record the answer. When they choose to add one, it is a **prerequisite**, not something you create here: this step doesn't mutate the solution.

## The designer rules

**For every designer you intend to mutate, fetch at minimum its `getDesignerSchema()` and its `getDesignerModelStructure()`** — the schema is the designer's _rules_ (which element / association / stereotype types it defines, their settings and containment), the structure is what already exists — plus `getElementDetails()` / `searchElements()` where you need detail.

**Adhere to those rules:** design only elements, associations, stereotypes and mappings the schema permits — never a type the designer doesn't define, nor a containment / cardinality its rules forbid.

## The stereotypes — the design's precision

The schema lists each designer's stereotype definitions with their `applicableTo` (use `getStereotypeDefinitions()` if the schema truncated them). When you design an element or member, **inspect the applicable stereotypes and apply the ones the requirement calls for** — e.g. a max-length / required / default-value constraint on a string attribute, a primary-key or index stereotype, an HTTP verb / route on a command or query, a security stereotype naming roles.

Requirements carry concrete bounds (lengths, formats, ranges); a plain untyped member throws that precision away — **naming the exact stereotype (and its property values) in the design is what carries the bound into generated code.** For each modeled member, ask "which stereotype makes this constraint real?" and record it.

## Feasibility — stress-test, don't assert

A realization that is novel, fights an installed pattern, or that you are **not confident the model can express cleanly** must be **stress-tested against the actual mutation flow** — load **changing-the-model** and check the specific schema / containment / cardinality rules for that exact change. If it doesn't hold up, re-approach it or put the alternatives to the user. Implementation has no cheap way to recover from an unresolved feasibility risk, so record in `design.md` the resolution (or the assumption) you settled on.
