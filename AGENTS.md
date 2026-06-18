# Intent.Modules.NET — Copilot Architecture & Naming Directives

> **Scope:** Applies to all C# module code under `Modules/**`. Loaded automatically by VS Code Copilot.
> **Directives:** Enforces structural integrity, type safety, and mandatory build validation. These rules are **non-negotiable**.
> **Ignore:** The `InternalTestModules/` folder — it is unrelated infrastructure and should not be referenced or used as a target for module work.

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

`CONTEXT.md` is the **durable knowledge layer** for a module or area. It captures:
* Important architectural decisions, invariants, technology constraints, and accepted patterns.
* Which other modules this module affects and how they interact.
* The design decisions taken during implementation.
This ensures future AI sessions understand the historical context and architectural implications when modifying a module, especially when `WORKING.md` starts fresh.

Read order:
1. **Repo root** — `CONTEXT.md` (cross-cutting repo knowledge)
2. **Same directory as files you are about to modify** — e.g. `Modules/Intent.Modules.Eventing.NServiceBus/CONTEXT.md`

Use `CONTEXT.md` for truths that should remain valid across multiple tasks and branches. If your intended change conflicts with `CONTEXT.md`, stop and flag the conflict rather than silently "improving" the design.

### `WORKING.md`

`WORKING.md` is the **temporary in-progress layer** located **only at the repository root** (`/WORKING.md`). It captures:
* The active task/project state, current goals, known issues, and checklists spanning multiple modules or test apps.
* **What has been tried**: Specific solutions, approaches, or paths that were attempted and either discarded or selected, along with why. This prevents future AI runs from repeating failed paths.

Both `WORKING.md` and `CONTEXT.md` are **managed and maintained entirely by the AI, for the AI**. 

**Lifecycle:** 
* The root `WORKING.md` file exists only while the overall project work is in progress. When the project is complete, the file is deleted or cleared, and any durable knowledge that should survive is extracted into the relevant `CONTEXT.md` files. Do not create module-specific `WORKING.md` files inside module subdirectories.
* **Stale File Handler**: If a `WORKING.md` file exists, but you receive a new task/request that is completely unrelated to what is described in the `WORKING.md`, you must prompt the user immediately: *"I see there is a project in progress in WORKING.md. Do you want to discard it and start fresh, or modify the existing plan?"* before taking any actions.


### Mandatory Pre-Code Verification Gate

Before calling any file modification tool, you must output a single line specifying only the target class name and the targeted line numbers:
`Target: [ClassName.cs] | Lines [Start]-[End]` (e.g., `Target: OrderController.cs | Lines 45-60`).

Do not output any prose, explanations, or additional bullet points.

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

### 0 — Folder Boundaries (Non-Negotiable)

| Work type | Root folder | Rule |
| :--- | :--- | :--- |
| Module code (templates, factory extensions, NuGet declarations, imodspec) | `Modules/` | All module projects live here. Never create or edit module source outside this folder. |
| Test applications & reference architectures | `Tests/` | All Intent-managed test apps and hand-crafted reference apps live here. Never create test/reference app projects inside `Modules/`. |

**Before creating or editing any file**, confirm its folder:
- Changing a template, factory extension, or module metadata → `Modules/<ModuleName>/`
- Creating or running a test app, reference architecture, or SF target → `Tests/<AppName>/`

If a task would place module code in `Tests/` or app code in `Modules/`, **stop and ask** before proceeding.

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

### Template Body Changes — Mandatory SF Iteration Cycle

**Any edit to a `*TemplatePartial.cs` or `*FactoryExtension.cs` file MUST follow the full iteration cycle from `module-increment-loop`. No exceptions.**

The cycle is:
1. Edit the template body
2. `dotnet build <module.csproj>` → exit 0
3. `install_or_update_modules` — reinstall into the target app
4. `run_software_factory(target_app_id)` — run SF on the target app
   - **If SF fails with a transient error, retry once immediately.** Do not skip to manual edits. Only escalate to the user if the second attempt also fails.
5. `get_staged_file_diffs` — **read and confirm the staged output matches intent**
   - When reviewing `[IntentMerge]` files, diff the entire method body against the prior committed state, not just the new additions. Any line present before but absent from the staged diff is being dropped — confirm this is intentional.
6. `apply_staged_file_changes` — only after step 5 confirms correctness
7. `dotnet build <target.sln>` → exit 0

**NEVER edit files in `Tests/` to "fix" what a template generates, then run SF to confirm "0 staged changes."** Zero staged changes after pre-editing proves nothing — it just means the disk matches the template output, not that the output is correct. The staged diff inspection at step 5 is the only valid verification gate.

### Loop Close Gate (Non-Negotiable)

**Before starting any new template-touching work, confirm the previous SF cycle reached step 7.** If the cycle is open — SF was never retried after a failure, or steps 5–7 were skipped — close it first regardless of any other instruction.

If the user instructs you to commit or move on while a cycle is open:
1. Commit the module-side code only (steps 1–2 are safe to commit).
2. State explicitly: *"Steps 4–7 are still open. I will not start new template-touching work until the SF cycle is closed."*
3. Create or update `/WORKING.md` to record the open cycle so the next session can resume it.
4. Close the cycle before touching any `*TemplatePartial.cs` or `*FactoryExtension.cs` file again.

## 🤖 Available Skills

Specialized skills are auto-discovered from `.agents/skills/` (Copilot) and `.claude/skills/` (Claude Code, via the symlink created by `.agents/setup.ps1`). Use the relevant skill **before** generating code for that scenario — each skill contains Musts, Must Nots, pattern indexes, and a resource folder.

### Module Building Skills (use in sequence when building a new module)

| Skill | When to use |
| :--- | :--- |
| **module-kickoff** | **Start here for any new module.** Gathers requirements from the developer, validates sufficiency, produces a Requirements Summary. Do not proceed without it. |
| **tech-pattern-researcher** | After module-kickoff. Researches the technology in isolation, maps it to Clean Architecture, defines files to generate. Produces a Pattern Document. |
| **reference-app-builder** | **After tech-pattern-researcher, before module-ecosystem-analyst. Mandatory — never skip.** Scaffolds a real Intent-managed Clean Architecture application, installs the standard modules, runs the Software Factory, then hand-crafts the Wolverine/technology-specific files on top of the real generated output. Proves the code shapes compile and the handler is hit at runtime. The running app is the ground truth for all subsequent analysis. Hard gate — module-ecosystem-analyst cannot start until this is green. |
| **module-ecosystem-analyst** | **After reference-app-builder.** Uses the reference app's actual generated code — not abstract docs — to scan the Intent ecosystem: what existing modules already generate, which SDK building blocks to use, which designer elements drive generation. Produces an Attack Plan with ordered implementation increments. |
| **intent-module-builder** | After module-ecosystem-analyst. Uses MCP to scaffold the module in the Module Builder designer: creates template elements, factory extensions, NuGet declarations, runs SF to generate stubs. Produces a compiled module skeleton. |
| **module-increment-loop** | After intent-module-builder, AND whenever editing any existing template body or factory extension. Drives the iterative loop: change → build module → reinstall → SF on target → inspect staged diff → apply → build → run → verify. Must be followed for **any** `*TemplatePartial.cs` or `*FactoryExtension.cs` change, not only during new module builds. |

### Implementation Skills (use as needed during module implementation)

| Skill | When to use |
| :--- | :--- |
| **file-builder-expert** | Converting a C# class to a `CSharpFile` fluent template; writing `OnBuild`/`AfterBuild` callbacks; creating template registration classes; resolving types via `GetTypeName`/`UseType`. |
| **intent-mapping-architect** | Generating update/creation mappings from designer metadata; implementing `CSharpClassMappingManager`, `IMappingTypeResolver`, or `CSharpMappingBase`; handling recursive object/collection mapping. |
| **intent-metadata-consumer** | Reading stereotype properties to drive code generation; authoring or extending `*StereotypeExtensions.cs`; writing LINQ queries against typed model collections (`ClassModel`, `DTOModel`, etc.). |
| **intent-module-orchestrator** | Dispatching `ContainerRegistrationRequest` / `AppSettingRegistrationRequest` via `EventDispatcher`; finding and modifying templates from other modules; authoring `*FactoryExtension` classes; priority-band ordering. |
| **intent-domain-interactions-expert** | Authoring `IInteractionStrategy` implementations (query/create/update/delete entity, publish/send integration message, processing actions); wiring `method.ImplementInteractions(model)` from handler factory extensions; using `CSharpMapping` resolvers and `ExecutionPhases`. |
| **add-designer-extension** | Add a context menu item, new element type creation option, or association creation option to an existing element or package type from another module. |
| **add-association-type** | Define a new association type in a module, including source and target end configuration. |
| **intent-architect-mcp** | Intent Architect MCP workflow: designer operations, element discovery, model modification, Software Factory execution, compilation verification, and cross-module integration patterns. |
| **module-docs** | Complete the three release documentation artifacts for an Intent Architect .NET module (release-notes.md, README.md, and .imodspec). |

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
