using System;
using System.Threading;
using System.Threading.Tasks;
using CosmosDB.PrivateSetters.Domain.Common.Exceptions;
using CosmosDB.PrivateSetters.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CosmosDB.PrivateSetters.Application.ClassContainers.DeleteClassContainer
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteClassContainerCommandHandler : IRequestHandler<DeleteClassContainerCommand>
    {
        private readonly IClassContainerRepository _classContainerRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteClassContainerCommandHandler(IClassContainerRepository classContainerRepository)
        {
            _classContainerRepository = classContainerRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteClassContainerCommand request, CancellationToken cancellationToken)
        {
            var existingClassContainer = await _classContainerRepository.FindByIdAsync((request.Id, request.ClassPartitionKey), cancellationToken);
            if (existingClassContainer is null)
            {
                throw new NotFoundException($"Could not find ClassContainer '({request.Id}, {request.ClassPartitionKey})'");
            }

            _classContainerRepository.Remove(existingClassContainer);
        }
    }
}