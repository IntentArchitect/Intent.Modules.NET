using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.RequestSuffixCommandsWithType.MyCommandRequest;
using FastEndpoints;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.EndpointTemplate", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Api.FastEndpoints.RequestSuffixCommandsWithType
{
    public class MyCommandRequestEndpoint : EndpointWithoutRequest
    {
        private readonly ISender _mediator;

        public MyCommandRequestEndpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override void Configure()
        {
            Post("api/request-suffix-commands-with-type/my");
            Description(b =>
            {
                b.WithTags("RequestSuffixCommandsWithType");
                b.Produces(StatusCodes.Status201Created);
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            await _mediator.Send(new MyCommandRequest(), ct);
            await SendResultAsync(TypedResults.Created(string.Empty, (string?)null));
        }
    }
}