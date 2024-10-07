using System;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.AsyncOperationsClasses.ExplicitWithReturn;
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
    public class ExplicitWithReturnCommandEndpoint : Endpoint<ExplicitWithReturnCommand, object>
    {
        private readonly ISender _mediator;

        public ExplicitWithReturnCommandEndpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override void Configure()
        {
            Put("api/async-operations-classes/{id}/explicit-with-return");
            Description(b =>
            {
                b.WithTags("AsyncOperationsClasses");
                b.Accepts<ExplicitWithReturnCommand>();
                b.Produces<object>(StatusCodes.Status200OK, contentType: MediaTypeNames.Application.Json);
                b.ProducesProblemDetails();
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
        }

        public override async Task HandleAsync(ExplicitWithReturnCommand req, CancellationToken ct)
        {
            var result = default(object);
            result = await _mediator.Send(req, ct);
            await SendResultAsync(TypedResults.Ok(result));
        }
    }
}