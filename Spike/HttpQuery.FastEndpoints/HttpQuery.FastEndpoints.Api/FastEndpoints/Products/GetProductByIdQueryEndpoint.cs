using System.Net.Mime;
using FastEndpoints;
using HttpQuery.FastEndpoints.Application.Products;
using HttpQuery.FastEndpoints.Application.Products.GetProductById;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.EndpointTemplate", Version = "1.0")]

namespace HttpQuery.FastEndpoints.Api.FastEndpoints.Products
{
    public class GetProductByIdQueryEndpoint : Endpoint<GetProductByIdQuery, ProductDto>
    {
        private readonly ISender _mediator;

        public GetProductByIdQueryEndpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override void Configure()
        {
            Get("api/products/{id}");
            Description(b =>
            {
                b.WithTags("Products");
                b.Accepts<GetProductByIdQuery>();
                b.Produces<ProductDto>(StatusCodes.Status200OK, contentType: MediaTypeNames.Application.Json);
                b.ProducesProblemDetails();
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
            AllowAnonymous();
        }

        public override async Task HandleAsync(GetProductByIdQuery req, CancellationToken ct)
        {
            var result = default(ProductDto);
            result = await _mediator.Send(req, ct);
            await Send.ResultAsync(TypedResults.Ok(result));
        }
    }
}