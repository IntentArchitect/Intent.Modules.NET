using System;
using AutoMapper;
using CleanArchitecture.Comprehensive.Application.Common.Mappings;
using CleanArchitecture.Comprehensive.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Ones
{
    public class FourDto : IMapFrom<Four>
    {
        public FourDto()
        {
            Name = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? FiveParentId { get; set; }

        public static FourDto Create(Guid id, string name, Guid? fiveParentId)
        {
            return new FourDto
            {
                Id = id,
                Name = name,
                FiveParentId = fiveParentId
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Four, FourDto>()
                .ForMember(d => d.FiveParentId, opt => opt.MapFrom(src => src.Five != null ? src.Five!.ParentId : (Guid?)null));
        }
    }
}