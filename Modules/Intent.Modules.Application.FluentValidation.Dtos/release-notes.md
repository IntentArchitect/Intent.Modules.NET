### Version 3.10.0

- Fixed: Domain constraints not being propagated into validators when the data graph is more than two levels deep.
- Fixed: Validations not being applied when mapping to constructor parameters.

### Version 3.9.3

- Improvement: Updated module NuGet packages infrastructure.

### Version 3.9.2

- Improvement: Updated NuGet packages to latest stables.

### Version 3.9.1

- Improvement: Added `TODO` comments on `NotImplementedException`.
- Fix: Unique Constraint validation ignores Included columns.

### Version 3.9.0

- Improvement: Added Regular Expressions for Validation.
- Improvement: Module project updated to .NET 8.
- Fix: Fixed issue where DTO's for compositional entities does not include validation rules.

### Version 3.8.2

- Improvement: Ignore custom validation rules when generating service proxies.

### Version 3.8.1

- Improvement: Added IntentManaged Body Merge attribute to the `ConfigureValidationRules` method to prevent it from being updated when the Software Factory is re-executed.

### Version 3.8.0

- Fixed: Only creates validations for inbound DTOs and not outbound ones.

### Version 3.7.3

- Improvement: Removed `// IntentMatch(...)` code management instructions from templates which are no longer needed since version `4.4.0` of the `Intent.OutputManager.RoslynWeaver` module.
- Improvement: Added CascadeMode option on Fluent Validations to specify this behaviour.
 
### Version 3.7.2

- Fixed: Migration improvement using Roslyn instead of Regex to avoid updating incorrect IntentManaged attributes. 

### Version 3.7.1

- Improvement: Added support for the [new Email Address](https://github.com/IntentArchitect/Intent.Modules.NET/blob/development/Modules/Intent.Modules.Application.FluentValidation/release-notes.md#version-383) option on the Validation stereotype.
- Fixed: A Software Factory crash around `DTO Unique Constraint validation` for DTO's mapping to constructors / operations.

### Version 3.7.0

- Improvement: DTO Validators will no longer be generated for DTOs which are outbound only.
- Improvement: `[IntentManaged(Mode.Fully)]` is no longer added to the `ConfigureValidationRules` method as it's redundant.
- Improvement: `[IntentManaged(Mode.Merge, Signature = Mode.Fully)]` is no longer added to the class, this is to reduce noise in the file as it's rare to add custom methods and in such cases they can have `[Mode.Ignore]` added to them.
- Fixed: Nested DTO Validators introduced and will inject `IServiceProvider` to resolve Validators via DI.

### Version 3.6.4

- Improvement: Further consolidated/generalized logic with other FluentValidation modules.

### Version 3.6.2

- Improvement: Removed compiler warning on possible null being returned.

### Version 3.6.1

- Improvement: All common logic for generating validations has been centralized into the `Intent.Application.FluentValidation` module version `3.7.2` to ensure consistency and parity between all `FluentValidation` modules.
- Improvement: `Has Custom Validation` stereotype property has been replaced with, and its value will be automatically migrated to, a new `Must` property named in alignment with the `MustAsync()` method call being created.
- Improvement: `Custom` stereotype property has been added which creates a `CustomAsync()` method call and corresponding method.

### Version 3.6.0

- Improvement: `FluentValidation` to version `11.6.0`.

### Version 3.4.1

- Improvement dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 3.3.7

- Improvement: Updated Validator template so that the developer can easily inject new Repositories for custom validation.
- Improvement: Updated Validator template so that custom validation is now async by default.
