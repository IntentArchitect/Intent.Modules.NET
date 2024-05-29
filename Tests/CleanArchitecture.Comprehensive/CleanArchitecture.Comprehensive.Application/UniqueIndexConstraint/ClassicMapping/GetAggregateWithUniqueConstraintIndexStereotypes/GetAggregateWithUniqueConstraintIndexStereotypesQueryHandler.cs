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

namespace CleanArchitecture.Comprehensive.Application.UniqueIndexConstraint.ClassicMapping.GetAggregateWithUniqueConstraintIndexStereotypes
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetAggregateWithUniqueConstraintIndexStereotypesQueryHandler : IRequestHandler<GetAggregateWithUniqueConstraintIndexStereotypesQuery, List<AggregateWithUniqueConstraintIndexStereotypeDto>>
    {
        private readonly IAggregateWithUniqueConstraintIndexStereotypeRepository _aggregateWithUniqueConstraintIndexStereotypeRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetAggregateWithUniqueConstraintIndexStereotypesQueryHandler(IAggregateWithUniqueConstraintIndexStereotypeRepository aggregateWithUniqueConstraintIndexStereotypeRepository,
            IMapper mapper)
        {
            _aggregateWithUniqueConstraintIndexStereotypeRepository = aggregateWithUniqueConstraintIndexStereotypeRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<AggregateWithUniqueConstraintIndexStereotypeDto>> Handle(
            GetAggregateWithUniqueConstraintIndexStereotypesQuery request,
            CancellationToken cancellationToken)
        {
            var aggregateWithUniqueConstraintIndexStereotypes = await _aggregateWithUniqueConstraintIndexStereotypeRepository.FindAllAsync(cancellationToken);
            return aggregateWithUniqueConstraintIndexStereotypes.MapToAggregateWithUniqueConstraintIndexStereotypeDtoList(_mapper);
        }
    }
}