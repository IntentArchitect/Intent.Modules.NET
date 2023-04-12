using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.EntityWithMutableOperations.DeleteEntityWithMutableOperation
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteEntityWithMutableOperationCommandHandler : IRequestHandler<DeleteEntityWithMutableOperationCommand>
    {
        private readonly IEntityWithMutableOperationRepository _entityWithMutableOperationRepository;

        [IntentManaged(Mode.Ignore)]
        public DeleteEntityWithMutableOperationCommandHandler(IEntityWithMutableOperationRepository entityWithMutableOperationRepository)
        {
            _entityWithMutableOperationRepository = entityWithMutableOperationRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Unit> Handle(
            DeleteEntityWithMutableOperationCommand request,
            CancellationToken cancellationToken)
        {
            var existingEntityWithMutableOperation = await _entityWithMutableOperationRepository.FindByIdAsync(request.Id, cancellationToken);
            _entityWithMutableOperationRepository.Remove(existingEntityWithMutableOperation);
            return Unit.Value;
        }
    }
}