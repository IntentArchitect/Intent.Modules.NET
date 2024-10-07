using System;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Api.FastEndpoints.ResponseTypes;
using CleanArchitecture.Comprehensive.Application.WithCompositeKeys.CreateWithCompositeKey;
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
    public class CreateWithCompositeKeyCommandEndpoint : Endpoint<CreateWithCompositeKeyCommand, JsonResponse<Guid>>
    {
        private readonly ISender _mediator;

        public CreateWithCompositeKeyCommandEndpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override void Configure()
        {
            Post("api/with-composite-key");
            Description(b =>
            {
                b.WithTags("WithCompositeKeys");
                b.Accepts<CreateWithCompositeKeyCommand>(MediaTypeNames.Application.Json);
                b.Produces<JsonResponse<Guid>>(StatusCodes.Status201Created, contentType: MediaTypeNames.Application.Json);
                b.ProducesProblemDetails();
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
        }

        public override async Task HandleAsync(CreateWithCompositeKeyCommand req, CancellationToken ct)
        {
            var result = default(Guid);
            result = await _mediator.Send(req, ct);
            await SendResultAsync(TypedResults.Created(string.Empty, new JsonResponse<Guid>(result)));
        }
    }
}