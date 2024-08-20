### Version 4.1.2

- Improvement: Updated module NuGet packages infrastructure.

### Version 4.1.1

- Improvement: Updated NuGet packages to latest stables.

### Version 4.1.0

- Improvement: Upgraded module to support new 4.1 SDK features.

### Version 4.0.8

- Improvement: Updated to be compatible with .NET 8.

### Version 4.0.7

- Various improvements when using the `Identity User` stereotype:
  - When applying the `Identity User` stereotype to a `Class`, the designer will automatically update the class to ensure it has a primary key which matches the default primary key of ASP.NET Identity's User entity.
  - The software factory will output errors for common misconfigurations of the `Identity User` stereotype.
  - For the entity on which `Identity User` stereotype is applied, the `Id` property will no longer generated as it was hiding the same named property on the base type.
  - If the `Identity User` stereotype is applied to a class called "User" or "Users", there will no longer be a `Users` property generated on the `DbContext` as it was hiding the same named property on the base type.

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
