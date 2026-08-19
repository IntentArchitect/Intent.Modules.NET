using IntegrationTesting.SQLLite.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace IntegrationTesting.SQLLite.Tests.Application.Customers.CreateCustomer
{
    public class CreateCustomerCommand : IRequest<Guid>, ICommand
    {
        public CreateCustomerCommand(string firstName, string lastName, string email, string? phoneNumber)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
}