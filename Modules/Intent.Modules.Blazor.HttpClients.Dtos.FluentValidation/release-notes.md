### Version 1.0.2

- Fixed: Migration improvement using Roslyn instead of Regex to avoid updating incorrect IntentManaged attributes.
- Fixed: When there were no validators being generated (no DTOs in the designer), the FluentValidation NuGet package would not get installed causing a compilation error.

### Version 1.0.1

- Improvement: Added support for the [new Email Address](https://github.com/IntentArchitect/Intent.Modules.NET/blob/development/Modules/Intent.Modules.Application.FluentValidation/release-notes.md#version-383) option on the Validation stereotype.
- Fixed: A Software Factory crash around `DTO Unique Constraint validation` for DTO's mapping to constructors / operations.

### Version 1.0.0

- Initial release.
