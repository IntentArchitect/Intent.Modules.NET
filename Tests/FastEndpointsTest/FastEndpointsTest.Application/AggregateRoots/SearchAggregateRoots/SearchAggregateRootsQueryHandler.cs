using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FastEndpointsTest.Domain.Repositories.CRUD;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateRoots.SearchAggregateRoots
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class SearchAggregateRootsQueryHandler : IRequestHandler<SearchAggregateRootsQuery, List<AggregateRootDto>>
    {
        private readonly IAggregateRootRepository _aggregateRootRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public SearchAggregateRootsQueryHandler(IAggregateRootRepository aggregateRootRepository, IMapper mapper)
        {
            _aggregateRootRepository = aggregateRootRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// HTTP QUERY endpoint: the search criteria travel in the request body.
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<AggregateRootDto>> Handle(
            SearchAggregateRootsQuery request,
            CancellationToken cancellationToken)
        {
            var aggregateRoots = await _aggregateRootRepository.FindAllAsync(x => x.AggregateAttr == request.AggregateAttr, cancellationToken);
            return aggregateRoots.MapToAggregateRootDtoList(_mapper);
        }
    }
}
