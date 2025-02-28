using System;
using System.Collections.Generic;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.Repositories.Tests.Domain;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.EntityListEnums
{
    public class EntityListEnumDto : IMapFrom<EntityListEnum>
    {
        public EntityListEnumDto()
        {
            Name = null!;
            OrderStatuses = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<OrderStatus> OrderStatuses { get; set; }

        public static EntityListEnumDto Create(Guid id, string name, List<OrderStatus> orderStatuses)
        {
            return new EntityListEnumDto
            {
                Id = id,
                Name = name,
                OrderStatuses = orderStatuses
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<EntityListEnum, EntityListEnumDto>();
        }
    }
}