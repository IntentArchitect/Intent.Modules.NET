using System;
using System.Threading;
using System.Threading.Tasks;
using Asp.Versioning;
using FastEndpoints;
using FastEndpoints.AspVersioning;
using FastEndpointsTest.Application.Common.Pagination;
using FastEndpointsTest.Application.Pagination;
using FastEndpointsTest.Application.Pagination.GetPeoplePaginated;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.EndpointTemplate", Version = "1.0")]

namespace FastEndpointsTest.Api.FastEndpoints.Pagination
{
    public class GetPeoplePaginatedQueryEndpoint : Endpoint<GetPeoplePaginatedQuery, PagedResult<PersonEntryDto>>
    {
        private readonly ISender _mediator;

        public GetPeoplePaginatedQueryEndpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override void Configure()
        {
            Get("api/pagination");
            Description(b =>
            {
                b.WithTags("Pagination");
                b.Accepts<GetPeoplePaginatedQuery>();
                b.Produces<PagedResult<PersonEntryDto>>(StatusCodes.Status200OK);
                b.ProducesProblemDetails();
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
            AllowAnonymous();
            Options(x => x.WithVersionSet(">>Api Version<<").MapToApiVersion(new ApiVersion(1.0)));
        }

        public override async Task HandleAsync(GetPeoplePaginatedQuery req, CancellationToken ct)
        {
            var result = default(PagedResult<PersonEntryDto>);
            result = await _mediator.Send(req, ct);
            await Send.ResultAsync(TypedResults.Ok(result));
        }
    }
}