using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Comprehensive.Domain.Repositories.CRUD;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateRoots.GetAggregateRoots
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetAggregateRootsQueryHandler : IRequestHandler<GetAggregateRootsQuery, List<AggregateRootDto>>
    {
        private readonly IAggregateRootRepository _aggregateRootRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetAggregateRootsQueryHandler(IAggregateRootRepository aggregateRootRepository, IMapper mapper)
        {
            _aggregateRootRepository = aggregateRootRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<AggregateRootDto>> Handle(
            GetAggregateRootsQuery request,
            CancellationToken cancellationToken)
        {
            var aggregateRoots = await _aggregateRootRepository.FindAllAsync(cancellationToken);
            return aggregateRoots.MapToAggregateRootDtoList(_mapper);
        }
    }
}