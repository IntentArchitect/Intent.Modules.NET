---
name: module-ecosystem-analyst
description: "Use after reference-app-builder produces a green reference application. Reads the reference app's actual generated code to understand what the Intent ecosystem already provides, then maps the remaining work to SDK building blocks and produces an Attack Plan: an ordered list of implementation increments with specific files, base classes, and success criteria. TRIGGER: when the reference app is green and the next step is planning the module implementation."
argument-hint: "[Pattern Document or path to it]"
---

# Module Ecosystem Analyst

## Purpose

Turn the Pattern Document from `tech-pattern-researcher` into a concrete Attack Plan: exactly what to scaffold, in what order, using which Intent SDK building blocks. This skill prevents wasted work by establishing the full implementation picture before a single line of module code is written.

## Governing Principle — Industry Standard First

Apply the same principle as `tech-pattern-researcher`: Intent SDK conventions are the standard for this layer. Only where the SDK is silent do we improvise — and every improvisation must be logged in the Pattern Document Decision Log with basis and rationale.

**Before doing any work:** read `[ModuleFolder]/PATTERN-DOCUMENT.md` and `[ModuleFolder]/ATTACK-PLAN.md` (if they exist). Check the Decision Log — do not re-derive any closed decision. Check Open Questions — close any that are now answerable. Check the Progress Tracker — do not re-implement work already marked ✅ Complete.

**After completing work:** update the Decision Log with new decisions, close resolved Open Questions, and update the Progress Tracker in `ATTACK-PLAN.md`.

## Musts

1. **Use `search_docs` to verify Intent SDK patterns** before assigning base classes or event types. Do not assume from memory — SDK APIs evolve.
2. **Read the reference app's actual generated output first.** The reference app was scaffolded with real Intent modules installed and SF run. Walk the generated files to discover what the ecosystem already provides — what is already there does not need to be re-generated. Use MCP tools to inspect the designer only to clarify element types or metadata, not as the primary source of what's generated.
3. **Read the MassTransit module structure as a reference** (`e:/Intent.Modules.NET/Modules/Intent.Modules.Eventing.MassTransit/`) — but only for structural conventions (imodspec shape, folder layout, NugetPackages.cs pattern). Never copy its template logic for a different technology.
4. **Assign an Intent SDK base class to every template** in the file list before producing the Attack Plan.
5. **Separate single-file templates from file-per-model templates.** File-per-model templates drive one output file per designer element instance (e.g. one consumer per handler). Single-file templates produce one output regardless of model count (e.g. NServiceBusConfiguration).
6. **Identify every Intent event** the module needs to subscribe to (e.g. `ContainerRegistrationRequest`, `AppSettingRegistrationRequest`, `TemplateDependancyRegistrationRequest`). These drive which FactoryExtensions to create.
7. **Order increments by dependency.** An increment that depends on a template from a previous increment must come after it. Increment 1 must be independently buildable and runnable.

## Must Nots

1. Never assign a template base class without verifying it exists in the SDK via `search_docs` or reading existing module code.
2. Never plan a FactoryExtension for a concern already handled by an existing module (e.g. Eventing.Contracts already registers `IIntegrationEventHandler<T>` — don't re-register it).
3. Never put NServiceBus (or any transport-specific) types in the dependency list for Eventing.Contracts templates — that module must stay transport-agnostic.
4. Never plan all increments at once without verifying Increment 1 success criteria are achievable with in-memory transport. If they aren't, revise the scope of Increment 1.
5. Never skip the MCP inspection step. Designer model state drives what templates fire and what models they receive.

---

## Phase 2.0.5 — Intent Mapping Validation

**Goal:** Verify that the stereotype and settings design from the Pattern Document covers every scenario in the Scenario Findings table. Find gaps before planning any templates.

This is the second adversarial pass. Phase 1.1.5 stress-tested the technology. This phase stress-tests the Intent design against the same scenarios.

### Tasks

For each row in the Pattern Document's Scenario Findings table:

1. **Simulate modelling it in the Intent designer.** What stereotype properties and settings would the developer fill in?
   - Walk through each field: is there a stereotype property for it? A module setting?
   - If not, the design has a gap — add the missing property or setting.

2. **Check for over-engineering.** Is there a stereotype property that no scenario exercises? If it serves no scenario, remove it — it will confuse users.

3. **Check for ambiguity.** Is there a scenario where two different stereotype configurations produce the same generated output? Tighten the design so each configuration maps to exactly one output.

4. **Check convention fallback.** For each optional stereotype property: what does the module generate when left blank? Does the fallback match what the technology actually does by default (from Scenario Findings)? If not, fix the default.

5. **Check the multi-instance case explicitly.** Can the developer model the "Two messages, different destinations" scenario entirely through stereotypes/settings without writing code? If not, the module is incomplete.

### When it doesn't click

If a mapping feels awkward — a stereotype that exists "just to make it work", a setting requiring knowledge of an internal convention, a generated output that needs a comment explaining its shape — **stop**. That friction is the signal that a "what if?" question hasn't been asked yet.

Go back to the scenario producing the awkward mapping and ask:
- What if this scenario was modelled differently in the designer?
- What if this stereotype wasn't needed — what would the technology do by default?
- What if the generated code took a different shape — would the technology still work?
- What does the technology's documentation say to configure explicitly vs. what should be inferred?

Only continue when the mapping feels natural: the developer models what they conceptually mean, the stereotype captures only what deviates from the default, and the generated code looks like what an experienced developer of that technology would write by hand.

If friction persists after 3+ "what if?" attempts, escalate: the Intent design needs adjustment. State the incompatibility and propose the revised design — do not hack through it.

### Output (add to Attack Plan — Section: Mapping Validation)

| Scenario | Stereotype/setting that covers it | Gap found | Resolution |
|---|---|---|---|
| Two Commands, different destinations | `Endpoint Name` on each Command | Initially missing | Added mandatory `Endpoint Name` on Integration Command stereotype |

---

## Phase 2.1 — Ecosystem Scan

**Goal:** Establish what the Intent ecosystem already provides so the new module only builds what is genuinely missing.

### Tasks

1. **Inspect Eventing.Contracts output.** Use `search_docs` or MCP to confirm what `Intent.Eventing.Contracts` already generates:
   - `IEventBus` / `IMessageBus` — the publish abstraction
   - `IIntegrationEventHandler<T>` — the handler abstraction
   - `MessageBusPublishBehaviour` — MediatR pipeline flush
   - Event / Command message POCOs
   - `*EventHandler` Application layer class (merge body)

   **Critical:** Do not re-generate anything from this list. The new module generates only the Infrastructure adapter layer.

2. **Identify the driving modeler modules.** Use `search_docs` to find which modeler packages expose the designer elements named in the Pattern Document's Designer Pattern section:
   - `Intent.Modelers.Eventing` — Message elements, Eventing Package
   - `Intent.Modelers.Services.EventInteractions` — Subscribe Integration Event, Publish Integration Event associations on Command/Query elements
   
   Confirm module IDs and minimum versions for the `.imodspec` dependencies block.

3. **Check the target application's designer via MCP.** Call `get_applications`, `get_designers`, then `get_designer_model_snapshot` on the relevant designer to see:
   - Are Message elements modeled in the Eventing Package?
   - Are Subscribe/Publish Integration Event associations present in the Services designer?
   - Are handler elements already modeled (and thus ready for the consumer adapter template to fire)?
   
   If none are modeled yet, note that designer modeling is a prerequisite for any template to fire.

4. **Identify Intent events to subscribe to.** For each generated file identified in the Pattern Document, determine how the module registers it into the host application:
   - DI container registration → `ContainerRegistrationRequest`
   - appsettings key → `AppSettingRegistrationRequest`
   - Startup extension method → `ServiceConfigurationRequest` or decorator on `IAppStartup`
   - NuGet package → `INugetPackages` implementation
   
   Use `search_docs` to confirm the correct event type and payload shape.

### Output (add to Attack Plan — Section: Ecosystem Dependencies)

```
Already provided by Eventing.Contracts (do not re-generate):
- [list]

Modeler modules required:
- [id] >= [version]

Designer state: [modeled / not yet modeled — note prereqs]

Intent events to subscribe to:
- [EventType] → drives [which file/concern]
```

---

## Phase 2.2 — Template Inventory

**Goal:** Assign each file from the Pattern Document to a concrete Intent SDK building block.

### Tasks

For every file in the Pattern Document's "Files to Generate" table, determine:

1. **Template base class:**
   - Single-file, no model → `IntentTemplateBase` (no generic param)
   - Single-file, driven by application settings → `IntentTemplateBase` with settings access
   - File-per-model, model is a designer element → `CSharpTemplateBase<TModel>` where `TModel` is the typed model class from the modeler (e.g. `IntegrationEventHandlerModel`)
   
   Use `search_docs` to find the correct typed model class for each designer element type.

2. **Registration class pattern:**
   - Every template needs a matching `*TemplateRegistration.cs`
   - File-per-model templates use `FilePerModelTemplateRegistration<TModel>` 
   - Single-file templates use `SingleFileListModel` or `NoModel`

3. **Merge vs. fully-generated:**
   - Merge body (developer writes business logic) → `[IntentManaged(Mode.Merge)]` at class level, `[IntentIgnoreBody]` on methods the developer fills
   - Fully generated → `[IntentManaged(Mode.Fully)]`

4. **FactoryExtension vs. template decision:**
   - If a concern adds code to a template owned by another module (e.g. adding a `services.AddXxx()` call to the host's `Startup`), it is a FactoryExtension — not a new template.
   - If it produces a standalone new file, it is a template.

### Output (add to Attack Plan — Section: Template Inventory)

| File | Base Class | Model Type | Registration | Managed |
|---|---|---|---|---|
| [FileName] | `CSharpTemplateBase<TModel>` | `IntegrationEventHandlerModel` | `FilePerModel` | `Fully` |
| [FileName] | `IntentTemplateBase` | (none) | `SingleFile` | `Fully` |

---

## Phase 2.3 — Module Blueprint

**Goal:** Define the complete module scaffold — everything needed before writing a single template.

### Tasks

1. **Module identity.** Decide:
   - Module ID: `Intent.[Technology].[Variant]` (e.g. `Intent.Eventing.NServiceBus`)
   - Display name
   - Version: start at `1.0.0`

2. **`.imodspec` dependencies block.** List every Intent module in the dependency graph:
   - `Intent.Common` (always)
   - `Intent.Common.CSharp` (always for C# output)
   - `Intent.Eventing.Contracts` (for eventing modules)
   - Modeler modules from Phase 2.1 scan
   - `Intent.OutputManager.RoslynWeaver` (for `[IntentManaged]` weaving)
   
   Use the MassTransit imodspec as a structural reference for minimum version format.

3. **NuGet package registration.** Identify which packages are always added vs. conditionally added based on settings. Map settings values to packages. Pattern: `NugetPackages.cs` implementing `INugetPackages`.

4. **Module settings.** If settings are needed (transport choice, endpoint name, connection string):
   - Define setting group ID (new GUID)
   - Define setting keys with default values
   - Pattern: `ModuleSettingsExtensions.cs` wrapping `IGroupSettings`

5. **Folder scaffold:**
   ```
   Intent.Modules.[Technology]/
   ├── Api/                    (StereotypeExtensions.cs if new stereotypes)
   ├── FactoryExtensions/      (one .cs per cross-cutting concern)
   ├── Settings/               (ModuleSettingsExtensions.cs)
   ├── Templates/
   │   ├── [TemplateName]/     (one folder per template)
   │   │   ├── *TemplatePartial.cs
   │   │   └── *TemplateRegistration.cs
   ├── NugetPackages.cs
   └── Intent.[Technology].imodspec
   ```

### Output (add to Attack Plan — Section: Module Blueprint)

Module ID, imodspec dependency list, NuGet packages table, settings definition, folder scaffold.

---

## Attack Plan Format

Write this document to `[ModuleFolder]/ATTACK-PLAN.md`. It is a **living document** — every subsequent skill reads it before working and updates it after concluding.

```markdown
# Attack Plan: [Module Name]

## Mapping Validation
[Phase 2.0.5 output — scenario coverage table]

## Ecosystem Dependencies
[Phase 2.1 output]

## Template Inventory
[Phase 2.2 output — table]

## Module Blueprint
[Phase 2.3 output — module ID, imodspec deps, NuGet packages, settings, folder scaffold]

## Progress Tracker
Updated by each skill after completing an increment. Status: ⬜ Not started / 🔄 In Progress / ✅ Complete / ❌ Blocked

| Increment | Status | Outcome / Blocker | Decisions made |
|---|---|---|---|
| Scaffold | ⬜ Not started | — | — |
| 1 | ⬜ Not started | — | — |
| 2 | ⬜ Not started | — | — |

## Implementation Increments

### Increment 1 — Scaffold + Core Infrastructure Files
**Goal:** Module compiles and Software Factory runs without errors. Core single-file templates fire.
**Designer prereq:** [minimum designer state needed — e.g. at least one Message element]
**Files:**
- [ ] Module scaffold (folder structure + empty imodspec)
- [ ] NugetPackages.cs
- [ ] [Single-file template 1] — e.g. NServiceBusConfiguration
- [ ] [Single-file template 2] — e.g. NServiceBusHostedService
- [ ] [Message bus template] — e.g. NServiceBusMessageBus
**Success criteria:**
- SF runs with exit code 0
- Generated files compile
- DI registration appears in host Startup

### Increment 2 — Per-Model Templates
**Goal:** Consumer adapter files generate for each modeled handler.
**Designer prereq:** At least one Subscribe Integration Event association modeled
**Files:**
- [ ] [Per-model consumer template] — e.g. *NServiceBusConsumer
**Success criteria:**
- One consumer file generated per modeled handler
- Consumer compiles and resolves IIntegrationEventHandler<T> from DI

### Increment 3 — Runtime Verification (In-Memory Transport)
**Goal:** Publishing an event via IEventBus results in the handler being called.
**Files:** (configuration changes only — no new templates)
**Success criteria:**
- [Per test strategy from Pattern Document]

### Increment 4 — Real Transport
**Goal:** Switch from in-memory to [transport from Requirements Summary].
**Files:**
- [ ] Transport-specific configuration (settings-driven)
- [ ] Updated NugetPackages.cs (conditional on transport setting)
**Success criteria:**
- Handler called via [RabbitMQ / Azure Service Bus / etc.] with Docker running

## Skills to Load per Increment
| Increment | Skills |
|---|---|
| 1 | `file-builder-expert`, `intent-module-orchestrator` |
| 2 | `file-builder-expert`, `intent-metadata-consumer` |
| 3 | (none — configuration only) |
| 4 | `file-builder-expert`, `intent-module-orchestrator` |
```

---

## Handoff

Once the Attack Plan is complete, load **`intent-module-builder`** and pass the Attack Plan as context. Start with Increment 1 only — do not jump ahead.
