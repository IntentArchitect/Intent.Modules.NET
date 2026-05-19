### Version 1.1.0
- Improvement: If a navigation of a field hops across classes or enities are fully qualified prefix `@`, mapperly's "full `nameof`", to the path to prevent field name collisions.  
See: https://mapperly.riok.app/docs/configuration/full-nameof/
- Fixed: When entities needed to be fully qualified, mapping file is now generated correcly
- Fixed: When an `Enum` is in the return type of an explict mapping it's associated `using` statements are added.
- Fixed: Domain entities with pluralised names are now fully qualified it to prevent conflicting with the namespace name
- Fixed: When a `List` is mapped accross multiple classes in a explicit mapping, the associated LINQ `Select` statement is added to perform the map

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
