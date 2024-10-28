using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FastEndpointsTest.Domain.Common.Exceptions;
using FastEndpointsTest.Domain.Repositories.UniqueIndexConstraint;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateWithUniqueConstraintIndexElements.GetAggregateWithUniqueConstraintIndexElementById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetAggregateWithUniqueConstraintIndexElementByIdQueryHandler : IRequestHandler<GetAggregateWithUniqueConstraintIndexElementByIdQuery, AggregateWithUniqueConstraintIndexElementDto>
    {
        private readonly IAggregateWithUniqueConstraintIndexElementRepository _aggregateWithUniqueConstraintIndexElementRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetAggregateWithUniqueConstraintIndexElementByIdQueryHandler(IAggregateWithUniqueConstraintIndexElementRepository aggregateWithUniqueConstraintIndexElementRepository,
            IMapper mapper)
        {
            _aggregateWithUniqueConstraintIndexElementRepository = aggregateWithUniqueConstraintIndexElementRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<AggregateWithUniqueConstraintIndexElementDto> Handle(
            GetAggregateWithUniqueConstraintIndexElementByIdQuery request,
            CancellationToken cancellationToken)
        {
            var aggregateWithUniqueConstraintIndexElement = await _aggregateWithUniqueConstraintIndexElementRepository.FindByIdAsync(request.Id, cancellationToken);
            if (aggregateWithUniqueConstraintIndexElement is null)
            {
                throw new NotFoundException($"Could not find AggregateWithUniqueConstraintIndexElement '{request.Id}'");
            }
            return aggregateWithUniqueConstraintIndexElement.MapToAggregateWithUniqueConstraintIndexElementDto(_mapper);
        }
    }
}