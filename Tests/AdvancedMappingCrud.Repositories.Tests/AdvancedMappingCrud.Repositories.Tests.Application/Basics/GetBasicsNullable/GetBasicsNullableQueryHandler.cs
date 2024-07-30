using System;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Pagination;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Basics.GetBasicsNullable
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetBasicsNullableQueryHandler : IRequestHandler<GetBasicsNullableQuery, Common.Pagination.PagedResult<BasicDto>>
    {
        private readonly IBasicRepository _basicRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetBasicsNullableQueryHandler(IBasicRepository basicRepository, IMapper mapper)
        {
            _basicRepository = basicRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Common.Pagination.PagedResult<BasicDto>> Handle(
            GetBasicsNullableQuery request,
            CancellationToken cancellationToken)
        {
            var basics = await _basicRepository.FindAllAsync(request.PageNo, request.PageSize, queryOptions => queryOptions.OrderBy(request.OrderBy ?? "Id"), cancellationToken);
            return basics.MapToPagedResult(x => x.MapToBasicDto(_mapper));
        }
    }
}