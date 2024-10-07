using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.WithCompositeKeys.DeleteWithCompositeKey;
using FastEndpoints;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.EndpointTemplate", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Api.FastEndpoints.WithCompositeKeys
{
    public class DeleteWithCompositeKeyCommandEndpoint : Endpoint<DeleteWithCompositeKeyCommand>
    {
        private readonly ISender _mediator;

        public DeleteWithCompositeKeyCommandEndpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override void Configure()
        {
            Delete("api/with-composite-key/{key1id}/{key2id}");
            Description(b =>
            {
                b.WithTags("WithCompositeKeys");
                b.Accepts<DeleteWithCompositeKeyCommand>();
                b.Produces(StatusCodes.Status200OK);
                b.ProducesProblemDetails();
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
        }

        public override async Task HandleAsync(DeleteWithCompositeKeyCommand req, CancellationToken ct)
        {
            await _mediator.Send(req, ct);
            await SendResultAsync(TypedResults.Ok());
        }
    }
}