using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FastEndpointsTest.Domain.Common.Exceptions;
using FastEndpointsTest.Domain.Repositories.CRUD;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateRoots.GetAggregateRootCompositeManyBS
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetAggregateRootCompositeManyBSQueryHandler : IRequestHandler<GetAggregateRootCompositeManyBSQuery, List<AggregateRootCompositeManyBDto>>
    {
        private readonly IAggregateRootRepository _aggregateRootRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetAggregateRootCompositeManyBSQueryHandler(IAggregateRootRepository aggregateRootRepository,
            IMapper mapper)
        {
            _aggregateRootRepository = aggregateRootRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<AggregateRootCompositeManyBDto>> Handle(
            GetAggregateRootCompositeManyBSQuery request,
            CancellationToken cancellationToken)
        {
            var aggregateRoot = await _aggregateRootRepository.FindByIdAsync(request.AggregateRootId, cancellationToken);
            if (aggregateRoot is null)
            {
                throw new NotFoundException($"Could not find CompositeManyB '{request.AggregateRootId}'");
            }

            var compositeManyBS = aggregateRoot.Composites.Where(x => x.AggregateRootId == request.AggregateRootId);
            return compositeManyBS.MapToAggregateRootCompositeManyBDtoList(_mapper);
        }
    }
}