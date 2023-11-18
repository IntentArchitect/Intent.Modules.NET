using System;
using AutoMapper;
using CleanArchitecture.TestApplication.Application.Common.Mappings;
using CleanArchitecture.TestApplication.Domain.Entities.General;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.General
{
    public class CustomMappingDto : IMapFrom<CustomMapping>
    {
        public CustomMappingDto()
        {
        }

        public Guid Id { get; set; }
        public DateTime? When { get; set; }
        public int CustomInt { get; set; }

        public static CustomMappingDto Create(Guid id, DateTime? when, int customInt)
        {
            return new CustomMappingDto
            {
                Id = id,
                When = when,
                CustomInt = customInt
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CustomMapping, CustomMappingDto>()
                .ForMember(d => d.CustomInt, opt => opt.MapFrom(src => src.When.HasValue ? src.When.Value.Day : 1));
        }
    }
}