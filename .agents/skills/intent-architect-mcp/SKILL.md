---
name: intent-architect-mcp
description: >
  Intent Architect MCP workflow: designer operations, element discovery, model modification, 
  Software Factory execution, compilation verification, and cross-module integration patterns.
  If you find yourself wanting to edit `.xml`, `.config`, `.settings` files inside an `intent` metadata folder directly, perform the change via the IA MCP. This workflow is designed to keep models as the single source of truth — direct file edits are a last resort for truly exceptional cases.
---

# Skill: intent-architect-mcp

---

## Core Principle
Intent Architect models are the **source of truth**. The codebase is a generated artifact. Never infer model state from generated source code — MCP tools are authoritative. Never edit generated files directly when the same change can be modelled.

### What Must Always Be Modelled
- Method signatures, API contracts, DTO shapes, service interfaces
- Routing and endpoint definitions
- Persistence structure (schema, entity mappings, relationships)

### Allowed Exceptions (Bespoke Code)
Direct code editing is allowed only for:
- Method bodies inside handlers and services
- Dependency injection inside bespoke implementation constructors
- Bodies of repository methods and custom queries
- Business rules that cannot be expressed in models (rare — ask the user first)

Protect bespoke code from regeneration with `[IntentIgnoreBody]` on the **member** (not the class), or `[IntentManaged(Mode.Fully, Body = Mode.Ignore)]`. The signature stays generated; only the body is preserved.

If you find that the MCP did not allow you to make a change you needed, rather stop and ask the user to perform it for you. DO NOT alter the `.xml`, `.config`, `.settings` files yourself!

---

## Required Workflow

> **`*_rules` pre-call pattern (current MCP).** Several tools require a one-time companion `<tool>_rules` call first — **once per session**, before the tool: `get_tool_call_rules` (sub-agents, before *any* MCP tool), `get_designer_schema_rules` before `get_designer_schema`, `run_designer_script_rules` before `run_designer_script`, `create_solution_rules` before `create_solution`, and similar for SF / staged-diffs / app-settings. If a tool has a `_rules` sibling, call it once up front.

### 1. Bootstrap (every session, in order)
```
get_status(workingDirectory)
  → openSolutions empty AND foundButNotOpenSolutionPath null:
        not an IA project → stop (or, to create one: create_solution_rules → create_solution)
  → foundButNotOpenSolutionPath set: open_solution(absolutePath), then RE-CALL get_status
  → use the solution where isSuggestedSolution = true (else confirm with the user)
  → use the application where isSuggestedApplication = true (else get_applications and match by name/types)
    → get_designer_schema(applicationId, designerId)   ← call ONCE per designer, reuse the result
```

`get_designer_schema` does not change as you edit elements — **never re-fetch it mid-session**. Its **"Element types" block is the containment source of truth**: per type it lists the exact child specialization names you may `addChild`/`createElementUnder`, marks `!` where a type reference is **required** (including on a child ref, e.g. `Class: Attribute!`), and `¹` for max-one children. Use those exact names — don't guess a child type or skip a required type.

### 2. Finding Elements
- **Prefer `find_designer_elements`** (regex + specialization filter) whenever you know what you're looking for.
- Use `get_designer_model_structure` only when you genuinely need topology (e.g. all packages and their children). Always pass `specializations` or `packageId` to keep the response small.
- Use `get_designer_element_details` to inspect full details of a specific element before modifying it.
- **Before implementing a file-per-model template for a new model type**: call `get_designer_element_details` on the target model element and verify that every type reference the template needs is reachable from that model. For example, `IIntegrationCommandHandler<TCmd, TResponse>` requires both the command type AND the response type — if the model only carries the command, the template cannot generate a correct signature and the designer must be extended first (new stereotype property, new association type, etc.) before any template work begins.

### 3. Modifying Models — via designer scripts (`run_designer_script`)

Model mutation is done with **designer scripts**, not a per-operation tool. **Statement order is operation order** — write the steps in the order they must happen.

- **Resolve by name, never guess IDs:** `lookupByName` / `lookupByPath` / `lookupById`, `resolveType` / `setType`, `getPackages()` / `lookupPackage(name)`. Lookups are **editable-first with a reference fallback** — they match your editable types first, then types from referenced designers/packages. Referenced results are **read-only** (use as association endpoints / operation & attribute types / mapping ends; you cannot rename or restructure them). A null on a type you know exists is almost always a name/path typo — verify with `getPackages()` / the schema, not by guessing a flag.
- The result reports **`changes`** (created/updated/deleted, each with `elementId` / `name` / `specialization`) — populated **even when `executed` is false**, because a mid-script throw still commits the steps before it. On failure, read `changes` and continue from there — do not blindly re-create elements that already exist.
- The result reports **`errors`** (validation, addressed by `path` or `associationId`). A non-empty `errors` after a "successful" run means the model is invalid — fix it before moving on. You may also call `get_designer_validation_errors` explicitly; do not consider work complete while errors remain.
- **Scripts change the MODEL only — never lay out diagrams from a script** (no `layoutVisuals`, no positioning in-script). Diagram layout is a **separate step after** the mutation: `get_designer_diagram_snapshot` → `apply_change_diagram_layout` (by `elementId`, with x/y/width/height; keep **≥150px** gaps between associated elements).
- **Package references** are also changed in-script (no add/remove tool): discover with `getAvailablePackageReferences()` (or the read-only `list_available_package_references` tool), then on the owning package handle call `pkg.addReference(absolutePath, module?)` — pass `module` = the candidate's `source` when `sourceType === "module"`, omit for a solution package. Remove with `pkg.removeReference(...)`; list with `pkg.getReferences()` (or `get_designer_package_references`).

### 4. Software Factory
```
run_software_factory(applicationId)
  → inspect get_staged_file_diffs    ← ALWAYS diff before applying
  → apply_staged_file_changes
  → dotnet build to verify compilation
```

### 5. Stop Conditions
Task is complete only when **all** are true:
- The requested capability is represented in the appropriate designer(s)
- Software Factory has been applied successfully
- Codebase compiles and existing tests pass
- No `NotImplementedException` or TODO in new files
- Required bespoke logic is in place, and a follow-up SF run proposes no changes to it

### 6. Read-Only Branch
If the request is read-only (explain, summarize, compare, audit), follow steps 1–5 only. **Do not** modify models, run the Software Factory, or edit code.

### 7. Failure Recovery
If the Software Factory fails to apply, or applied changes don't compile, **report the failure to the user with the error** — do **not** patch generated code to make it compile. Patching masks the modelling error and is overwritten on the next regeneration. Fix the model, not the output.

### Creating Solutions / Applications
The MCP can scaffold new Intent solutions and applications: call `create_solution_rules` (once), then `create_solution` / `create_application`. This is the supported path for a from-scratch (Phase-2) verification app — attempt it via MCP first, and only ask the user to scaffold manually if the tools are unavailable.

### Element Comments
When modelling, add comments to elements for non-obvious design decisions — state the element's purpose and (for operations) its expected behaviour. This is passed to coding agents that implement the bodies.

### Reusing Saved Scripts (`includedScriptPaths`)
`get_scripts` lists available saved scripts; pass their `path` values in `includedScriptPaths` to prepend them as a library before your script body. To run a saved script with no extra logic, pass its path and an empty string for `script`. Author/save reusable scripts with `save_script`.

---

## Tool Calling Rules (IA MCP tools only)

- **Sub-agents: call `get_tool_call_rules` first**, before any other IA MCP tool. Honour the `*_rules` pre-call pattern (see top of Required Workflow).
- **NEVER call IA MCP tools in parallel** — they must be sequential
- **Every IA MCP call must include `intention`** — describe intent in ≤10 words (do NOT pass `intention` to host-native tools like Read/Edit/Bash)
- **Never invent IDs** — only use IDs returned by prior tool calls
- **Do not read or modify `.intent` / `intent` folders**
- **Do not infer model state from generated source code** — MCP tools are the source of truth
- Do not include IDs in plans shown to the user — reference by name and type

---

## Operation Ordering (in a designer script)

**Statement order is operation order** — just write the steps in sequence:
1. Create parents before children
2. Create both endpoints before creating an association between them
3. Create the element → add a stereotype → update its properties
4. Move all children to a new parent before deleting the old parent
5. To move an element: update its parent reference — **never delete + recreate**

If a designer rule is violated the operation fails — adjust and retry. A mid-script throw still commits prior steps (read `changes`).

---

## Designer Quick Reference

| Designer | Contents |
|---|---|
| **Services** | Commands, Queries, DTOs, Services, Operations (CQRS / API surface) |
| **Domain** | Entities, Value Objects, Aggregates, Repositories |
| **User Interface** | Pages, Components, Layouts |
| **Codebase Structure** | Folder/project layout, template output anchors |

Folder names in a designer map to namespaces or output paths — they may not match disk folders. Trust the designer.

---

## Known Gotchas

### Diagram Snapshots
`get_designer_diagram_snapshot(applicationId, designerId)` returns the **currently active diagram** for that designer — there is no `diagramId` parameter. If a designer has multiple diagrams and you need a non-active one, switch to it manually in the IA UI first.

### Stereotype Operations — In-Script: Add, Then Set
In a designer script, add the stereotype first, then set its property values (in order):
```js
const el = lookupByName("Order");
el.addStereotype("NServiceBus");                                              // add first
el.getStereotype("NServiceBus").getProperty("Endpoint Name").value = "orders"; // then set
```
`addStereotype` ignores `applicableTo` schema restrictions — intentionally exploitable (e.g. setting a property the UI would normally reject).

### Stereotype Definition Elements Not in Tree
`get_designer_model_structure` with `includeChildren: true` does **not** include Stereotype Definition elements — the Stereotypes folder is excluded from traversal. Access them by GUID via `get_designer_element_details`. Find the GUID from generated `.xml` files in the module source.

### `open_solution` — Parameter Is `absolutePath`, Not `solutionPath`
The required parameter name is `absolutePath`. Passing any other name (e.g. `solutionPath`) causes InputValidationError. When in doubt, fetch the schema via `ToolSearch("select:mcp__intent-architect__open_solution")` before calling.

### `install_or_update_modules` — Target Solution Must Be Open
This tool only works if the target application is in a currently-open IA solution. If it fails with a SignalR/object error, do **not** retry immediately. Instead:
1. Call `get_status` to see which solutions are currently open.
2. If the target app's solution is missing from `openSolutions`, call `open_solution(absolutePath: "<path-to-isln>")`.
3. Retry `install_or_update_modules` after the solution is confirmed open.

Fallback when MCP install is still unavailable after two attempts and a solution re-open: **stop and ask the developer to update the module from the Intent Architect UI Modules panel**. Do NOT attempt to hand-edit `modules.config` — the file format requires precise XML and a bad edit corrupts the application's module state.

### `get_staged_file_diffs` — Takes a `filePaths` Array of Absolute Paths
The parameter is `filePaths` (an array of **absolute** file paths). There is no glob parameter. Passing relative paths (as returned by `run_software_factory`) silently produces "Absolute file path required" per-file errors and shows 0 staged changes — no InputValidationError is raised, making the failure invisible. Passing a glob string will cause InputValidationError.

### Multiple Solutions — Safe, But Never the Same Path Twice
Intent Architect can have multiple distinct solutions open simultaneously (e.g. Modules solution + Tests solution). `get_status` lists all open solutions — check this before calling `open_solution` to decide which is needed. Do **not** open the same solution path a second time — there is a known IA bug where duplicate registrations cause unpredictable MCP behavior.

### Opening a Second Solution Mid-Session — Stale Context After Close
If you open a second IA solution during a session and then close it, the MCP server may retain stale staged-changes state for it. Subsequent `run_software_factory` calls on any application then fail with "solution no longer open" errors even for the original solution. The only reliable recovery is a full IA restart with only the target solution open. **Do not open reference solutions or sibling modules in the same IA instance during a session.** If you need to inspect a reference module's generated files, ask the user to check them on disk instead.

### SF Staged Changes — Diff First
If SF shows pending staged changes immediately after a designer edit, those may be **carry-over** from before your edit. Always call `get_staged_file_diffs` before `apply_staged_file_changes`. Applying without reviewing can silently revert your work.

### Module Installation — Never Copy DLLs
Never manually copy DLL files. The correct flow:
1. Compile the module `.csproj`
2. Build outputs packaged file to the configured module output folder
3. IA watches that folder and auto-detects + installs the new version

Manual DLL copying causes file lock errors and hot-reload issues.

### Module Deploy Loop — Compile Only When Already Installed
**Do NOT call `install_or_update_modules` on every iteration.** It is only needed when:
- The module is not yet installed in the target application, OR
- The module version has changed (imodspec version bump)

When the module is already installed at the correct version, the deploy loop is:
1. Edit template source
2. `dotnet build` the module `.csproj`
3. IA hot-reloads the new DLL automatically
4. Run SF (via MCP or IA UI)

Calling `install_or_update_modules` unnecessarily can corrupt IA's internal package reference cache, causing `Failed to resolve package reference` errors on the next SF run that require a UI restart to clear.

### `NugetPackages.cs` — Do Not Edit
This file is `[DefaultIntentManaged(Mode.Fully)]`. Hand edits are silently overwritten by the next SF run. All NuGet package and version changes must go through the **Module Builder designer**.

### Model Type IDs Are Solution-Specific
The `Model Type` property on a C# Template's `C# Template Settings` stereotype takes a GUID that identifies the element specialization type (e.g. `Integration Event Handler`). This GUID **differs between IA solutions** — the same element type has a different ID in the Module Builder solution vs. the production Modules.NET solution. Never copy a Model Type ID from memory or from another module's XML. Always verify by either:
- Running `find_designer_elements` on the target solution and inspecting the `specializationTypeId` of a live element of that type, OR
- Inspecting the installed `.designer.settings` file for the modeler package in the target solution

### `run_designer_script` — `lookupById(id)`, Not `getElementById(id)`
Inside a designer script, find an element by GUID with `lookupById(id)`. Do not use `getElementById` — that is a browser DOM API and does not exist in the IA designer script context; calling it throws `ReferenceError`.

### `run_designer_script` — Property API Uses `.value`, Not `.getValue()`
Inside a designer script, accessing a stereotype property value uses the `.value` property directly on the property object:
```js
// Correct
const val = element.getStereotype("C# Template Settings").getProperty("Model Type").value;

// Wrong — throws "getValue is not a function"
const val = element.getStereotype("C# Template Settings").getProperty("Model Type").getValue();
```

---

## Documentation
Use `search_docs` for questions about Intent Architect features, designers, attributes, code management, or workflow concepts before answering from memory.
