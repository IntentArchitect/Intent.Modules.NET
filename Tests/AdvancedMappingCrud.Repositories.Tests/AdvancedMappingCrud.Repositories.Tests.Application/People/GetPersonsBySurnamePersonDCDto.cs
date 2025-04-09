using AdvancedMappingCrud.Repositories.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.Repositories.Tests.Domain.Contracts;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.People
{
    public class GetPersonsBySurnamePersonDCDto : IMapFrom<PersonDC>
    {
        public GetPersonsBySurnamePersonDCDto()
        {
            FirstName = null!;
            Surname = null!;
        }

        public string FirstName { get; set; }
        public string Surname { get; set; }

        public static GetPersonsBySurnamePersonDCDto Create(string firstName, string surname)
        {
            return new GetPersonsBySurnamePersonDCDto
            {
                FirstName = firstName,
                Surname = surname
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<PersonDC, GetPersonsBySurnamePersonDCDto>();
        }
    }
}