---
name: tech-pattern-researcher
description: "Use after module-kickoff produces a Requirements Summary. Researches the technology in isolation, maps it to Clean Architecture, and defines exactly what the module must generate. TRIGGER: when a Requirements Summary is in hand and the next step is understanding how to implement it. Produces a Pattern Document — the input to reference-app-builder."
argument-hint: "[Requirements Summary or path to it]"
---

# Tech Pattern Researcher

## Purpose

Turn the Requirements Summary from `module-kickoff` into a precise Pattern Document: what the technology's native idioms are, how those map to Clean Architecture layers, and a concrete list of files the module must generate. This skill stops you from copying the wrong module's structural shape.

> **Sliced builds & offload:** when the build is sliced (`WORKING.md` slice map), research per slice's feature. This phase is context-heavy and read-only — a strong candidate to **delegate to a sub-agent** (see `module-building-strategies` → *Offload discovery to keep the orchestrator lean*).

## Governing Principle — Industry Standard First

The primary goal of every design decision is to produce output that matches what the technology's own documentation, community, and ecosystem consider correct. Map to Clean Architecture second — not the other way round.

- When the technology has an established pattern, use it exactly. Do not invent a variation that feels cleaner or mirrors another module.
- When Intent Architect imposes structure (CA layers, DI, appsettings), use Intent's established patterns as the standard for that concern.
- Improvise only where neither the technology's conventions nor Intent's conventions cover a case — and base the improvisation on the *principles* behind the relevant standards, not on borrowing from a different technology.
- **Record every improvisation in the Pattern Document Decision Log** with: why the standard is silent, what principle guided the improvisation, and what would change if the technology later adds a standard pattern.

### Verify Against Live Documentation — Do Not Trust Training Data

During the research and PoC phases (1.1 and 1.1.5), always confirm API usage and patterns against current sources:

1. **Use `context7`** (if available in the session) — fetch live library docs for the technology being researched. Training data may reflect outdated APIs, deprecated patterns, or pre-release behaviour.
2. **Use web search** — for anything context7 doesn't cover: community best-practices, GitHub issues, migration guides, known caveats, transport-specific conventions.
3. **Verify NuGet package versions against the live NuGet feed** — never trust training data for version numbers. Package versions publish frequently; training data is especially unreliable about which version of a library is compatible with a given framework version. Check the live NuGet page for each package before declaring a version.

This is not optional. A design based on stale training data can look correct but generate code against APIs that have moved. Verify before concluding any phase.

## Scope Batching — The Spanning Set Principle

There are two independent axes when tackling a module build, and they must not be conflated:

- **Process axis (how work is verified)** — the increment loop: one template change per Software Factory cycle. Batching here is forbidden — two changes in one cycle make a failure exponentially harder to isolate. Iterative is mandatory.
- **Scope axis (what a unit of work covers)** — the reference-app scenarios and the shapes they exercise. Batching here is desirable.

> **Batch the scope to span the distinct shapes; iterate the verification over them.**

The unit you batch is the **spanning set**: the minimal set of scenarios that, together, exercise every *distinct code shape* the module must generate. A "distinct shape" is a structurally different generated output — not a config-value variation of an output you already cover.

This bounds two opposite kinds of waste:
- **Re-discovery (expensive)** — scope too narrow → the ecosystem analyst plans blind to a shape → the increment loop hits it for the first time → the pattern is wrong → back to research.
- **Premature breadth (cheap but real)** — batching scenarios that re-prove the *same* shape → the same mechanism is proven several times for nothing.

The spanning set sits exactly between them. This principle is referenced by `reference-app-builder` (which builds the set) and `module-ecosystem-analyst` (which sequences it). Phase 1.1.5 is where the set is first identified.

---

## Research Mode Gate

**Fire this gate exactly twice:** once at skill entry, and once whenever a major pivot is detected (see below). Never fire it for routine mid-phase web searches or minor scenario corrections.

Before beginning any research, ask the user:

> "I'm about to research **[technology/topic]** online. Do you want me to proceed with the built-in search tools, or would you prefer a research prompt you can run in an external deep-research tool (e.g. Gemini Deep Research, Perplexity)?"

**If external:** produce a focused, copy-paste-ready research prompt (covering the technology profile, scenario stress-test questions, and known tension areas from the Requirements Summary), then **stop** — do not proceed with any phases until the user returns results.

**If built-in:** continue with Phase 1.1 as normal.

**Major pivot definition — re-fire the gate when any of these occur:**
- The entire Technology Profile needs to be rebuilt (not just one section updated)
- A different technology or library is being evaluated as a replacement
- The architectural approach changes fundamentally (e.g. from per-message-type adapters to a generic adapter, or a CA layer assignment is inverted)
- A tension in Phase 1.1.5 concludes as `Status: Design Adjustment Required` and the adjustment requires researching a new approach from scratch

A single scenario finding that updates one row in the Profile is **not** a major pivot — handle it in-flow without re-firing the gate.

---

## Musts

1. **Start from the reference sample.** Read the actual code — do not assume or recall patterns from training data. The Requirements Summary tells you where the sample lives.
2. **Ask "what is THIS technology's native pattern?"** before mapping to CA. Every technology solves the same problems differently. Understand the idiom first, then map it — not the other way round.
3. **Keep Application layer framework-agnostic.** If the technology has a handler interface (e.g. `IHandleMessages<T>`, `IConsumer<T>`), it must NOT appear in the Application layer. Only framework-agnostic abstractions from `Eventing.Contracts` (e.g. `IIntegrationEventHandler<T>`) belong there.
4. **Produce a concrete file list.** The Pattern Document must name every file the module will generate, which CA layer it lands in, and whether it is fully-generated or merge-body.
5. **Work through all four phases** (Tech in Isolation → Scenario Stress-Testing → Architecture Fit → Verify Runs) before producing the Pattern Document. Do not skip or merge phases.
6. **Record anti-patterns explicitly.** If you spot a tempting shortcut that would violate CA or copy another module's structure incorrectly, name it in the Pattern Document as a Must Not.
7. **Read the existing Pattern Document before starting** (if one exists). Check the Decision Log — do not re-derive or re-open any closed decision. Check Open Questions — close any that are now answerable.

## Must Nots

1. Never copy another module's structural shape without verifying it fits this technology's native idiom. MassTransit's generic wrapper pattern is wrong for NServiceBus. NServiceBus's concrete-per-message-type pattern may be wrong for the next technology.
2. Never assume DI registration, lifecycle management, or assembly scanning work the same way across transports. Always verify from the sample.
3. Never produce a Pattern Document that leaves any file in the output list without a layer assignment (Application / Infrastructure / Domain / API).
4. Never skip the "Verify Runs" phase. A Pattern Document without a testable first increment is incomplete.

---

## Phase 1.1 — Tech in Isolation

**Goal:** Understand how the technology works on its own terms, independent of Intent Architect or Clean Architecture.

### Tasks

1. **Read the reference sample** identified in the Requirements Summary (`U5`). Focus on the files that were hand-crafted (not generated by Intent).
2. **Fetch live documentation** before drawing any conclusions:
   - Use `context7` (if available) to load current API docs for the technology
   - Use web search for transport-specific conventions, migration notes, known caveats, and community patterns
   - Do not rely on training data alone — APIs change between major versions
3. **Identify the technology's core abstractions:**
   - What is the primary interface the technology exposes for sending / handling / consuming messages (or equivalent)?
   - How does the technology discover handlers / consumers — explicit registration or assembly scanning?
   - What is the lifecycle model — how is the endpoint or bus started and stopped?
   - How does the technology integrate with ASP.NET Core DI?
3. **List the key types** the technology requires you to implement or configure:
   - Entry point / bus / endpoint
   - Handler / consumer interface
   - Configuration / builder
   - Lifecycle / hosted service (if needed)
4. **Note serialization, transport, and retry defaults** relevant to the module settings.

### Output (add to Pattern Document — Section: Technology Profile)

```
- Primary send/publish API: [type name + method]
- Handler/consumer interface: [type name]
- Discovery model: [explicit registration | assembly scanning | attribute-driven]
- Lifecycle: [built-in | IHostedService required | other]
- DI integration: [native | external container helper | other]
- Key NuGet packages: [list from reference sample]
```

---

## Phase 1.1.5 — Scenario Stress-Testing

**Goal:** Discover where the technology's conventions, defaults, and multi-instance behaviour diverge from your Phase 1.1 understanding — before locking in any architecture decisions.

A mental model built on a single example is almost always incomplete. This phase is intentionally adversarial: its job is to break your Phase 1.1 assumptions.

### Mandatory scenario classes (work through all of them)

1. **Single message, single handler** — the hello world case; confirms basic wiring
2. **Two messages of the same type (e.g. two Commands), different destinations**
   - What happens to routing when both have no explicit configuration?
   - Do they share an endpoint or need separate ones?
   - What is the default convention and can it be overridden per-type?
3. **Two messages of different types (Command + Event) in the same application**
   - Does the technology treat them differently for routing/topology?
   - Can they share infrastructure, or must they be separated?
4. **Optional vs. required configuration**
   - What happens when a property is left blank/default? Fail fast or silent convention?
   - Which conventions are safe to rely on, and which cause silent conflicts in multi-message apps?
5. **Multi-service scenario** — publisher in Service A, subscriber in Service B
   - What must each side configure explicitly?
   - What is auto-discovered vs. what must match by convention?
6. **Conflict scenario** — two types whose defaults would collide (same name, same exchange/topic, same convention)
   - Does the technology detect this? Fail? Silently merge?
   - What is the escape hatch?
7. **Technology-specific edge case** — one scenario unique to this technology's model
   (e.g. for NServiceBus: what happens with `IHandleMessages<T>` when multiple handlers are in scope for the same message type?)

### For each scenario, answer

- What does the technology's **own documentation** say is the correct approach?
- What does the technology do by default (no explicit config)?
- What explicit configuration is required to make this work correctly?
- Does this reveal a constraint that contradicts my Phase 1.1 summary?
- Does this require a new stereotype property, a new setting, or a change to an existing one?
- If the technology has no documented pattern for this case: what do its *design principles* suggest — and is this an improvisation that must be logged?

### The Industry-vs-Intent Tension

Many technologies have idioms that appear to clash with Intent Architect's architectural requirements. **This is expected and normal — do not rush past it.**

When you find a tension (e.g. "the technology uses assembly scanning for discovery, but Intent generates one file per type which could conflict"), treat it as a signal to ask more "what if?" questions. Do not pick whichever option seems workable and move on.

**The design is not done until the tension resolves naturally** — meaning you can explain why both the technology's pattern and Intent's requirements are satisfied *without compromise*. A design that satisfies both by coincidence is not the same as one where you understand *why* they fit.

#### Tension resolution loop (repeat until resolved)

1. **Name the tension explicitly.** State what the technology requires and what Intent requires, and why they appear to conflict.
2. **Ask "what if?" for each side.** What if Intent generates X differently? What if the technology is configured Y way? What if this constraint is actually optional, not required?
3. **Look for the pivot.** Usually one assumption is wrong — either about what the technology actually requires, or about what Intent actually constrains. Find that assumption.
4. **Test the resolution.** Does the resolved design work for *all* scenarios in the stress-test table? If a scenario still breaks, the tension is not resolved — go back to step 1.
5. **If genuinely irresolvable — stop and escalate. Do not hack.**
   - Do not implement a workaround. Clever code that forces a technology to do something it was not designed for is worse than no solution — it creates maintenance debt, breaks the technology's upgrade path, and hides the incompatibility.
   - Go back to the Intent design. State clearly: *"This approach cannot be achieved with [library] without violating its design principles. The Intent designer model needs to be adjusted."*
   - Propose a design adjustment: describe what the technology CAN do cleanly, and what the revised Intent model would look like to match that.
   - Record the dead end in the Decision Log as `Status: Design Adjustment Required`.

**Stopping condition:** If you have asked 3+ distinct "what if?" questions about a tension, tried each path, and all lead to a hack or a violation of the technology's design principles — the loop is done. Escalate.

**Do not declare a tension resolved just because a workaround exists.** A workaround that adds complexity, surprises developers, or breaks in the next scenario is not a resolution.

### Revision rule

If any scenario answer contradicts your Phase 1.1 Technology Profile, **rewrite the affected Profile section before moving to Phase 1.2**. Unresolved tensions go to Open Questions — not silently absorbed.

### Output (add to Pattern Document — Section: Scenario Findings)

| Scenario | Default behaviour | Explicit config required | Mental model revision (if any) |
|---|---|---|---|
| Two Commands, different destinations | Each gets own endpoint by convention | Must register explicit endpoint names | `Endpoint Name` mandatory for Commands, not optional |
| ... | ... | ... | ... |

Do not proceed to Phase 1.2 until all scenario rows are filled and any revisions are applied.

### Distinct Shapes & Spanning Set Proposal

The stress-test above is where the module's distinct code shapes first become visible. Capture them and propose the spanning set (see **Scope Batching — The Spanning Set Principle**) that `reference-app-builder` will build.

1. **Classify each scenario by shape.** For every scenario (and every transport/host/dependency variant in the Requirements Summary E-questions and U10), mark whether it introduces a **new code shape** or is only a **config-value variation** of a shape already covered.
2. **Classify each variant's runtime dependency** using the same classes `reference-app-builder` uses: *AI-spinnable* (Docker — e.g. RabbitMQ, SQL Server), *developer-provided* (licensed/cloud — e.g. Azure Service Bus), or *none* (in-memory).
3. **Propose the spanning set.** Include a scenario when it is **both** shape-distinct **and** its dependency is AI-spinnable or none. Defer a scenario (with a recorded reason) when it only re-proves an existing shape, or its dependency is developer-provided and unconfirmed.
4. **Present the proposal to the developer for confirmation** before handoff. The developer may add or defer scenarios. Record the confirmed set.

**Structural model shapes are distinct shapes too.** When the module generates from domain / DTO / entity models, the model's OOP structure is its own shape axis — **composition, aggregates (independent relationships), inheritance, nullability, collections, operations, parameter lists, parameterized constructors.** Include the ones the target domain exercises in the spanning set; a module proven only against flat 2–3-property models silently breaks on richer ones. Model comprehensively, then let the developer cut.

Example (Wolverine eventing, transports = in-memory, RabbitMQ, Azure Service Bus):

| Scenario / variant | New shape or variation? | Dependency | Spanning set |
|---|---|---|---|
| In-memory pub/sub | New — consumer wiring, IEventBus impl | none | **Include** |
| RabbitMQ | New — transport config block, reflective driver registration | AI-spinnable | **Include** |
| Azure Service Bus | Variation — another transport-config branch value | developer-provided | **Defer** (reason: same mechanism as RabbitMQ; dependency unconfirmed; verify driver at runtime in a later pass) |

### Output (add to Pattern Document — Section: Spanning Set)

```
Distinct shapes identified: [list]
Confirmed spanning set (developer-approved):
  - [scenario] — [dependency class] — proves [shape]
Deferred scenarios:
  - [scenario] — reason: [redundant shape / developer-provided dependency]
```

---

## Phase 1.2 — Architecture Fit

**Goal:** Map the technology's native types to Clean Architecture layers without polluting Application with framework references.

### Tasks

1. **Draw the boundary.** For each type identified in Phase 1.1, decide: does the Application layer need to know about it, or can it be hidden behind an existing abstraction from `Eventing.Contracts` (e.g. `IIntegrationEventHandler<T>`, `IEventBus`)?

2. **Identify the adapter pattern.** When a framework-specific interface cannot appear in Application, what is the thin Infrastructure adapter that bridges it?
   - Example: Application has `OrderPlacedEventHandler : IIntegrationEventHandler<OrderPlacedEvent>`. Infrastructure has `OrderPlacedEventNServiceBusConsumer : IHandleMessages<OrderPlacedEvent>` — fully generated, calls the handler, auto-discovered.

3. **Check for per-instance vs. generic adapters.** Does this technology need one adapter class per message type (like NServiceBus), or a single generic adapter (like MassTransit)? This drives template cardinality — one template per handler model element vs. one shared template.

4. **Map every technology type to a layer:**

   | Technology Type | CA Layer | Notes |
   |---|---|---|
   | [e.g. NServiceBusMessageBus] | Infrastructure | Implements IEventBus from Eventing.Contracts |
   | [e.g. *NServiceBusConsumer] | Infrastructure | One per modeled handler, fully generated |
   | [e.g. *EventHandler] | Application | Merge body, developer writes business logic |
   | [e.g. NServiceBusConfiguration] | Infrastructure | Endpoint + transport setup |
   | [e.g. NServiceBusHostedService] | Infrastructure | Only if lifecycle needs IHostedService |

5. **Identify DI registration points.** What does the module need to call in `Program.cs` / `Startup.cs`? How is that registered — extension method, `AddXxx`, lambda? This will drive the `intent-module-orchestrator` skill later.

6. **Check appsettings needs.** What configuration keys (connection string, endpoint name, transport) does the technology need? These drive `AppSettingRegistrationRequest`.

### Output (add to Pattern Document — Section: Architecture Mapping)

Completed layer table from task 4, plus DI registration pattern and appsettings keys.

---

## Phase 1.3 — Verify Runs

**Goal:** Define a testable first increment and record the observable success criteria that `reference-app-builder` will use to gate the build.

> **Important:** This phase defines *what* success looks like and *what code shapes* prove it. The actual compilation and runtime verification is done by `reference-app-builder` — a mandatory skill that runs after `module-ecosystem-analyst` and before `intent-module-builder`. Phase 1.3 sets the target; `reference-app-builder` hits it.

### Tasks

1. **Identify the simplest transport or dispatch path.** Start with in-memory / local dispatch — no Docker, no external infrastructure. Confirm from the Requirements Summary (question E5) whether this approach was agreed.

2. **Define the minimal working scenario:**
   - What does the developer model in the designer to trigger generation? (Minimum: one message, one handler.)
   - What files does the Software Factory generate?
   - What manual wiring (if any) does the developer do after generation?
   - What is the observable proof that the handler was reached — a log line, a return value, a side effect?

3. **Write the reference code shapes.** For every file in the "Files to Generate" table, write the exact class skeleton the template must produce. These become the blueprints `reference-app-builder` will hand-craft into the test app:
   - Class name, namespace, method signature, return type, injected dependencies
   - DI registration block verbatim
   - No hand-wavy descriptions — actual C# skeletons

4. **List the success criteria for Increment 1** (used by both `reference-app-builder` and `module-increment-loop`):
   - `dotnet build` exits 0
   - Handler body is reached (log / return value / observable side effect)
   - DI resolves all registered types without exception at startup

5. **Note the graduation path.** After Increment 1 passes, what is Increment 2?

### Output (add to Pattern Document — Section: Test Strategy)

```
Increment 1 dispatch: [in-memory / local / learning transport]
Minimum designer model: [what the developer creates]
Expected generated files: [list with exact class/method skeletons]
Reference app location: [from Requirements Summary U9, or "to be scaffolded"]
Success criteria:
  - dotnet build exits 0
  - [observable handler proof]
  - [DI startup clean]
Increment 2: [next step after Increment 1 passes]
```

> **Gate:** The Pattern Document is not complete until the Test Strategy section contains concrete C# skeletons (not descriptions) for every generated file. `reference-app-builder` will copy these skeletons verbatim — vague shapes produce broken reference apps.

---

## Pattern Document Format

Write this document to `.module-builder/[ModuleName]/PATTERN-DOCUMENT.md`. It is a **temporary design document** used during the design and implementation phases — every subsequent skill reads it before working and updates it after concluding. Once the module's implementation is fully completed, all durable technical decisions and details are consolidated into `CONTEXT.md`, and `PATTERN-DOCUMENT.md` is deleted.


```markdown
# Pattern Document: [Module Name]

## Technology Profile
[Phase 1.1 output — native idioms, key types, discovery model, lifecycle, DI integration]

## Scenario Findings
[Phase 1.1.5 output — stress-test table]

| Scenario | Default behaviour | Explicit config required | Mental model revision (if any) |
|---|---|---|---|

## Spanning Set
[Phase 1.1.5 output — distinct shapes, developer-confirmed spanning set, deferred scenarios with reasons. Consumed by reference-app-builder (builds the set) and module-ecosystem-analyst (sequences it).]

## Architecture Mapping
[Phase 1.2 output — layer table, DI registration pattern, appsettings keys]

## Files to Generate
| File | Layer | Generated | Notes |
|---|---|---|---|
| [FileName] | [Layer] | Fully / Merge body | [Purpose] |

## Designer Pattern
[What the developer models in the Intent designer to trigger each generated file]
- Message element → [generates...]
- Integration Event Handler element → [generates...]
- Subscribe Integration Event association → [generates...]
- Publish Integration Event association → [generates...]

## Anti-Patterns (Must Nots)
[Explicit list of tempting wrong approaches identified during research]

## Test Strategy
[Phase 1.3 output — increments, success criteria, graduation path]

## Decision Log
Closed decisions — do not re-derive or re-open without explicit user instruction.

| # | Decision | Basis | Rationale | Closed by |
|---|---|---|---|---|
| 1 | [decision] | Standard — [source] / Intent Convention — [source] / Improvisation — [why standard is silent] | [why] | [phase/increment] |

Basis values:
- `Standard — [source]` — technology's own docs or community consensus
- `Intent Convention — [source]` — Intent's established module patterns
- `Improvisation — [why standard is silent]` — triggers mandatory rationale and a revisit note for when the technology later adds a standard

## Open Questions
Unresolved items carried forward — do not proceed past the blocking increment until resolved.

| # | Question | Blocking | Raised by |
|---|---|---|---|
| 1 | [question] | Increment N | [phase] |
```

---

## Handoff

Once the Pattern Document is complete, load **`reference-app-builder`** and pass the Pattern Document as context.

> The reference app comes next — not `module-ecosystem-analyst`. The app is scaffolded by Intent Architect with standard modules installed, then the technology-specific files are hand-crafted on top of the real generated output. This gives the ecosystem analyst real code to analyse rather than abstract docs. `module-ecosystem-analyst` runs after the reference app is green.
