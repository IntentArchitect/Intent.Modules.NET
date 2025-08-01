using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FastEndpointsTest.Application.Common.Pagination;
using FastEndpointsTest.Domain.Entities.Pagination;
using FastEndpointsTest.Domain.Repositories.Pagination;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using static System.Linq.Dynamic.Core.DynamicQueryableExtensions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace FastEndpointsTest.Application.Pagination.GetPeopleByNullableFirstNamePaginated
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetPeopleByNullableFirstNamePaginatedQueryHandler : IRequestHandler<GetPeopleByNullableFirstNamePaginatedQuery, PagedResult<PersonEntryDto>>
    {
        private readonly IPersonEntryRepository _personEntryRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetPeopleByNullableFirstNamePaginatedQueryHandler(IPersonEntryRepository personEntryRepository,
            IMapper mapper)
        {
            _personEntryRepository = personEntryRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<PagedResult<PersonEntryDto>> Handle(
            GetPeopleByNullableFirstNamePaginatedQuery request,
            CancellationToken cancellationToken)
        {
            IQueryable<PersonEntry> FilterPersonEntries(IQueryable<PersonEntry> queryable)
            {
                if (request.FirstName != null)
                {
                    queryable = queryable.Where(x => x.FirstName == request.FirstName);
                }
                return queryable;
            }

            var entity = await _personEntryRepository.FindAllAsync(request.PageNo, request.PageSize, q => FilterPersonEntries(q).OrderBy(request.OrderBy ?? "Id"), cancellationToken);
            return entity.MapToPagedResult(x => x.MapToPersonEntryDto(_mapper));
        }
    }
}