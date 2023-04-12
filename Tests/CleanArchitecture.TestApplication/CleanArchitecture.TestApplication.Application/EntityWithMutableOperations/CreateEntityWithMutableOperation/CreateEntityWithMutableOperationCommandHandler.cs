using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.EntityWithMutableOperations.CreateEntityWithMutableOperation
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateEntityWithMutableOperationCommandHandler : IRequestHandler<CreateEntityWithMutableOperationCommand>
    {
        private readonly IEntityWithMutableOperationRepository _entityWithMutableOperationRepository;

        [IntentManaged(Mode.Ignore)]
        public CreateEntityWithMutableOperationCommandHandler(IEntityWithMutableOperationRepository entityWithMutableOperationRepository)
        {
            _entityWithMutableOperationRepository = entityWithMutableOperationRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Unit> Handle(
            CreateEntityWithMutableOperationCommand request,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}