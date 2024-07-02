using System;
using AutoMapper;
using IntegrationTesting.Tests.Application.Common.Mappings;
using IntegrationTesting.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.RichProducts
{
    public class RichProductDto : IMapFrom<RichProduct>
    {
        public RichProductDto()
        {
            Name = null!;
        }

        public Guid Id { get; set; }
        public Guid BrandId { get; set; }
        public string Name { get; set; }

        public static RichProductDto Create(Guid id, Guid brandId, string name)
        {
            return new RichProductDto
            {
                Id = id,
                BrandId = brandId,
                Name = name
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<RichProduct, RichProductDto>();
        }
    }
}