using System;
using AutoMapper;
using FastEndpointsTest.Application.Common.Mappings;
using FastEndpointsTest.Domain.Entities.CRUD;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FastEndpointsTest.Application.SimpleProducts
{
    public record SimpleProductDto : IMapFrom<SimpleProduct>
    {
        public SimpleProductDto()
        {
            Name = null!;
            Value = null!;
        }

        public string Name { get; init; }
        public string Value { get; init; }
        public Guid Id { get; init; }

        public static SimpleProductDto Create(string name, string value, Guid id)
        {
            return new SimpleProductDto
            {
                Name = name,
                Value = value,
                Id = id
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<SimpleProduct, SimpleProductDto>();
        }
    }
}