using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Companies
{
    public class CompanyCreateCompanyContactDetailsVODto
    {
        public CompanyCreateCompanyContactDetailsVODto()
        {
            Cell = null!;
            Email = null!;
        }

        public string Cell { get; set; }
        public string Email { get; set; }

        public static CompanyCreateCompanyContactDetailsVODto Create(string cell, string email)
        {
            return new CompanyCreateCompanyContactDetailsVODto
            {
                Cell = cell,
                Email = email
            };
        }
    }
}