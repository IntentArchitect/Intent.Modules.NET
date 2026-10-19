# Intent.ValueObjects — Context

Durable design decisions and invariants behind this module. Read before changing the `ValueObject` template.

## What a Value Object contains

- A Value Object class is built from its **attributes** and from its **navigable association ends that target another Value Object**. `ValueObjectModel` (from `Intent.Modelers.Domain.ValueObjects`) only exposes `Attributes`, so the association ends are read from `Model.InternalElement.AssociatedElements` and wrapped with `AsAssociationEndModel()` (see `GetValueObjectAssociationEnds`). Only target ends are taken: a Value Object graph must not loop, so a reverse navigable end is skipped.
- Associations to anything other than a Value Object are ignored by design. A Value Object is a value, not a reference to an aggregate.
- Each member becomes a constructor parameter introduced as a private-setter property that `RepresentsModel(...)` its attribute or association end. Association-end properties additionally carry explicit `AddMetadata("model", associationEnd)`: `RepresentsModel` on an introduced property does **not** populate the `"model"` metadata key, and `Intent.CosmosDB` matches association-end model ids against that key to know what the domain class really exposes. Keep it when changing how members are emitted. Record Value Objects declare members through a primary constructor and carry no per-member metadata.
- Collection association ends are nullable in the designer (`0..*` produces `T[*]?`), so a collection can be `null` and the equality code guards for that.

## Equality

- `GetEqualityComponents` yields one component per member, except collections, whose **elements** are yielded (never the collection, which would compare by reference and make two equal Value Objects unequal). Nullable members are yielded with `!` when the project is nullable-aware, since the override returns `IEnumerable<object>`.

## Not covered here

- Mapping DTOs or commands *into* a Value Object association (Create/Update Entity mappings in the Services designer) belongs to `Intent.Modelers.Services.DomainInteractions`, which does not show Value Object association ends in its mapping tree.
- ORM modules (EF Core) treat Value Objects through their own metadata; this module only shapes the domain class.

## History

- 4.3.0 — Value Object → Value Object associations generate members; collection equality and the nullable `CS8603` warning fixed. Covered in `Tests/CosmosDB` by `Venue`, `Coordinates` and `Schedule`.
