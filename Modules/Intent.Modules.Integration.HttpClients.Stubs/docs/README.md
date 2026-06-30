# Intent.Integration.HttpClients.Stubs

This module generates stub implementations of the service contracts produced by the `Intent.Integration.HttpClients` module. It generates a dedicated `<App>.Infrastructure.Stubs` project containing one stub per HTTP client, plus a dependency-injection registration that swaps the real HTTP clients for stubs when enabled through configuration.

Stubs let you run and test an application without the downstream services its HTTP clients call — useful for local development, demos, and integration tests where the real endpoints are unavailable or undesirable.

## What This Module Generates

On install the module adds a dedicated stub project beside the application's Infrastructure project, then generates the stubs and their registration into it:

- **`<App>.Infrastructure.Stubs` project** — created by an on-install migration. It is placed in the same solution folder as the `<App>.Infrastructure` project and inherits its .NET settings (SDK, target framework, implicit usings), so the stubs compile against the same framework as the rest of the application.
- **`<Service>HttpClientStub`** (role `Stubs.HttpClientStub`) — one class per HTTP client service proxy. Each implements the generated service contract interface and provides a method per endpoint that returns a safe default value.
- **`StubHttpClientConfiguration`** (role `Stubs.Configuration`) — a static class exposing the `AddStubHttpClients(this IServiceCollection, IConfiguration)` extension method that conditionally replaces the real client registrations with stubs.

## The Stub Project

The `<App>.Infrastructure.Stubs` project does not exist in a fresh application — it is created the first time this module is installed, by an on-install migration that extends the Codebase Structure designer:

1. It locates the application's Infrastructure project (via its `Infrastructure` output anchor, falling back to the `*.Infrastructure` naming convention).
2. It creates an `<App>.Infrastructure.Stubs` C# project in the same solution folder, copying the Infrastructure project's `.NET Settings` so the stub project targets the same framework.
3. It adds a single `Stubs` output anchor inside the new project. The stub templates bind to this anchor by role, so their output lands inside the framework-targeted stub project.

The migration is idempotent: if an `<App>.Infrastructure.Stubs` project already exists it makes no changes, so re-running the Software Factory or re-installing the module is safe.

## Stub Implementations

For each HTTP client, the module generates a stub class implementing the service contract. Every endpoint method returns a default value so the application can run without the downstream service, and the body is yours to customize:

```csharp
public class CustomersServiceHttpClientStub : ICustomersService
{
    [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
    public async Task<CustomerDto> GetCustomerByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(new CustomerDto
        {
            Id = Guid.Empty,
            Email = string.Empty,
            Name = string.Empty,
            Surname = string.Empty
        });
    }

    // ... a method per endpoint
}
```

Each method is marked `[IntentManaged(Mode.Fully, Body = Mode.Ignore)]`: the Software Factory writes the safe default body once, then leaves it untouched on subsequent runs while keeping the method signature in sync with the contract. Edit a stub's body to give it meaningful canned behaviour — your implementation is preserved across regenerations, with no attribute change needed.

### Paged results

When an endpoint returns a `PagedResult<T>`, the stub returns a single-item page that reflects the request rather than an empty, all-zero result. `TotalCount` and `PageCount` are set to `1` (the one item returned), and `PageNumber`/`PageSize` echo the request's paging fields:

```csharp
public async Task<PagedResult<OrderDto>> GetOrdersPagedAsync(
    GetOrdersPagedQuery query,
    CancellationToken cancellationToken = default)
{
    return await Task.FromResult(new PagedResult<OrderDto>
    {
        TotalCount = 1,
        PageCount = 1,
        PageSize = query.PageSize,
        PageNumber = query.PageNo,
        Data = new List<OrderDto> { /* one fully-populated item */ }
    });
}
```

The paging fields are located by name on the query/command, using the same conventions as the rest of Intent — `page` / `pageno` / `pagenum` / `pagenumber` / `pageindex` for the page number and `size` / `pagesize` for the page size. If a paged request has no recognizable paging fields, those two counters fall back to `0`.

## Conditional Registration

The generated `StubHttpClientConfiguration` exposes `AddStubHttpClients`, which replaces a real client registration with its stub only when the corresponding `UseStub` setting is enabled:

```csharp
public static IServiceCollection AddStubHttpClients(this IServiceCollection services, IConfiguration configuration)
{
    if (UseStubHttpClient(configuration, "CleanArchitecture.Comprehensive.Services", "CustomersService"))
    {
        services.RemoveAll<ICustomersService>();
        services.AddTransient<ICustomersService, CustomersServiceHttpClientStub>();
    }
    // ... an entry per client
    return services;
}
```

The module wires `services.AddStubHttpClients(configuration);` into the application's composition root at a priority that orders it **after** the real HTTP client registrations (`AddInfrastructure`), so the real registrations exist to be removed and replaced. When `UseStub` is `false` (the default) the real clients are left untouched.

### Why a dedicated project

The stubs and their `AddStubHttpClients` registration are isolated in `<App>.Infrastructure.Stubs` rather than folded into the main infrastructure. This makes stubbing a single, removable capability with clear operational guarantees:

- **Excludable from production.** The entire mechanism — the stub classes *and* the registration that activates them — lives in one project that can be omitted from production build and deployment pipelines, so stub code never ships to environments where it does not belong.
- **Safe by construction, not just by configuration.** If the stubs project is not deployed, `AddStubHttpClients` is not there to run. No `appsettings.json` edit or environment override can swap a real client for a stub, so in production the real endpoints simply cannot be switched off.
- **Contained control.** Whether stubs are in play is governed entirely by this one project and its `UseStub` settings, keeping the decision explicit and in one place rather than threaded through the main registration code.

## Module Settings

This module adds no Module Builder settings. Stubbing is controlled entirely by one `appsettings.json` flag — **`UseStub`** — which the module registers (defaulting to `false`) alongside the `Uri` / `Timeout` / `IdentityClientKey` that `Intent.Integration.HttpClients` already generates.

**What a "group" is.** Every generated HTTP client belongs to a *group* — the downstream application it calls. All clients to the same application share that group's connection settings, which is why `Uri` / `Timeout` are configured once per group rather than per client. `UseStub` works the same way and can be set at two levels:

| Level | Where the key goes | Effect |
|---|---|---|
| **Group** | `UseStub` inside the group object (beside its `Uri`) | stubs **every** client that calls that application |
| **Service** | `UseStub` in a key named after a single service | stubs **just that one** client; overrides its group |

> Both are plain keys directly under `HttpClients` — a flat list, not a nesting. A service key sits *beside* its group key, never inside it.

**Group level — stub a whole downstream application:**

```json
"HttpClients": {
  "Ordering.Services": {
    "Uri": "https://localhost:44365/",
    "IdentityClientKey": "default",
    "Timeout": "00:01:00",
    "UseStub": true
  }
}
```

Every client that calls the Ordering application is now stubbed. (There's no single master switch — to stub *all* downstream applications, set `UseStub: true` in each group.)

**Service level — stub or un-stub one client.** The service key is a sibling of the group. Here the whole Ordering application is stubbed by default, except `OrdersService`, which keeps hitting the real endpoint:

```json
"HttpClients": {
  "Ordering.Services": {
    "Uri": "https://localhost:44365/",
    "IdentityClientKey": "default",
    "Timeout": "00:01:00",
    "UseStub": true
  },
  "OrdersService": {
    "UseStub": false
  }
}
```

`UseStub` is resolved **most-specific-first**: the service key (`HttpClients:OrdersService:UseStub`) wins if present, otherwise the group key (`HttpClients:Ordering.Services:UseStub`), otherwise `false`.

## Related Modules

### [Intent.Integration.HttpClients](https://docs.intentarchitect.com/articles/modules-dotnet/intent-integration-httpclients/intent-integration-httpclients.html)
Generates the real HTTP client implementations and their `appsettings.json` configuration. This module generates stand-in implementations of the same service contracts and reuses the `HttpClients` configuration section, adding the `UseStub` switch.

### [Intent.Application.Contracts.Clients](https://docs.intentarchitect.com/articles/modules-dotnet/intent-application-contracts-clients/intent-application-contracts-clients.html)
Generates the service contract interfaces and DTOs that the stubs implement and return.

### [Intent.VisualStudio.Projects](https://docs.intentarchitect.com/articles/modules-dotnet/intent-visualstudio-projects/intent-visualstudio-projects.html)
Provides the Codebase Structure project model that the on-install migration extends to add the `<App>.Infrastructure.Stubs` project.
