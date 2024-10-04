using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Users
{
    public class TestContactDetailsVODto : IMapFrom<ContactDetailsVO>
    {
        public TestContactDetailsVODto()
        {
            Cell = null!;
            Email = null!;
        }

        public string Cell { get; set; }
        public string Email { get; set; }

        public static TestContactDetailsVODto Create(string cell, string email)
        {
            return new TestContactDetailsVODto
            {
                Cell = cell,
                Email = email
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<ContactDetailsVO, TestContactDetailsVODto>();
        }
    }
}