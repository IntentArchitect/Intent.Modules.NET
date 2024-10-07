using System;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.WithCompositeKeys;
using CleanArchitecture.Comprehensive.Application.WithCompositeKeys.GetWithCompositeKeyById;
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
    public class GetWithCompositeKeyByIdQueryEndpoint : Endpoint<GetWithCompositeKeyByIdQuery, WithCompositeKeyDto>
    {
        private readonly ISender _mediator;

        public GetWithCompositeKeyByIdQueryEndpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override void Configure()
        {
            Get("api/with-composite-key/{key1id}/{key2id}");
            Description(b =>
            {
                b.WithTags("WithCompositeKeys");
                b.Accepts<GetWithCompositeKeyByIdQuery>();
                b.Produces<WithCompositeKeyDto>(StatusCodes.Status200OK, contentType: MediaTypeNames.Application.Json);
                b.ProducesProblemDetails();
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
        }

        public override async Task HandleAsync(GetWithCompositeKeyByIdQuery req, CancellationToken ct)
        {
            var result = default(WithCompositeKeyDto);
            result = await _mediator.Send(req, ct);
            await SendResultAsync(TypedResults.Ok(result));
        }
    }
}