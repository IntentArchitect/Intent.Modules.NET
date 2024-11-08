using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MudBlazor.ExampleApp.Application.Common.Mappings;
using MudBlazor.ExampleApp.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MudBlazor.ExampleApp.Application.Products
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
        public string? ImageUrl { get; set; }

        public static ProductDto Create(Guid id, string name, string description, decimal price, string? imageUrl)
        {
            return new ProductDto
            {
                Id = id,
                Name = name,
                Description = description,
                Price = price,
                ImageUrl = imageUrl
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Product, ProductDto>();
        }
    }
}