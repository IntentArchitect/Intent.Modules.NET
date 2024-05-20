using AdvancedMappingCrud.RichDomain.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.RichDomain.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Users
{
    public class TestCompanyDto : IMapFrom<Company>
    {
        public TestCompanyDto()
        {
            Name = null!;
        }

        public string Name { get; set; }

        public static TestCompanyDto Create(string name)
        {
            return new TestCompanyDto
            {
                Name = name
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Company, TestCompanyDto>();
        }
    }
}