using System;
using AutoMapper;
using GrpcServer.Application.Common.Mappings;
using GrpcServer.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace GrpcServer.Application.Products
{
    public class ProductDto : IMapFrom<Product>
    {
        public ProductDto()
        {
            Name = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public static ProductDto Create(Guid id, string name)
        {
            return new ProductDto
            {
                Id = id,
                Name = name
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Product, ProductDto>();
        }
    }
}