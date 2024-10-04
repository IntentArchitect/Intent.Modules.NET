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
