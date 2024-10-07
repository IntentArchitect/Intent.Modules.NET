using System;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.GeometryTypes;
using CleanArchitecture.Comprehensive.Application.GeometryTypes.GetGeometryTypeById;
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
    public class GetGeometryTypeByIdQueryEndpoint : Endpoint<GetGeometryTypeByIdQuery, GeometryTypeDto>
    {
        private readonly ISender _mediator;

        public GetGeometryTypeByIdQueryEndpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override void Configure()
        {
            Get("api/geometry-types/{id}");
            Description(b =>
            {
                b.WithTags("GeometryTypes");
                b.Accepts<GetGeometryTypeByIdQuery>();
                b.Produces<GeometryTypeDto>(StatusCodes.Status200OK, contentType: MediaTypeNames.Application.Json);
                b.ProducesProblemDetails();
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
        }

        public override async Task HandleAsync(GetGeometryTypeByIdQuery req, CancellationToken ct)
        {
            var result = default(GeometryTypeDto);
            result = await _mediator.Send(req, ct);
            await SendResultAsync(TypedResults.Ok(result));
        }
    }
}