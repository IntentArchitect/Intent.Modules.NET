---
name: intent-module-builder
description: "Use after module-ecosystem-analyst produces an Attack Plan. Uses the Intent Architect Module Builder designer via MCP to scaffold the new module: creates the package, templates, factory extensions, and NuGet declarations, then runs Software Factory to generate the code scaffold. TRIGGER: when an Attack Plan is in hand and the next step is scaffolding the module structure. Produces a compiled module skeleton ready for implementation."
argument-hint: "[Attack Plan or path to it]"
---

# Intent Module Builder

## Purpose

Use the Intent Architect Module Builder designer (via MCP tools) to scaffold the new module from the Attack Plan. By the end of this skill, the module's folder structure, imodspec, template stubs, and NuGet declarations all exist as generated code — ready for the implementation skills to fill in.

## Musts

1. **Call `get_full_instructions` first, every time.** MCP instructions are authoritative — read them before taking any action.
2. **Call `find_solution_files` on the working directory before opening anything.** Locate the correct `.isln` before any MCP interaction.
3. **Explore before mutating.** Use `get_designer_model_snapshot` or `find_designer_elements` to understand the current designer state before calling `apply_change_model_operations`. Never guess IDs — always read them from the model.
4. **Create one element at a time, verify, then continue.** Don't batch all `apply_change_model_operations` calls into one step before checking results. After each meaningful group of changes, call `get_designer_model_snapshot` to confirm the model reflects what was intended.
5. **Run Software Factory after all module elements are declared** (not before). Apply staged file changes only after reviewing the diff.
6. **Verify compilation** after applying staged changes. Run `dotnet build` on the generated `.csproj` before marking this skill complete.
7. **Follow the full iteration cycle for every change** — including post-scaffold changes. The cycle is always: designer change → SF on module → apply staged changes → compile module → SF on target app → inspect output. Never shortcut it.
8. **Check for Intent markers before touching any `.cs` file.** If a file has `[assembly: IntentTemplate(...)]` or `[assembly: DefaultIntentManaged(Mode.Fully)]`, it is SF-owned. Only `[IntentManaged(Body = Mode.Ignore)]` sections within it are safe to edit directly.

## Must Nots

1. Never hardcode element IDs from memory or from other modules — IDs are solution-specific GUIDs. Always read them from the live model.
2. Never create a template element without configuring its "C# Template Settings" stereotype (Templating Method, Model Type, Role, Default Location at minimum).
3. Never create a template element without also setting its **type reference**. For single-file templates, set the type reference to the Single File element (typeId: `f65d2904-88c9-4501-873a-a4eec8303b1d`). If left as `<type not set>`, the Template Settings stereotype becomes invalid and SF will report errors.
4. Never skip the Software Factory run. The scaffold only exists after SF generates it from the Module Builder metadata — hand-creating files bypasses the designer and breaks synchronization.
5. Never apply staged file changes without reviewing them first. Use `get_designer_model_snapshot` or read the diff before calling `apply_staged_file_changes`.
6. Never run SF on the target application (the sample app) before compiling the module — the target app must pick up the updated module first.
7. **Never directly edit `*TemplateRegistration.cs` files** for structural changes (base class, method signatures, registration type). These files carry `[assembly: IntentTemplate(...)]` and are SF-owned. To change a template's registration type (e.g. `FilePerModel` → `SingleFileListModel`), change it in the Module Builder designer and let SF regenerate the file. Only `[IntentManaged(Body = Mode.Ignore)]` bodies (e.g. `GetModels`) are editable directly.
8. **Never manually look up NuGet versions for existing packages.** Use the **"Get latest from NuGet.org"** context menu action in the Intent Architect Module Builder designer — right-click the NuGet Packages element to update all versions at once. Only look up versions manually when creating a brand-new package that doesn't yet exist in the designer.

---

## The Iteration Cycle (applies to every change, not just initial scaffold)

Any modification to module behaviour — template type, NuGet version, factory extension logic, registration type — follows this fixed sequence:

```
1. Change in designer (Module Builder)
2. Run SF on the module app       → run_software_factory(module_app_id)
3. Apply staged changes           → apply_staged_file_changes(module_app_id)
4. Edit bespoke bodies if needed  → only Body = Mode.Ignore sections
5. Compile the module             → dotnet build <module.csproj>
6. Run SF on the target/test app  → run_software_factory(target_app_id)
7. Inspect the generated output
```

Skipping or reordering steps (e.g. editing generated files before running SF, or running target SF before compiling the module) produces incorrect or overwritten output.

---

## Key Module Builder Element Types

These typeIds are stable across solutions — use them when calling `apply_change_model_operations` with `kind: "createElement"`:

| Element | typeId |
|---|---|
| C# Template | `f6456232-0f1b-4235-b5f8-b4cce548ca59` |
| Factory Extension | `7d008e84-bb28-4b10-ba28-7439202fca76` |
| Folder | `4d95d53a-8855-4f35-aa82-e312643f5c5f` |
| NuGet Package | `f747cc37-29ee-488a-8dbe-755e856a842d` |
| Package Version | `231f8cf8-517b-4801-9682-991d22f4e662` |
| NuGet Dependency | `3097322a-a058-4058-beed-4fcd6272f61d` |

### C# Template Required Stereotype Configuration

Every C# Template element must have the `C# Template Settings` stereotype applied with these properties set:

| Property | Description | Example |
|---|---|---|
| Templating Method | Always `C# File Builder` for CSharpFile-based templates | `C# File Builder` |
| Source | How the model is found | `Lookup Type` (for file-per-model) or omit for single-file |
| Designer | Which Intent designer provides the model | `Services` or `Eventing` |
| Model Type | Fully qualified name of the typed model class | `Intent.Modelers.Services.EventInteractions.Api.IntegrationEventHandlerModel` |
| Role | Template role string used for lookups | `Infrastructure.Eventing.NServiceBus.Consumer` |
| Default Location | Output subfolder within the layer | `Infrastructure/Eventing` |

For **single-file templates** (no model): omit `Source`, `Designer`, and `Model Type`. Set Role and Default Location only.

---

## Phase 3.1 — Locate and Open the Module Builder

### Tasks

1. **Find the solution file:**
   ```
   find_solution_files(working_directory)
   ```
   Identify the `.isln` file for the Intent.Modules.NET repository (not a sample app solution).

2. **Open the solution:**
   ```
   open_solution(isln_path)
   ```

3. **List applications:**
   ```
   get_applications()
   ```
   Find the application whose name contains "Module Builder" or matches the module-building context.

4. **List designers in that application:**
   ```
   get_designers(application_id)
   ```
   Identify the Module Builder designer ID.

5. **Snapshot the current model:**
   ```
   get_designer_model_snapshot(application_id, designer_id)
   ```
   Find existing module packages to understand the naming/structure convention. Identify the parent package ID where the new module package will be created.

---

## Phase 3.2 — Create the Module Package

### Tasks

Using the parent package ID discovered in Phase 3.1, create the module package:

```
apply_change_model_operations([
  {
    kind: "createElement",
    specializationId: "<package typeId — read from existing packages>",
    parentId: "<parent package ID from snapshot>",
    name: "<Module ID from Attack Plan — e.g. Intent.Eventing.NServiceBus>",
    newElementId: "<generate a GUID>"
  }
])
```

After creating, snapshot again to confirm the package exists and retrieve its ID for use in subsequent steps.

---

## Phase 3.3 — Declare Dependencies and NuGet Packages

### Task A — Module Dependencies

For each Intent module dependency in the Attack Plan's Module Blueprint, add a dependency element under the package. Read the pattern from an existing module's snapshot before creating.

### Task B — NuGet Packages

NuGet packages follow a 3-level hierarchy: **NuGet Package → Package Version → NuGet Dependency**

For each NuGet package in the Attack Plan:

```
Step 1: Create NuGet Package element (typeId: f747cc37-29ee-488a-8dbe-755e856a842d)
  - name: friendly name (e.g. "NServiceBus")
  - parent: the module package

Step 2: Create Package Version element (typeId: 231f8cf8-517b-4801-9682-991d22f4e662)  
  - name: version string (e.g. "10.1.4")
  - parent: the NuGet Package element

Step 3: Create NuGet Dependency element (typeId: 3097322a-a058-4058-beed-4fcd6272f61d)
  - name: actual NuGet package ID (e.g. "NServiceBus")
  - parent: the Package Version element
```

Add the "Package Settings" stereotype to the NuGet Package element with Locked = false (packages managed by Intent, not locked to a single version).

### Task C — Module Settings (if needed)

If the Attack Plan defines module settings (e.g. transport choice, endpoint name), create a Module Settings Configuration element with the appropriate setting groups and keys. Read an existing module's settings structure first as a template.

---

## Phase 3.4 — Add Templates

For each row in the Attack Plan's Template Inventory, create a C# Template element:

```
Step 1: Create C# Template element (typeId: f6456232-0f1b-4235-b5f8-b4cce548ca59)
  - name: template name (e.g. "NServiceBusConsumer")
  - parent: module package (or a Folder element for organisation)
  - newElementId: generate a GUID

Step 2: Apply "C# Template Settings" stereotype
  - kind: "addStereotype"
  - Use the typeId for "C# Template Settings" (read from an existing template's snapshot)
  - Set properties: Templating Method, Role, Default Location
  - For file-per-model: also set Source, Designer, Model Type

Step 3: Verify by snapshotting the package again
```

**File-per-model template checklist (e.g. `*NServiceBusConsumer` per handler):**
- Source: `Lookup Type`
- Designer: The designer containing the driving model (e.g. `Services`)
- Model Type: Fully qualified model class name — verify via `search_docs` before setting

**Single-file template checklist (e.g. `NServiceBusConfiguration`, `NServiceBusMessageBus`):**
- No Source / Designer / Model Type
- Role must be unique and descriptive

---

## Phase 3.5 — Add Factory Extensions

For each FactoryExtension in the Attack Plan, create a Factory Extension element:

```
apply_change_model_operations([
  {
    kind: "createElement",
    specializationId: "7d008e84-bb28-4b10-ba28-7439202fca76",
    parentId: "<module package ID>",
    name: "<FactoryExtension name — e.g. NServiceBusRegistrationExtension>",
    newElementId: "<generate a GUID>"
  }
])
```

---

## Phase 3.6 — Run Software Factory

### Tasks

1. **Final snapshot** — confirm the module package contains all expected elements before running SF.

2. **Run Software Factory:**
   ```
   run_software_factory(application_id)
   ```

3. **Review staged changes.** Examine what files SF intends to create:
   - `*TemplatePartial.cs` — one per template (empty stub ready for implementation)
   - `*TemplateRegistration.cs` — one per template
   - `NugetPackages.cs` — NuGet declarations
   - `*.imodspec` — updated module definition
   - `.csproj` — with Intent SDK references

4. **Apply staged changes:**
   ```
   apply_staged_file_changes()
   ```

5. **Fix SDK package versions (known IA 5.x bug).** After applying staged changes, open the generated `.csproj` and verify the three Intent SDK package versions match what other modules in the solution use. SF regenerates the csproj with incorrect versions — manually correct them before building. Read an adjacent module's `.csproj` to get the correct versions.

6. **Verify compilation:**
   ```powershell
   dotnet build "<path-to-new-module.csproj>" --no-incremental --verbosity minimal --nologo
   ```
   Exit code must be `0` before proceeding. If it fails, read the errors and fix before calling the build complete.

---

## Phase 3.7 — Orient for Implementation

Before handing off to `module-increment-loop`, confirm:

- [ ] Module `.csproj` compiles with exit code 0
- [ ] SDK package versions in `.csproj` match adjacent modules (not the SF-generated defaults)
- [ ] One `*TemplatePartial.cs` stub exists per template in the Attack Plan
- [ ] One `*TemplateRegistration.cs` exists per template
- [ ] `NugetPackages.cs` exists and references the correct packages
- [ ] `*.imodspec` is present and lists all templates and factory extensions
- [ ] Folder structure matches the scaffold from the Attack Plan's Module Blueprint
- [ ] `*.imodspec` `supportedClientVersions`: use the two-step rule — the SDK build error gives the floor version; check a neighbouring module's `modules.config` to find the ceiling. Set the range to `[floor, ceiling]`.
- [ ] Module version in `*.imodspec`, `.csproj`, and the designer Module Settings stereotype are in sync. Start at `1.0.0-pre.0`. The designer value wins if it is higher than the others.

Note the path to the `.csproj` and the list of stub files — this is the starting state for Increment 1.

### Eventing Module Post-Scaffold Checklist (additional)

If the module is an eventing/messaging transport (pub/sub), verify message delivery end-to-end after the target app SF run — compilation alone is not sufficient:

1. Run the target app and make a request that publishes an event.
2. Confirm the handler log entry appears in the output. If it does not, check `[AppBin]/.diagnostics/[EndpointName]-configuration.txt` → `Manifest-MessageTypes[*].IsEvent`. If `"IsEvent": false`, the transport's event convention is not configured.
3. **NServiceBus-specific:** `DefiningEventsAs(type => type.Name.EndsWith("Event"))` must be called in `ConfigureEndpoint`. The event POCOs from `Eventing.Contracts` do not implement `IEvent` — without this convention, NServiceBus treats all messages as basic messages and silently drops pub/sub delivery.

---

## Handoff

Once the checklist above is complete, load **`module-increment-loop`** and pass:
- The Attack Plan (as context for increment ordering and success criteria)
- The path to the new module `.csproj`
- The list of template stub files to implement
- Increment 1 scope (from the Attack Plan)
