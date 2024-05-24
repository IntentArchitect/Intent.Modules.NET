using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Users
{
    public class TestCompanyResultDto : IMapFrom<Company>
    {
        public TestCompanyResultDto()
        {
            Name = null!;
        }

        public string Name { get; set; }

        public static TestCompanyResultDto Create(string name)
        {
            return new TestCompanyResultDto
            {
                Name = name
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Company, TestCompanyResultDto>();
        }
    }
}