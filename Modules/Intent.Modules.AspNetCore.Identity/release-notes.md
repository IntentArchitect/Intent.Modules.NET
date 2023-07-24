### Version 4.0.6

- Updated `IdentityServer4.EntityFramework` nuget package to 7.0.9.

### Version 4.0.5

- Fixed: Under certain circumstances `Startup.cs` would not have `services.ConfigureIdentity();` added to it.
- Fixed: When applying `Identity` stereotype on a Class, it didn't update everywhere the appropriate Identity class.

### Version 4.0.4

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 4.0.3

- Updated supported client version to [3.2.0, 5.0.0).

### Version 4.0.1

- Registration of ASP.NET Core Identity is now done without registration of Cookie based authentication.
