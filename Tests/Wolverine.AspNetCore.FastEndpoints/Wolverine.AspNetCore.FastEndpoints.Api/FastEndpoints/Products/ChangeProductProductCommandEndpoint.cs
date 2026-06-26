using System.Net.Mime;
using FastEndpoints;
using Intent.RoslynWeaver.Attributes;
using Wolverine.AspNetCore.FastEndpoints.Application.Products.ChangeProductProduct;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.EndpointTemplate", Version = "1.0")]

namespace Wolverine.AspNetCore.FastEndpoints.Api.FastEndpoints.Products
{
    public class ChangeProductProductCommandEndpoint : Endpoint<ChangeProductProductCommand>
    {
        private readonly IMessageBus _sender;

        public ChangeProductProductCommandEndpoint(IMessageBus sender)
        {
            _sender = sender ?? throw new ArgumentNullException(nameof(sender));
        }

        public override void Configure()
        {
            Put("api/products/{id}/change-product");
            Description(b =>
            {
                b.WithTags("Products");
                b.Accepts<ChangeProductProductCommand>(MediaTypeNames.Application.Json);
                b.Produces(StatusCodes.Status204NoContent);
                b.ProducesProblemDetails();
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
            AllowAnonymous();
        }

        public override async Task HandleAsync(ChangeProductProductCommand req, CancellationToken ct)
        {
            await _sender.InvokeAsync(req, ct);
            await Send.ResultAsync(TypedResults.NoContent());
        }
    }
}