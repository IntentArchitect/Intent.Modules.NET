using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.QueryDtoParameter.HasDtoParameter;
using FastEndpoints;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.EndpointTemplate", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Api.FastEndpoints.QueryDtoParameter
{
    public class HasDtoParameterQueryEndpoint : Endpoint<HasDtoParameterQuery, int>
    {
        private readonly ISender _mediator;

        public HasDtoParameterQueryEndpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override void Configure()
        {
            Get("api/query-dto-parameter/new");
            Description(b =>
            {
                b.WithTags("QueryDtoParameter");
                b.Accepts<HasDtoParameterQuery>();
                b.Produces<int>(StatusCodes.Status200OK);
                b.ProducesProblemDetails();
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
        }

        public override async Task HandleAsync(HasDtoParameterQuery req, CancellationToken ct)
        {
            var result = default(int);
            result = await _mediator.Send(req, ct);
            await SendResultAsync(TypedResults.Ok(result));
        }
    }
}