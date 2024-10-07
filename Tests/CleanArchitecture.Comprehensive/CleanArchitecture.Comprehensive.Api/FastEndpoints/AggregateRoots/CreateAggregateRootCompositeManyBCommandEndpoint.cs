using System;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Api.FastEndpoints.ResponseTypes;
using CleanArchitecture.Comprehensive.Application.AggregateRoots.CreateAggregateRootCompositeManyB;
using FastEndpoints;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.EndpointTemplate", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Api.FastEndpoints.AggregateRoots
{
    public class CreateAggregateRootCompositeManyBCommandEndpoint : Endpoint<CreateAggregateRootCompositeManyBCommand, JsonResponse<Guid>>
    {
        private readonly ISender _mediator;

        public CreateAggregateRootCompositeManyBCommandEndpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override void Configure()
        {
            Post("api/aggregate-roots/{aggregaterootid}/compositemanybs");
            Description(b =>
            {
                b.WithTags("AggregateRoots");
                b.Accepts<CreateAggregateRootCompositeManyBCommand>(MediaTypeNames.Application.Json);
                b.Produces<JsonResponse<Guid>>(StatusCodes.Status201Created, contentType: MediaTypeNames.Application.Json);
                b.ProducesProblemDetails();
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
        }

        public override async Task HandleAsync(CreateAggregateRootCompositeManyBCommand req, CancellationToken ct)
        {
            var result = default(Guid);
            result = await _mediator.Send(req, ct);
            await SendCreatedAtAsync<GetAggregateRootCompositeManyBByIdQueryEndpoint>(new { aggregateRootId = req.AggregateRootId, id = result }, new JsonResponse<Guid>(result), cancellation: ct);
        }
    }
}