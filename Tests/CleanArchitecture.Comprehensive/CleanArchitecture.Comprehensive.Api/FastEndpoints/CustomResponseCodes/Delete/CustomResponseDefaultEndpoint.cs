using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Delete.CustomResponseDefault;
using FastEndpoints;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.EndpointTemplate", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Api.FastEndpoints.CustomResponseCodes.Delete
{
    public class CustomResponseDefaultEndpoint : EndpointWithoutRequest
    {
        private readonly ISender _mediator;

        public CustomResponseDefaultEndpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override void Configure()
        {
            Delete("api/custom-response-codes/custom-response-default");
            Description(b =>
            {
                b.WithTags("CustomResponseCodesDelete");
                b.Produces(StatusCodes.Status200OK);
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            await _mediator.Send(new CustomResponseDefault(), ct);
            await SendResultAsync(TypedResults.Ok());
        }
    }
}