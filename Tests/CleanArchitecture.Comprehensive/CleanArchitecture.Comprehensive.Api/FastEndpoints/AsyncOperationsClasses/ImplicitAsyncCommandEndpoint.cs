using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.AsyncOperationsClasses.ImplicitAsync;
using FastEndpoints;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.EndpointTemplate", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Api.FastEndpoints.AsyncOperationsClasses
{
    public class ImplicitAsyncCommandEndpoint : Endpoint<ImplicitAsyncCommand>
    {
        private readonly ISender _mediator;

        public ImplicitAsyncCommandEndpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override void Configure()
        {
            Put("api/async-operations-classes/{id}/implicit-async");
            Description(b =>
            {
                b.WithTags("AsyncOperationsClasses");
                b.Accepts<ImplicitAsyncCommand>();
                b.Produces(StatusCodes.Status204NoContent);
                b.ProducesProblemDetails();
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
        }

        public override async Task HandleAsync(ImplicitAsyncCommand req, CancellationToken ct)
        {
            await _mediator.Send(req, ct);
            await SendResultAsync(TypedResults.NoContent());
        }
    }
}