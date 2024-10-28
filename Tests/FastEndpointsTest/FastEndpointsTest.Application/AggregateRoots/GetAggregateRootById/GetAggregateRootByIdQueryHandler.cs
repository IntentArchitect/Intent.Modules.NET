using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FastEndpointsTest.Domain.Common.Exceptions;
using FastEndpointsTest.Domain.Repositories.CRUD;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateRoots.GetAggregateRootById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetAggregateRootByIdQueryHandler : IRequestHandler<GetAggregateRootByIdQuery, AggregateRootDto>
    {
        private readonly IAggregateRootRepository _aggregateRootRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetAggregateRootByIdQueryHandler(IAggregateRootRepository aggregateRootRepository, IMapper mapper)
        {
            _aggregateRootRepository = aggregateRootRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<AggregateRootDto> Handle(GetAggregateRootByIdQuery request, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _aggregateRootRepository.FindByIdAsync(request.Id, cancellationToken);
            if (aggregateRoot is null)
            {
                throw new NotFoundException($"Could not find AggregateRoot '{request.Id}'");
            }
            return aggregateRoot.MapToAggregateRootDto(_mapper);
        }
    }
}