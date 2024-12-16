# Intent.FastEndpoints

The `Intent.FastEndpoints` module provides an alternative to Minimal APIs and ASP.NET Core Controllers, focusing on a developer-friendly approach that maintains performance metrics comparable to Minimal APIs. By using Intent Architect, developers can seamlessly transition from traditional ASP.NET Controllers to FastEndpoints.

FastEndpoints serves as an implementation detail, allowing you to model your Services, Commands, and Queries in Intent Architect just as you would for ASP.NET Core Controllers.

### Why Use FastEndpoints?

Choosing FastEndpoints offers several advantages:

- **Improved Modularity**: Each endpoint is encapsulated in its own class, promoting clean code and separation of concerns.
- **High Performance**: FastEndpoints maintains performance metrics on par with Minimal APIs while facilitating a developer-friendly coding environment.
- **Simplicity**: Reduces boilerplate code often associated with traditional ASP.NET Controllers, allowing for faster development and easier maintenance.

For more information on the FastEndpoints library, please visit the official site: [FastEndpoints](https://fast-endpoints.com/).

## Installation Instructions

To integrate the FastEndpoints module, it's essential to uninstall the following modules first, if they are currently in use:

- **Intent.AspNetCore.Versioning** (if present)
- **Intent.AspNetCore.Controllers.Dispatch.MediatR** (if present)
- **Intent.AspNetCore.Controllers.Dispatch.ServiceContract** (if present)
- **Intent.AspNetCore.Controllers**

Upon installation, you will find a new **FastEndpoints** folder in your API project, which will contain the individual endpoint classes.

## Getting Started with FastEndpoints

FastEndpoints promotes modular design by allowing you to define a separate class for each endpoint instead of combining multiple action methods within a single controller class or a file full of Minimal API routes.

### Example of a FastEndpoints Class

Here’s a simple example of a FastEndpoints class demonstrating how to configure a `CreateClientCommandEndpoint`:

```csharp
public class CreateClientCommandEndpoint : Endpoint<CreateClientCommand, JsonResponse<Guid>>
{
    private readonly ISender _mediator;

    public CreateClientCommandEndpoint(ISender mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public override void Configure()
    {
        Post("api/clients");
        Description(b =>
        {
            b.WithTags("Clients");
            b.Accepts<CreateClientCommand>(MediaTypeNames.Application.Json);
            b.Produces<JsonResponse<Guid>>(StatusCodes.Status201Created, contentType: MediaTypeNames.Application.Json);
            b.ProducesProblemDetails();
            b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
        });
        AllowAnonymous();
        Options(x => x.WithVersionSet(">>Api Version<<").MapToApiVersion(new ApiVersion(1.0)));
    }

    public override async Task HandleAsync(CreateClientCommand req, CancellationToken ct)
    {
        var result = await _mediator.Send(req, ct);
        await SendCreatedAtAsync<GetClientByIdQueryEndpoint>(new { id = result }, new JsonResponse<Guid>(result), cancellation: ct);
    }
}
```

>[!NOTE]
> While FastEndpoints offers a streamlined and efficient method for creating endpoints, it may not support every feature that ASP.NET Core Controllers do. The `Intent.FastEndpoints` module is currently in beta, and there might be limitations in capabilities compared to the traditional ASP.NET framework.
>
>If you notice any missing features or capabilities that you need, please reach out to us on GitHub to log a feature request for future enhancements.
