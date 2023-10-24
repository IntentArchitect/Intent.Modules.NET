### Version 2.1.3

- Fixed: Login not working with Postgres, issue related to non UTC DateTime.

### Version 2.0.2

- Update: Nuget package version of `Microsoft.AspNetCore.Authentication.JwtBearer` is now bumped to the latest version based on the configured .NET version to eliminate the known security vulnerabilities.

### Version 2.0.1

- Fixed: Fixed a issue around "role" claim not being configured.

### Version 2.0.0

- Added Refresh Token functionality.
- `AccountController` class refactored to have `TokenService` separately.

### Version 1.1.2

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 1.1.1

- Update: Role Claims are now introduced.

### Version 1.1.0

- Update: Updates applied due to changes made in `Intent.Security.JWT`.

### Version 1.0.0

- New: Lightweight Account Controller to handle Authentication requests and returning JWT tokens.