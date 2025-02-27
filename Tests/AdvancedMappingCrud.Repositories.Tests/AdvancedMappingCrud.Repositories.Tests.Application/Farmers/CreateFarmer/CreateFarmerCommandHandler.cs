using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.DomainInvoke;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories.DomainInvoke;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Farmers.CreateFarmer
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateFarmerCommandHandler : IRequestHandler<CreateFarmerCommand, Guid>
    {
        private readonly IFarmerRepository _farmerRepository;

        [IntentManaged(Mode.Merge)]
        public CreateFarmerCommandHandler(IFarmerRepository farmerRepository)
        {
            _farmerRepository = farmerRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateFarmerCommand request, CancellationToken cancellationToken)
        {
            var farmer = new Farmer(
                name: request.Name,
                surname: request.Surname);

            _farmerRepository.Add(farmer);
            await _farmerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return farmer.Id;
        }
    }
}