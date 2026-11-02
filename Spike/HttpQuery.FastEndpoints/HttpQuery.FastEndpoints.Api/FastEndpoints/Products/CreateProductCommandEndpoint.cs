using System.Net.Mime;
using FastEndpoints;
using HttpQuery.FastEndpoints.Application.Products.CreateProduct;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.EndpointTemplate", Version = "1.0")]

namespace HttpQuery.FastEndpoints.Api.FastEndpoints.Products
{
    public class CreateProductCommandEndpoint : Endpoint<CreateProductCommand, Guid>
    {
        private readonly ISender _mediator;

        public CreateProductCommandEndpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
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
            result = await _mediator.Send(req, ct);
            await Send.CreatedAtAsync<GetProductByIdQueryEndpoint>(new { id = result }, result, cancellation: ct);
        }
    }
}