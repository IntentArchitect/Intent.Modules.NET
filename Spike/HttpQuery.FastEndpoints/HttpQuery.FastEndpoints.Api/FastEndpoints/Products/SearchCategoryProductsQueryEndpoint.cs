using System.Net.Mime;
using FastEndpoints;
using HttpQuery.FastEndpoints.Application.Products;
using HttpQuery.FastEndpoints.Application.Products.SearchCategoryProducts;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.EndpointTemplate", Version = "1.0")]

namespace HttpQuery.FastEndpoints.Api.FastEndpoints.Products
{
    public class SearchCategoryProductsQueryEndpoint : Endpoint<SearchCategoryProductsQuery, List<ProductDto>>
    {
        private readonly ISender _mediator;

        public SearchCategoryProductsQueryEndpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override void Configure()
        {
            Post("api/categories/{category}/products/search");
            Description(b =>
            {
                b.WithTags("Products");
                b.WithSummary(@"HTTP QUERY endpoint (modelled as POST until Http Settings supports QUERY). Category comes from the route; the remaining criteria from the request body.");
                b.Accepts<SearchCategoryProductsQuery>(MediaTypeNames.Application.Json);
                b.Produces<List<ProductDto>>(StatusCodes.Status200OK, contentType: MediaTypeNames.Application.Json);
                b.ProducesProblemDetails();
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
            AllowAnonymous();
        }

        public override async Task HandleAsync(SearchCategoryProductsQuery req, CancellationToken ct)
        {
            var result = default(List<ProductDto>);
            result = await _mediator.Send(req, ct);
            await Send.ResultAsync(TypedResults.Ok(result));
        }
    }
}