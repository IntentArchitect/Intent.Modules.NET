using System;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.Versioned.TestCommandV1;
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
    public class TestCommandV1Endpoint : Endpoint<TestCommandV1>
    {
        private readonly ISender _mediator;

        public TestCommandV1Endpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override void Configure()
        {
            Post("api/v1/versioned/test-command");
            Description(b =>
            {
                b.WithTags("Versioned");
                b.Accepts<TestCommandV1>(MediaTypeNames.Application.Json);
                b.Produces(StatusCodes.Status201Created);
                b.ProducesProblemDetails();
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
        }

        public override async Task HandleAsync(TestCommandV1 req, CancellationToken ct)
        {
            await _mediator.Send(req, ct);
            await SendResultAsync(TypedResults.Created(string.Empty, (string?)null));
        }
    }
}