using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Asp.Versioning;
using FastEndpoints;
using FastEndpoints.AspVersioning;
using FastEndpointsTest.Api.FastEndpoints.ResponseTypes;
using FastEndpointsTest.Application.Interfaces.ScalarCollectionReturnType;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.EndpointTemplate", Version = "1.0")]

namespace FastEndpointsTest.Api.FastEndpoints.ScalarCollectionReturnType.ServiceWithScalarWithCollectionReturn
{
    public class DoScalarWithCollectionReturnEndpoint : EndpointWithoutRequest<JsonResponse<List<string>>>
    {
        private readonly IServiceWithScalarWithCollectionReturnService _appService;

        public DoScalarWithCollectionReturnEndpoint(IServiceWithScalarWithCollectionReturnService appService)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
        }

        public override void Configure()
        {
            Get("do-scalar-with-collection-return");
            Description(b =>
            {
                b.WithTags("ServiceWithScalarWithCollectionReturn");
                b.Produces<JsonResponse<List<string>>>(StatusCodes.Status200OK, contentType: MediaTypeNames.Application.Json);
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
            AllowAnonymous();
            Options(x => x.WithVersionSet(">>Api Version<<").MapToApiVersion(new ApiVersion(1.0)));
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var result = default(List<string>);
            result = await _appService.DoScalarWithCollectionReturn(ct);
            await SendResultAsync(TypedResults.Ok(new JsonResponse<List<string>>(result)));
        }
    }
}