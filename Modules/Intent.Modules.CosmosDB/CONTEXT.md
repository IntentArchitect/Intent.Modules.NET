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
- `CosmosDBValueObjectDocument` registers itself as a type source (`AddTypeSource(TemplateId)`) so that a Value Object nested inside a Value Object resolves to its `{Name}Document` type rather than the domain type.

## History

- 2.0.2 — Nested Value Objects (a Value Object attribute typed as another Value Object, single or collection) used to crash the Software Factory with `Could not find template from dependency: TemplateId / Role = `, because the Value Object document passed no interface template id. Covered in `Tests/CosmosDB` by the `Schedule` / `ScheduleSlot` Value Objects (deliberately standalone so the CRUD handlers for `Address` are not disturbed) and `IdTesting.Schedules` (a Value Object collection on an aggregate root, which exercises `AddPropertiesForAggregate`, not the shared helper).
