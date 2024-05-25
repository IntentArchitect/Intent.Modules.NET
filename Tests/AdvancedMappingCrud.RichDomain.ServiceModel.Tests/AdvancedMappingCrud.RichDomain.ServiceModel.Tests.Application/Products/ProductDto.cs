using System;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Products
{
    public class ProductDto : IMapFrom<Product>
    {
        public ProductDto()
        {
            Name = null!;
        }

        public string Name { get; set; }
        public Guid Id { get; set; }

        public static ProductDto Create(string name, Guid id)
        {
            return new ProductDto
            {
                Name = name,
                Id = id
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Product, ProductDto>();
        }
    }
}