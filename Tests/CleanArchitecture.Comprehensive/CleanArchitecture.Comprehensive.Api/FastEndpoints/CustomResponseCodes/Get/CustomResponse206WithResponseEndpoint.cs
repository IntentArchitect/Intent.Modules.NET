using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Get.CustomResponse206WithResponse;
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
    public class CustomResponse206WithResponseEndpoint : EndpointWithoutRequest<string>
    {
        private readonly ISender _mediator;

        public CustomResponse206WithResponseEndpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override void Configure()
        {
            Get("api/custom-response-codes/custom-response206-response");
            Description(b =>
            {
                b.WithTags("CustomResponseCodesGet");
                b.Produces<string>(StatusCodes.Status206PartialContent);
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var result = default(string);
            result = await _mediator.Send(new CustomResponse206WithResponse(), ct);
            await SendAsync(result, 206, ct);
        }
    }
}