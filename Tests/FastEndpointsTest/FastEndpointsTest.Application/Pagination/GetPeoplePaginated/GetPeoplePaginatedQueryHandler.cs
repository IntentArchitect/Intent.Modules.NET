using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FastEndpointsTest.Application.Common.Pagination;
using FastEndpointsTest.Domain.Repositories.Pagination;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using static System.Linq.Dynamic.Core.DynamicQueryableExtensions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace FastEndpointsTest.Application.Pagination.GetPeoplePaginated
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetPeoplePaginatedQueryHandler : IRequestHandler<GetPeoplePaginatedQuery, PagedResult<PersonEntryDto>>
    {
        private readonly IPersonEntryRepository _personEntryRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetPeoplePaginatedQueryHandler(IPersonEntryRepository personEntryRepository, IMapper mapper)
        {
            _personEntryRepository = personEntryRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<PagedResult<PersonEntryDto>> Handle(
            GetPeoplePaginatedQuery request,
            CancellationToken cancellationToken)
        {
            var entity = await _personEntryRepository.FindAllAsync(request.PageNo, request.PageSize, queryOptions => queryOptions.OrderBy(request.OrderBy ?? "Id"), cancellationToken);
            return entity.MapToPagedResult(x => x.MapToPersonEntryDto(_mapper));
        }
    }
}