---
name: intent-metadata-consumer
description: Read Intent Architect metadata (stereotypes, attributes, model properties) to drive C# code generation.
argument-hint: "[model type] [stereotype name] [target builder action]"
---

# Intent Metadata Consumer

> [!IMPORTANT]
> **Resource Read Constraint:** You are forbidden from reading the resource files under `/resources/` unless a `dotnet build` tool execution fails or an explicit type resolution error occurs.

## Musts
1. **Typed Accessors:** Use generated typed extension methods for every stereotype access (e.g., `*StereotypeExtensions.cs`). Never address a stereotype by its string name when a generated extension exists.
2. **Access Wrapper Properties:** Access properties through the typed wrapper methods, never via raw property-name strings.
3. **Null Guards:** Guard optional accessors with null-conditional operators or `TryGet` patterns before calling any property method on the wrapper.
4. **Enum Helpers:** Use `.AsEnum()` or the `.IsX()` boolean helpers for enum-like stereotype fields. Never do raw string comparisons on `.Value`.
5. **Guid Resolution:** If a typed extension does not exist yet, resolve stereotypes by `DefinitionId` (GUID) rather than display name.
6. **Primitive Checks:** Use `TypeCheckExtensions` (e.g., `IsStringType()`, `IsGuidType()`) to check primitive types on metadata elements.

## Must Nots
1. Never call `model.GetStereotype("StereotypeName")` when a typed extension method exists.
2. Never call `.GetProperty("PropertyName")` with a string literal for properties that are surfaced by generated wrappers.
3. Never branch on `.Value` of a stereotype option property using raw string comparison.
4. Never compose multi-stereotype LINQ queries using only string-based `HasStereotype` predicates when typed helpers are available.
5. Never skip the null guard on an optional stereotype accessor.
6. Never introduce display-name string lookups as a fallback when a `DefinitionId`-based lookup is available.
