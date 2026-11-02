using System.Net.Mime;
using FastEndpoints;
using HttpQuery.FastEndpoints.Application.Products;
using HttpQuery.FastEndpoints.Application.Products.SearchProducts;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.EndpointTemplate", Version = "1.0")]

namespace HttpQuery.FastEndpoints.Api.FastEndpoints.Products
{
    public class SearchProductsQueryEndpoint : Endpoint<SearchProductsQuery, List<ProductDto>>
    {
        private readonly ISender _mediator;

        public SearchProductsQueryEndpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override void Configure()
        {
            Post("api/products/search");
            Description(b =>
            {
                b.WithTags("Products");
                b.WithSummary(@"HTTP QUERY endpoint (modelled as POST until Http Settings supports QUERY). Filters products by the criteria in the request body; every criterion is optional.");
                b.Accepts<SearchProductsQuery>(MediaTypeNames.Application.Json);
                b.Produces<List<ProductDto>>(StatusCodes.Status200OK, contentType: MediaTypeNames.Application.Json);
                b.ProducesProblemDetails();
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
            AllowAnonymous();
        }

        public override async Task HandleAsync(SearchProductsQuery req, CancellationToken ct)
        {
            var result = default(List<ProductDto>);
            result = await _mediator.Send(req, ct);
            await Send.ResultAsync(TypedResults.Ok(result));
        }
    }
}