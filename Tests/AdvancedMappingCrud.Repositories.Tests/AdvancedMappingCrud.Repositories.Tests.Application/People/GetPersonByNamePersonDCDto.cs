using AdvancedMappingCrud.Repositories.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.Repositories.Tests.Domain.Contracts;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.People
{
    public class GetPersonByNamePersonDCDto : IMapFrom<PersonDC>
    {
        public GetPersonByNamePersonDCDto()
        {
            FirstName = null!;
            Surname = null!;
        }

        public string FirstName { get; set; }
        public string Surname { get; set; }

        public static GetPersonByNamePersonDCDto Create(string firstName, string surname)
        {
            return new GetPersonByNamePersonDCDto
            {
                FirstName = firstName,
                Surname = surname
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<PersonDC, GetPersonByNamePersonDCDto>();
        }
    }
}