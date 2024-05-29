using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.Common.Pagination;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.PaginationForProxies.PaginatedResult
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class PaginatedResultQueryHandler : IRequestHandler<PaginatedResultQuery, PagedResult<string>>
    {
        [IntentManaged(Mode.Ignore)]
        public PaginatedResultQueryHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<PagedResult<string>> Handle(PaginatedResultQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}