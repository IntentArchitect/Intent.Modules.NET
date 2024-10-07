using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.OpenApiIgnoreAllImplicit.OperationA;
using FastEndpoints;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.EndpointTemplate", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Api.FastEndpoints.OpenApiIgnoreAllImplicit
{
    public class OperationAEndpoint : EndpointWithoutRequest
    {
        private readonly ISender _mediator;

        public OperationAEndpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override void Configure()
        {
            Put("api/open-api-ignore-all-implicit/operation-a");
            Description(b =>
            {
                b.WithTags("OpenApiIgnoreAllImplicit");
                b.Produces(StatusCodes.Status204NoContent);
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            await _mediator.Send(new OperationA(), ct);
            await SendResultAsync(TypedResults.NoContent());
        }
    }
}