using System;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.People
{
    public class PersonDto : IMapFrom<Person>
    {
        public PersonDto()
        {
            Details = null!;
        }

        public PersonDetailsDto Details { get; set; }
        public Guid Id { get; set; }

        public static PersonDto Create(PersonDetailsDto details, Guid id)
        {
            return new PersonDto
            {
                Details = details,
                Id = id
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Person, PersonDto>();
        }
    }
}