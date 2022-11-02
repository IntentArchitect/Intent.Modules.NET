using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CqrsAutoCrud.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRootAS.GetAggregateRootAById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetAggregateRootAByIdQueryHandler : IRequestHandler<GetAggregateRootAByIdQuery, AggregateRootADTO>
    {
        private IAggregateRootARepository _aggregateRootARepository;
        private IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetAggregateRootAByIdQueryHandler(IAggregateRootARepository aggregateRootARepository, IMapper mapper)
        {
            _aggregateRootARepository = aggregateRootARepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<AggregateRootADTO> Handle(GetAggregateRootAByIdQuery request, CancellationToken cancellationToken)
        {
            var aggregateRootA = await _aggregateRootARepository.FindByIdAsync(request.Id, cancellationToken);
            return aggregateRootA.MapToAggregateRootADTO(_mapper);
        }
    }
}