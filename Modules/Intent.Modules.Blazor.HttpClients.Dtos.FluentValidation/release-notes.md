### Version 1.2.8

- Fixed: DTO Validation generation is only generated when valid DTOs are present.

### Version 1.2.7

- Improvement: Updated referenced packages versions
- Fixed: Under certain circumstances the Software Factory would fail with a "More than one instance of `<templateId>` was found with `<modelId>`" error.

### Version 1.2.6

- Improvement: Updated referenced packages versions

### Version 1.2.5

- Fix: Updated dependency on 'Intent.Blazor.FluentValidation' to be a newer version, to fox module dependency issue.

### Version 1.2.4

- Improvement: Ability to set a a timeout on `Regular Expression` validations

### Version 1.2.3

- Improvement: Abililty to set a custom message on `Regular Expression` and `Must` validations

### Version 1.2.2

- Improvement: Updated module icon
- Improvement: New fluent validation setting to toggle the creation of empty `Validators` for `Commands` and `Queries`.

### Version 1.2.1

- Improvement: Updated referenced libraries

### Version 1.2.0

TODO

### Version 1.1.3

- Improvement: Updated module NuGet packages infrastructure.

### Version 1.1.2

- Improvement: Updated NuGet packages to latest stables.

### Version 1.1.1

- Improvement: Added `TODO` comments on `NotImplementedException`.
- Fix: Unique Constraint validation ignores Included columns.

### Version 1.1.0

- Improvement: Added Regular Expressions for Validation.
- Improvement: Module project updated to .NET 8.
- Fix: Fixed issue where DTO's for compositional entities does not include validation rules.

### Version 1.0.6

- Improvement: Ignore custom validation rules when generating service proxies.

### Version 1.0.5

- Improvement: Added IntentManaged Body Merge attribute to the `ConfigureValidationRules` method to prevent it from being updated when the Software Factory is re-executed.

### Version 1.0.4

- Improvement: Upgraded module to support new 4.1 SDK features.

### Version 1.0.3

- Improvement: Removed `// IntentMatch(...)` code management instructions from templates which are no longer needed since version `4.4.0` of the `Intent.OutputManager.RoslynWeaver` module.
- Improvement: Added CascadeMode option on Fluent Validations to specify this behaviour.
- 
### Version 1.0.2

- Fixed: Migration improvement using Roslyn instead of Regex to avoid updating incorrect IntentManaged attributes.
- Fixed: When there were no validators being generated (no DTOs in the designer), the FluentValidation NuGet package would not get installed causing a compilation error.

### Version 1.0.1

- Improvement: Added support for the [new Email Address](https://github.com/IntentArchitect/Intent.Modules.NET/blob/development/Modules/Intent.Modules.Application.FluentValidation/release-notes.md#version-383) option on the Validation stereotype.
- Fixed: A Software Factory crash around `DTO Unique Constraint validation` for DTO's mapping to constructors / operations.

### Version 1.0.0

- Initial release.
