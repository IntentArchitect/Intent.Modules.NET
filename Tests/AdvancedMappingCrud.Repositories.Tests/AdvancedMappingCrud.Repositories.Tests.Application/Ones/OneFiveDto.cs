using System;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.NullableNested;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Ones
{
    public class OneFiveDto : IMapFrom<Five>
    {
        public OneFiveDto()
        {
            FiveName5 = null!;
        }

        public string FiveName5 { get; set; }
        public Guid Id { get; set; }

        public static OneFiveDto Create(string fiveName5, Guid id)
        {
            return new OneFiveDto
            {
                FiveName5 = fiveName5,
                Id = id
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Five, OneFiveDto>()
                .ForMember(d => d.FiveName5, opt => opt.MapFrom(src => src.FiveName));
        }
    }
}