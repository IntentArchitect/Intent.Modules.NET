---
name: intent-mapping-architect
description: "Use when translating designer-defined advanced mappings into recursive C# Builder statements. Trigger phrases: Generate an update mapping, Map DTO to Entity, Implement a custom mapping manager, Handle recursive object mapping. USE FOR: CSharpClassMappingManager-driven source/target resolution, IMappingTypeResolver registration and selection, CSharpMappingBase recursion/path resolution, terminal-vs-object mapping decisions, and custom mapping statement participation via IHasMapping. DO NOT USE FOR: IInteractionStrategy orchestration, persistence-story ownership, DI/container orchestration, appsettings events, or generic stereotype consumption workflows outside mapping generation."
argument-hint: "[mapping type] [source model] [target model]"
---

# Intent Mapping Architect

## Trigger Phrases

- Generate an update mapping
- Map DTO to Entity
- Implement a custom mapping manager
- Handle recursive object mapping

## Two-Layer Model: Designer Metadata vs Code Generation Tree

Advanced mapping in Intent Architect operates at two distinct layers that must never be conflated:

### Layer 1 — Designer Metadata (`IElementToElementMapping`)

`IElementToElementMapping` is the raw mapping record authored in the designer. It lives on a model element (e.g., an action target end) and is accessed via `.Mappings`.

Key properties:
- `.Type` — human-readable mapping kind, e.g. `"Update Entity Mapping"`, `"Query Entity Mapping"`, `"Creation Mapping"`.
- `.TypeId` (same value exposed as `.MappingTypeId` on `MappingModel`) — stable GUID; prefer this over `.Type` for resolver matching.
- `.SourceElement` — root source element (e.g., the Command/Query DTO).
- `.TargetElement` — root target element (e.g., the domain Entity, Constructor, or Operation).
- `.MappedEnds` — flat `IList<IElementToElementMappedEnd>`; one entry per drawn line in the designer.

Accessing mappings from an action element:
```csharp
IEnumerable<IElementToElementMapping> mappings = updateAction.Mappings;
IElementToElementMapping? updateMapping = mappings.GetUpdateEntityMapping(); // extension on MappingExtensions
```

### Layer 1a — Individual Mapped Pairs (`IElementToElementMappedEnd`)

Each entry in `.MappedEnds` represents one source→target connection drawn in the designer.

Key properties:
- `.SourceElement` — the specific source element for this pair (may be `null` for calculated/constant expressions).
- `.TargetElement` — the specific target element (e.g., an `Attribute`, `Operation`, or `Association Target End`).
- `.SourcePath` — `IList<IElementMappingPathTarget>` — full path from root down to the source element.
- `.TargetPath` — `IList<IElementMappingPathTarget>` — full path from root down to the target element.
- `.MappingExpression` — string template with `{path}` placeholders for complex expressions.
- `.Sources` — collection of source references when the expression references multiple source paths.
- `IsOneToOne()` — extension; `true` when the expression is a simple 1-to-1 reference.

`MappedEnds` are the **flat designer metadata**. They are consumed by:
- **Query predicates** — iterate `MappedEnds` to build `x => x.Field == value` filter expressions.
- **Detecting special targets** — inspect `TargetElement.SpecializationTypeId` to find lookup-ID ends or operation invocations.
- **Operation post-processing** — scan `MappedEnds` for `TargetElement.IsOperationModel()` to adjust async/return wrappers after generation.

Do **not** use `MappedEnds` to drive recursive nested-object traversal; use the `MappingModel` tree and its `Children` for that.

### Layer 2 — Code Generation Tree (`MappingModel`)

`MappingModel` is the hierarchical tree the engine builds from an `IElementToElementMapping`. It groups the flat `MappedEnds` list by `TargetPath` depth so resolvers see a proper tree rather than a flat list.

Construction: `new MappingModel(mapping, manager)` groups `MappedEnds` by the next `TargetPath` segment at each level, ordered by `((IElement)model).Order`.

Key properties:
- `.Model` — the `ICanBeReferencedType` this node represents (Attribute, Class, Constructor, Operation, Association Target End, etc.).
- `.Mapping` — the `IElementToElementMappedEnd` for this node, or `null` if the node is a traversal/grouping node with no direct pair.
- `.MappingType` / `.MappingTypeId` — mirrors the parent `IElementToElementMapping`.
- `.Children` — child `MappingModel` nodes; each represents the next level of target path depth.
- `.Parent` — link to the parent node.
- `GetSourcePath()` / `GetTargetPath()` — returns the path even for traversal nodes (inferred from children).
- `GetMapping()` — invokes the manager's resolver pipeline to produce an `ICSharpMapping` for this node.
- `GetCollectionItemModel()` / `GetCollectionItemMapping()` — produces a non-collection adapter for the item type of a collection node.

`Mapping == null` → traversal/grouping node: no direct source→target pair; recurse into `Children`.
`Mapping != null` → live pair: this node has a mappable source and target.

## Terminology Alignment (Must Apply Everywhere)

- "Map Via" means "Invocation Mapping".
- "Data Type Mapping" means "Data Mapping".
- Guidance, snippets, and decisions in this skill always use the current terms first, with legacy aliases in parentheses when needed.

## Musts

1. Use MappingManager replacement resolution before generating statements.
   - Configure source and target replacements through the manager (`SetFromReplacement(...)` / `SetToReplacement(...)`) using model element identities.
   - Generate statements through mapping APIs (`GenerateUpdateStatements(...)`, `GenerateCreationStatement(...)`) so source/target identifiers are resolved from mapping metadata.

2. Resolve assignments via mapping element IDs and paths, never by hardcoded property names.
   - Resolve mapped ends from the designer model (`SourcePath`/`TargetPath` and mapped element IDs).
   - Build assignment statements from resolved source/target statements, not string-literal member guesses.

3. Distinguish Terminal Mappings from Object Mappings at every node.
   - Terminal Mapping (leaf): direct scalar/value assignment.
   - Object Mapping (non-leaf): recursive traversal for nested objects and collections with appropriate null and collection handling.

4. Implement `IHasMapping` on custom statement types that participate in recursive mapping generation.
   - Any custom mapping statement inserted into recursion must expose its associated mapping metadata through `IHasMapping` so downstream mapping-aware behavior can inspect and continue traversal correctly.

5. Respect MappingOptions from the designer model on all generated logic.
   - Apply Null-Safe behavior for nullable paths and object graph traversal.
   - Apply Validate All behavior where configured so generated code does not bypass required validation semantics.

6. Custom mapping logic must be registered in a Resolver so the MappingManager can instantiate it during `GenerateUpdateStatements`.
   - Implement `IMappingTypeResolver` and return an `ICSharpMapping` when the `MappingModel` matches your scenario.
   - Register the resolver with explicit priority using `AddMappingResolver(...)`.

7. Inherit from `CSharpMappingBase` to ensure the mapping engine can recursively traverse children and resolve paths via element IDs.
   - Use `GetSourceStatement()` / `GetTargetStatement()` (and path helpers) from `CSharpMappingBase` to avoid manual path stitching.
   - Keep recursive behavior mapping-aware by delegating child traversal through `Children` and mapping statement generation.

## Must Nots

1. Never hardcode property-to-property assignments for mapping code generation.
2. Never bypass MappingManager-driven resolution and replacements.
3. Never treat object/collection mappings like scalar terminals.
4. Never create mapping engine statement types that omit `IHasMapping` when they participate in recursive mapping flow.
5. Never ignore `MappingOptions`; always honor Null-Safe and Validate All settings.
6. Never move interaction-story responsibilities into this skill (entity retrieval strategy, persistence orchestration, transaction/save flow ownership).

## Pattern Index

- Core loop, update mapping resolution, and recursive examples:
  - `resources/mapping-cheatsheet.md`

## Practical Workflow

1. Retrieve the intended mapping model (for update scenarios this may come from `GetUpdateEntityMapping()` in the caller).
2. Initialize `CSharpClassMappingManager` (or equivalent MappingManager implementation).
3. Register custom `IMappingTypeResolver` implementations before generation where needed.
4. Configure source/target replacements against model elements.
5. Generate statements via `GenerateUpdateStatements(...)` / `GenerateCreationStatement(...)`.
6. For custom mapping behavior, inherit from `CSharpMappingBase` and return mapping-aware statement types that implement `IHasMapping`.
7. Ensure terminal vs object-node handling is explicit and MappingOptions-compliant.
8. Return generated statements to the caller; orchestration context belongs to `intent-domain-interactions-expert`.

## Mapping Type Resolvers

- Pattern source: `IMappingTypeResolver` and `CommandQueryMappingResolver`.
- Resolver responsibility:
   - Inspect `MappingModel` shape and specialization.
   - Return the correct `ICSharpMapping` implementation (`ConstructorMapping`, `ObjectInitializationMapping`, `ObjectUpdateMapping`, etc.).
   - Return `null` to delegate to the next resolver.
- Registration responsibility:
   - Register custom resolver(s) on the manager before statement generation so the pipeline can instantiate the right mapping type.

### Resolver Pipeline Mechanics

Resolvers are sorted by priority (ascending — lower integer runs first). The pipeline is built as a delegate chain. The modern signature supplies a `next` delegate:

```csharp
public ICSharpMapping? ResolveMappings(MappingModel model, MappingTypeResolverDelegate next)
{
    if (/* not my concern */) return next(model); // delegate to next resolver
    return new MyMapping(model, _template);
}
```

The legacy single-argument overload `ResolveMappings(MappingModel model)` is still valid (no `next` access) but cannot pass control to the next resolver explicitly — the engine falls through automatically when it returns `null`.

Engine fallback (when all resolvers return `null`):
- `Mapping != null` → `DefaultCSharpMapping` (simple scalar assignment).
- `Mapping == null` → `MapChildrenMapping` (delegates all output to children).

### Built-in `ICSharpMapping` Implementations

| Class | Purpose |
| :--- | :--- |
| `DefaultCSharpMapping` | Scalar leaf assignment (engine fallback when `Mapping != null`) |
| `MapChildrenMapping` | Traversal node — delegates `GetMappingStatements()` to all children (engine fallback when `Mapping == null`) |
| `ObjectUpdateMapping` | Updates an existing object in-place; handles single entities, nullable entities (null-guard + `??=`), and collections (via `UpdateHelper.CreateOrUpdateCollection`) |
| `ObjectInitializationMapping` | Creates a new object using property-initializer syntax, hybrid ctor+init, or constructor lookup |
| `ConstructorMapping` | Invokes a constructor; resolves parameter order from CSharpFile builder metadata |
| `MethodInvocationMapping` | Invokes an instance method (`Operation` specialization) |
| `StaticMethodInvocationMapping` | Invokes a static factory method |
| `SelectToListMapping` | `.Select(x => ...).ToList()` for collection-to-collection projection |
| `ValueObjectCollectionUpdateMapping` | Replaces a collection of value objects wholesale |
| `ForLoopMethodInvocationMapping` | `foreach` loop over a source collection calling an operation |
| `IfNotNullMapping` | Wraps another mapping in an `if (source != null)` guard (PATCH / nullable-source semantics) |

### Associations in the Mapping Tree

Domain entities have navigation properties modelled as associations. When the designer maps through an association, the `TargetPath` of the `IElementToElementMappedEnd` passes through an `"Association Target End"` element. In the `MappingModel` tree this appears as a node where:

```
model.SpecializationType == "Association Target End"
model.TypeReference.Element.SpecializationType == "Class" | "Value Object"
model.TypeReference.IsCollection == true | false
```

Resolver decision table for association target end nodes:

| Referenced type | `IsCollection` | Correct mapping (update) | Correct mapping (create) |
| :--- | :--- | :--- | :--- |
| `"Class"` | false | `ObjectUpdateMapping` | `ObjectInitializationMapping` |
| `"Class"` | true | `ObjectUpdateMapping` (uses `CreateOrUpdateCollection`) | `ObjectInitializationMapping` |
| `"Value Object"` | true | `ValueObjectCollectionUpdateMapping` | `SelectToListMapping` |
| `"Value Object"` | false | `ConstructorMapping` | `ConstructorMapping` |

`ObjectUpdateMapping` auto-generates a `private static CreateOrUpdate{EntityName}(entity, dto)` helper method via `AfterBuild` when a collection reconciliation is needed.

## CSharpMappingBase Mastery

- Inherit custom mapping implementations from `CSharpMappingBase`.
- Use `GetSourceStatement()` / `GetTargetStatement()` for resolved, ID-based path translation.
- Use inherited recursive tree behavior (`Children`, parent linkage, replacements) instead of writing ad-hoc traversal code.
- Use expression/path helpers for mapping expressions and null-conditional transitions.

## Boundary with Domain Interactions

- This skill owns statement generation semantics and recursive mapping composition.
- This skill does not own interaction-story orchestration (`Find -> Map -> Save`) or persistence flow.
- For operation-context orchestration, use `intent-domain-interactions-expert`.

## Source of Truth

- Repository: <https://github.com/IntentArchitect/Intent.Modules>
- Documentation: <https://github.com/IntentArchitect/Docs>

Primary engine references:
- `Modules/Intent.Modules.Common.CSharp/Mapping/MappingManagerBase.cs`
- `Modules/Intent.Modules.Common.CSharp/Mapping/Mappings/ObjectUpdateMapping.cs`
- `Modules/Intent.Modules.Common.CSharp/Mapping/Mappings/CSharpMappingBase.cs`
- `Modules/Intent.Modules.Common.CSharp/Mapping/IMappingTypeResolver.cs`
- `Modules/Intent.Modules.Common.CSharp/Mapping/CSharpClassMappingManager.cs`
- `Modules/Intent.Modules.Application.DomainInteractions/Extensions/MappingExtensions.cs`
- `Modules/Intent.Modules.Application.DomainInteractions/Mapping/Resolvers/CommandQueryMappingResolver.cs`
