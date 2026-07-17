# Intent.Modules.NET — Copilot Architecture & Naming Directives

> **Scope:** Applies to all C# module code under `Modules/**`. Loaded automatically by VS Code Copilot.
> **Directives:** Enforces structural integrity, type safety, and mandatory build validation. These rules are **non-negotiable**.
> **Ignore:** The `InternalTestModules/` folder — it is unrelated infrastructure and should not be referenced or used as a target for module work.

> **⛔ Reference App First — non-negotiable.** No module code (templates, factory extensions, NuGet declarations) may be written or modified until `reference-app-builder` has produced a reference application that builds with exit code 0 and exercises the handler at runtime. This is the single most important sequencing rule in the framework — it cannot be skipped under any circumstance. If the developer only says *"build a module that does X,"* establish the sample first (see `module-kickoff`).

---

## 📋 Context & Working State (Read First)

Before making **any** code changes, check for both `CONTEXT.md` and `WORKING.md` files and
read every one that exists and is relevant to the files you are about to touch.
The required order is:

1. Read `CONTEXT.md` first
2. Read `WORKING.md` second

Do not treat them as interchangeable. `CONTEXT.md` establishes the durable architecture and
constraints; `WORKING.md` tells you how the current branch/task fits inside that context.

### Where these live — `.module-builder/`

All **transitory** build artifacts live under **`.module-builder/`** at the repository root (gitignored — never committed):

| Artifact | Path | Scope |
|---|---|---|
| `WORKING.md` (global build state), `RETROSPECTIVE.md` | `.module-builder/<file>` | global — one each |
| `PATTERN-DOCUMENT.md`, `ATTACK-PLAN.md`, localized `WORKING.md` | `.module-builder/<ModuleName>/<file>` | per module being built |

`CONTEXT.md` is the **only durable** artifact and the **exception**: it stays in the **module project folder** (e.g. `Modules/Intent.Modules.X/CONTEXT.md`) — never under `.module-builder/`, never at the repo root.

**Transient-folder contract.** `.module-builder/` is **per-worktree** (never committed — so parallel worktrees never inherit each other's in-flight state) and **AI-managed**: the AI clears it **before and after** each task so the next task starts clean — **except `RETROSPECTIVE.md`**, which is append-only and never cleared (the developer harvests/commits it). The **PRD is developer-owned input**: the AI reads it and freezes an `acceptance-spec.md` from it, but never places or commits the PRD itself.

### `CONTEXT.md`

`CONTEXT.md` is the **durable knowledge layer** for a module or area. It captures:
* Important architectural decisions, invariants, technology constraints, and accepted patterns.
* Which other modules this module affects and how they interact.
* The design decisions taken during implementation.
This ensures future AI sessions understand the historical context and architectural implications when modifying a module, especially when `WORKING.md` starts fresh.

Location: `CONTEXT.md` lives **only inside module projects** — e.g. `Modules/Intent.Modules.Eventing.NServiceBus/CONTEXT.md`. **There is no root `CONTEXT.md`.** Read the `CONTEXT.md` of every module you are about to modify. (Cross-cutting, in-progress state lives in `.module-builder/WORKING.md` instead — never in a root `CONTEXT.md`.)

Use `CONTEXT.md` for truths that should remain valid across multiple tasks and branches. If your intended change conflicts with `CONTEXT.md`, stop and flag the conflict rather than silently "improving" the design.

### `WORKING.md` — the single canonical router / state file

There is **exactly one** canonical `WORKING.md` at **`.module-builder/WORKING.md`**. It is the **router**: the first file a resuming agent reads, telling it *where we are* and *the minimal set of files to load next*. Point a fresh session (after a stop, a compact, or an entirely new session) at `.module-builder/` and say "proceed" — it reads `WORKING.md` and continues from where the last one left off. (A lightweight per-module `.module-builder/<ModuleName>/WORKING.md` may still be used for a tiny isolated bugfix; it is *not* a second global router.)

It captures:
* **Where we are** — active phase / slice / increment, and what to load next (staged loading keeps context lean).
* **The slice map** (below) — for large builds, the feature slices, each with its status and acceptance check.
* **The module version ledger** — one row per module touched (`version · online-published? · bumped-this-session?`), so a module isn't re-bumped every iteration. The bump rules themselves live in `module-increment-loop`.
* **What has been tried** — approaches attempted and discarded/selected, with why, so no run repeats a dead end.

**Entry state-machine (run on every task entry):**

| Folder state | Action |
|---|---|
| Empty / clean | Fresh task — initialise the router and freeze the acceptance spec |
| In-progress, **matches** the request | **Resume** via the router |
| In-progress/complete, request **diverges** | 🛑 **Ask** — *"we were going in direction X; this looks like Y — new feature, extend, or start fresh?"* |
| Completed but **not cleared** | Previous task failed to clean up — clear the transient files (keep `RETROSPECTIVE.md`), then treat as fresh |

**Vertical slices (scaling to complexity).** When the scope of work is too big to build in one pass, decompose it into **vertical slices** — each a *feature* (which may span several modules) that can be modelled and proven end-to-end, carrying its own acceptance check. The slice is the tracking layer **above** the per-module `ATTACK-PLAN.md`; within a slice the spanning set covers distinct shapes, and each increment is one SF cycle: `PRD → slice → spanning set → increment`. Record the slice map in `WORKING.md`; the per-module Pattern-Doc and Attack-Plan are unchanged in scope.

Both `WORKING.md` and `CONTEXT.md` are **managed and maintained entirely by the AI, for the AI**. 

**Lifecycle:** 
* The `.module-builder/WORKING.md` file exists only while the overall project work is in progress. When the project is complete, the file is deleted or cleared, and any durable knowledge that should survive is extracted into the relevant `CONTEXT.md` files (which live in their module project folders). Per-module WORKING.md for minor/bugfix work goes under `.module-builder/<ModuleName>/WORKING.md` — never inside the module's own source folder.
* **Stale File Handler (WORKING.md is yours to maintain)**: `WORKING.md` is under your jurisdiction — keep it a faithful mirror of the user's current intent. If a `WORKING.md` exists but the incoming request **diverges** from what it documents — completely unrelated work, a changed objective, dropped/added scope, or a likely branch switch — do **not** plough ahead on stale assumptions. Prompt the user first, e.g. *"We were working on X in WORKING.md; this request looks like Y — are we still aligned, or did something change (branch, scope, direction)?"*, and act on their answer (discard, modify, or extend the plan) before taking any actions.


### Mandatory Pre-Code Verification Gate

Before calling any file modification tool, you must output a single line specifying only the target class name and the targeted line numbers:
`Target: [ClassName.cs] | Lines [Start]-[End]` (e.g., `Target: OrderController.cs | Lines 45-60`).

Do not output any prose, explanations, or additional bullet points.

---

## 💬 Developer Communication Style

All developer-facing output during module work must be **easy to scan — never a wall of text**:
- Lead with a concise summary; cut preamble and restated context.
- Prefer **tables, short sections, and bullets** over long paragraphs.
- Use **light, purposeful emoji** as visual anchors (✅ ❌ ⚠️ 📝) — not decoration.
- Surface decisions, blocks, and "what I need from you" explicitly and early.
- Developer-facing only — this does **not** apply to artifact files (CONTEXT.md, generated code, imodspec) or structured output another tool consumes.

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
3. Create or update `.module-builder/WORKING.md` to record the open cycle so the next session can resume it.
4. Close the cycle before touching any `*TemplatePartial.cs` or `*FactoryExtension.cs` file again.

### Docs Update (Mandatory)
After **every** change that adds, removes, or modifies user-facing behaviour — a new transport option, a new setting, a new generated file, a changed generated shape — update **both** artifacts in the **same turn** as the code change. Do not defer to a follow-up:

| Artifact | What to update |
| :--- | :--- |
| `docs/README.md` | Add/update the relevant section (transport table, settings table, generated code example, etc.) |
| `release-notes.md` | Add a bullet under the current version using `New Feature:` / `Improvement:` / `Fixed:` prefix |

Use the **`module-docs`** skill for the canonical format rules. A code change without a matching docs update is **incomplete**.

## 🤖 Available Skills

Skills are auto-discovered from `.agents/skills/` (Copilot) and `.claude/skills/` (Claude Code). Use the relevant skill **before** generating code — each skill contains Musts, Must Nots, pattern indexes, and a resource folder.

### Module Building Skills (use in sequence when building a new module)

> Reference App First gates this entire chain — see the ⛔ callout at the top of this file.

| Skill | When to use |
| :--- | :--- |
| **module-kickoff** | **Start here for any new module.** Gathers requirements from the developer, validates sufficiency, produces a Requirements Summary. Do not proceed without it. |
| **tech-pattern-researcher** | After module-kickoff. Researches the technology in isolation, maps it to Clean Architecture, defines files to generate. Produces a Pattern Document. |
| **reference-app-builder** | **HARD GATE — runs BEFORE `module-ecosystem-analyst` without exception. May loop for multiple scenarios.** Classifies runtime dependencies (AI-spinnable via Docker vs developer-provided), scaffolds a real Intent-managed Clean Architecture application, proves code shapes compile and the handler is hit at runtime. Additional scenarios and pivots are handled here before proceeding. The running app(s) are the ground truth that `module-ecosystem-analyst` reads. |
| **module-ecosystem-analyst** | **After all reference apps are green.** Uses the actual generated code — not abstract docs — to scan the Intent ecosystem: what existing modules generate, which SDK building blocks to use, which designer elements drive generation. Synthesizes across all reference app scenarios. Produces an Attack Plan. |
| **intent-module-builder** | After module-ecosystem-analyst. Uses MCP to scaffold the module in the Module Builder designer: creates template elements, factory extensions, NuGet declarations, runs SF to generate stubs. Produces a compiled module skeleton. |
| **module-increment-loop** | After intent-module-builder, AND whenever editing any existing template body or factory extension. Drives the iterative loop: change → build module → reinstall → SF on target → inspect staged diff → apply → build → run → verify. Must be followed for **any** `*TemplatePartial.cs` or `*FactoryExtension.cs` change, not only during new module builds. |
| **module-auditor** | **Independent verification — runs after the reference app is green and again before wrap-up.** A clean-context reviewer (via `create_sub_agent` or the host harness's own sub-agent) grades the actual output against the frozen `acceptance-spec.md`, distrusting the trackers and the builder's self-report. Deviations go to `.module-builder/audit-findings.md` and back to the implementer. The primary quality gate in autonomous runs. |
| **module-wrap-up** | **Final mandatory phase after all increments pass.** Version bump (assess impact, apply rule, align imodspec + csproj + designer), invoke `module-docs`, write `CONTEXT.md` (in the module folder), clear `.module-builder/WORKING.md`, confirm SF clean. Release notes header uses non-pre version. |
| **module-retrospective** | Runs automatically throughout every build. Appends findings to `.module-builder/RETROSPECTIVE.md` (append-only) whenever a workaround, skill gap, missing requirement, or architecture problem is encountered — even when the task still completed. Notifies the user with a one-line note. At session end proposes targeted edits to SKILL.md files and module-kickoff Q&A. Four buckets: Intent gaps (flag for IA team), Process gaps (update relevant skill), Module Architecture gaps (flag for architecture owners / `module-building-strategies`), PRD/user gaps (strengthen kickoff questions). |

---

### Implementation Skills (use as needed during module implementation)

| Skill | When to use |
| :--- | :--- |
| **module-building-strategies** | The accumulated strategic playbook — load at every design decision across the whole chain: module decomposition (root/bridging/common, shared-project avoidance, DLL-skew), template vs factory extension, file cardinality, managed modes, design-time config (setting vs stereotype), convention-vs-explicit, and two-phase (reproduce + from-scratch) verification. |
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
