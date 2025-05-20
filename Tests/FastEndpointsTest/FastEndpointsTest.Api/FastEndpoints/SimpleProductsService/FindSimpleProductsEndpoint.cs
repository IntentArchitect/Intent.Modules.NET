using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Asp.Versioning;
using FastEndpoints;
using FastEndpoints.AspVersioning;
using FastEndpointsTest.Application.Interfaces;
using FastEndpointsTest.Application.SimpleProducts;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.EndpointTemplate", Version = "1.0")]

namespace FastEndpointsTest.Api.FastEndpoints.SimpleProductsService
{
    public class FindSimpleProductsEndpoint : EndpointWithoutRequest<List<SimpleProductDto>>
    {
        private readonly ISimpleProductsService _appService;

        public FindSimpleProductsEndpoint(ISimpleProductsService appService)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
        }

        public override void Configure()
        {
            Get("api/simple-products");
            Description(b =>
            {
                b.WithTags("SimpleProductsService");
                b.Produces<List<SimpleProductDto>>(StatusCodes.Status200OK, contentType: MediaTypeNames.Application.Json);
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
            AllowAnonymous();
            Options(x => x.WithVersionSet(">>Api Version<<").MapToApiVersion(new ApiVersion(1.0)));
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var result = default(List<SimpleProductDto>);
            result = await _appService.FindSimpleProducts(ct);
            await SendResultAsync(TypedResults.Ok(result));
        }
    }
}