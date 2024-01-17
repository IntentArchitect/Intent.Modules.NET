using System;
using System.Collections.Generic;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Products
{
    public class ProductDto : IMapFrom<Product>
    {
        public ProductDto()
        {
            Name = null!;
            Tags = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<TagDto> Tags { get; set; }

        public static ProductDto Create(Guid id, string name, List<TagDto> tags)
        {
            return new ProductDto
            {
                Id = id,
                Name = name,
                Tags = tags
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Product, ProductDto>()
                .ForMember(d => d.Tags, opt => opt.MapFrom(src => src.Tags));
        }
    }
}