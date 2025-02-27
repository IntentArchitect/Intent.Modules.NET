using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories.DomainInvoke;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Farmers.UpdateFarmer
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateFarmerCommandHandler : IRequestHandler<UpdateFarmerCommand>
    {
        private readonly IFarmerRepository _farmerRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateFarmerCommandHandler(IFarmerRepository farmerRepository)
        {
            _farmerRepository = farmerRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateFarmerCommand request, CancellationToken cancellationToken)
        {
            var farmer = await _farmerRepository.FindByIdAsync(request.Id, cancellationToken);
            if (farmer is null)
            {
                throw new NotFoundException($"Could not find Farmer '{request.Id}'");
            }

            farmer.Name = request.Name;
            farmer.Surname = request.Surname;
        }
    }
}