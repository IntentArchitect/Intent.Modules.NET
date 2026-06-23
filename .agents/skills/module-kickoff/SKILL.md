---
name: module-kickoff
description: "Use at the very start of any new module build. Gathers and validates requirements from the developer before any analysis or implementation begins. TRIGGER: whenever the developer asks to build, create, or add a new Intent Architect module. BLOCK on this skill until all required answers are in hand — do not proceed to ecosystem analysis or implementation without a completed requirements summary."
argument-hint: "[module name or description]"
---

# Module Kickoff

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

## Purpose

Gather enough information upfront so that every subsequent step (pattern research, ecosystem analysis, implementation) can proceed without stopping to ask the developer for clarification. If requirements are insufficient, ask follow-up questions before moving on.

## Musts

1. Ask all universal questions (U1–U9) first — applies to every module.
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

If any item is NO — ask a targeted follow-up before proceeding.

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

## Handoff

Once the Requirements Summary is complete, load **`tech-pattern-researcher`** and pass the summary as context.

> **Note on reference app:** The Requirements Summary records the reference app status (U9) but does not build it. `reference-app-builder` is invoked immediately after `tech-pattern-researcher` produces the Pattern Document — before `module-ecosystem-analyst`. The ecosystem analyst reads the reference app's actual generated output to produce the Attack Plan; it cannot run until the reference app is green. Do not attempt to build the reference app during kickoff.
