using AdvancedMappingCrudMongo.Tests.Application.Common.Mappings;
using AdvancedMappingCrudMongo.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.Application.Orders
{
    public class OrderOrderItemProductDto : IMapFrom<Product>
    {
        public OrderOrderItemProductDto()
        {
            Name = null!;
            Description = null!;
            Id = null!;
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Id { get; set; }

        public static OrderOrderItemProductDto Create(string name, string description, string id)
        {
            return new OrderOrderItemProductDto
            {
                Name = name,
                Description = description,
                Id = id
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Product, OrderOrderItemProductDto>();
        }
    }
}