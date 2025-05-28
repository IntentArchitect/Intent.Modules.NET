### Version 1.1.1

- Improvement: Added stereotype descriptions in preperation for Intent Architect 4.5. 

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
