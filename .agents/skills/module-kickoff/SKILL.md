---
name: module-kickoff
description: "Use at the very start of any new module build. Gathers and validates requirements from the developer before any analysis or implementation begins. TRIGGER: whenever the developer asks to build, create, or add a new Intent Architect module. BLOCK on this skill until all required answers are in hand — do not proceed to ecosystem analysis or implementation without a completed requirements summary."
argument-hint: "[module name or description]"
---

# Module Kickoff

## Purpose

Gather enough information upfront so that every subsequent step (pattern research, ecosystem analysis, implementation) can proceed without stopping to ask the developer for clarification. If requirements are insufficient, ask follow-up questions before moving on.

## Musts

1. **Ask all universal questions first.** Do not skip any — they apply to every module regardless of type.
2. **Determine the module type** from the developer's answers, then ask the applicable type-specific questions.
3. **Validate sufficiency** using the checklist at the end of this skill. If any item cannot be answered from what the developer has provided, ask targeted follow-up questions.
4. **Produce a Requirements Summary** as a structured output before handing off. This document is the input to `tech-pattern-researcher`.
5. **Ask follow-ups in one batch**, not one question at a time. Group all gaps into a single message.

## Must Nots

1. Never proceed to ecosystem analysis or implementation without a completed Requirements Summary.
2. Never assume a transport, library version, or architectural layer — always confirm.
3. Never accept "standard" or "the usual" as an answer — ask what that means in this context.
4. Never skip the sufficiency check even if answers seem complete at first read.

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
