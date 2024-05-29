using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Comprehensive.Domain.Repositories.UniqueIndexConstraint;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.UniqueIndexConstraint.ClassicMapping.GetAggregateWithUniqueConstraintIndexElements
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetAggregateWithUniqueConstraintIndexElementsQueryHandler : IRequestHandler<GetAggregateWithUniqueConstraintIndexElementsQuery, List<AggregateWithUniqueConstraintIndexElementDto>>
    {
        private readonly IAggregateWithUniqueConstraintIndexElementRepository _aggregateWithUniqueConstraintIndexElementRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetAggregateWithUniqueConstraintIndexElementsQueryHandler(IAggregateWithUniqueConstraintIndexElementRepository aggregateWithUniqueConstraintIndexElementRepository,
            IMapper mapper)
        {
            _aggregateWithUniqueConstraintIndexElementRepository = aggregateWithUniqueConstraintIndexElementRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<AggregateWithUniqueConstraintIndexElementDto>> Handle(
            GetAggregateWithUniqueConstraintIndexElementsQuery request,
            CancellationToken cancellationToken)
        {
            var aggregateWithUniqueConstraintIndexElements = await _aggregateWithUniqueConstraintIndexElementRepository.FindAllAsync(cancellationToken);
            return aggregateWithUniqueConstraintIndexElements.MapToAggregateWithUniqueConstraintIndexElementDtoList(_mapper);
        }
    }
}