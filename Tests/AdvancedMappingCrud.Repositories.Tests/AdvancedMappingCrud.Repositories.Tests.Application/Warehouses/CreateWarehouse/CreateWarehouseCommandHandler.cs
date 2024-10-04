using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Warehouses.CreateWarehouse
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateWarehouseCommandHandler : IRequestHandler<CreateWarehouseCommand, Guid>
    {
        private readonly IWarehouseRepository _warehouseRepository;

        [IntentManaged(Mode.Merge)]
        public CreateWarehouseCommandHandler(IWarehouseRepository warehouseRepository)
        {
            _warehouseRepository = warehouseRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateWarehouseCommand request, CancellationToken cancellationToken)
        {
            var warehouse = new Warehouse
            {
                Name = request.Name,
                Size = request.Size,
                Address = request.Address is not null
                    ? new Address(
                        line1: request.Address.Line1,
                        line2: request.Address.Line2,
                        city: request.Address.City,
                        postal: request.Address.Postal)
                    : null
            };

            _warehouseRepository.Add(warehouse);
            await _warehouseRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return warehouse.Id;
        }
    }
}