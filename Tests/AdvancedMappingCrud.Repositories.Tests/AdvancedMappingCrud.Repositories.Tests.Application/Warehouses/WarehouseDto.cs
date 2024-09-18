using System;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Warehouses
{
    public class WarehouseDto : IMapFrom<Warehouse>
    {
        public WarehouseDto()
        {
            Name = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Size { get; set; }

        public static WarehouseDto Create(Guid id, string name, int size)
        {
            return new WarehouseDto
            {
                Id = id,
                Name = name,
                Size = size
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Warehouse, WarehouseDto>();
        }
    }
}