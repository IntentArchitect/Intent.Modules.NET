# Intent.Modules.NET — Copilot Architecture & Naming Directives

> **Scope:** Applies to all C# module code under `Modules/**`. Loaded automatically by VS Code Copilot.
> **Directives:** Enforces structural integrity, type safety, and mandatory build validation. These rules are **non-negotiable**.

---

## 📋 Context & Working State (Read First)

Before making **any** code changes, check for both `CONTEXT.md` and `WORKING.md` files and
read every one that exists and is relevant to the files you are about to touch.
The required order is:

1. Read `CONTEXT.md` first
2. Read `WORKING.md` second

Do not treat them as interchangeable. `CONTEXT.md` establishes the durable architecture and
constraints; `WORKING.md` tells you how the current branch/task fits inside that context.

### `CONTEXT.md`

`CONTEXT.md` is the **durable knowledge layer** for a module or area. It should capture
important architectural decisions, invariants, technology constraints, accepted patterns,
rejected approaches worth remembering, test/acceptance expectations, and commit references
that future AI sessions must not lose.

Read order:

1. **Repo root** — `CONTEXT.md` (cross-cutting repo knowledge)
2. **Same directory as files you are about to modify** — e.g.
   `Modules/Intent.Modules.Eventing.NServiceBus/CONTEXT.md`

Use `CONTEXT.md` for truths that should remain valid across multiple tasks and branches.
If your intended change conflicts with `CONTEXT.md`, stop and flag the conflict rather than
silently "improving" the design.

### `WORKING.md`

`WORKING.md` is the **temporary in-progress layer**. It captures active branch/task state:
current goals, known breakages, partial implementations, temporary decisions, and the
specific path the current work is taking.

Read order:

1. **Repo root** — `WORKING.md` (cross-cutting work spanning multiple modules or test apps)
2. **Same directory as files you are about to modify** — e.g.
   `Modules/Intent.Modules.Eventing.NServiceBus/WORKING.md`

These files are the authoritative record of active design decisions, rejected approaches,
and current known issues for the current work. Reading them is **mandatory, not optional**.
If what you are about to do contradicts something in a `WORKING.md`, stop and flag the
conflict rather than proceeding.

### Mandatory Comprehension Check

After reading the relevant `CONTEXT.md` and `WORKING.md` files, and **before making code
changes**, restate the following in 3-5 concise bullets:

1. the current goal
2. the key architectural constraints / non-goals
3. the primary file(s) you expect to modify
4. the validation/build steps you will use after the change

If you cannot clearly restate those points, do not proceed with code changes yet.

**Lifecycle:** `WORKING.md` files exist only while work is in progress. When a piece of work
is complete, the file is deleted or reduced, and any durable knowledge that should survive
must be extracted into `CONTEXT.md` (or a proper skill if appropriate). Do not create or
leave behind `WORKING.md` files for completed work.

---

## 🏷️ Naming Conventions & Standards

### FactoryExtensions & Templates
* **Suffix:** Use `*FactoryExtension` (e.g., `DomainConstraintsFactoryExtension`). One concern per extension; do not merge unrelated cross-cutting concerns.
* **Template Files:**
    * `*TemplatePartial.cs`: Contains constructor, model wiring, and metadata attachment.
    * `*TemplateBase.cs`: Generated; do not hand-edit except for scaffolded `AfterBuild` callbacks.
* **ID Handling:** Prefer using Template Role names (using string constants) over the template's `TemplateId` constant (static `const string`) for lookups. As last resort using hardcoded `TemplateId` strings.

---

## 🏗️ Architectural Rules

### 1 — Engineering Integrity
* **Scan Before You Name:** Search for existing patterns before creating new classes. `grep_search` → `semantic_search` → then decide. Prefer extending abstractions over parallel ones.
* **Access Modifiers:** Define all new types as `internal` by default. Only use `public` if explicitly required for the external API.
* **Shared Projects:** **Do not introduce `.shproj` / `.projitems`** for new components without explicit approval. Prefer a referenced `.csproj` with `PrivateAssets="All"`.

### 2 — Implementation Quality
* **Eliminate Magic Values:** Use `const` or `static readonly` fields. No inline magic numbers or strings.
* **Modern Strings:** Use **verbatim literals** (`@"..."`) for quotes and **raw string literals** (`"""..."""`) for multi-line blocks.
* **Builder API First:** When generating or modifying `CSharpFile` code, use the most specific builder API available. Treat raw `AddStatement`, rendered-text replacement, and `GetText()` rewrites as fallback techniques requiring an explicit reason.
* **Warning:** Never use global singletons for template-family scope (state must be clearable between Software Factory runs).

### 3 — Template Metadata & Priority Bands
* **Protocol:** Owning templates attach managers in constructors. External extensions use `TryGetMetadata`. Owning templates call `manager.ApplyRules()` in `AfterBuild` at priority `0`.
* **Execution Priorities:**
    | Band | Integer | Usage |
    | :--- | :--- | :--- |
    | **Core** | `0` | Owning template builds primary structure |
    | **Enrichment** | `100` | Same-module cross-cutting additions |
    | **Extension** | `500` | Factory extensions from other modules |
    | **Final** | `1000` | `FindMethod`/`FindClass` on fully-built output |

---

## 🚀 Lifecycle & Validation

### Lifecycle Contract
| Phase | Allowed Actions |
| :--- | :--- |
| `OnBeforeTemplateExecution` | Publish events (Registration Requests). **No CSharpFile mutation.** |
| `OnAfterTemplateRegistrations` | Find instances, schedule callbacks, register into managers. **No event publishing.** |
| `OnBuild` / `AfterBuild` | Mutate `CSharpFile`, read metadata, call `ApplyRules`. |

### Build Validation (Mandatory)
After **every** code change, verify the exit code is `0`:
```powershell
dotnet build "path/to/affected.csproj" --no-incremental --verbosity minimal --nologo
```

## 🤖 Available Skills

Specialized skills are auto-discovered from `.agents/skills/` (Copilot) and `.claude/skills/` (Claude Code, via the symlink created by `.agents/setup.ps1`). Use the relevant skill **before** generating code for that scenario — each skill contains Musts, Must Nots, pattern indexes, and a resource folder.

### Module Building Skills (use in sequence when building a new module)

| Skill | When to use |
| :--- | :--- |
| **module-kickoff** | **Start here for any new module.** Gathers requirements from the developer, validates sufficiency, produces a Requirements Summary. Do not proceed without it. |
| **tech-pattern-researcher** | After module-kickoff. Researches the technology in isolation, maps it to Clean Architecture, defines files to generate. Produces a Pattern Document. |
| **module-ecosystem-analyst** | After tech-pattern-researcher. Scans the Intent ecosystem (what Eventing.Contracts provides, which modeler modules drive generation, which SDK base classes to use). Produces an Attack Plan with ordered implementation increments. |
| **intent-module-builder** | After module-ecosystem-analyst. Uses MCP to scaffold the module in the Module Builder designer: creates template elements, factory extensions, NuGet declarations, runs SF to generate stubs. Produces a compiled module skeleton. |
| **module-increment-loop** | After intent-module-builder. Drives the iterative loop of implementing template bodies one increment at a time: change → SF on module → DLL deploy → SF on target → inspect → build → run → verify behaviour. Loops until the Attack Plan's increments are all verified. |

### Implementation Skills (use as needed during module implementation)

| Skill | When to use |
| :--- | :--- |
| **file-builder-expert** | Converting a C# class to a `CSharpFile` fluent template; writing `OnBuild`/`AfterBuild` callbacks; creating template registration classes; resolving types via `GetTypeName`/`UseType`. |
| **intent-mapping-architect** | Generating update/creation mappings from designer metadata; implementing `CSharpClassMappingManager`, `IMappingTypeResolver`, or `CSharpMappingBase`; handling recursive object/collection mapping. |
| **intent-metadata-consumer** | Reading stereotype properties to drive code generation; authoring or extending `*StereotypeExtensions.cs`; writing LINQ queries against typed model collections (`ClassModel`, `DTOModel`, etc.). |
| **intent-module-orchestrator** | Dispatching `ContainerRegistrationRequest` / `AppSettingRegistrationRequest` via `EventDispatcher`; finding and modifying templates from other modules; authoring `*FactoryExtension` classes; priority-band ordering. |
| **intent-domain-interactions-expert** | Authoring `IInteractionStrategy` implementations (query/create/update/delete entity, publish/send integration message, processing actions); wiring `method.ImplementInteractions(model)` from handler factory extensions; using `CSharpMapping` resolvers and `ExecutionPhases`. |

> **Maintenance:** Use the `refresh-intent-skills` prompt to audit skills against the latest SDK and update any stale patterns or resource files.

---

## ⚠️ Exception Guidelines

See `.agents/instructions/exception-guidelines.md` for the full decision table.

**Summary:**
- **`FriendlyException(string message)`** — user-facing, no element reference. For missing modules, invalid setting combinations. Supports Markdown.
- **`ElementException(model.InternalElement, string message)`** — user-facing, tied to a specific designer element. Intent Architect highlights the element in the UI. Supports Markdown.
- **`InvalidOperationException`** — developer-facing (module bug, unhandled enum value). Raw stack trace, not shown in a friendly panel.
- Generated code strings (`method.AddStatement(@"... ?? throw new InvalidOperationException(...)")`) are app-startup code — always stay as `InvalidOperationException`.

---

## 🛠️ Debugging & Troubleshooting

### Runtime Context Acquisition
If architectural or logic paths are unclear and require runtime context:
1. **Instrument the Code:** Add temporary log entries using `Intent.Utils`.
   * Example: `Logging.Log.Debug("Context: " + variable);`
2. **Request Execution:** Ask the user to run the module/Software Factory.
3. **Analyze Output:** Request the specific log output from the user before proceeding with further code changes.
