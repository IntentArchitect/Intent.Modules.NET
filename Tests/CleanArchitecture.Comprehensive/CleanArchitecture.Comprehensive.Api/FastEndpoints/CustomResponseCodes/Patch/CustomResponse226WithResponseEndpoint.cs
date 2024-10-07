using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Patch.CustomResponse226WithResponse;
using FastEndpoints;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.EndpointTemplate", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Api.FastEndpoints.CustomResponseCodes.Patch
{
    public class CustomResponse226WithResponseEndpoint : EndpointWithoutRequest<string>
    {
        private readonly ISender _mediator;

        public CustomResponse226WithResponseEndpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override void Configure()
        {
            Patch("api/custom-response-codes/custom-response226-response");
            Description(b =>
            {
                b.WithTags("CustomResponseCodesPatch");
                b.Produces<string>(StatusCodes.Status226IMUsed);
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var result = default(string);
            result = await _mediator.Send(new CustomResponse226WithResponse(), ct);
            await SendAsync(result, 226, ct);
        }
    }
}