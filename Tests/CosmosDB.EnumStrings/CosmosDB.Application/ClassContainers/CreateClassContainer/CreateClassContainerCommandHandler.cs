using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CosmosDB.Domain.Entities;
using CosmosDB.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CosmosDB.Application.ClassContainers.CreateClassContainer
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateClassContainerCommandHandler : IRequestHandler<CreateClassContainerCommand, string>
    {
        private readonly IClassContainerRepository _classContainerRepository;

        [IntentManaged(Mode.Merge)]
        public CreateClassContainerCommandHandler(IClassContainerRepository classContainerRepository)
        {
            _classContainerRepository = classContainerRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> Handle(CreateClassContainerCommand request, CancellationToken cancellationToken)
        {
            var newClassContainer = new ClassContainer
            {
                ClassPartitionKey = request.ClassPartitionKey,
            };

            _classContainerRepository.Add(newClassContainer);
            await _classContainerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newClassContainer.Id;
        }
    }
}