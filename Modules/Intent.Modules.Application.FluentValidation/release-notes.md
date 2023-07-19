### Version 3.7.1

- Updated interop dependencies for installing Fluent validation modules based on the presence of App Services or MediatR dispatch.

### Version 3.7.0

- Updated: `FluentValidation.DependencyInjectionExtensions` to version `11.6.0`.
- Field mapping targets are now checked recursively to see if they ultimately map to an `Attribute` in order to possibly apply MaximumLength validation due to the presense of an RDBMS `MaxLength` `Text Constraint`. This now covers scenarios where a field is mapped to a Class Operation's Parameter, which itself is mapped to a Class Attribute.

### Version 3.6.0

- Update: Removed a deprecated Fluent Validation module dependency. 

### Version 3.5.3

- Fixed: Individual Enums have the `IsInEnum()` rules, and now collection Enums will have `ForEach(x => x.IsInEnum())`.

### Version 3.5.2

- Update: Upgraded Fluent Validations to support `CSharpFileBuilder` template paradigm.

### Version 3.5.1

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 3.4.2

- Fixed: Would throw an exception when encountering a mapped parameter.

### Version 3.4.0

- Update: Refactored module based on new developments.