using System;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.People
{
    public class PersonDto : IMapFrom<Person>
    {
        public PersonDto()
        {
            Details = null!;
        }

        public Guid Id { get; set; }
        public PersonPersonPersonDetailsDto Details { get; set; }

        public static PersonDto Create(Guid id, PersonPersonPersonDetailsDto details)
        {
            return new PersonDto
            {
                Id = id,
                Details = details
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Person, PersonDto>();
        }
    }
}