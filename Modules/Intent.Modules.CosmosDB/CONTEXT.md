# Intent.CosmosDB — Context

Durable design decisions and invariants behind this module. Read before changing the document templates.

## Document templates and the shared property helper

- `DocumentTemplateHelpers.AddCosmosDBDocumentProperties` is shared by two templates:
  - `CosmosDBDocument` — for **non-aggregate** entities only (aggregate roots go through `AddPropertiesForAggregate` in the template itself, which has its own accessor logic and an `interfaceAccessorAdded` guard).
  - `CosmosDBValueObjectDocument` — for every Value Object.
- Every document class implements its document interface (`I{Name}Document`), whose properties use the interface's own collection formatter (`IReadOnlyList<…>`) and document-interface types. Whenever the class property type differs from the interface property type — a Value Object or a collection — the helper emits an **explicit interface accessor** against `documentInterfaceTemplateId`. Each caller must therefore pass the template id of *its own* interface:
  - entity documents → `CosmosDBDocumentInterfaceTemplate.TemplateId`
  - Value Object documents → `CosmosDBValueObjectDocumentInterfaceTemplate.TemplateId`
- A Value Object-typed attribute gets exactly one accessor, from `AddDocumentInterfaceAccessor` (whose `GetDocumentInterfaceName` already handles collections and nullability). The generic collection branch skips Value Objects, otherwise a Value Object collection would get two explicit implementations of the same member.
- A Value Object document is built from the Value Object's attributes **and** its navigable association ends to other Value Objects. The association ends come from `ValueObjectAssociationHelpers.GetValueObjectAssociationEnds`, which keeps only the ends the domain Value Object class actually exposes (matched by model id against the properties `Intent.ValueObjects` generated). The domain class is the source of truth: a document member with no domain counterpart could not be mapped back by `ToEntity`, so an older `Intent.ValueObjects` (which ignores associations) pairs safely with this module. Associations to anything other than a Value Object are not part of a Value Object's shape and are ignored.
- Both the Value Object document and its interface populate their members in an `OnBuild` (priority 1000), like the entity documents, because the domain class is not guaranteed to be built when the template constructor runs.
- `CosmosDBValueObjectDocument` registers itself as a type source (`AddTypeSource(TemplateId)`) so that a Value Object nested inside a Value Object resolves to its `{Name}Document` type rather than the domain type.

## History

- 2.1.0 — Value Object → Value Object associations (single, nullable, collection) are included in Value Object documents and interfaces. Before this the Value Object document passed no association ends, so an association drawn between two Value Objects was silently dropped (and a DTO projecting it failed to build). Requires `Intent.ValueObjects` 4.3.0 to produce the matching domain members. Covered in `Tests/CosmosDB` by `Venue` (associations to `Coordinates`: single, `0..1`, `0..*`, plus an attribute-typed twin) and `Schedule` (attributes and associations on the same Value Object).

- 2.0.2 — Nested Value Objects (a Value Object attribute typed as another Value Object, single or collection) used to crash the Software Factory with `Could not find template from dependency: TemplateId / Role = `, because the Value Object document passed no interface template id. Covered in `Tests/CosmosDB` by the `Schedule` / `ScheduleSlot` Value Objects (deliberately standalone so the CRUD handlers for `Address` are not disturbed) and `IdTesting.Schedules` (a Value Object collection on an aggregate root, which exercises `AddPropertiesForAggregate`, not the shared helper).
