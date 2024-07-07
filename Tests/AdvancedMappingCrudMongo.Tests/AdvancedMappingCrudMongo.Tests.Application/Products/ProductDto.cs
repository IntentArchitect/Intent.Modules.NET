using AdvancedMappingCrudMongo.Tests.Application.Common.Mappings;
using AdvancedMappingCrudMongo.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.Application.Products
{
    public class ProductDto : IMapFrom<Product>
    {
        public ProductDto()
        {
            Id = null!;
            Name = null!;
            Description = null!;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public static ProductDto Create(string id, string name, string description)
        {
            return new ProductDto
            {
                Id = id,
                Name = name,
                Description = description
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Product, ProductDto>();
        }
    }
}