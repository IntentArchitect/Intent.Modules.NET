using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.WithCompositeKeys;
using CleanArchitecture.Comprehensive.Application.WithCompositeKeys.GetWithCompositeKeys;
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
    public class GetWithCompositeKeysQueryEndpoint : EndpointWithoutRequest<List<WithCompositeKeyDto>>
    {
        private readonly ISender _mediator;

        public GetWithCompositeKeysQueryEndpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override void Configure()
        {
            Get("api/with-composite-key");
            Description(b =>
            {
                b.WithTags("WithCompositeKeys");
                b.Produces<List<WithCompositeKeyDto>>(StatusCodes.Status200OK, contentType: MediaTypeNames.Application.Json);
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var result = default(List<WithCompositeKeyDto>);
            result = await _mediator.Send(new GetWithCompositeKeysQuery(), ct);
            await SendResultAsync(TypedResults.Ok(result));
        }
    }
}