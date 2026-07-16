---
name: intent-module-builder
description: "Use after module-ecosystem-analyst produces an Attack Plan. Uses the Intent Architect Module Builder designer via MCP to scaffold the new module: creates the package, templates, factory extensions, and NuGet declarations, then runs Software Factory to generate the code scaffold. TRIGGER: when an Attack Plan is in hand and the next step is scaffolding the module structure. Produces a compiled module skeleton ready for implementation."
argument-hint: "[Attack Plan or path to it]"
---

# Intent Module Builder

## Purpose

Use the Intent Architect Module Builder designer (via MCP tools) to scaffold the new module from the Attack Plan. By the end of this skill, the module's folder structure, imodspec, template stubs, and NuGet declarations all exist as generated code — ready for the implementation skills to fill in.

## Governing Principle — Industry Standard First

All scaffolding decisions must be grounded in Intent's established module patterns (the standard for this layer). Improvise only where the SDK provides no pattern — and log every improvisation in the Pattern Document Decision Log.

**Before doing any work:** read `.module-builder/[ModuleName]/PATTERN-DOCUMENT.md` and `.module-builder/[ModuleName]/ATTACK-PLAN.md`. Check the Decision Log — do not re-derive any closed decision. Check the Progress Tracker — if the Scaffold row is ✅ Complete, this skill has already run; do not re-scaffold.

**First action — the build-state file is yours to maintain.** Before any designer or file work, ensure `.module-builder/WORKING.md` (global) or `.module-builder/<ModuleName>/WORKING.md` (localized) exists — if not, create it now. It is under your jurisdiction: keep it a faithful mirror of the user's current intent throughout. **Reconcile on entry:** if the incoming request diverges *at all* from what WORKING.md documents (different module, changed objective, dropped scope, a possible branch switch), pause and confirm with the user before touching anything — *"we were on X; this looks like Y — are we still aligned?"* Act on their answer; never plough ahead on stale assumptions.

**After completing the scaffold:** update the Progress Tracker row for "Scaffold" to ✅ Complete, noting the module `.csproj` path and any new decisions made.

## Musts

1. **Call `get_full_instructions` first, every time** — plus the `*_rules` pre-call tools once per session (`get_tool_call_rules`, `get_designer_schema_rules`, `run_designer_script_rules`). MCP instructions are authoritative.
2. **Call `get_status` on the working directory before opening anything.** It reports the Intent solution(s) and the suggested application; open with `open_solution(absolutePath)` if needed. (There is no `find_solution_files` tool.)
3. **Explore before mutating.** Use `get_designer_model_structure` / `find_designer_elements` / `get_designer_element_details` to understand the current designer state. Never guess IDs — resolve by name in-script (`lookupByName` / `lookupByPath`).
4. **Mutate via `run_designer_script`** — there is no `apply_change_model_operations` tool. **Statement order is operation order.** After each run, read the returned `errors[]` (and `changes[]`) before continuing: a mid-script throw still commits prior steps, so continue from `changes` rather than blindly re-creating elements.
5. **Run Software Factory after all module elements are declared** (not before). Apply staged file changes only after reviewing the diff. The SF run is also what **persists the Module Builder designer changes** — there is no separate save tool, so **never compile or reinstall before running SF**, or the designer edits are unsaved and lost.
6. **Verify compilation** after applying staged changes. Run `dotnet build` on the generated `.csproj` before marking this skill complete.
7. **Follow the full iteration cycle for every change** — including post-scaffold changes. The cycle is always: designer change → SF on module → apply staged changes → compile module → SF on target app → inspect output. Never shortcut it.
8. **Check for Intent markers before touching any `.cs` file.** If a file has `[assembly: IntentTemplate(...)]` or `[assembly: DefaultIntentManaged(Mode.Fully)]`, it is SF-owned. Only `[IntentManaged(Body = Mode.Ignore)]` sections within it are safe to edit directly.

## Must Nots

1. Never hardcode element IDs from memory or from other modules — IDs are solution-specific GUIDs. Always read them from the live model.
2. Never create a template element without configuring its stereotypes. `C# Template Settings` (always applicable) carries **Templating Method**; the separate **`Template Settings`** stereotype carries **Source, Designer, Model Type, Role, Default Location** and only becomes applicable *after* the type reference is set (see #3). Setting those properties before the type reference — or onto `C# Template Settings`, which has none of them — throws mid-script.
3. Never create a template element without setting its **type reference immediately after creation, before any stereotype work.** For single-file templates set it to the Single File element (typeId: `f65d2904-88c9-4501-873a-a4eec8303b1d`); for model-driven use File Per Model or Custom. Until the type reference is set, `Template Settings` is unavailable and SF/the script will error.
4. Never skip the Software Factory run. The scaffold only exists after SF generates it from the Module Builder metadata — hand-creating files bypasses the designer and breaks synchronization.
5. Never apply staged file changes without reviewing them first. Read the diff with `get_staged_file_diffs` (absolute paths) before calling `apply_staged_file_changes`.
6. Never run SF on the target application (the sample app) before compiling the module — the target app must pick up the updated module first.
7. **Never directly edit `*TemplateRegistration.cs` files** for structural changes (base class, method signatures, registration type). These files carry `[assembly: IntentTemplate(...)]` and are SF-owned. To change a template's registration type (e.g. `FilePerModel` → `SingleFileListModel`), change it in the Module Builder designer and let SF regenerate the file. Only `[IntentManaged(Body = Mode.Ignore)]` bodies (e.g. `GetModels`) are editable directly.
8. **Never manually look up NuGet versions for existing packages.** Use the **"Get latest from NuGet.org"** context menu action in the Intent Architect Module Builder designer — right-click the NuGet Packages element to update all versions at once. Only look up versions manually when creating a brand-new package that doesn't yet exist in the designer.
9. **Never add a NuGet package by editing `NugetPackages.cs` directly.** This file carries `[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages")]` and `[assembly: DefaultIntentManaged(Mode.Fully)]`. Every line is regenerated from the designer on the next SF run. A direct edit looks like it works — the build passes — but the change vanishes silently the moment SF runs. The only correct path is: add a NuGet Package element in the Module Builder designer via MCP → SF on module → apply staged changes.

---

## Learnings

### `supportedClientVersions` — see `module-wrap-up`

If SF fails on `<supportedClientVersions/>` after a scaffold or a version bump, apply the **two-step rule** (SDK floor from the SF error; ceiling from a neighbouring module's `modules.config`, set `[floor, ceiling)`). The full rule, plus the package-rename caveat, lives in **`module-wrap-up`**. The Phase 3.7 checklist gates on it.

---

### All NuGet Dependencies in Constructor Body — Never in `OnBuild`

All `AddNugetDependency(...)` calls — unconditional and conditional alike — belong in the **template constructor body**, before the `CSharpFile` assignment. Conditional ones (e.g. based on a transport setting) use a `switch (settings.AsEnum())` in the constructor with an explicit `default: throw new InvalidOperationException(...)` so unknown values fail loudly.

`OnBuild` is for mutating `CSharpFile` structure (members, statements, usings). The pattern `.OnBuild(file => { AddNugetDependency(...) })` looks like it works but causes dependency-tracking issues in some SF scenarios. Keep all NuGet declarations in the constructor.

---

### NuGet Packages must go through the designer — not `NugetPackages.cs`

**What happened:** A session added `NServiceBus.Persistence.Sql.TransactionalSession` by directly editing `NugetPackages.cs`, including the registry call and static factory method. The module built successfully. However, `NugetPackages.cs` is `[DefaultIntentManaged(Mode.Fully)]` — the next SF run regenerated it from the designer model and silently dropped the new package entirely.

**Root cause:** The agent correctly identified that the NuGet package was needed, then bypassed the MCP workflow and edited the generated file directly. Must #8 (check for Intent markers) was listed but not strong enough to prevent this.

**Correct approach:**
1. In the Intent Architect Module Builder designer (via MCP), find the module package and create a `NuGet Package` element (`specializationId: f747cc37-29ee-488a-8dbe-755e856a842d`) under it.
2. Add a `Package Version` child element with the version string.
3. Run SF on the module → `NugetPackages.cs` is regenerated with the new entry.
4. Only then reference `NugetPackages.MyNewPackage(OutputTarget)` in template code.

**The tell:** If a file in the module's own source has `[assembly: IntentTemplate(...)]`, it is owned by SF. Do not edit it. Period.

### The `.imodspec` Is Generated — Register Templates/Factory Extensions via the Designer

Never hand-author `<template>` or factory-extension entries into the `.imodspec`. A template `.cs` file added by hand compiles and runs via reflection, but with no `C# Template` element in the Module Builder designer it never gets a manifest entry — consuming apps then fail at SF with *"Unable to find output target for template […] with role []"*, and no reinstall fixes it because install reads the same incomplete manifest. Always register the element via `run_designer_script`, run SF on the module, and let it regenerate the manifest. **Cross-check before declaring the scaffold done:** every `*TemplateRegistration.cs` that declares a `TemplateId` has a matching `<template>` entry in the module's `.imodspec`.

### Changing a Template's Registration Type (e.g. `SingleFileNoModel` → `SingleFileListModel`)

Must Not #7 says never edit `*TemplateRegistration.cs` directly for structural changes. Here is the correct designer path when you need to convert a template from single-file to model-driven:

1. In the Module Builder designer, find the **C# Template element** (`find_designer_elements` by name).
2. Open the element's details (`get_designer_element_details`) and locate its `C# Template Settings` stereotype.
3. Set in a `run_designer_script` (idempotent — `ensureStereotype` + `setProperty`):
   ```js
   const s = lookupByName("MyTemplate").ensureStereotype("C# Template Settings");
   s.setProperty("Source", "Lookup Type");
   s.setProperty("Designer", "<designer that contains the model>");
   s.setProperty("Model Type", "<Integration Event Handler specialization id>");
   ```
4. Run SF on the module → `*TemplateRegistration.cs` is regenerated with the correct base class (`SingleFileListModelTemplateRegistration<TModel>`) and `GetModels` override.
5. The `*TemplatePartial.cs` constructor signature is also regenerated — its `[IntentManaged(Body = Mode.Ignore)]` body must be re-implemented.

> ⚠️ **Model Type GUIDs are solution-specific.** The same element type (`Integration Event Handler`) has a different GUID in different IA solutions. Always read the GUID from the live model — never copy from memory. See the `intent-architect-mcp` skill for how to find them.

---

## The Iteration Cycle

Any modification to module behaviour — template type, NuGet version, factory extension logic, registration type — follows a fixed designer → SF → install → SF → inspect → build → run sequence. This skill applies it once for the **initial scaffold**. Every subsequent change is owned by `module-increment-loop` — load that skill once this one's scaffold is verified.

---

## Key Module Builder Element Types

Create these via `run_designer_script` with `createElementUnder(parent, "<Specialization Name>", "<name>")` — **pass the specialization NAME** (left column), confirmed from `get_designer_schema`'s "Element types" block. The Module Builder specialization names are stable across solutions; the typeId GUIDs (right column) are rarely needed with the script API:

| Element | typeId |
|---|---|
| C# Template | `f6456232-0f1b-4235-b5f8-b4cce548ca59` |
| Factory Extension | `7d008e84-bb28-4b10-ba28-7439202fca76` |
| Folder | `4d95d53a-8855-4f35-aa82-e312643f5c5f` |
| NuGet Package | `f747cc37-29ee-488a-8dbe-755e856a842d` |
| Package Version | `231f8cf8-517b-4801-9682-991d22f4e662` |
| NuGet Dependency | `3097322a-a058-4058-beed-4fcd6272f61d` |

### C# Template Stereotype Configuration — Two Stereotypes, In Order

A C# Template carries **two** stereotypes, and **order matters** (see Must Not #3):

1. **Set the type reference first** (`setType` → Single File / File Per Model / Custom) — immediately after creating the element.
2. **`C# Template Settings`** (always applicable) — carries only **Templating Method** (`C# File Builder` for CSharpFile templates).
3. **`Template Settings`** — becomes applicable **only after** the type reference is set. It carries the rest:

| Property | Description | Example |
|---|---|---|
| Source | How the model is found | `Lookup Type` (file-per-model) or omit for single-file |
| Designer | Which Intent designer provides the model | `Services` or `Eventing` |
| Model Type | Fully qualified name of the typed model class | `Intent.Modelers.Services.EventInteractions.Api.IntegrationEventHandlerModel` |
| Role | Template role string used for lookups | `Infrastructure.Eventing.NServiceBus.Consumer` |
| Default Location | Output subfolder within the layer | `Infrastructure/Eventing` |

Setting a `Template Settings` property (e.g. `Source`) before the type reference is set — or onto `C# Template Settings`, which has none of them — throws mid-script.

For **single-file templates** (no model): omit `Source`, `Designer`, `Model Type`; set `Role` and `Default Location` only.

---

## Phase 3.1 — Locate and Open the Module Builder

### Tasks

1. **Get workspace status:**
   ```
   get_status(working_directory)
   ```
   It reports the Intent solution(s) and the suggested application. Identify the Intent.Modules.NET `.isln` (the Modules solution, not a sample app).

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

5. **Inspect the current model:**
   ```
   get_designer_model_structure(application_id, designer_id)   // or find_designer_elements for a targeted lookup
   ```
   Find existing module packages to understand the naming/structure convention. Identify the parent package where the new module package will be created.

---

## Phase 3.2 — Create the Module Package

### Tasks

Using the parent package ID discovered in Phase 3.1, create the module package:

Run a `run_designer_script` (resolve the parent by name; never hand-write GUIDs):
```js
// parent: a package name, a package-rooted path, or a handle from getPackages()/lookupPackage
const modulePkg = createElementUnder("<parent package name>", "<Package specialization>", "Intent.Eventing.NServiceBus");
console.log(modulePkg.getId());   // only echo ids you actually need downstream
```

Read the returned `errors[]`, then confirm with `find_designer_elements` / `get_designer_model_structure` before subsequent steps.

### Naming & Version — Intent Convention (do this immediately after scaffolding)

The Module Builder defaults a new module to `Intent.Modules.<Name>`. Correct it **before adding any templates, factory extensions, or NuGet packages** — the package name becomes the module ID in the `.imodspec` and drives cross-module lookups, so fixing it later means renaming across many generated files.

This is an **Intent team naming convention** — it matters most when a module is copied/exported into other codebases, where a consistent ID is what keeps cross-references resolvable:

1. Rename the **application** `Intent.Modules.<Name>` → `<Name>` (e.g. `Application.Dtos.ObjectMapping`) — rename the app, not the underlying files.
2. Rename the **Intent Module package** (in the designer) `Intent.Modules.<Name>` → `Intent.<Name>` (e.g. `Intent.Application.Dtos.ObjectMapping`).
3. Set **Module Settings → Version → `1.0.0-pre.0`** (all new modules start pre-release).

> ⚠️ If a rename desyncs the `.imodspec` name from the package name, `supportedClientVersions` can misresolve later — see `module-wrap-up`.

---

## Phase 3.3 — Declare Dependencies and NuGet Packages

### Task A — Module Dependencies

For each Intent module dependency in the Attack Plan's Module Blueprint, add a dependency element under the package. Read the pattern from an existing module's snapshot before creating.

**Exposing external element types (metadata install).** When your module needs **element types from another Module Builder package** (e.g. the `C# Template` element type ships in `Intent.ModuleBuilder.CSharp`), those types are made available by **installing that module into the module-builder application** — not by hand-editing `pkg.config` and not by `pkg.addReference()` alone (neither loads the element-type registry, and manual `pkg.config` edits are overwritten on reload).

- **Norm:** install the dependency **metadata-only** — you want its element types/designer available, not its code generation running in your module project.
- **Exception:** if a metadata-only install still doesn't surface the types or **reference elements** you need, **install the designer as well** and retry. Example: a new eventing module (e.g. Wolverine) needs the **Eventing Contracts** module installed **as a designer, not just metadata** — otherwise its reference elements aren't available and you can't set the message-bus configuration.

### Task B — NuGet Packages

NuGet packages follow a 3-level hierarchy: **NuGet Package → Package Version → NuGet Dependency**. Create them in one `run_designer_script` (parent before child — statement order is operation order):

```js
const pkg = createElementUnder(modulePkg, "NuGet Package", "NServiceBus");  // friendly name
const ver = createElementUnder(pkg, "Package Version", "10.1.4");           // version string
createElementUnder(ver, "NuGet Dependency", "NServiceBus");                 // actual NuGet package id
pkg.ensureStereotype("Package Settings").setProperty("Locked", false);      // Intent-managed, not version-locked
```

### Task C — Module Settings (if needed)

If the Attack Plan defines module settings (e.g. transport choice, endpoint name), create a Module Settings Configuration element with the appropriate setting groups and keys. Read an existing module's settings structure first as a template.

---

## Phase 3.4 — Add Templates

For each row in the Attack Plan's Template Inventory, create a C# Template element:

```js
// one run_designer_script per template (or batch several — statement order is operation order)
const tmpl = createElementUnder(modulePkg, "C# Template", "NServiceBusConsumer"); // or pass a Folder handle as the parent
tmpl.setType("f65d2904-88c9-4501-873a-a4eec8303b1d");            // TYPE REFERENCE FIRST (Single File shown; use File Per Model / Custom for model-driven)

const cs = tmpl.ensureStereotype("C# Template Settings");         // always applicable
cs.setProperty("Templating Method", "C# File Builder");

const ts = tmpl.ensureStereotype("Template Settings");            // ONLY applicable after setType
ts.setProperty("Role", "Infrastructure.Eventing.NServiceBus.Consumer");
ts.setProperty("Default Location", "Infrastructure/Eventing");
// file-per-model only (set the type reference to File Per Model above, not Single File):
ts.setProperty("Source", "Lookup Type");
ts.setProperty("Designer", "<Services designer>");
ts.setProperty("Model Type", "<model specialization id>");
```
Then read `errors[]` and confirm with `find_designer_elements`.

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

```js
// in a run_designer_script; resolve the package by name/path or reuse a handle
createElementUnder(modulePkg, "Factory Extension", "NServiceBusRegistrationExtension");
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
