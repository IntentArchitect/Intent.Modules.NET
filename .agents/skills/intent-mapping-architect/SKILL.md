---
name: intent-mapping-architect
description: Translate designer-defined advanced mappings into recursive C# Builder statements.
argument-hint: "[mapping type] [source model] [target model]"
---

# Intent Mapping Architect

> [!IMPORTANT]
> **Resource Read Constraint:** You are forbidden from reading the resource files under `/resources/` unless a `dotnet build` tool execution fails or an explicit type resolution error occurs.

## Musts
1. **Replacement Resolution:** Always configure replacements via `SetFromReplacement(...)` / `SetToReplacement(...)` using model element identities, and generate statements via MappingManager APIs (`GenerateUpdateStatements(...)`, etc.).
2. **Path Resolution:** Resolve assignments via mapping element IDs and paths, never by hardcoded property names.
3. **Node Differentiation:** Explicitly handle Terminal Mappings (leaf/scalar) vs Object Mappings (non-leaf/nested/collection).
4. **Metadata Preservation:** Custom mapping statement types that participate in recursive mapping generation must implement `IHasMapping` to expose their underlying mapping metadata.
5. **Mapping Options:** Always honor `MappingOptions` from the designer model: Null-Safe (emit null guards) and Validate All.
6. **Custom Resolver Registration:** Register custom `IMappingTypeResolver` implementations with explicit priority via `AddMappingResolver(...)`.
7. **Inherit CSharpMappingBase:** Inherit custom mappings from `CSharpMappingBase` to leverage recursive tree traversal (`Children`, parent links, element ID resolution).

## Must Nots
1. Never hardcode property-to-property assignments.
2. Never bypass MappingManager-driven replacement resolution.
3. Never treat object/collection mappings as scalar terminals.
4. Never create mapping statement types that omit `IHasMapping` inside recursive mapping flow.
5. Never ignore `MappingOptions` Null-Safe and Validate All settings.
6. Never place transaction/retrieval/persistence orchestration in this skill (belongs to `intent-domain-interactions-expert`).
