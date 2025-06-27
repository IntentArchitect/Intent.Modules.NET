### Version 3.11.2

- Improvement: Updated referenced packages versions

### Version 3.11.1

- Improvement: Added stereotype descriptions in preperation for Intent Architect 4.5. 

### Version 3.11.0

- Improvement: Updated NuGet package versions.

> ?? **NOTE**
>
> This module update may cause a compilation breaks if you have written any custom code which uses FluentValidation features which are not supported on v12.
> Any generated code will be compliant.
> For details on what the breaking changes are check out the [upgrade guide](https://docs.fluentvalidation.net/en/latest/upgrading-to-12.html).

### Version 3.10.2

- Improvement: Ability to set a a timeout on `Regular Expression` validations

### Version 3.10.1

- Improvement: Ability to set a custom message on `Regular Expression` and `Must` validations

### Version 3.10.0

- Improvement: Help topics added to documention

### Version 3.9.5

- Improvement: Updated module icon
- Improvement: Updated NuGet package versions.
- Improvement: New fluent validation setting to toggle the creation of empty `Validators` for `Commands` and `Queries`.

### Version 3.9.4

- Fixed: Stack overflow occurring in rare circumstances where Advanced Mappings and Basic Mappings are interconnected.

### Version 3.9.3

- TODO

### Version 3.9.2

- Improvement: Updated module NuGet packages infrastructure.

### Version 3.9.1

- Improvement: Updated NuGet packages to latest stables.

### Version 3.9.0

- Improvement: Added Regular Expressions for Validation.
- Improvement: Module project updated to .NET 8.

### Version 3.8.10

- Improvement: Updated Interoperable dependency versions.

### Version 3.8.9

- Improvement: Upgraded module to support new 4.1 SDK features.

### Version 3.8.7

- Improvement: Added CascadeMode option on Fluent Validations to specify this behaviour.

### Version 3.8.6

- Fixed: Would cause slow designer load times when it contained very many DTOs.

### Version 3.8.3

- Improvement: Added "Email Address" option to the "Validation" stereotype.
- Fixed: A Software Factory crash around `DTO Unique Constraint validation` for DTO's mapping to constructors / operations.

### Version 3.8.1

- Fixed: Using composite Index unique constraint validation didn't use the correct using directives or indicate that a repository was needed.

### Version 3.8.0

- Improvement: Further consolidated/generalized logic with other FluentValidation modules.

### Version 3.7.2

- Improvement: All common logic for generating validations has been centralized from other `FluentValidation` modules in this one to ensure consistency and parity between all of them.

### Version 3.7.1

- Improvement interop dependencies for installing Fluent validation modules based on the presence of App Services or MediatR dispatch.

### Version 3.7.0

- Improvement: `FluentValidation.DependencyInjectionExtensions` to version `11.6.0`.
- Fixed: Field mapping targets are now checked recursively to see if they ultimately map to an `Attribute` in order to possibly apply MaximumLength validation due to the presense of an RDBMS `MaxLength` `Text Constraint`. This now covers scenarios where a field is mapped to a Class Operation's Parameter, which itself is mapped to a Class Attribute.

### Version 3.6.0

- Improvement: Removed a deprecated Fluent Validation module dependency. 

### Version 3.5.3

- Fixed: Individual Enums have the `IsInEnum()` rules, and now collection Enums will have `ForEach(x => x.IsInEnum())`.

### Version 3.5.2

- Improvement: Upgraded Fluent Validations to support `CSharpFileBuilder` template paradigm.

### Version 3.5.1

- Improvement: Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 3.4.2

- Fixed: Would throw an exception when encountering a mapped parameter.

### Version 3.4.0

- Improvement: Refactored module based on new developments.
