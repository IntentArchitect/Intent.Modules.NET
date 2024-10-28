using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Asp.Versioning;
using FastEndpoints;
using FastEndpoints.AspVersioning;
using FastEndpointsTest.Api.FastEndpoints.ResponseTypes;
using FastEndpointsTest.Application.ScalarCollectionReturnType.QueryWithCollectionReturn;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.EndpointTemplate", Version = "1.0")]

namespace FastEndpointsTest.Api.FastEndpoints.ScalarCollectionReturnType
{
    public class QueryWithCollectionReturnEndpoint : EndpointWithoutRequest<JsonResponse<List<string>>>
    {
        private readonly ISender _mediator;

        public QueryWithCollectionReturnEndpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override void Configure()
        {
            Get("api/scalar-collection-return-type/query-with-collection-return");
            Description(b =>
            {
                b.WithTags("ScalarCollectionReturnType");
                b.Produces<JsonResponse<List<string>>>(StatusCodes.Status200OK, contentType: MediaTypeNames.Application.Json);
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
            AllowAnonymous();
            Options(x => x.WithVersionSet(">>Api Version<<").MapToApiVersion(new ApiVersion(1.0)));
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var result = default(List<string>);
            result = await _mediator.Send(new QueryWithCollectionReturn(), ct);
            await SendResultAsync(TypedResults.Ok(new JsonResponse<List<string>>(result)));
        }
    }
}