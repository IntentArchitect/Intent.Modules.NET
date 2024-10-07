using System;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.BugFixes;
using CleanArchitecture.Comprehensive.Application.BugFixes.GetTaskName;
using FastEndpoints;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.EndpointTemplate", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Api.FastEndpoints.BugFixes
{
    public class GetTaskNameQueryEndpoint : EndpointWithoutRequest<TaskNameDto>
    {
        private readonly ISender _mediator;

        public GetTaskNameQueryEndpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override void Configure()
        {
            Get("api/bug-fixes");
            Description(b =>
            {
                b.WithTags("BugFixes");
                b.Produces<TaskNameDto>(StatusCodes.Status200OK, contentType: MediaTypeNames.Application.Json);
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var result = default(TaskNameDto);
            result = await _mediator.Send(new GetTaskNameQuery(), ct);
            await SendResultAsync(TypedResults.Ok(result));
        }
    }
}