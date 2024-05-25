using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Customers
{
    public class CustomerCreateDto
    {
        public CustomerCreateDto()
        {
            User = null!;
            Login = null!;
        }

        public CustomerCreateCustomerUserDto User { get; set; }
        public string Login { get; set; }

        public static CustomerCreateDto Create(CustomerCreateCustomerUserDto user, string login)
        {
            return new CustomerCreateDto
            {
                User = user,
                Login = login
            };
        }
    }
}