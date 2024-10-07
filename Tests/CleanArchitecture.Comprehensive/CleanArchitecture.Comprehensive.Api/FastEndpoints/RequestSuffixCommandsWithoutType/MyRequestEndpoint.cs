using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.RequestSuffixCommandsWithoutType.MyRequest;
using FastEndpoints;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.EndpointTemplate", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Api.FastEndpoints.RequestSuffixCommandsWithoutType
{
    public class MyRequestEndpoint : EndpointWithoutRequest
    {
        private readonly ISender _mediator;

        public MyRequestEndpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override void Configure()
        {
            Post("api/request-suffix-commands-without-type/my");
            Description(b =>
            {
                b.WithTags("RequestSuffixCommandsWithoutType");
                b.Produces(StatusCodes.Status201Created);
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            await _mediator.Send(new MyRequest(), ct);
            await SendResultAsync(TypedResults.Created(string.Empty, (string?)null));
        }
    }
}