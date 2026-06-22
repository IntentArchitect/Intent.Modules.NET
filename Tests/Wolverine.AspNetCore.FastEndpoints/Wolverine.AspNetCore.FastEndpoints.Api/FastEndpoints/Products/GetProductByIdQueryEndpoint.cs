using System.Net.Mime;
using FastEndpoints;
using Intent.RoslynWeaver.Attributes;
using Wolverine.AspNetCore.FastEndpoints.Application.Products;
using Wolverine.AspNetCore.FastEndpoints.Application.Products.GetProductById;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.EndpointTemplate", Version = "1.0")]

namespace Wolverine.AspNetCore.FastEndpoints.Api.FastEndpoints.Products
{
    public class GetProductByIdQueryEndpoint : Endpoint<GetProductByIdQuery, ProductDto>
    {
        private readonly IMessageBus _sender;

        public GetProductByIdQueryEndpoint(IMessageBus sender)
        {
            _sender = sender ?? throw new ArgumentNullException(nameof(sender));
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
            result = await _sender.InvokeAsync<ProductDto>(req, ct);
            await Send.ResultAsync(TypedResults.Ok(result));
        }
    }
}