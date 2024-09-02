using System;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Comprehensive.Application.Common.Pagination;
using CleanArchitecture.Comprehensive.Domain.Repositories.Pagination;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Pagination.GetPeopleByFirstNamePaginated
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetPeopleByFirstNamePaginatedQueryHandler : IRequestHandler<GetPeopleByFirstNamePaginatedQuery, Common.Pagination.PagedResult<PersonEntryDto>>
    {
        private readonly IPersonEntryRepository _personEntryRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetPeopleByFirstNamePaginatedQueryHandler(IPersonEntryRepository personEntryRepository, IMapper mapper)
        {
            _personEntryRepository = personEntryRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Common.Pagination.PagedResult<PersonEntryDto>> Handle(
            GetPeopleByFirstNamePaginatedQuery request,
            CancellationToken cancellationToken)
        {
            var entity = await _personEntryRepository.FindAllAsync(x => x.FirstName == request.FirstName, request.PageNo, request.PageSize, queryOptions => queryOptions.OrderBy(request.OrderBy ?? "Id"), cancellationToken);
            return entity.MapToPagedResult(x => x.MapToPersonEntryDto(_mapper));
        }
    }
}