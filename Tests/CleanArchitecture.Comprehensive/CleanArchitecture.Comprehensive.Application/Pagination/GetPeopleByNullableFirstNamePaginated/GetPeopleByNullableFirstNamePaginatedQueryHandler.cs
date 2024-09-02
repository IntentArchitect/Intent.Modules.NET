using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Comprehensive.Application.Common.Pagination;
using CleanArchitecture.Comprehensive.Domain.Entities.Pagination;
using CleanArchitecture.Comprehensive.Domain.Repositories.Pagination;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Pagination.GetPeopleByNullableFirstNamePaginated
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetPeopleByNullableFirstNamePaginatedQueryHandler : IRequestHandler<GetPeopleByNullableFirstNamePaginatedQuery, Common.Pagination.PagedResult<PersonEntryDto>>
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
        public async Task<Common.Pagination.PagedResult<PersonEntryDto>> Handle(
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