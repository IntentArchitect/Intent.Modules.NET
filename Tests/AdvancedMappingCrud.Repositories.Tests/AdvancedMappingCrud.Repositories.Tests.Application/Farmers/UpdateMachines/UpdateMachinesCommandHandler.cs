using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories.DomainInvoke;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Farmers.UpdateMachines
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateMachinesCommandHandler : IRequestHandler<UpdateMachinesCommand>
    {
        private readonly IFarmerRepository _farmerRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateMachinesCommandHandler(IFarmerRepository farmerRepository)
        {
            _farmerRepository = farmerRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateMachinesCommand request, CancellationToken cancellationToken)
        {
            var farmer = await _farmerRepository.FindByIdAsync(request.FarmerId, cancellationToken);
            if (farmer is null)
            {
                throw new NotFoundException($"Could not find Machines '{request.FarmerId}'");
            }

            var machines = farmer.Machines.FirstOrDefault(x => x.Id == request.Id);
            if (machines is null)
            {
                throw new NotFoundException($"Could not find Machines '{request.Id}'");
            }

            machines.Name = request.Name;
            machines.FarmerId = request.FarmerId;
        }
    }
}