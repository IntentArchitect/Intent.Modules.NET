using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.EntityWithMutableOperations.UpdateEntityWithMutableOperation
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateEntityWithMutableOperationCommandHandler : IRequestHandler<UpdateEntityWithMutableOperationCommand>
    {
        private readonly IEntityWithMutableOperationRepository _entityWithMutableOperationRepository;

        [IntentManaged(Mode.Ignore)]
        public UpdateEntityWithMutableOperationCommandHandler(IEntityWithMutableOperationRepository entityWithMutableOperationRepository)
        {
            _entityWithMutableOperationRepository = entityWithMutableOperationRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Unit> Handle(UpdateEntityWithMutableOperationCommand request, CancellationToken cancellationToken)
        {
            var existingEntityWithMutableOperation = await _entityWithMutableOperationRepository.FindByIdAsync(request.Id, cancellationToken);
            existingEntityWithMutableOperation.Name = request.Name;
            return Unit.Value;
        }
    }
}