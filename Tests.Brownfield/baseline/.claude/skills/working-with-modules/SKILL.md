---
description: "How to discover, install, update, reconfigure, and uninstall Intent Architect modules — run_solution_query (searchAvailableModules / listInstalledModules), install_or_update_modules, uninstall_modules. Load before any module change: modules add the designers, templates, and generators an application's capabilities are built from. Installing/uninstalling is high-stakes — confirm with the user first."
short-description: Discover, install, update, reconfigure, or uninstall modules.
intent-updates: automatic
requiredTools:
  - run_solution_query
  - install_or_update_modules
  - uninstall_modules
  - run_software_factory
group: Modelling
---

# Working with Modules

Modules are the packages that give an Intent Architect application its capabilities — they contribute **designers, stereotypes, templates, factory extensions, and application settings**. Adding or changing a module changes what the application can model and generate, so treat installs/uninstalls as **high-stakes, user-confirmed** operations: state the module(s) and version(s) you intend to install/remove and get agreement before executing.

The workflow: **search → check what's installed → install/update → run the Software Factory → apply**. After any install/update/uninstall, run the Software Factory to regenerate against the new module set (see the **generating-and-applying-code** skill). (Creating an application already installs an architecture's module bundle — see **creating-applications**; use these tools to change modules on an existing application.)

## `searchAvailableModules()` — discover before installing

A read-only global of `run_solution_query`.

- Searches the configured module feeds. **Always search before installing** to confirm the exact module id and the available versions — never guess a module id or version.
- `searchString` filters by name/keyword. `includePrerelease: true` to surface pre-release versions (default false). `repositoryUrl` restricts to one feed's full URL; omit to search all configured repositories.
- Read-only. Present meaningful matches to the user when the choice isn't obvious.

## `listInstalledModules()` — see the current state

- Lists the modules installed in an application (`applicationId` defaults to the one passed to `run_solution_query`, else the solution's only application), with their versions and installation settings. `searchString` filters by name.
- Call this **before installing/updating** (to see current versions and avoid redundant work) and **before uninstalling** (to confirm the module is present and inspect what depends on it). Read-only.

## `install_or_update_modules` — add, upgrade, or reconfigure

- Installs or updates one or more modules on an application (`applicationId` + a `modules` array). **Dependencies are resolved automatically.**
- Each module entry: `moduleId`, an optional exact `version` (e.g. `"1.2.3"`; omit it for the latest compatible version — an installed module is never downgraded, and the result names the version resolved), plus optional install feature flags:
  - `enableFactoryExtensions` — Software Factory extensions (templates, decorators, factory extensions).
  - `installDesigners` — designers the module provides.
  - `installTemplateOutputs` — template output configurations.
  - `installDesignerMetadata` — designer metadata.
  - `installApplicationSettings` — application settings.
- **Re-run for an already-installed module to change its feature flags** (this is the reconfigure path). **Omitted flags default to enabled** — pass a flag as `false` only to deliberately turn that facet off.
- **Picking up a module you rebuilt without bumping its version needs `forceReinstall: true`.** Without it, a same-version install reuses the extracted cache, and the result reports `SameVersion`. Only the modules you list are re-extracted; their dependencies keep their cache.
- After it completes, **run the Software Factory and apply** the staged changes so the new module's generated code lands (see **generating-and-applying-code**).
- Confirm the module(s) and version(s) with the user before running — it modifies the application's configuration.

## `uninstall_modules` — remove (highest-stakes)

- Removes one or more modules from an application (`applicationId` + `moduleIds`); modules are removed one at a time. `removeDependencies: true` also removes dependency modules that no other installed module still uses.
- **Confirm with the user first**, and check `listInstalledModules()` beforehand — removing a module that others depend on, or that owns designers/elements in use, can invalidate the model or delete generated code. Call out dependents before proceeding.
- After uninstalling, run the Software Factory and apply so the codebase reflects the removed module (see **generating-and-applying-code**); then check the affected designers for validation errors (see **exploring-the-model** / **changing-the-model**).
