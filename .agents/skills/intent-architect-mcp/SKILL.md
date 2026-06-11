---
name: intent-architect-mcp
description: >
  Intent Architect MCP workflow: designer operations, element discovery, model modification, 
  Software Factory execution, compilation verification, and cross-module integration patterns.
  If you find yourself wanting to edit `.xml`, `.config` files inside an `intent` folder directly, stop and ask the user to model the change instead. The IA MCP workflow is designed to keep models as the single source of truth — direct file edits are a last resort for truly exceptional cases.
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

---

## Required Workflow

### 1. Bootstrap (every session, in order)
```
get_status(workingDirectory)
  → if HasIntentSolution = false: stop, not an IA project
  → if IsSolutionOpen = false: call open_solution(SolutionPath)
  → SuggestedApplicationId (or call get_applications to pick one)
    → get_designers(applicationId)
      → get_designer_schema(applicationId, designerId)   ← call ONCE per designer, reuse the result
```

`get_designer_schema` does not change as you edit elements — **never re-fetch it mid-session**.

### 2. Finding Elements
- **Prefer `find_designer_elements`** (regex + specialization filter) whenever you know what you're looking for.
- Use `get_designer_model_structure` only when you genuinely need topology (e.g. all packages and their children). Always pass `specializations` or `packageId` to keep the response small.
- Use `get_designer_element_details` to inspect full details of a specific element before modifying it.

### 3. Modifying Models
- Apply changes via `apply_change_model_operations`.
- After **any** model change, immediately call `get_designer_validation_errors`. If errors exist, resolve them and re-check until clean. Do not consider work complete while validation errors remain.
- Update diagrams with `apply_change_diagram_layout` after structural changes — place new elements near the most connected existing element, maintain spacing, give associations ≥100px gap.

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

---

## Tool Calling Rules (IA MCP tools only)

- **NEVER call IA MCP tools in parallel** — they must be sequential
- **Every IA MCP call must include `intention`** — describe intent in ≤10 words (do NOT pass `intention` to host-native tools like Read/Edit/Bash)
- **Never invent IDs** — only use IDs returned by prior tool calls
- **Do not read or modify `.intent` / `intent` folders**
- Do not include IDs in plans shown to the user — reference by name and type

---

## Operation Ordering (within `apply_change_model_operations`)

1. Create parents before children
2. Create both endpoints before creating an association between them
3. Create element → add stereotype → update stereotype properties (three separate operations)
4. Move all children to new parent before deleting the old parent
5. To move an element: update its parent reference — **never delete + recreate**

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

### Stereotype Operations — Three Separate Calls
These are three distinct operations — each must be its own MCP call:
1. `apply_change_model_operations` with `kind: addStereotype`
2. `apply_change_model_operations` with `kind: updateStereotype` (set property values)

`addStereotype` ignores `applicableTo` schema restrictions. This is intentionally exploitable — e.g. setting `Is Active Function` on individual stereotype properties that the UI would normally reject.

### Stereotype Definition Elements Not in Tree
`get_designer_model_structure` with `includeChildren: true` does **not** include Stereotype Definition elements — the Stereotypes folder is excluded from traversal. Access them by GUID via `get_designer_element_details`. Find the GUID from generated `.xml` files in the module source.

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

---

## Documentation
Use `search_docs` for questions about Intent Architect features, designers, attributes, code management, or workflow concepts before answering from memory.
