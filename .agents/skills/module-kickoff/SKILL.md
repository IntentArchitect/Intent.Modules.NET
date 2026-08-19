---
name: module-kickoff
description: "Use at the very start of any new module build. Gathers and validates requirements from the developer before any analysis or implementation begins. TRIGGER: whenever the developer asks to build, create, or add a new Intent Architect module. BLOCK on this skill until all required answers are in hand — do not proceed to ecosystem analysis or implementation without a completed requirements summary."
argument-hint: "[module name or description]"
---

# Module Kickoff

## First Gate — Reference App (ask before anything else)

The reference-app rule (`AGENTS.md` top callout) is non-negotiable, so it is the **first** thing to settle — before questions, research, or planning. The moment the developer asks to build a module:

> *"Do you have a sample of what that output looks like — an existing app, hand-written code, or a repo I can learn from? If not, I'll build a small reference app first and show you what it would look like before writing any module code."*

- **Sample exists** → record its location (feeds U5 / U9); it becomes the ground truth.
- **No sample** → the AI scaffolds one in `reference-app-builder`; confirm with the developer what it must demonstrate.

Never skip ahead to templates assuming a sample will appear later. No reference app, no module.

## Entry Mode

Determine the input type before asking any questions:

**PRD provided** — developer has attached or pasted a requirements document:
1. Parse the document and map every section against the U1–U9 and type-specific questions below.
2. Mark each field `[from PRD]` if answered, `[MISSING]` if not.
3. Batch all `[MISSING]` gaps into a single targeted follow-up — do not ask one question at a time.
4. The Requirements Summary annotates each field with its source (`[from PRD]` or `[confirmed by developer]`).

**No PRD** — proceed directly to the Universal Questions below.

Both paths produce an identical Requirements Summary. All downstream skills are unaffected by which path was taken.

---

## Build Type — New vs Modify

Determine early whether this is a **new module** or a **modification of something existing** — a fix, an improvement, or a pivot on a prior decision. For a modification, read the existing module's `CONTEXT.md` first to establish current state and scope the *delta*.

Then triage a modification by **output impact** — this decides whether the reference-architecture gate applies downstream:

| Change | Example | Reference-output gate (`reference-app-builder`) |
|---|---|---|
| **Designer-only, no output impact** | Dialog/menu behaviour, designer-extension UX, validation message | ❌ Skip — verify the designer change works; no test-app proof needed |
| **Affects generated output** | Template / factory-extension change, new generated file, changed shape | ✅ Required — prove the output in a reference architecture *before* changing the module |

When output impact is genuinely ambiguous, **default to treating it as output-affecting** — verifying an unneeded change is cheaper than shipping an unverified one.

---

## Purpose

Gather enough information upfront so that every subsequent step (pattern research, ecosystem analysis, implementation) can proceed without stopping to ask the developer for clarification. If requirements are insufficient, ask follow-up questions before moving on.

## Musts

1. Ask all universal questions (U1–U10) first — applies to every module.
2. Determine module type from answers, then ask type-specific questions.
3. Validate using the sufficiency checklist. Ask targeted follow-ups for any gap.
4. Produce a Requirements Summary before handing off to `tech-pattern-researcher`.
5. Batch all follow-ups into one message — never one question at a time.

## Must Nots

1. Never proceed without a completed Requirements Summary.
2. Never assume transport, library version, or architectural layer — always confirm.
3. Never accept "standard" or "the usual" — ask specifically what that means.
4. Never skip the sufficiency check.

---

## Universal Questions

Ask these regardless of module type:

| # | Question | Why it matters |
|---|---|---|
| U1 | What technology or library is this module integrating? Include the name and target version. | Drives NuGet package declarations and API usage |
| U2 | What does this module generate? Describe the output files at a high level. | Scopes the template work |
| U3 | What does the developer model in the Intent designer? What elements or stereotypes do they create? | Determines whether new designer elements are needed |
| U4 | What existing Intent modules does this build on or depend on? | Drives dependency declarations and ecosystem analysis |
| U5 | Is there a reference sample, existing implementation, or hand-crafted code we can learn from? | Required input for `tech-pattern-researcher` |
| U6 | Is this a new standalone module or does it extend/replace something that already exists? | Determines whether we're creating or extending |
| U7 | What Clean Architecture layer(s) does the generated code belong in? (Domain / Application / Infrastructure / API) | Constrains where templates output and what they reference |
| U8 | What is the target .NET version? | Affects API choices and generated code |
| U9 | Is there an existing test/reference application the module can be verified against, or does one need to be created? If it exists, where is it? | **Mandatory for `reference-app-builder`.** The reference app is built or identified before any templates are written — without it there is no ground truth to verify against. |
| U10 | What platform modules, host types, and deployment environments must this module integrate with or support? For each integration target: (a) are bridging or companion modules needed? (b) are there known constraints with the chosen framework in that environment (e.g. serverless disk restrictions, startup entry-point overrides, codegen prerequisites)? Verify against current online documentation — do not rely on training data. | Drives the `<interoperability>` block in the `.imodspec` and the integration compatibility check in `reference-app-builder`. Framework–environment incompatibilities discovered after a full module build cost complete rework cycles. |

---

## Type-Specific Questions

Determine the module type from U1–U2, then ask the relevant section.

### Eventing / Messaging

| # | Question |
|---|---|
| E1 | What message patterns are needed — publish/subscribe, send/receive, or both? |
| E2 | What transports need to be supported? List all (e.g. RabbitMQ, Azure Service Bus, in-memory)? |
| E3 | How does the developer configure the transport — module settings, stereotypes, or both? |
| E4 | Are any advanced patterns needed — sagas, outbox, request/response, scheduled messages? |
| E5 | Should the module start with an in-memory/learning transport for testing, graduating to real infrastructure? |

### Persistence / ORM

| # | Question |
|---|---|
| P1 | What database and ORM? |
| P2 | Is a repository pattern and/or unit of work required? |
| P3 | How are schema migrations handled? |
| P4 | Does this need to integrate with an existing EF Core or DbContext module? |

### API / Web

| # | Question |
|---|---|
| A1 | What protocol — REST, GraphQL, gRPC? |
| A2 | What authentication/authorisation model? |
| A3 | Is API versioning required? |
| A4 | Does this extend an existing controller or endpoint template? |

### Infrastructure / Cross-cutting

| # | Question |
|---|---|
| I1 | What services does this register in the DI container? |
| I2 | What appsettings keys does this module need? |
| I3 | Does this add middleware to the request pipeline? |

---

## Sufficiency Checklist

Before producing the Requirements Summary, verify you can answer YES to every item:

- [ ] I know the exact NuGet package(s) and version(s) the module will reference.
- [ ] I know what files the module will generate and which Clean Architecture layer they land in.
- [ ] I know what the developer models in the designer (elements, stereotypes, or nothing new).
- [ ] I know which existing Intent modules this depends on or extends.
- [ ] I know where to find a reference sample or existing implementation.
- [ ] I know how to test a working output (what does success look like?).
- [ ] I know the target .NET version.
- [ ] I know whether a test/reference application already exists (U9). If not, I have confirmed with the user whether to scaffold one or whether they will provide it. **This item cannot be skipped — `reference-app-builder` is a mandatory chain step.**
- [ ] I know which platform modules, host types, and deployment environments the module must integrate with (U10), and I have verified online whether the chosen technology has known limitations in any of those environments.

If any item is NO — ask a targeted follow-up before proceeding.

---

## Size-up — how big, and where are the unknowns?

Before producing the summary, make one lightweight judgment call (a couple of sentences, **not** a scored matrix) on **two independent axes**. This shapes the whole run — record the outcome in `WORKING.md`.

| Axis | Question | Sets |
|---|---|---|
| **Scope size** | Small enough to build in one pass, or too big? | **Slicing** — if too big, decompose into vertical slices (features; may span modules), each with its own acceptance check, and write the **slice map** to `WORKING.md`. |
| **Certainty** | How many unknowns, and *where* — in getting the reference architecture right, or in the module space? | **Research depth** — the more unknown, the more `reference-app-builder` / increment cycling before module work. Requirement-unknowns → more elicitation here; technical-unknowns → more reference-app proving. |

The two are independent: a detailed PRD can still carry deep technical unknowns. Small + certain → near-direct build; big + unknown → many slices, each proven incrementally. Confidence also informs the autonomy default the AI *suggests* (Expectations Charter) — but the **developer always makes the final autonomy call**.

---

## Requirements Summary Format

Produce this document as the output of this skill. It becomes the input to `tech-pattern-researcher`.

```markdown
# Module Requirements: [Module Name]

## Technology
- Library: [name + version]
- .NET target: [version]
- NuGet packages: [list]

## What It Generates
[Description of output files and which layer they land in]

## Designer Impact
[What the developer models — new elements, stereotypes, or none]

## Dependencies
[Existing Intent modules this builds on]

## Reference Material
[Sample location, existing implementation, or hand-crafted code]

## Reference App
- Location: [path to existing test app, or "to be scaffolded by reference-app-builder"]
- Status: [exists and builds / needs scaffolding / user will provide]

## Module Type
[Eventing / Persistence / API / Infrastructure / Other]

## Type-Specific Notes
[Answers to type-specific questions]

## Definition of Done (First Increment)
[What a working first increment looks like — how we know it works]
```

---

## Freeze the Acceptance Spec

Once the Requirements Summary is complete, **freeze a copy as `.module-builder/acceptance-spec.md`** — the immutable rubric the independent `module-auditor` grades against. Beyond the summary fields it must capture:
- **Per-slice acceptance checks** — the observable "done looks like X" for each slice (or the single build).
- **Negative / out-of-scope checks** — what must be *absent*, so a shipped-but-unasked feature is caught as a scope leak.

Frozen means frozen: if requirements genuinely change mid-build, update the spec **deliberately** and note the change — the auditor always grades against the *current* spec, and drift between builder and spec is exactly the failure this prevents.

---

## Expectations Charter — present before handoff

Before diving into research and build, give the developer a short, scannable charter so there are no surprises. Cover:

- **Artifacts I'll produce** — Requirements Summary, Pattern Document, Attack Plan, and a durable `CONTEXT.md` that stays in the module folder. All transitory build files live under `.module-builder/` (`WORKING.md` for build state, `RETROSPECTIVE.md`, and per-module `PATTERN-DOCUMENT.md` / `ATTACK-PLAN.md`) and are cleaned up at the end. All of these are AI-managed.
- **The plan & the gates ahead** — a high-level playback of the phases and where I'll need a decision from you.
- **What I'll need from you** — especially any **developer-provided infrastructure or credentials** (cloud services, licensed brokers), surfaced now, not mid-build.
- **Why the test app comes first** — the reference/test app is the ground truth; without it the module is built blind, and it's the cheapest place to catch errors. If you can't supply one, I'll obtain or build one — **this step is never skipped**.
- **Autonomy mode — the developer decides, not the AI.** Ask which they want: **fully autonomous** (I proceed from what's set here and surface only when genuinely stuck or the `module-auditor` can't clear a gate) or **gated check-ins** at the reference-app, ecosystem, and pre-wrap-up gates. I may *suggest* a default from the Size-up confidence, but the choice is theirs — never assume it. Recorded in `WORKING.md` as `autonomy_mode` (see the `build-module` agent).
- **Resumability** — long runs can span sessions; I checkpoint to `.module-builder/WORKING.md`, so you can stop or redirect anytime without losing progress.

Keep it concise (a short list or table), confirm, then proceed.

---

## Handoff

Once the Requirements Summary is complete, load **`tech-pattern-researcher`** and pass the summary as context.

> **Note on reference app:** The Requirements Summary records the reference app status (U9) but does not build it. `reference-app-builder` is invoked immediately after `tech-pattern-researcher` produces the Pattern Document — before `module-ecosystem-analyst`. The ecosystem analyst reads the reference app's actual generated output to produce the Attack Plan; it cannot run until the reference app is green. Do not attempt to build the reference app during kickoff.
