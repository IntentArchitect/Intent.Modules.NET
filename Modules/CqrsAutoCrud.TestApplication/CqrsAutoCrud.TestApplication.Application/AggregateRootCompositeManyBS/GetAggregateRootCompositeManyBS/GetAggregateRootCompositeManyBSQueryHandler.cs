using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CqrsAutoCrud.TestApplication.Domain.Entities;
using CqrsAutoCrud.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRootCompositeManyBS.GetAggregateRootCompositeManyBS
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetAggregateRootCompositeManyBSQueryHandler : IRequestHandler<GetAggregateRootCompositeManyBSQuery, List<AggregateRootCompositeManyBDTO>>
    {
        private readonly IAggregateRootRepository _aggregateRootRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetAggregateRootCompositeManyBSQueryHandler(IAggregateRootRepository aggregateRootRepository, IMapper mapper)
        {
            _aggregateRootRepository = aggregateRootRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<AggregateRootCompositeManyBDTO>> Handle(GetAggregateRootCompositeManyBSQuery request, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _aggregateRootRepository.FindByIdAsync(request.AggregateRootId, cancellationToken);
            if (aggregateRoot == null)
            {
                throw new InvalidOperationException($"{nameof(AggregateRoot)} of Id '{request.AggregateRootId}' could not be found");
            }
            return aggregateRoot.Composites.MapToAggregateRootCompositeManyBDTOList(_mapper);
        }
    }
}