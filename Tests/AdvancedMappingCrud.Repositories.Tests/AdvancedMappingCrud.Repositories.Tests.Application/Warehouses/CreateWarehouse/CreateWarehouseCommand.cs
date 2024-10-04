using System;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Warehouses.CreateWarehouse
{
    public class CreateWarehouseCommand : IRequest<Guid>, ICommand
    {
        public CreateWarehouseCommand(string name, int size, CreateWarehouseCommandAddressDto? address)
        {
            Name = name;
            Size = size;
            Address = address;
        }

        public string Name { get; set; }
        public int Size { get; set; }
        public CreateWarehouseCommandAddressDto? Address { get; set; }
    }
}