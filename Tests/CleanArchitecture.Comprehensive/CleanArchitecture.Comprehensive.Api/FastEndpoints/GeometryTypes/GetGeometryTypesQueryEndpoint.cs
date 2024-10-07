using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.GeometryTypes;
using CleanArchitecture.Comprehensive.Application.GeometryTypes.GetGeometryTypes;
using FastEndpoints;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.EndpointTemplate", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Api.FastEndpoints.GeometryTypes
{
    public class GetGeometryTypesQueryEndpoint : EndpointWithoutRequest<List<GeometryTypeDto>>
    {
        private readonly ISender _mediator;

        public GetGeometryTypesQueryEndpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override void Configure()
        {
            Get("api/geometry-types");
            Description(b =>
            {
                b.WithTags("GeometryTypes");
                b.Produces<List<GeometryTypeDto>>(StatusCodes.Status200OK, contentType: MediaTypeNames.Application.Json);
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var result = default(List<GeometryTypeDto>);
            result = await _mediator.Send(new GetGeometryTypesQuery(), ct);
            await SendResultAsync(TypedResults.Ok(result));
        }
    }
}