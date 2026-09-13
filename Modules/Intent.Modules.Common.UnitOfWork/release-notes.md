### Version 1.0.5

- Added: An `ApplyUnitOfWorkImplementations` overload for consumers with no existing invocation statement to wrap, so a middleware-style consumer can generate just the persistence-save logic.

### Version 1.0.4

- Fixed: `GetUnitOfWorkSettings()` threw a `NullReferenceException` instead of returning `null` when called by another module while `Intent.Common.UnitOfWork` is not installed.

### Version 1.0.3

- Improvement: Updated module documentation to use centralized documentation site.

### Version 1.0.2

- Updated: Module icon

### Version 1.0.1

- Fixed: Reference no longer added to the VS designer

### Version 1.0.0

Initial Release
