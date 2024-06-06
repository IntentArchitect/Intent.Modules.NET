using System;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using TrainingModel.Tests.Application.Common.Mappings;
using TrainingModel.Tests.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace TrainingModel.Tests.Application.Products
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
        public Guid BrandId { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public decimal Price { get; set; }

        public static ProductDto Create(Guid id, string name, Guid brandId, string description, bool isActive, decimal price)
        {
            return new ProductDto
            {
                Id = id,
                Name = name,
                BrandId = brandId,
                Description = description,
                IsActive = isActive,
                Price = price
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Product, ProductDto>()
                .ForMember(d => d.Price, opt => opt.MapFrom(src => src.GetCurrentPrice()));
        }
    }
}