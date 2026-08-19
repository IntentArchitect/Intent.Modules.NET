using IntegrationTesting.SQLLite.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace IntegrationTesting.SQLLite.Tests.Application.Customers.UpdateCustomer
{
    public class UpdateCustomerCommand : IRequest, ICommand
    {
        public UpdateCustomerCommand(Guid id, string firstName, string lastName, string email, string? phoneNumber)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
        }

        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
}