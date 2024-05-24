using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.People
{
    public class PersonDetailsDto : IMapFrom<PersonDetails>
    {
        public PersonDetailsDto()
        {
            Name = null!;
        }

        public PersonDetailsNameDto Name { get; set; }

        public static PersonDetailsDto Create(PersonDetailsNameDto name)
        {
            return new PersonDetailsDto
            {
                Name = name
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<PersonDetails, PersonDetailsDto>();
        }
    }
}