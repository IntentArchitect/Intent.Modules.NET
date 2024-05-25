using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Companies
{
    public class CompanyCreateDto
    {
        public CompanyCreateDto()
        {
            Name = null!;
            ContactDetailsVOS = null!;
        }

        public string Name { get; set; }
        public List<CompanyCreateCompanyContactDetailsVODto> ContactDetailsVOS { get; set; }

        public static CompanyCreateDto Create(string name, List<CompanyCreateCompanyContactDetailsVODto> contactDetailsVOS)
        {
            return new CompanyCreateDto
            {
                Name = name,
                ContactDetailsVOS = contactDetailsVOS
            };
        }
    }
}