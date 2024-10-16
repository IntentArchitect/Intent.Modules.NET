using System;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Warehouses.UpdateWarehouse
{
    public class UpdateWarehouseCommand : IRequest, ICommand
    {
        public UpdateWarehouseCommand(string name, int size, Guid id, UpdateWarehouseCommandAddressDto? address)
        {
            Name = name;
            Size = size;
            Id = id;
            Address = address;
        }

        public string Name { get; set; }
        public int Size { get; set; }
        public Guid Id { get; set; }
        public UpdateWarehouseCommandAddressDto? Address { get; set; }
    }
}