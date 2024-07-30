using System;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Cosmos.Tests.Application.Common.Pagination;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.BasicOrderBies.GetBasicOrderBy
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetBasicOrderByQueryHandler : IRequestHandler<GetBasicOrderByQuery, Common.Pagination.PagedResult<BasicOrderByDto>>
    {
        private readonly IBasicOrderByRepository _basicOrderByRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetBasicOrderByQueryHandler(IBasicOrderByRepository basicOrderByRepository, IMapper mapper)
        {
            _basicOrderByRepository = basicOrderByRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Common.Pagination.PagedResult<BasicOrderByDto>> Handle(
            GetBasicOrderByQuery request,
            CancellationToken cancellationToken)
        {
            var basicOrderBies = await _basicOrderByRepository.FindAllAsync(request.PageNo, request.PageSize, queryOptions => queryOptions.OrderBy(request.OrderBy), cancellationToken);
            return basicOrderBies.MapToPagedResult(x => x.MapToBasicOrderByDto(_mapper));
        }
    }
}