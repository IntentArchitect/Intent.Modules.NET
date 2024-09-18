using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Warehouses.DeleteWarehouse
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteWarehouseCommandHandler : IRequestHandler<DeleteWarehouseCommand>
    {
        private readonly IWarehouseRepository _warehouseRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteWarehouseCommandHandler(IWarehouseRepository warehouseRepository)
        {
            _warehouseRepository = warehouseRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteWarehouseCommand request, CancellationToken cancellationToken)
        {
            var warehouse = await _warehouseRepository.FindByIdAsync(request.Id, cancellationToken);
            if (warehouse is null)
            {
                throw new NotFoundException($"Could not find Warehouse '{request.Id}'");
            }

            _warehouseRepository.Remove(warehouse);
        }
    }
}