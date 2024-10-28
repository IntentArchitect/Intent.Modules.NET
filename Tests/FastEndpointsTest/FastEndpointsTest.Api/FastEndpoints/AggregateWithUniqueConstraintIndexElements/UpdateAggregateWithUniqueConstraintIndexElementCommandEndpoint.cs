using System;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Asp.Versioning;
using FastEndpoints;
using FastEndpoints.AspVersioning;
using FastEndpointsTest.Application.AggregateWithUniqueConstraintIndexElements.UpdateAggregateWithUniqueConstraintIndexElement;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.EndpointTemplate", Version = "1.0")]

namespace FastEndpointsTest.Api.FastEndpoints.AggregateWithUniqueConstraintIndexElements
{
    public class UpdateAggregateWithUniqueConstraintIndexElementCommandEndpoint : Endpoint<UpdateAggregateWithUniqueConstraintIndexElementCommand>
    {
        private readonly ISender _mediator;

        public UpdateAggregateWithUniqueConstraintIndexElementCommandEndpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override void Configure()
        {
            Put("api/aggregate-with-unique-constraint-index-elements/{id}");
            Description(b =>
            {
                b.WithTags("AggregateWithUniqueConstraintIndexElements");
                b.Accepts<UpdateAggregateWithUniqueConstraintIndexElementCommand>(MediaTypeNames.Application.Json);
                b.Produces(StatusCodes.Status204NoContent);
                b.ProducesProblemDetails();
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
            AllowAnonymous();
            Options(x => x.WithVersionSet(">>Api Version<<").MapToApiVersion(new ApiVersion(1.0)));
        }

        public override async Task HandleAsync(
            UpdateAggregateWithUniqueConstraintIndexElementCommand req,
            CancellationToken ct)
        {
            await _mediator.Send(req, ct);
            await SendResultAsync(TypedResults.NoContent());
        }
    }
}