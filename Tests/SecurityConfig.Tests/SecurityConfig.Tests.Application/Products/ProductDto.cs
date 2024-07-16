using System;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using SecurityConfig.Tests.Application.Common.Mappings;
using SecurityConfig.Tests.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace SecurityConfig.Tests.Application.Products
{
    public class ProductDto : IMapFrom<Product>
    {
        public ProductDto()
        {
            Name = null!;
            Surname = null!;
        }

        public string Name { get; set; }
        public string Surname { get; set; }
        public Guid Id { get; set; }

        public static ProductDto Create(string name, string surname, Guid id)
        {
            return new ProductDto
            {
                Name = name,
                Surname = surname,
                Id = id
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Product, ProductDto>();
        }
    }
}