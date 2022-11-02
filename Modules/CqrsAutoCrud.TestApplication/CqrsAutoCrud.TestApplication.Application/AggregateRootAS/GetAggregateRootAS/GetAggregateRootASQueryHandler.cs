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

namespace CqrsAutoCrud.TestApplication.Application.AggregateRootAS.GetAggregateRootAS
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetAggregateRootASQueryHandler : IRequestHandler<GetAggregateRootASQuery, List<AggregateRootADTO>>
    {
        private IAggregateRootARepository _aggregateRootARepository;
        private IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetAggregateRootASQueryHandler(IAggregateRootARepository aggregateRootARepository, IMapper mapper)
        {
            _aggregateRootARepository = aggregateRootARepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<AggregateRootADTO>> Handle(GetAggregateRootASQuery request, CancellationToken cancellationToken)
        {
            var aggregateRootAs = await _aggregateRootARepository.FindAllAsync(cancellationToken);
            return aggregateRootAs.MapToAggregateRootADTOList(_mapper);
        }
    }
}