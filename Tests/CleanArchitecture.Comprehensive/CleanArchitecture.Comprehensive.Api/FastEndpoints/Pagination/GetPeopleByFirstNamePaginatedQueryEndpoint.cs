using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.Common.Pagination;
using CleanArchitecture.Comprehensive.Application.Pagination;
using CleanArchitecture.Comprehensive.Application.Pagination.GetPeopleByFirstNamePaginated;
using FastEndpoints;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Mode = Intent.RoslynWeaver.Attributes.Mode;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.FastEndpoints.EndpointTemplate", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Api.FastEndpoints.Pagination
{
    public class GetPeopleByFirstNamePaginatedQueryEndpoint : Endpoint<GetPeopleByFirstNamePaginatedQuery, PagedResult<PersonEntryDto>>
    {
        private readonly ISender _mediator;

        public GetPeopleByFirstNamePaginatedQueryEndpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override void Configure()
        {
            Get("api/pagination/firstname/{firstname}");
            Description(b =>
            {
                b.WithTags("Pagination");
                b.Accepts<GetPeopleByFirstNamePaginatedQuery>();
                b.Produces<PagedResult<PersonEntryDto>>(StatusCodes.Status200OK);
                b.ProducesProblemDetails();
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
        }

        public override async Task HandleAsync(GetPeopleByFirstNamePaginatedQuery req, CancellationToken ct)
        {
            var result = default(PagedResult<PersonEntryDto>);
            result = await _mediator.Send(req, ct);
            await SendResultAsync(TypedResults.Ok(result));
        }
    }
}