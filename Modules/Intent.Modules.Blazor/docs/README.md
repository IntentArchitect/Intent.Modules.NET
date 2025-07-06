# Intent.Blazor

Blazor is a modern web framework from Microsoft that enables you to build rich, interactive web UIs using C# instead of JavaScript. It runs on WebAssembly (Blazor WebAssembly) or on the server via SignalR (Blazor Server), allowing you to share code and libraries across your client and server.

This module generates the foundational Blazor configuration and functionality for your application. Additional modules, such as [Intent.Modules.Blazor.Components.MudBlazor](https://docs.intentarchitect.com/articles/modules-dotnet/intent-blazor-components-mudblazor/intent-blazor-components-mudblazor.html), can then be used to realize the UI design as code.

## Securing Pages and UI Elements

The `Secured` stereotype can be used to secure specific UI elements or entire pages so that only users with the required `policy` or `roles` can access or view them.

The `Secured` stereotype can be applied to the following UI elements to restrict them for unauthorized users:

- **Component**
- **Auto Complete**
- **Button**
- **CheckBox**
- **Link**
- **Menu Item**
- **Radio Group**
- **Select**
- **Table**
- **Text Input**

This ensures that the element is not rendered if the user does not have the required permissions.

You can apply multiple `Secured` stereotypes to an element if multiple `policies` are required, or use a single `Secured` stereotype to specify multiple `roles`.

### AuthenticationStateProvider Configuration

For authorization to work correctly, an `AuthenticationStateProvider` implementation *must* be registered with the DI container. Without this, your application’s navigation and authorization checks will not function properly.

The implementation you register should be based on your specific user authentication method. However, below are examples for development purposes, simulating a **non-authenticated user** and an **always-authenticated user**.

### Non-Authenticated User

The following `AuthenticationStateProvider` implementation simulates an unauthorized user visiting the website.

Create this class in the `.Client` project (for example under `Common/Auth/`):

``` csharp
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace MudBlazor.Sample.Client.Common.Auth;

public class NeverAuthenticatedAuthStateProvider : AuthenticationStateProvider
{
    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity());
        return Task.FromResult(new AuthenticationState(user));
    }
}
```

Then register it with DI. In Program.cs of the client project:

``` csharp
.
.
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddClientServices(builder.Configuration);
// IntentIgnore
builder.Services.AddScoped<AuthenticationStateProvider, NeverAuthenticatedAuthStateProvider>();
builder.Services.AddAuthorizationCore();
.
.
```

### Always-Authenticated User

The following `AuthenticationStateProvider` implementation simulates an always-authorized user visiting the website.

Create this class in the .Client project (for example under Common/Auth/):

``` csharp
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace MudBlazor.Sample.Client.Common.Auth;

public class AlwaysAuthenticatedAuthStateProvider : AuthenticationStateProvider
{
    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
                var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, "Test User"),
            new Claim(ClaimTypes.Email, "testuser@intentarchitect.com"),
            new Claim(ClaimTypes.Role, "Admin")
        }, "FakeAuthentication");

        var user = new ClaimsPrincipal(identity);
        return Task.FromResult(new AuthenticationState(user));
    }
}
```

Finally, register this provider in `Program.cs` of the client project.

> 💡 Only the `// IntentIgnore` comment and the line directly below it need to be added — all other lines will be generated automatically.

``` csharp
.
.
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddClientServices(builder.Configuration);
// IntentIgnore
builder.Services.AddScoped<AuthenticationStateProvider, AlwaysAuthenticatedAuthStateProvider>();
builder.Services.AddAuthorizationCore();
.
.
```
