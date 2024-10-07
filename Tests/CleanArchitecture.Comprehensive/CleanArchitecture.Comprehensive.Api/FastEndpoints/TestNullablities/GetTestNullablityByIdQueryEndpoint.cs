using System;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.TestNullablities;
using CleanArchitecture.Comprehensive.Application.TestNullablities.GetTestNullablityById;
using FastEndpoints;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.EndpointTemplate", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Api.FastEndpoints.TestNullablities
{
    public class GetTestNullablityByIdQueryEndpoint : Endpoint<GetTestNullablityByIdQuery, TestNullablityDto>
    {
        private readonly ISender _mediator;

        public GetTestNullablityByIdQueryEndpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override void Configure()
        {
            Get("api/test-nullablity/{id}");
            Description(b =>
            {
                b.WithTags("TestNullablities");
                b.Accepts<GetTestNullablityByIdQuery>();
                b.Produces<TestNullablityDto>(StatusCodes.Status200OK, contentType: MediaTypeNames.Application.Json);
                b.ProducesProblemDetails();
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
        }

        public override async Task HandleAsync(GetTestNullablityByIdQuery req, CancellationToken ct)
        {
            var result = default(TestNullablityDto);
            result = await _mediator.Send(req, ct);
            await SendResultAsync(TypedResults.Ok(result));
        }
    }
}