using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Repositories.Tests.Domain.DomainInvoke;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories.DomainInvoke;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Farmers.AddPlotFarmer
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class AddPlotFarmerCommandHandler : IRequestHandler<AddPlotFarmerCommand>
    {
        private readonly IFarmerRepository _farmerRepository;

        [IntentManaged(Mode.Merge)]
        public AddPlotFarmerCommandHandler(IFarmerRepository farmerRepository)
        {
            _farmerRepository = farmerRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(AddPlotFarmerCommand request, CancellationToken cancellationToken)
        {
            var farmer = await _farmerRepository.FindByIdAsync(request.Id, cancellationToken);
            if (farmer is null)
            {
                throw new NotFoundException($"Could not find Farmer '{request.Id}'");
            }

            farmer.AddPlot(new Plot(
                line1: request.Address.Line1,
                line2: request.Address.Line2));
        }
    }
}