using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Entities;
using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Containers.CreateContainer
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateContainerCommandHandler : IRequestHandler<CreateContainerCommand, Guid>
    {
        private readonly IContainerRepository _containerRepository;

        [IntentManaged(Mode.Merge)]
        public CreateContainerCommandHandler(IContainerRepository containerRepository)
        {
            _containerRepository = containerRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateContainerCommand request, CancellationToken cancellationToken)
        {
            var container = new Container
            {
                ContainerNumber = request.ContainerNumber,
                SealNumber = request.SealNumber
            };

            _containerRepository.Add(container);
            await _containerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return container.Id;
        }
    }
}