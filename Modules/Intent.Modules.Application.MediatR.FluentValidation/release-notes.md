### Version 4.9.6

- Improvement: Updated NuGet package versions.

### Version 4.9.5

- Improvement: Updated NuGet package versions.

### Version 4.9.4

- Improvement: Updated module documentation to use centralized documentation site.

### Version 4.9.3

- Fixed: Removed unnecessary `MediatR` using directive for Dependency Injection.

### Version 4.9.2

- Improvement: Methods with no auto-implementation will now generate with an `IntentManaged` Body mode of Merge and their `throw new NotImplemented()` statements have been commented with [IntentInitialGen to prevent them from returning when deleted](https://docs.intentarchitect.com/articles/application-development/code-management/code-management-csharp/code-management-csharp.html#the--intentinitialgen-instruction). (Note: A side effect of this improvement is that entries may appear in the Customizations tab of the Software Factory or existing entries may no longer be approved and they will need to reviewed).

### Version 4.9.1

- Improvement: Updated referenced packages versions

### Version 4.9.0

- Improvement: `CancellationToken` is now used as an argument to the `next()` delegate in the MediatR behaviour template.
- Improvement: Updated NuGet package versions.

> ⚠️ **NOTE**
>
> This module update may cause a compilation breaks if you have written any custom code which uses FluentValidation features which are not supported on v12.
> Any generated code will be compliant.
> For details on what the breaking changes are check out the [upgrade guide](https://docs.fluentvalidation.net/en/latest/upgrading-to-12.html).


### Version 4.8.4

- Fixed: Unique constraint enabled wrongfully enhances DTO validation on DTO representing a composite Entity that doesn't even have a unique index applied.

### Version 4.8.3

- Improvement: Ability to set a timeout on `Regular Expression` validations

### Version 4.8.2

- Improvement: Included module help topic.

### Version 4.8.1

- Improvement: Ability to set a custom message on `Regular Expression` and `Must` validations

### Version 4.8.0

- Improvement: Help topics added to documentation.

### Version 4.7.3

- Improvement: Updated module icon
- Improvement: New fluent validation setting to toggle the creation of empty `Validators` for `Commands` and `Queries`.

### Version 4.7.2

- Improvement: Updated NuGet package versions.
- Improvement: Comment added to `Command`/`Query` validators to provide context of empty method

### Version 4.7.1

- Improvement: Updated `ValidationBehaviour` template to use the Builder pattern. Updated styling to more align with best practices
- Fixed: Fixed warning around `catch (ValidationException ex)` where `ex` is not used in `UnnhandledExceptionHandler` .
- Fixed: Validator now generated for a base DTO, with the child DTO validator calling the base validator

### Version 4.7.0

- Fixed: Domain constraints are not being propagated into validators when the data graph is more than two levels deep.
- Fixed: Validations not being applied when mapping to constructor parameters.

### Version 4.6.4

- Fixed: Update the `UnhandledExceptionBehaviour` in `Intent.Application.MediatR.Behaviours` to not allow logging of `ValidationExceptions` as part of unhandled exceptions.

### Version 4.6.3

- Improvement: Updated module NuGet packages infrastructure.

### Version 4.6.2

- Improvement: Updated NuGet packages to latest stables.

### Version 4.6.1

- Improvement: Added `TODO` comments on `NotImplementedException`.
- Fix: Unique Constraint validation ignores Included columns.

### Version 4.6.0

- Improvement: Added Regular Expressions for Validation.
- Improvement: Module project updated to .NET 8.
- Fix: Fixed the issue where DTO's for compositional entities does not include validation rules.

### Version 4.5.3

- Improvement: Ignore custom validation rules when generating service proxies.

### Version 4.5.2

- Improvement: Added IntentManaged Body Merge attribute to the `ConfigureValidationRules` method to prevent it from being updated when the Software Factory is re-executed.

### Version 4.5.1

- Improvement: Upgraded module to support new 4.1 SDK features.

### Version 4.5.0

- Improvement: Removed `// IntentMatch(...)` code management instructions from templates which are no longer needed since version `4.4.0` of the `Intent.OutputManager.RoslynWeaver` module.
- Improvement: Added CascadeMode option on Fluent Validations to specify this behaviour.
- Fixed: Text constraint and Index unique constraints will work again with advanced mapping scenarios.

### Version 4.4.3

- Fixed: Migration improvement using Roslyn instead of Regex to avoid updating incorrect IntentManaged attributes.

### Version 4.4.2

- Improvement: Added support for the [new Email Address](https://github.com/IntentArchitect/Intent.Modules.NET/blob/development/Modules/Intent.Modules.Application.FluentValidation/release-notes.md#version-383) option on the Validation stereotype.
- Fixed: A Software Factory crash around `DTO Unique Constraint validation` for DTO's mapping to constructors / operations.

### Version 4.4.0

- Improvement: `[IntentManaged(Mode.Fully)]` is no longer added to the `ConfigureValidationRules` method as it's redundant.
- Fixed: Nested DTO Validators introduced and will inject `IServiceProvider` to resolve Validators via DI.

### Version 4.3.2

- Improvement: Further consolidated/generalized logic with other FluentValidation modules.

### Version 4.3.0

- Update : Upgraded MediatR Package v12.
- Updated: All common logic for generating validations has been centralized into the `Intent.Application.FluentValidation` module version `3.7.2` to ensure consistency and parity between all `FluentValidation` modules.
- Updated: `Has Custom Validation` stereotype property has been replaced with, and its value will be automatically migrated to, a new `Must` property named in alignment with the `MustAsync()` method call being created.
- Updated: `Custom` stereotype property has been added which creates a `CustomAsync()` method call and corresponding method.

### Version 4.2.1

- Fixed spelling mistake in generated exception message.

### Version 4.2.0

- Updated: `FluentValidation` to version `11.6.0`.

### Version 4.1.2

- Upgrade: `CommandValidatorTemplate` moved to the `CSharpFileBuilder` paradigm.

### Version 4.1.1

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 4.0.1

- Update: Refactored module based on new developments.
