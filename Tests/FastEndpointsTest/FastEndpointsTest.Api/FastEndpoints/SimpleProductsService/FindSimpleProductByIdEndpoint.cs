using System;
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
    public class FindSimpleProductByIdEndpoint : Endpoint<FindSimpleProductByIdRequestModel, SimpleProductDto>
    {
        private readonly ISimpleProductsService _appService;

        public FindSimpleProductByIdEndpoint(ISimpleProductsService appService)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
        }

        public override void Configure()
        {
            Get("api/simple-products/{id}");
            Description(b =>
            {
                b.WithTags("SimpleProductsService");
                b.Accepts<FindSimpleProductByIdRequestModel>();
                b.Produces<SimpleProductDto>(StatusCodes.Status200OK, contentType: MediaTypeNames.Application.Json);
                b.ProducesProblemDetails();
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
            AllowAnonymous();
            Options(x => x.WithVersionSet(">>Api Version<<").MapToApiVersion(new ApiVersion(1.0)));
        }

        public override async Task HandleAsync(FindSimpleProductByIdRequestModel req, CancellationToken ct)
        {
            var result = default(SimpleProductDto);
            result = await _appService.FindSimpleProductById(req.Id, ct);
            await Send.ResultAsync(TypedResults.Ok(result));
        }
    }

    public class FindSimpleProductByIdRequestModel
    {
        public Guid Id { get; set; }
    }
}