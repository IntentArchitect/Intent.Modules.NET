using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Companies
{
    public class CreateCompanyContactDetailsVODto
    {
        public CreateCompanyContactDetailsVODto()
        {
            Cell = null!;
            Email = null!;
        }

        public string Cell { get; set; }
        public string Email { get; set; }

        public static CreateCompanyContactDetailsVODto Create(string cell, string email)
        {
            return new CreateCompanyContactDetailsVODto
            {
                Cell = cell,
                Email = email
            };
        }
    }
}