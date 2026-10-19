### Version 4.3.0

- Improvement: A Value Object can now contain other Value Objects through associations, not only through attributes. Each navigable association from a Value Object to another Value Object generates a property, a constructor parameter and an equality component (`1` required, `0..1` nullable, `0..*` a collection). Previously these associations were accepted by the designer but silently produced nothing.
- **Breaking change**: Value Objects with associations to other Value Objects gain a constructor parameter per association, appended after the attribute parameters. Hand-written `new MyValueObject(...)` calls for those Value Objects must pass the new arguments (`null` for `0..1` and `0..*` associations).
- Fixed: Value Objects holding a collection were never equal to each other, because `GetEqualityComponents` yielded the collection itself (compared by reference). Each collection now contributes its element count followed by its elements, so equal collections compare equal and items cannot shift between adjacent collections (`[a, b] + [c]` is not equal to `[a] + [b, c]`). A `null` collection counts as empty.
- Fixed: `GetHashCode` no longer throws `InvalidOperationException: Sequence contains no elements` for a Value Object whose only members are empty collections.
- Fixed: `CS8603` warning in `GetEqualityComponents` for nullable properties.
- Improvement: Removed the boilerplate `// Using a yield return statement...` comment from `GetEqualityComponents`.
- Known limitation: Record Value Objects (Value Object Type = Record) use compiler-generated equality, which compares collection members by reference, so two records holding equal collections are never equal. This now also applies to collection associations on records.

### Version 4.2.10

- Improvement: Updated output path of some templates to be more consistant with other templates.

### Version 4.2.9

- Improvement: Added default Template Classification and Priorities.

### Version 4.2.8

- Fixed: Minimum client version.

### Version 4.2.7

- Improvement: Updated module documentation to use centralized documentation site.

### Version 4.2.6

- Improvement: Bumped dependency on `Intent.Modelers.Domain.ValueObjects` to latest (3.6.2).

### Version 4.2.5

- Improvement: Added module documentation.

### Version 4.2.4

- Improvement: `ValueObject`s will now get a default constructor (if no other constructor is defined) to set the value of qualifying attributes to `null!`
- Improvement: Updated module icon

### Version 4.2.3

- Reorder of methods in `ValueObject` according to access level to align with best practices

### Version 4.2.2

- Improvement: Cleaned up various code warnings.

### Version 4.2.1

- Improvement: Updated module NuGet packages infrastructure.

### Version 4.2.0

- New Feature: Value Objects can now be `Record` types.
- Improvement: Updated to .NET 8 for Module project.

### Version 4.1.3

- Fixed: Value Object equality comparisons now taking into account EF Lazy Loading Proxies (Castle Proxies).

### Version 4.1.1

- Fixed: Nullability related compiler warnings.

### Version 4.1.0

- Improvement: Adds protected constructors for serializers to use.
- Improvement: Value Objects now have a `Serialization Settings` stereotype to indicate how this Value Object should be serialized.

### Version 4.0.0 

- Improvement: Upgraded Templates to use new Builder Pattern paradigm.
