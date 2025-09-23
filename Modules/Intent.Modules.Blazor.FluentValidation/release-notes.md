### Version 1.1.6

- Improvement: Updated `ModelDefinition` validators namespaces to respect folders.
- Improvement: Add a setting to opt into  `ModelDefinition` validators, as these are not standard for all component libraries.

### Version 1.1.5

- Fixed: DTO Validation generation is only generated when valid DTOs are present.

### Version 1.1.4

- Improvement: Updated referenced packages versions

### Version 1.1.3

- Improvement: Updated referenced packages versions
- Improvement: Methods with no auto-implementation will now generate with an `IntentManaged` Body mode of Merge and their `throw new NotImplemented()` statements have been commented with [IntentInitialGen to prevent them from returning when deleted](https://docs.intentarchitect.com/articles/application-development/code-management/code-management-csharp/code-management-csharp.html#the--intentinitialgen-instruction). (Note: A side effect of this improvement is that entries may appear in the Customizations tab of the Software Factory or existing entries may no longer be approved and they will need to reviewed).

### Version 1.1.2

- Improvement: Updated referenced packages versions

### Version 1.1.1

- Improvement: Added stereotype descriptions in preparation for Intent Architect 4.5. 

### Version 1.1.0

- Improvement: Updated NuGet package versions.

> ?? **NOTE**
>
> This module update may cause a compilation breaks if you have written any custom code which uses FluentValidation features which are not supported on v12.
> Any generated code will be compliant.
> For details on what the breaking changes are check out the [upgrade guide](https://docs.fluentvalidation.net/en/latest/upgrading-to-12.html).

### Version 1.0.4

- Fixed: Validation classes for Model Definitions will generate correctly when Model Definitions are placed inside Components.

### Version 1.0.3

- Improvement: Ability to set a timeout on `Regular Expression` validations

### Version 1.0.2

- Improvement: Ability to set a custom message on `Regular Expression` and `Must` validations

### Version 1.0.1

- Improvement: Updated module icon
- Improvement: New fluent validation setting to toggle the creation of empty `Validators` for `Commands` and `Queries`.
- Improvement: Updated NuGet package versions.

### Version 1.0.0

- Initial release.
