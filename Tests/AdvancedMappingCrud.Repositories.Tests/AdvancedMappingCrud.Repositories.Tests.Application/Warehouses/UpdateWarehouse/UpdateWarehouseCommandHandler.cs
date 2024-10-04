using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Warehouses.UpdateWarehouse
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateWarehouseCommandHandler : IRequestHandler<UpdateWarehouseCommand>
    {
        private readonly IWarehouseRepository _warehouseRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateWarehouseCommandHandler(IWarehouseRepository warehouseRepository)
        {
            _warehouseRepository = warehouseRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateWarehouseCommand request, CancellationToken cancellationToken)
        {
            var warehouse = await _warehouseRepository.FindByIdAsync(request.Id, cancellationToken);
            if (warehouse is null)
            {
                throw new NotFoundException($"Could not find Warehouse '{request.Id}'");
            }

            warehouse.Name = request.Name;
            warehouse.Size = request.Size;
            warehouse.Address = request.Address is not null
                ? new Address(
                    line1: request.Address.Line1,
                    line2: request.Address.Line2,
                    city: request.Address.City,
                    postal: request.Address.Postal)
                : null;
        }
    }
}