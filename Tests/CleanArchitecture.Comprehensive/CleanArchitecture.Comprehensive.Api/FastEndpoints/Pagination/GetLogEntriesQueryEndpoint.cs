using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.Common.Pagination;
using CleanArchitecture.Comprehensive.Application.Pagination;
using CleanArchitecture.Comprehensive.Application.Pagination.GetLogEntries;
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
    public class GetLogEntriesQueryEndpoint : Endpoint<GetLogEntriesQuery, PagedResult<LogEntryDto>>
    {
        private readonly ISender _mediator;

        public GetLogEntriesQueryEndpoint(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override void Configure()
        {
            Get("api/pagination/log-entries");
            Description(b =>
            {
                b.WithTags("Pagination");
                b.Accepts<GetLogEntriesQuery>();
                b.Produces<PagedResult<LogEntryDto>>(StatusCodes.Status200OK);
                b.ProducesProblemDetails();
                b.ProducesProblemDetails(StatusCodes.Status500InternalServerError);
            });
        }

        public override async Task HandleAsync(GetLogEntriesQuery req, CancellationToken ct)
        {
            var result = default(PagedResult<LogEntryDto>);
            result = await _mediator.Send(req, ct);
            await SendResultAsync(TypedResults.Ok(result));
        }
    }
}