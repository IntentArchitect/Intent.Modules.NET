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
