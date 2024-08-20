### Version 4.0.10

- Improvement: Updated module NuGet packages infrastructure.

### Version 4.0.9

- Improvement: Improved internal NuGet package management.

### Version 4.0.8

- Fixed: Member mapping configurations traversing a nullable association could result in a runtime error when the mapping was used with Entity Framework Core with an `IQueryable`.
- Improvement: Generated member mapping configurations traversing nullable associations will now also include a null-forgiving operator (`!`) to prevent code analysis tools from warning of potential null reference exceptions. The warning is a false positive as AutoMapper itself ensures that no null reference exceptions will be thrown.

### Version 4.0.7

- Fixed: Mappings adding castings for expression based mapping paths, sometimes cause expression to break. Removed the casting in these scenarios.

### Version 4.0.6

- Fixed: Making a mapping from Entity that inherits from a base class to a DTO causes compiler errors.

### Version 4.0.4

- Mapping extensions now use expression statements instead of method bodies.

### Version 4.0.4

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 4.0.3
- Fixed: Pluralized `Entity` names causing uncompilable `DTO` code.

### Version 4.0.2

- Change: `AutoMapperDTODecorator` replaced with `AutoMapperDTOFactoryExtension` to take advantage of the new `ICSharpFileBuilderTemplate` paradigm.

### Version 4.0.1

- Fixed: Mappping paths like `property.Count(x => x.Method())` would be incorrectly transformed to `property.Count(x => X.Method())` in AutoMapper profiles.

### Version 3.3.6

- Fixed: Mapping from a DTO field that is of a complex type to a Domain Entity association that is also of Complex type no longer results in trying to map a surrogate key to the DTO Field.

### Version 3.3.4

- New: DTO Field mappings that map from an Entity on an association level can now be mapped to a primitive type that represents a surrogate key and it will automatically map the Ids.
