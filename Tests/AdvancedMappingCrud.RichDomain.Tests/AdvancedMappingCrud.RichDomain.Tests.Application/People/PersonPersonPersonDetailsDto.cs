using AdvancedMappingCrud.RichDomain.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.RichDomain.Tests.Domain;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.People
{
    public class PersonPersonPersonDetailsDto : IMapFrom<PersonDetails>
    {
        public PersonPersonPersonDetailsDto()
        {
            Name = null!;
        }

        public PersonPersonPersonDetailsNameDto Name { get; set; }

        public static PersonPersonPersonDetailsDto Create(PersonPersonPersonDetailsNameDto name)
        {
            return new PersonPersonPersonDetailsDto
            {
                Name = name
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<PersonDetails, PersonPersonPersonDetailsDto>();
        }
    }
}