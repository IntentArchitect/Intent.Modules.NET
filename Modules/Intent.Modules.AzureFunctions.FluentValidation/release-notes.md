### Version 4.2.2

- Improvement: Updated module NuGet packages infrastructure.

### Version 4.2.1

- Improvement: Updated NuGet packages to latest stables.

### Version 4.2.0

- Improvement: Added Regular Expressions for Validation.
- Improvement: Module project updated to .NET 8.

### Version 4.1.7

- Improvement: Updated icon to SVG format.

### Version 4.1.6

- Fixed: Updated dependencies to introduce latest fixes.

### Version 4.1.3

- Update: Adds cancellation tokens where appropriate.

### Version 4.1.2

- Fix: There where some interplay issues between this module and  `Intent.Application.FluentValidation.Dtos` which have been addressed. There may be some left over using clauses which you simply remove, they will be of the format `using {application name}.Application.Common.Validation`.

### Version 4.1.1

- Fixed so that Validation Exception is now expected to be caught on a Function level when any Fluent Validation module is present.

### Version 4.1.0

- Updated: `FluentValidation` to version `11.6.0`.

### Version 4.0.4

- Update: Internal dependency version update to keep compatibility with other modules.

### Version 4.0.2

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 4.0.1

- Support for Intent.AzureFunctions 4.0.1.

### Version 4.0.0

- Support for Intent.AzureFunctions 4.0.0.

### Version 3.3.10

- Update: Renamed ValidationBehaviour to ValidationService with an associated Interface.
- Update: Introduced interface for ValidationService.

### Version 3.3.7

- Update: Done internal code cleanup.
