using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Get.CustomResponse207WithResponse;
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
    public class CustomResponse207WithResponseEndpoint : EndpointWithoutRequest<string>
    {
        private readonly ISender _mediator;

        public CustomResponse207WithResponseEndpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override void Configure()
        {
            Get("api/custom-response-codes/custom-response207-response");
            Description(b =>
            {
                b.WithTags("CustomResponseCodesGet");
                b.Produces<string>(StatusCodes.Status207MultiStatus);
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
            AllowAnonymous();
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var result = default(string);
            result = await _mediator.Send(new CustomResponse207WithResponse(), ct);
            await SendAsync(result, 207, ct);
        }
    }
}