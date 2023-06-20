using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CosmosDB.Domain.Common.Exceptions;
using CosmosDB.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace CosmosDB.Application.ClassContainers.UpdateClassContainer
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateClassContainerCommandHandler : IRequestHandler<UpdateClassContainerCommand>
    {
        private readonly IClassContainerRepository _classContainerRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateClassContainerCommandHandler(IClassContainerRepository classContainerRepository)
        {
            _classContainerRepository = classContainerRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Unit> Handle(UpdateClassContainerCommand request, CancellationToken cancellationToken)
        {
            var existingClassContainer = await _classContainerRepository.FindByIdAsync(request.Id, cancellationToken);

            if (existingClassContainer is null)
            {
                throw new NotFoundException($"Could not find ClassContainer {request.Id}");
            }
            existingClassContainer.ClassPartitionKey = request.ClassPartitionKey;

            _classContainerRepository.Update(existingClassContainer);
            return Unit.Value;
        }
    }
}