using System.Net.Mime;
using FastEndpoints;
using Intent.RoslynWeaver.Attributes;
using Wolverine.AspNetCore.FastEndpoints.Application.Products.CreateProduct;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.EndpointTemplate", Version = "1.0")]

namespace Wolverine.AspNetCore.FastEndpoints.Api.FastEndpoints.Products
{
    public class CreateProductCommandEndpoint : Endpoint<CreateProductCommand, Guid>
    {
        private readonly IMessageBus _sender;

        public CreateProductCommandEndpoint(IMessageBus sender)
        {
            _sender = sender ?? throw new ArgumentNullException(nameof(sender));
        }

        public override void Configure()
        {
            Post("api/products");
            Description(b =>
            {
                b.WithTags("Products");
                b.Accepts<CreateProductCommand>(MediaTypeNames.Application.Json);
                b.Produces<Guid>(StatusCodes.Status201Created);
                b.ProducesProblemDetails();
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
            AllowAnonymous();
        }

        public override async Task HandleAsync(CreateProductCommand req, CancellationToken ct)
        {
            var result = Guid.Empty;
            result = await _sender.InvokeAsync<Guid>(req, ct);
            await Send.CreatedAtAsync<GetProductByIdQueryEndpoint>(new { id = result }, result, cancellation: ct);
        }
    }
}