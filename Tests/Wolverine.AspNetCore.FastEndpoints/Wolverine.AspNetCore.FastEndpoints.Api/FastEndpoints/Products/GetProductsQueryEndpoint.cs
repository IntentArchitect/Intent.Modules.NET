using System.Net.Mime;
using FastEndpoints;
using Intent.RoslynWeaver.Attributes;
using Wolverine.AspNetCore.FastEndpoints.Application.Products;
using Wolverine.AspNetCore.FastEndpoints.Application.Products.GetProducts;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.EndpointTemplate", Version = "1.0")]

namespace Wolverine.AspNetCore.FastEndpoints.Api.FastEndpoints.Products
{
    public class GetProductsQueryEndpoint : EndpointWithoutRequest<List<ProductDto>>
    {
        private readonly IMessageBus _sender;

        public GetProductsQueryEndpoint(IMessageBus sender)
        {
            _sender = sender ?? throw new ArgumentNullException(nameof(sender));
        }

        public override void Configure()
        {
            Get("api/products");
            Description(b =>
            {
                b.WithTags("Products");
                b.Produces<List<ProductDto>>(StatusCodes.Status200OK, contentType: MediaTypeNames.Application.Json);
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
            AllowAnonymous();
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var result = default(List<ProductDto>);
            result = await _sender.InvokeAsync<List<ProductDto>>(new GetProductsQuery(), ct);
            await Send.ResultAsync(TypedResults.Ok(result));
        }
    }
}