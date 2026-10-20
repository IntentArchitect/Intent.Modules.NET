# Intent.ValueObjects — Context

Durable design decisions and invariants behind this module. Read before changing the `ValueObject` template.

## What a Value Object contains

- A Value Object class is built from its **attributes** and from its **navigable association ends that target another Value Object**. `ValueObjectModel` (from `Intent.Modelers.Domain.ValueObjects`) only exposes `Attributes`, so the association ends are read from `Model.InternalElement.AssociatedElements` and wrapped with `AsAssociationEndModel()` (see `GetValueObjectAssociationEnds`). Only target ends are taken: a Value Object graph must not loop, so a reverse navigable end is skipped.
- Associations to anything other than a Value Object are ignored by design. A Value Object is a value, not a reference to an aggregate.
- Each member becomes a constructor parameter introduced as a private-setter property that `RepresentsModel(...)` its attribute or association end. Association-end properties additionally carry explicit `AddMetadata("model", associationEnd)`: `RepresentsModel` on an introduced property does **not** populate the `"model"` metadata key, and `Intent.CosmosDB` matches association-end model ids against that key to know what the domain class really exposes. Keep it when changing how members are emitted. Record Value Objects declare members through a primary constructor and carry no per-member metadata.
- Collection association ends can be nullable (`0..*` produces `T[*]?`) or not (the Value Object `New Association` menu's `… to Many` options produce `1..*`, `T[*]`). Equality guards for `null` only on the nullable ones.
- Only the **target** end shapes the generated member. The source end is irrelevant here, and the designer's on-changed macro (in `Intent.Modelers.Domain.ValueObjects`) forces it to `1` whenever the target is a Value Object anyway. That macro does not run for scripted associations, which can leave other source multiplicities; the output is identical.

## Record Value Objects and persistence (known gaps, not addressed)

- Record Value Objects have only their primary constructor. Two pre-existing persistence gaps follow; neither was introduced by 4.3.0, and both were deliberately left out of scope:
  - EF Core cannot bind owned navigations (Value Object members) through a record's primary constructor, so a record holding another Value Object (attribute or association) fails to load with `No suitable constructor was found`. Scalar-only records load fine.
  - `Intent.CosmosDB` creates every Value Object through a parameterless constructor (`ReflectionHelper.CreateNewInstanceOf<T>`), so record Value Objects cannot be read back from Cosmos at all. No CosmosDB test app uses records.
- A fix was prototyped in `4.3.0-pre.2` and reverted as out of scope: `protected <Name>() : this(default(T1)!, …)` (C# requires extra record constructors to chain to the primary one, CS8862; the defaults must be typed, because a bare `default!` is ambiguous with the copy constructor for one-member records, CS0121). For Cosmos, falling back to `RuntimeHelpers.GetUninitializedObject` in its `ReflectionHelper` may be the better fix, since it leaves the domain shape alone.

## Value Object → Class associations

- No persistence module supports them, by design: `Intent.ValueObjects` ignores them, `Intent.CosmosDB` filters Value Object documents to Value Object targets, DynamoDB and Redis OM pass no association ends for Value Objects at all, and MongoDb, Table Storage and Dapper have no Value Object association handling. EF Core crashes on them (`NullReferenceException` in `EntityTypeConfiguration`): it configures every target-end association on a Value Object and sends a Class target down the `HasOne` + foreign key path, which assumes a Class source.
- Blocking them belongs in the designer (`Intent.Modelers.Domain.ValueObjects`), not here.

## Equality

- `GetEqualityComponents` yields one component per member, except collections, which yield their **count and then their elements**. Never the collection itself, which compares by reference and makes two equal Value Objects unequal. The count is load-bearing, do not drop it:
  - Without it, items from adjacent collections run together, so `[a, b] + [c]` equals `[a] + [b, c]`. With the fixed member order, the count makes the sequence uniquely decodable regardless of item values. A name or sentinel marker would not, because a collection of strings could contain it.
  - It guarantees at least one component per collection. The generated `ValueObject.GetHashCode` aggregates without a seed and throws on an empty sequence (a Value Object whose only members are empty collections).
  - A `null` nullable collection counts as empty (`?.Count() ?? 0`).
- Nullable members are yielded with `!` when the project is nullable-aware, since the override returns `IEnumerable<object>`.
- `using System.Linq` is requested only when the class has a collection (for `Count()`). The builder may keep it even where implicit usings cover it, which is harmless.
- No boilerplate comment is generated in `GetEqualityComponents`. The only comment is the one explaining the count prefix, emitted once in Value Objects that have a collection.
- Record Value Objects rely on compiler-generated record equality, which compares collections by reference. A fix needs a generated `Equals`/`GetHashCode` and is not done.

## Not covered here

- Mapping DTOs or commands *into* a Value Object association (Create/Update Entity mappings in the Services designer) belongs to `Intent.Modelers.Services.DomainInteractions`, which does not show Value Object association ends in its mapping tree.
- ORM modules (EF Core) treat Value Objects through their own metadata; this module only shapes the domain class.

## History

- 4.3.0:
  - Value Object → Value Object associations generate members.
  - Collection equality is now count-prefixed.
  - The nullable `CS8603` warning is fixed.
  - Coverage in `Tests/CosmosDB`:
    - `Venue`, `Coordinates` and `Schedule` cover associations, including mixed with attributes.
    - `Itinerary` has only two `Coordinates` collections (an attribute and a nullable association). It covers adjacent-collection equality and `GetHashCode` when every collection is empty.
  - `4.3.0-pre.0` shipped element-only collection equality, which merged adjacent collections and could throw from `GetHashCode`. `4.3.0-pre.1` corrects it.
  - `4.3.0-pre.2` depends on `Intent.Modelers.Domain.ValueObjects` `3.6.5-pre.0` (Value Object `New Association` menu).
  - Coverage in `Tests/ValueObjects.Class` and `Tests/ValueObjects.Record`: `Journey` owns `Route`, which has one association to `Waypoint` per `New Association` menu option (nine). Verified with an EF Core save/reload round trip on SQL Server LocalDB for the class variant.
