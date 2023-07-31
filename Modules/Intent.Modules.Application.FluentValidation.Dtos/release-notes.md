### Version 3.6.1

- Updated: All common logic for generating validations has been centralized into the `Intent.Application.FluentValidation` module version `3.7.2` to ensure consistency and parity between all `FluentValidation` modules.
- Updated: `Has Custom Validation` stereotype property has been replaced with, and its value will be automatically migrated to, a new `Must` property named in alignment with the `MustAsync(…)` method call being created.
- Updated: `Custom` stereotype property has been added which creates a `CustomAsync(…)` method call and corresponding method.

### Version 3.6.0

- Updated: `FluentValidation` to version `11.6.0`.

### Version 3.4.1

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 3.3.7

- Updated Validator template so that the developer can easily inject new Repositories for custom validation.
- Updated Validator template so that custom validation is now async by default.