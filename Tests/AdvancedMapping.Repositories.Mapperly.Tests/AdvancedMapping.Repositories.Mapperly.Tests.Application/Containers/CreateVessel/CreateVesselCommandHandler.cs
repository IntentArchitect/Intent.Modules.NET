using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Common.Exceptions;
using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Entities;
using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Containers.CreateVessel
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateVesselCommandHandler : IRequestHandler<CreateVesselCommand, Guid>
    {
        private readonly IContainerRepository _containerRepository;

        [IntentManaged(Mode.Merge)]
        public CreateVesselCommandHandler(IContainerRepository containerRepository)
        {
            _containerRepository = containerRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateVesselCommand request, CancellationToken cancellationToken)
        {
            var container = await _containerRepository.FindByIdAsync(request.ContainerId, cancellationToken);
            if (container is null)
            {
                throw new NotFoundException($"Could not find Container '{request.ContainerId}'");
            }
            var vessel = new Vessel
            {
                ContainerId = request.ContainerId,
                IMOCode = request.IMOCode
            };

            container.Vessels.Add(vessel);
            await _containerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return vessel.Id;
        }
    }
}