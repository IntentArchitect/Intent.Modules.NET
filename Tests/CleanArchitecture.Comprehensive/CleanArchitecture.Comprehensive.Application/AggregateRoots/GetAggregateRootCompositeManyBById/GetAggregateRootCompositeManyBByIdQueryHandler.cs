using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Comprehensive.Domain.Common.Exceptions;
using CleanArchitecture.Comprehensive.Domain.Entities.CRUD;
using CleanArchitecture.Comprehensive.Domain.Repositories.CRUD;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateRoots.GetAggregateRootCompositeManyBById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetAggregateRootCompositeManyBByIdQueryHandler : IRequestHandler<GetAggregateRootCompositeManyBByIdQuery, AggregateRootCompositeManyBDto>
    {
        private readonly IAggregateRootRepository _aggregateRootRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
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
                throw new NotFoundException($"{nameof(AggregateRoot)} of Id '{request.AggregateRootId}' could not be found");
            }

            var element = aggregateRoot.Composites.FirstOrDefault(p => p.Id == request.Id);
            if (element is null)
            {
                throw new NotFoundException($"Could not find CompositeManyB '{request.Id}'");
            }

            return element.MapToAggregateRootCompositeManyBDto(_mapper);
        }
    }
}