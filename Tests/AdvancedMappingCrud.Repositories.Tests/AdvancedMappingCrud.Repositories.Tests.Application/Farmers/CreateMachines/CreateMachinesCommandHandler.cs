using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.DomainInvoke;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories.DomainInvoke;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Farmers.CreateMachines
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateMachinesCommandHandler : IRequestHandler<CreateMachinesCommand, Guid>
    {
        private readonly IFarmerRepository _farmerRepository;

        [IntentManaged(Mode.Merge)]
        public CreateMachinesCommandHandler(IFarmerRepository farmerRepository)
        {
            _farmerRepository = farmerRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateMachinesCommand request, CancellationToken cancellationToken)
        {
            var farmer = await _farmerRepository.FindByIdAsync(request.FarmerId, cancellationToken);
            if (farmer is null)
            {
                throw new NotFoundException($"Could not find Farmer '{request.FarmerId}'");
            }
            var machines = new Machines
            {
                Name = request.Name,
                FarmerId = request.FarmerId
            };

            farmer.Machines.Add(machines);
            await _farmerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return machines.Id;
        }
    }
}