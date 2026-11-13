---
description: "Reading designer metadata (stereotypes, properties, models) inside a module template or factory extension through generated typed accessors, never raw string lookups. Use only when a module template must branch on or read a stereotype, property or model value. Requires the typed *StereotypeExtensions.cs to be generated already. Not for emitting builder statements (see file-builder-expert)."
short-description: "Read stereotypes and model metadata in module templates via typed accessors."
argument-hint: "[model type] [stereotype name] [target builder action]"
intent-updates: automatic
requiredTools:
  - read_file
group: Module building
---
# Intent Metadata Consumer

> [!TIP]
> **`read_file` these from this skill's directory (`use_skill` reports it) if you want to know about** typed extension wrapper code patterns, stereotype GUID lookups, enum helpers, or metadata navigation API:
> *   Metadata Cheatsheet (`resources/metadata-cheatsheet.md`)
> *(To conserve tokens, avoid reading this file for simple or minor updates.)*

## Musts

1. **Typed Accessors:** Use generated typed extension methods for stereotype access (e.g. `*StereotypeExtensions.cs`).
2. **Access Wrapper Properties:** Access properties through typed wrapper methods, never via raw property name strings.
3. **Null Guards:** Guard optional accessors with null-conditional operators or `TryGet` patterns. `Get<Stereotype>()` returns `null` when the stereotype is not applied, and a **Number** property's accessor returns `int?` (a **Decimal** one `decimal?`), so null-check the value before using it.
4. **Enum Helpers:** Use `.AsEnum()` or the `.IsX()` boolean helpers (avoid raw string comparisons on `.Value`). The options enum is nested inside the stereotype's wrapper class (`MySettings.ModeOptionsEnum`).
5. **Guid Resolution:** If a typed extension doesn't exist yet, resolve by `DefinitionId` (GUID) rather than display name — then model the stereotype in the Module Builder designer and run the Software Factory so the typed extension is generated.
6. **Primitive Checks:** Use `TypeCheckExtensions` (e.g., `IsStringType()`, `IsGuidType()`) for primitive metadata checks. It ships in `Intent.Modules.Common.Types` (namespace `Intent.Modules.Common`), so the module needs an `Intent.Common.Types` dependency.

## Must Nots

1. Never call `model.GetStereotype("StereotypeName")` when a typed extension method exists.
2. Never call `.GetProperty("PropertyName")` with a string literal for properties that are surfaced by generated wrappers.
3. Never branch on `.Value` of a stereotype option property using raw string comparison.
4. Never compose multi-stereotype LINQ queries using only string-based `HasStereotype` predicates when typed helpers are available.
5. Never skip the null guard on an optional stereotype accessor.
6. Never introduce display-name string lookups as a fallback when a `DefinitionId`-based lookup is available.
7. Never hand-write a `*StereotypeExtensions.cs` file. It carries the generator's `[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", ...)]` attribute and belongs to the Software Factory: a hand-written copy is overwritten or drifts from the model. Model the stereotype and regenerate instead.
