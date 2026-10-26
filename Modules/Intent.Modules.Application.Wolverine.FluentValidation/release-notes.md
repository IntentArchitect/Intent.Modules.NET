### Version 1.0.3

- Fixed: The Software Factory failed in a Wolverine application without `Intent.Application.FluentValidation.Dtos`, for example one without Application Services. This module now depends on `Intent.Application.FluentValidation.Dtos`, whose validator provider both `Intent.Application.Wolverine`'s `ValidationMiddleware` and validation rules for nested DTOs need.

### Version 1.0.2

- Improvement: Added default Template Classification and Priorities.

### Version 1.0.1

- Improvement: Put `ConfigureValidationRules` method in `Merge` mode to reduce friction with AI changes.

### Version 1.0.0

- Fixed: Corrected the declared `Intent.Common`/`Intent.Common.CSharp` module dependency versions, and added the missing `Intent.Modelers.Domain` dependency declaration required by the generated validators.
- Improvement: Updated NuGet package versions.
- New Feature: Initial release.
- New Feature: Generates `AbstractValidator<TCommand>` and `AbstractValidator<TQuery>` classes for each Command and Query element using the shared FluentValidation infrastructure.
- New Feature: Validators support repository injection, custom validation logic, and unique constraint validation based on application settings.
- Improvement: Now depends on `Intent.Application.FluentValidation` so Wolverine apps inherit the `fluent-validation-custom-validation` AI agent skill file, matching what MediatR apps already receive.
