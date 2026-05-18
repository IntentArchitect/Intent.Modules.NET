### Version 1.1.0
- Fixed: When entities needed to be fully qualified, mapping file has no compilation errors
- Fixed: When a navigation of a field hops across classes or fully qualified namespaces prefix `@`, mapperly's "full `nameof`", to the path to prevent field name collisions.
- Fixed: When a domain entity is fully qualified prefix `@`, mapperly's "full `nameof`", to the path to prevent field name collisions.
- Fixed: When an `Enum` is used in the return type of an explict mapping it's associated `using` statements are now added.
- Fixed: When a domain entity has a pluralised name mappings fully qualify it to prevent conflict with the namespace
- Fixed: When a `List` is mapped accross multiple classes in a explicit mapping, the associated LINQ `Select` statement is added to perform the map
- Fixed: When two or more Dtos in the same folder were of the form `<Entity>Dto.cs` and `<Entity><AnyString>Dto` the correct dto is now injected into the constructor of the corresponding handlers

### Version 1.0.4

- Improvement: Split Mapperly mapping path generation for attribute paths vs runtime expressions to avoid trimming and keep `nameof(...)` paths clean.
- Fixed: Nullable navigation expression generation to use `?` for nullable-to-nullable mappings and `!` for nullable-to-non-nullable mappings.

### Version 1.0.3

- Improvement: Improved Mapperly templates to handle complex mappings.

### Version 1.0.2

- Improvement: Updated NuGet package versions.

### Version 1.0.1

- Improvement: Updated NuGet package versions.

### Version 1.0.0

Initial release.
