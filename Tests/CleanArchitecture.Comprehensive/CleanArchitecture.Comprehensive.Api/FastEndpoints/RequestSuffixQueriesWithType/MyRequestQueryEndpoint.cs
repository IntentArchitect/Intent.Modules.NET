using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.RequestSuffixQueriesWithType.MyRequest;
using FastEndpoints;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.EndpointTemplate", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Api.FastEndpoints.RequestSuffixQueriesWithType
{
    public class MyRequestQueryEndpoint : EndpointWithoutRequest<int>
    {
        private readonly ISender _mediator;

        public MyRequestQueryEndpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override void Configure()
        {
            Get("api/request-suffix-queries-with-type/my-request");
            Description(b =>
            {
                b.WithTags("RequestSuffixQueriesWithType");
                b.Produces<int>(StatusCodes.Status200OK);
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var result = default(int);
            result = await _mediator.Send(new MyRequestQuery(), ct);
            await SendResultAsync(TypedResults.Ok(result));
        }
    }
}