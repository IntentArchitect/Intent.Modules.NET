using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Customers
{
    public class CreateCustomerUserContactDetailsVODto
    {
        public CreateCustomerUserContactDetailsVODto()
        {
            Cell = null!;
            Email = null!;
        }

        public string Cell { get; set; }
        public string Email { get; set; }

        public static CreateCustomerUserContactDetailsVODto Create(string cell, string email)
        {
            return new CreateCustomerUserContactDetailsVODto
            {
                Cell = cell,
                Email = email
            };
        }
    }
}