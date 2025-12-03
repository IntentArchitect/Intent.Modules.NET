# Intent.AspNetCore.Versioning

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
- Typically set to the oldest supported version

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

## Related Modules

- **Intent.Metadata.WebApi** - HTTP metadata for versioning
