using System;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Api.FastEndpoints.ResponseTypes;
using CleanArchitecture.Comprehensive.Application.AggregateRootLongs.CreateAggregateRootLong;
using FastEndpoints;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.EndpointTemplate", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Api.FastEndpoints.AggregateRootLongs
{
    public class CreateAggregateRootLongCommandEndpoint : Endpoint<CreateAggregateRootLongCommand, JsonResponse<long>>
    {
        private readonly ISender _mediator;

        public CreateAggregateRootLongCommandEndpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override void Configure()
        {
            Post("api/aggregate-root-longs");
            Description(b =>
            {
                b.WithTags("AggregateRootLongs");
                b.Accepts<CreateAggregateRootLongCommand>(MediaTypeNames.Application.Json);
                b.Produces<JsonResponse<long>>(StatusCodes.Status201Created, contentType: MediaTypeNames.Application.Json);
                b.ProducesProblemDetails();
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
        }

        public override async Task HandleAsync(CreateAggregateRootLongCommand req, CancellationToken ct)
        {
            var result = default(long);
            result = await _mediator.Send(req, ct);
            await SendCreatedAtAsync<GetAggregateRootLongByIdQueryEndpoint>(new { id = result }, new JsonResponse<long>(result), cancellation: ct);
        }
    }
}