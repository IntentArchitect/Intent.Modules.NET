using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.EntityWithMutableOperations.GetEntityWithMutableOperationById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetEntityWithMutableOperationByIdQueryHandler : IRequestHandler<GetEntityWithMutableOperationByIdQuery, EntityWithMutableOperationDto>
    {
        private readonly IEntityWithMutableOperationRepository _entityWithMutableOperationRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetEntityWithMutableOperationByIdQueryHandler(IEntityWithMutableOperationRepository entityWithMutableOperationRepository, IMapper mapper)
        {
            _entityWithMutableOperationRepository = entityWithMutableOperationRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<EntityWithMutableOperationDto> Handle(
            GetEntityWithMutableOperationByIdQuery request,
            CancellationToken cancellationToken)
        {
            var entityWithMutableOperation = await _entityWithMutableOperationRepository.FindByIdAsync(request.Id, cancellationToken);
            return entityWithMutableOperation.MapToEntityWithMutableOperationDto(_mapper);
        }
    }
}