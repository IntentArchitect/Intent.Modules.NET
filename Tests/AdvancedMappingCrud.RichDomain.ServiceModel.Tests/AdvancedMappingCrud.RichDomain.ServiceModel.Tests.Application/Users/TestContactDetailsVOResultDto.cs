using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Users
{
    public class TestContactDetailsVOResultDto : IMapFrom<ContactDetailsVO>
    {
        public TestContactDetailsVOResultDto()
        {
            Cell = null!;
            Email = null!;
        }

        public string Cell { get; set; }
        public string Email { get; set; }

        public static TestContactDetailsVOResultDto Create(string cell, string email)
        {
            return new TestContactDetailsVOResultDto
            {
                Cell = cell,
                Email = email
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<ContactDetailsVO, TestContactDetailsVOResultDto>();
        }
    }
}