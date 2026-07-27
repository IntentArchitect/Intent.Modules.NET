### Version 1.0.1

- Improvement: Put `ConfigureValidationRules` method in `Merge` mode to reduce friction with AI changes.

### Version 1.0.0

- Fixed: Corrected the declared `Intent.Common`/`Intent.Common.CSharp` module dependency versions, and added the missing `Intent.Modelers.Domain` dependency declaration required by the generated validators.
- Improvement: Updated NuGet package versions.
- New Feature: Initial release.
- New Feature: Generates `AbstractValidator<TCommand>` and `AbstractValidator<TQuery>` classes for each Command and Query element using the shared FluentValidation infrastructure.
- New Feature: Validators support repository injection, custom validation logic, and unique constraint validation based on application settings.
- Improvement: Now depends on `Intent.Application.FluentValidation` so Wolverine apps inherit the `fluent-validation-custom-validation` AI agent skill file, matching what MediatR apps already receive.
