### Version 3.5.3

- Improvement: Updated module NuGet packages infrastructure.

### Version 3.5.2

- Improvement: Added support for different UserId Types (guid, long, int).

### Version 3.5.1

- Improvement: The generated `AuthorizeAttribute` will no longer cause nullable warnings.

### Version 3.5.0

- Improvement: Updated role names from `Startup` to `Distributed` on some Templates.

### Version 3.4.2

- Improvement: Removed various compiler warnings.

### Version 3.4.1

- Improvement: Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 3.4.0

- Improvement: Supplies skeleton `CurrentUserService` in order to allow custom implementations to supply current user information.
- Improvement: Configuration class is also now supplied through this module to wire up Dependency Injection concerns.
