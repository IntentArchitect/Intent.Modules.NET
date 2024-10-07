using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Post.CustomResponse206;
using FastEndpoints;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.EndpointTemplate", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Api.FastEndpoints.CustomResponseCodes.Post
{
    public class CustomResponse206Endpoint : EndpointWithoutRequest
    {
        private readonly ISender _mediator;

        public CustomResponse206Endpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override void Configure()
        {
            Post("api/custom-response-codes/custom-response206");
            Description(b =>
            {
                b.WithTags("CustomResponseCodesPost");
                b.Produces(StatusCodes.Status206PartialContent);
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            await _mediator.Send(new CustomResponse206(), ct);
            await SendResultAsync(TypedResults.StatusCode(206));
        }
    }
}