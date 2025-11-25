# API Versioning

Apply versioning strategies to ASP.NET Core APIs for managing endpoint evolution.

## What This Module Does

This module enables API versioning, allowing you to support multiple API versions simultaneously and evolve your API without breaking existing clients.

When enabled, you can:
- Define different versions of the same endpoint
- Support clients on older API versions while developing newer versions
- Deprecate old API versions with clear migration paths
- Add new operations in newer versions

## Generated Artifacts

### API Versioning Configuration
- **ApiVersioningConfiguration** - Sets up versioning service in dependency injection
- **ApiVersionSwaggerGenOptions** - Configures Swagger/OpenAPI to display all API versions

### Controller Attributes
For each versioned endpoint:
- `[ApiVersion("1.0")]` - Marks endpoint as version 1.0
- `[ApiVersion("2.0")]` - Marks endpoint as version 2.0
- Support multiple versions on same endpoint: `[ApiVersion("1.0"), ApiVersion("2.0")]`

### API Version Mapping
Supports multiple versioning schemes:
- **URL Path** - `/api/v1/endpoint`, `/api/v2/endpoint`
- **Query String** - `?api-version=1.0`
- **HTTP Header** - `X-Api-Version: 1.0`
- **Media Type** - `application/vnd.myapi.v1+json`

## Key Design Patterns

### Version-Specific Endpoints
Different implementations per version:
```csharp
// Version 1.0 - older implementation
[ApiVersion("1.0")]
public async Task<ActionResult<UserV1Dto>> GetUser(int id) { }

// Version 2.0 - enhanced implementation
[ApiVersion("2.0")]
public async Task<ActionResult<UserV2Dto>> GetUser(int id) { }
```

### Deprecation Marking
Mark old versions for removal:
- `[Deprecated]` - Indicates version is deprecated
- Swagger shows deprecation warnings to API consumers
- Clear migration timeline for clients

### Default Version
Define default API version:
- When no version specified in request, default version used
- Provides backward compatibility for unaware clients
- Typically set to oldest supported version

### Version Sunset
Graceful API evolution:
1. Release new feature in v2.0
2. Mark v1.0 as deprecated
3. Communicate sunset date to API consumers
4. Remove v1.0 support after grace period
5. Only v2.0 remains

## Integration with Other Modules

### Required Dependencies
- **Intent.AspNetCore** - ASP.NET Core hosting foundation
- **Intent.AspNetCore.Controllers** - Controller generation with versioning attributes
- **Intent.Metadata.WebApi** - HTTP metadata including version specifications

### Recommended Integrations
- **Intent.AspNetCore.Swashbuckle** - Swagger documentation showing all API versions
- **Intent.AspNetCore.Scalar** - Interactive API documentation with version selector
- **Intent.Application.Contracts** - Versioned service contracts
- **Intent.AspNetCore.Controllers.Dispatch.MediatR** or **ServiceContract** - Request dispatching

## Customization Points

### Versioning Scheme Selection
Via factory extensions:
- **ControllerInstaller** - Applies version attributes to controllers
- Choose URL path, query string, or header-based versioning

### Version Configuration
Customize via `ApiVersioningConfiguration` settings:
- **Supported Versions** - List of versions to support (e.g., 1.0, 2.0, 3.0)
- **Default Version** - Version used when none specified
- **Deprecated Versions** - Versions marked for removal

### Swagger Generation
Via `ApiVersionSwaggerGenOptions`:
- **Document per Version** - Separate Swagger docs for each version
- **Description** - Version-specific descriptions
- **Deprecated Indicator** - Visual marking of deprecated versions

### Class Name Overrides
- `ClassName` - ApiVersioningConfiguration class name
- `Namespace` - Configuration class namespace

## Versioning Strategies Comparison

### URL Path Versioning (Most Common)
- **Pros**: Clear in URL, easy to debug, SEO-friendly
- **Cons**: URL structure changes per version
- **Example**: `/api/v1/products`, `/api/v2/products`

### Query String Versioning
- **Pros**: Single base URL, backward compatible
- **Cons**: Less discoverable, easily overlooked
- **Example**: `GET /api/products?api-version=1.0`

### Header-Based Versioning
- **Pros**: Clean URLs, semantic versioning
- **Cons**: Not visible in browser, harder to test manually
- **Example**: `X-Api-Version: 1.0` header

### Content Negotiation
- **Pros**: Standards-based, follows REST principles
- **Cons**: Complex, rarely used in practice
- **Example**: `Accept: application/vnd.myapi.v1+json`

## When To Use

**Use API Versioning when:**
- Your API is public or consumed by multiple clients
- You need to evolve API without breaking existing clients
- You support long-term backward compatibility requirements
- You have clear contracts with API consumers

**Don't use when:**
- Building internal APIs with single code deployment
- All clients update simultaneously
- API is consumed only by first-party applications
- Breaking changes acceptable with client updates

## Best Practices

1. **Semantic Versioning** - Use MAJOR.MINOR (e.g., 1.0, 1.1, 2.0)
2. **Deprecation Timeline** - Provide 6-12 month deprecation period
3. **Clear Documentation** - Document what changed in each version
4. **Minimal Versions** - Support only 2-3 versions simultaneously
5. **Migration Guides** - Help clients upgrade from old to new versions
6. **Automatic Routing** - Route requests to correct handler per version

## Documentation Link

For comprehensive guidance on API versioning strategies and implementation patterns, see:
https://docs.intentarchitect.com/articles/modules-dotnet/intent-aspnetcore-versioning/intent-aspnetcore-versioning.html

## Related Modules

- **Intent.AspNetCore** - ASP.NET Core foundation
- **Intent.AspNetCore.Controllers** - Controller generation
- **Intent.AspNetCore.Swashbuckle** - Swagger documentation with versioning support
- **Intent.AspNetCore.Scalar** - Interactive API documentation
- **Intent.Metadata.WebApi** - HTTP metadata for versioning
