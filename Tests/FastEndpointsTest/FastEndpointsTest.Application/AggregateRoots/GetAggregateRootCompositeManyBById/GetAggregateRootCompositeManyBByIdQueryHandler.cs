using System;
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

namespace FastEndpointsTest.Application.AggregateRoots.GetAggregateRootCompositeManyBById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetAggregateRootCompositeManyBByIdQueryHandler : IRequestHandler<GetAggregateRootCompositeManyBByIdQuery, AggregateRootCompositeManyBDto>
    {
        private readonly IAggregateRootRepository _aggregateRootRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetAggregateRootCompositeManyBByIdQueryHandler(IAggregateRootRepository aggregateRootRepository,
            IMapper mapper)
        {
            _aggregateRootRepository = aggregateRootRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<AggregateRootCompositeManyBDto> Handle(
            GetAggregateRootCompositeManyBByIdQuery request,
            CancellationToken cancellationToken)
        {
            var aggregateRoot = await _aggregateRootRepository.FindByIdAsync(request.AggregateRootId, cancellationToken);
            if (aggregateRoot is null)
            {
                throw new NotFoundException($"Could not find CompositeManyB '{request.AggregateRootId}'");
            }

            var compositeManyB = aggregateRoot.Composites.FirstOrDefault(x => x.Id == request.Id && x.AggregateRootId == request.AggregateRootId);
            if (compositeManyB is null)
            {
                throw new NotFoundException($"Could not find CompositeManyB '({request.Id}, {request.AggregateRootId})'");
            }
            return compositeManyB.MapToAggregateRootCompositeManyBDto(_mapper);
        }
    }
}