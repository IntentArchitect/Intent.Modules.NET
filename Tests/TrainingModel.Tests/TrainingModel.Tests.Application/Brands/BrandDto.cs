using System;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using TrainingModel.Tests.Application.Common.Mappings;
using TrainingModel.Tests.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace TrainingModel.Tests.Application.Brands
{
    public class BrandDto : IMapFrom<Brand>
    {
        public BrandDto()
        {
            Name = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }

        public static BrandDto Create(Guid id, string name, bool isActive)
        {
            return new BrandDto
            {
                Id = id,
                Name = name,
                IsActive = isActive
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Brand, BrandDto>();
        }
    }
}