using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.People
{
    public class PersonDetailsNameDto : IMapFrom<Names>
    {
        public PersonDetailsNameDto()
        {
            First = null!;
            Last = null!;
        }

        public string First { get; set; }
        public string Last { get; set; }

        public static PersonDetailsNameDto Create(string first, string last)
        {
            return new PersonDetailsNameDto
            {
                First = first,
                Last = last
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Names, PersonDetailsNameDto>();
        }
    }
}