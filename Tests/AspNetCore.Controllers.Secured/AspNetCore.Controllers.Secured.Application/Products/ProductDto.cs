using System;
using AspNetCore.Controllers.Secured.Application.Common.Mappings;
using AspNetCore.Controllers.Secured.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AspNetCore.Controllers.Secured.Application.Products
{
    public class ProductDto : IMapFrom<Product>
    {
        public ProductDto()
        {
            Name = null!;
            Description = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

        public static ProductDto Create(Guid id, string name, string description, decimal price)
        {
            return new ProductDto
            {
                Id = id,
                Name = name,
                Description = description,
                Price = price
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Product, ProductDto>();
        }
    }
}