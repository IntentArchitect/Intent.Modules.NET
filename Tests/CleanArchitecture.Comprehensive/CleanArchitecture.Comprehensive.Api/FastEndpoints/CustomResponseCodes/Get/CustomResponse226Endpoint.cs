using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Get.CustomResponse226;
using FastEndpoints;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.EndpointTemplate", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Api.FastEndpoints.CustomResponseCodes.Get
{
    public class CustomResponse226Endpoint : EndpointWithoutRequest
    {
        private readonly ISender _mediator;

        public CustomResponse226Endpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override void Configure()
        {
            Get("api/custom-response-codes/custom-response226");
            Description(b =>
            {
                b.WithTags("CustomResponseCodesGet");
                b.Produces(StatusCodes.Status226IMUsed);
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            await _mediator.Send(new CustomResponse226(), ct);
            await SendResultAsync(TypedResults.StatusCode(226));
        }
    }
}