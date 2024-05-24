using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Customers
{
    public class CustomerCreateCustomerUserContactDetailsVODto
    {
        public CustomerCreateCustomerUserContactDetailsVODto()
        {
            Cell = null!;
            Email = null!;
        }

        public string Cell { get; set; }
        public string Email { get; set; }

        public static CustomerCreateCustomerUserContactDetailsVODto Create(string cell, string email)
        {
            return new CustomerCreateCustomerUserContactDetailsVODto
            {
                Cell = cell,
                Email = email
            };
        }
    }
}