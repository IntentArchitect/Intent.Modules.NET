using System;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Standard.AspNetCore.TestApplication.Application.Common.Mappings;
using Standard.AspNetCore.TestApplication.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Standard.AspNetCore.TestApplication.Application.Plurals
{
    public class PluralsDto : IMapFrom<Domain.Entities.Plurals>
    {
        public PluralsDto()
        {
        }

        public Guid Id { get; set; }

        public static PluralsDto Create(Guid id)
        {
            return new PluralsDto
            {
                Id = id
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Domain.Entities.Plurals, PluralsDto>();
        }
    }
}