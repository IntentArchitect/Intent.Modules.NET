using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Comprehensive.Application.Common.Pagination;
using CleanArchitecture.Comprehensive.Domain.Repositories.CRUD;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateRootLongs.GetAggregateRootLongs
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetAggregateRootLongsQueryHandler : IRequestHandler<GetAggregateRootLongsQuery, PagedResult<AggregateRootLongDto>>
    {
        private readonly IAggregateRootLongRepository _aggregateRootLongRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetAggregateRootLongsQueryHandler(IAggregateRootLongRepository aggregateRootLongRepository, IMapper mapper)
        {
            _aggregateRootLongRepository = aggregateRootLongRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<PagedResult<AggregateRootLongDto>> Handle(
            GetAggregateRootLongsQuery request,
            CancellationToken cancellationToken)
        {
            var results = await _aggregateRootLongRepository.FindAllAsync(
                pageNo: request.PageNo,
                pageSize: request.PageSize,
                cancellationToken: cancellationToken);
            return results.MapToPagedResult(x => x.MapToAggregateRootLongDto(_mapper));
        }
    }
}