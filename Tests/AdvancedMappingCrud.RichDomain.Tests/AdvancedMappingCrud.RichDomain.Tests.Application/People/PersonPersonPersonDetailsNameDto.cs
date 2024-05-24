using AdvancedMappingCrud.RichDomain.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.RichDomain.Tests.Domain;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.People
{
    public class PersonPersonPersonDetailsNameDto : IMapFrom<Names>
    {
        public PersonPersonPersonDetailsNameDto()
        {
            First = null!;
            Last = null!;
        }

        public string First { get; set; }
        public string Last { get; set; }

        public static PersonPersonPersonDetailsNameDto Create(string first, string last)
        {
            return new PersonPersonPersonDetailsNameDto
            {
                First = first,
                Last = last
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Names, PersonPersonPersonDetailsNameDto>();
        }
    }
}