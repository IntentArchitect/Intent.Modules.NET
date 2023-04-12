using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.EntityWithMutableOperations.GetEntityWithMutableOperations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetEntityWithMutableOperationsQueryHandler : IRequestHandler<GetEntityWithMutableOperationsQuery, List<EntityWithMutableOperationDto>>
    {
        private readonly IEntityWithMutableOperationRepository _entityWithMutableOperationRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetEntityWithMutableOperationsQueryHandler(IEntityWithMutableOperationRepository entityWithMutableOperationRepository, IMapper mapper)
        {
            _entityWithMutableOperationRepository = entityWithMutableOperationRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<EntityWithMutableOperationDto>> Handle(
            GetEntityWithMutableOperationsQuery request,
            CancellationToken cancellationToken)
        {
            var entityWithMutableOperations = await _entityWithMutableOperationRepository.FindAllAsync(cancellationToken);
            return entityWithMutableOperations.MapToEntityWithMutableOperationDtoList(_mapper);
        }
    }
}