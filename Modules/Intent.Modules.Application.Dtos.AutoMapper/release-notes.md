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