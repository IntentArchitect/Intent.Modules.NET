### Version 2.0.13

- Improvement: Updated module documentation to use centralized documentation site.

### Version 2.0.12

- Improvement: Updated NuGet package versions.
- Fixed: Under certain circumstances the `EFContainerFixture` file would be missing a `Microsoft.EntityFrameworkCore.Migrations` using directive.

### Version 2.0.11

- Improvement: Updated NuGet package versions.
- Fixed: Nullable primitive return types (non wrapped as json responses) caused un-compilable code.


### Version 2.0.10

- Improvement: Updated NuGet package versions.
- Improvement: adding using clause for `HttpRequestMessage` in `HttpCLient`s.
- Improvement: Updated referenced projects.
 
### Version 2.0.9

- Improvement: Service proxies will now generate within any folders they are modelled within.

### Version 2.0.8

- Improvement: Updated NuGet package versions.
- Improvement: Added MongoDB.Drive support

### Version 2.0.7

- Fixed: Correct variable path used when invoking an external service and mapping to entity

### Version 2.0.6

- Improvement: Updated NuGet package versions.
- Fixed: Fixed Software Factory error when generic DTO is reused as endpoint return object.

### Version 2.0.5

- Improvement: Updated NuGet package versions.

### Version 2.0.4


- Improvement: Adjusted text namespaces to align with folders, which were causing xunit issues in some scenarios.
- Improvement: Updated NuGet package versions.

### Version 2.0.3

- Improvement: Updated NuGet package versions.

### Version 2.0.2

- Improvement: Updated NuGet package versions.

### Version 2.0.1

- Improvement: Updated NuGet package versions.
- Fixed: `Content-length` parameter now set correctly when creating a proxy service.

### Version 2.0.0

- Improvement: Upgraded to XUnit v3. 

> ⚠️ **NOTE**
>
> This module update may cause a compilation breaks if you are using xUnit v2 features which are not supported on v3.
> Any generated code will be migrated to be v3 compliant.
> For details on what the breaking changes are check out the [XUnit documentation on migrating from v2 to v3](https://xunit.net/docs/getting-started/v3/migration).

- Fixed: If the `HTTP endpoint` is versioned, the version number is added to the test class and filename to avoid naming conflicts.

### Version 1.1.2

- Improvement: Updated NuGet package versions.

### Version 1.1.1

- Improvement: Updated NuGet package versions.
- Improvement: Updated Test Fixture to work with new TestContainer connection string.

### Version 1.1.0

- Fixed: Integration Tests wouldn't run when Minimal Hosting Model was enabled with the use of Serilog. See this [GitHub issue](https://github.com/serilog/serilog-aspnetcore/issues/289). This will update the Program.cs file to adjust the Serilog bootstrap configuration.

### Version 1.0.13

- Fixed: `Create` method on `DtoContracts` will now use the default values from the original `Command/Query`.

### Version 1.0.12

- Improvement: Included module help topic.

### Version 1.0.11

- Improvement: Updated module icon

### Version 1.0.10

- Improvement: Improvements in generated http client code: recommended implementation of `IDisposable pattern`, `JsonSerializerOptions` only added if required, as well as small synatax updates
- Improvement: Default values set on non-nullable properties on `HttpClientRequestException`
- Improvement: Updated `Microsoft.NET.Test.Sdk` package to 17.6.0.
- Improvement: MediaType `constant` defined and reused in http client, instead of duplicated string literals
- Improvement: Updated NuGet package versions.

### Version 1.0.9

- Improvement: Generated code is more aligned with best practices

### Version 1.0.8

- Fixed: Internal change on how we model PagedResults using new 4.3 tech.

### Version 1.0.7

- Fixed: HttpClients are able to generate into `netstandard2.*` projects.

### Version 1.0.6

- Improvement: Updated module NuGet packages infrastructure.
- Improvement: Updated NuGet packages to latest stables.
- Improvement: Bump due to shared dependency.

### Version 1.0.5

- New Feature: Added container fixture support for MongoDb.
- Improvement: DbContext registrations for Integration tests, retain all application configuration options.
- Improvement: Upgraded `Testcontainers.*` packages to 3.9.0.
- Fixed: Integration Service proxies now respect the `ApiSetting` `Serialze Enums as Strings`.

### Version 1.0.4

- Improvement: Updated the logic which converts types to query parameters to be more standard in how it coverts types to string.

### Version 1.0.3

- Improvement: Added support for deserializing ProblemDetails for client HTTP calls.

### Version 1.0.2

- Fixed: Added missing using clause in generated CRUD tests.

### Version 1.0.1

- Improvement: File transfer ( upload / download ) support on proxies.

### Version 1.0.0

- New Feature: Asp.Net Core Integration Testing module.
