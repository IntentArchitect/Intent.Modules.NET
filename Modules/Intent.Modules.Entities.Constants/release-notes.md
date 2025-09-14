### Version 1.0.5

- Fixed: Increased `order` value for some `OnBuild(...)` method invocations to ensure they happen sufficiently after `OnBuild(...)` from other modules have been invoked.

### Version 1.0.4

- Improvement: Updated to cater for scenario where the `ConfigureValidationRules` method is not present on a validator
- Improvement: Updated module icon
 
### Version 1.0.3

- Improvement: Updated module NuGet packages infrastructure.

### Version 1.0.2

- Improvement: Made interoperability with validators more robust.

### Version 1.0.1

- Will now also apply to validations for `DTO`s (was previously only applied to validations for `Command`s and `Query`s).

### Version 1.0.0

- Added: Template which introduces constants for `Text Constraints` and references the constants rather than numeric literals.
Note: Some of the features of this moddule are only available with
`Intent.Application.MediatR.FluentValidation` version >= 4.1.2
`Intent.EntityFrameworkCore` version >= 4.4.1
