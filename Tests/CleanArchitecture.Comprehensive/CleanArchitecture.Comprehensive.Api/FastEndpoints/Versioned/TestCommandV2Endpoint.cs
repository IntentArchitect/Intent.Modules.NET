using System;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.Versioned.TestCommandV2;
using FastEndpoints;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.EndpointTemplate", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Api.FastEndpoints.Versioned
{
    public class TestCommandV2Endpoint : Endpoint<TestCommandV2>
    {
        private readonly ISender _mediator;

        public TestCommandV2Endpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override void Configure()
        {
            Post("api/v2/versioned/test-command");
            Description(b =>
            {
                b.WithTags("Versioned");
                b.WithSummary(@"Command comment");
                b.Accepts<TestCommandV2>(MediaTypeNames.Application.Json);
                b.Produces(StatusCodes.Status201Created);
                b.ProducesProblemDetails();
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
        }

        public override async Task HandleAsync(TestCommandV2 req, CancellationToken ct)
        {
            await _mediator.Send(req, ct);
            await SendResultAsync(TypedResults.Created(string.Empty, (string?)null));
        }
    }
}