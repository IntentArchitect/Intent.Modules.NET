using System;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Api.FastEndpoints.ResponseTypes;
using CleanArchitecture.Comprehensive.Application.GeometryTypes.CreateGeometryType;
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
    public class CreateGeometryTypeCommandEndpoint : Endpoint<CreateGeometryTypeCommand, JsonResponse<Guid>>
    {
        private readonly ISender _mediator;

        public CreateGeometryTypeCommandEndpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override void Configure()
        {
            Post("api/geometry-types");
            Description(b =>
            {
                b.WithTags("GeometryTypes");
                b.Accepts<CreateGeometryTypeCommand>(MediaTypeNames.Application.Json);
                b.Produces<JsonResponse<Guid>>(StatusCodes.Status201Created, contentType: MediaTypeNames.Application.Json);
                b.ProducesProblemDetails();
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
        }

        public override async Task HandleAsync(CreateGeometryTypeCommand req, CancellationToken ct)
        {
            var result = default(Guid);
            result = await _mediator.Send(req, ct);
            await SendCreatedAtAsync<GetGeometryTypeByIdQueryEndpoint>(new { id = result }, new JsonResponse<Guid>(result), cancellation: ct);
        }
    }
}