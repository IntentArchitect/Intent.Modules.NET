### Version 4.1.4

- Update: Nuget package version of `Microsoft.AspNetCore.Authentication.JwtBearer` is now bumped to the latest version based on the configured .NET version to eliminate the known security vulnerabilities.

### Version 4.1.2

- Update: removed various compiler warnings.

### Version 4.1.1

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 4.1.0

- Update: Incorporated changes based on changes made in `Intent.Application.Identity`.

### Version 4.0.1

- `AspNetCoreIdentityConfigurationTemplate` now uses the `CSharpFileBuilder` templating method.

### Version 3.3.9

- Update: Null check added to HttpContext in the event that this gets used outside of an HttpContext scope.
