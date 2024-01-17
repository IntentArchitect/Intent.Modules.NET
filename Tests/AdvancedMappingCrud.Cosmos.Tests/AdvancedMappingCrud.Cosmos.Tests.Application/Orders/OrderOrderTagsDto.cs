using System;
using AdvancedMappingCrud.Cosmos.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.Cosmos.Tests.Domain;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Orders
{
    public class OrderOrderTagsDto : IMapFrom<OrderTags>
    {
        public OrderOrderTagsDto()
        {
            Name = null!;
            Value = null!;
        }

        public string Name { get; set; }
        public string Value { get; set; }
        public Guid Id { get; set; }

        public static OrderOrderTagsDto Create(string name, string value, Guid id)
        {
            return new OrderOrderTagsDto
            {
                Name = name,
                Value = value,
                Id = id
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<OrderTags, OrderOrderTagsDto>();
        }
    }
}