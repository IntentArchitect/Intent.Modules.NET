### Version 3.12.3

- Improvement: Updated module documentation to use centralized documentation site.

### Version 3.12.2

- Improvement: Methods with no auto-implementation will now generate with an `IntentManaged` Body mode of Merge and their `throw new NotImplemented()` statements have been commented with [IntentInitialGen to prevent them from returning when deleted](https://docs.intentarchitect.com/articles/application-development/code-management/code-management-csharp/code-management-csharp.html#the--intentinitialgen-instruction). (Note: A side effect of this improvement is that entries may appear in the Customizations tab of the Software Factory or existing entries may no longer be approved and they will need to reviewed).

### Version 3.12.1

- Improvement: Updated referenced packages versions

### Version 3.12.0

- Improvement: Updated NuGet package versions.

> ?? **NOTE**
>
> This module update may cause a compilation breaks if you have written any custom code which uses FluentValidation features which are not supported on v12.
> Any generated code will be compliant.
> For details on what the breaking changes are check out the [upgrade guide](https://docs.fluentvalidation.net/en/latest/upgrading-to-12.html).

### Version 3.11.3

- Fixed: Unique constraint enabled wrongfully enhances DTO validation on DTO representing a composite Entity that doesn't even have a unique index applied.

### Version 3.11.2

- Improvement: Ability to set a timeout on `Regular Expression` validations

### Version 3.11.1

- Improvement: Ability to set a custom message on `Regular Expression` and `Must` validations

### Version 3.11.0

- Improvement: Help topics added to documentation.

### Version 3.10.3

- Improvement: Updated module icon
- Improvement: New fluent validation setting to toggle the creation of empty `Validators` for `Commands` and `Queries`.

### Version 3.10.2

- Fixed: Parent validators are now invoked using dependency injection, and empty validators are no longer generated in some scenarios

### Version 3.10.1

- Fixed: Stackoverflow occurs in rare circumstances where Advanced Mappings and Basic Mappings are interconnected.
- Fixed: Validator now generated for a base DTO, with the child DTO validator calling the base validator
 
### Version 3.10.0

- Fixed: Domain constraints are not being propagated into validators when the data graph is more than two levels deep.
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
- Fix: Fixed the issue where DTO's for compositional entities does not include validation rules.

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
