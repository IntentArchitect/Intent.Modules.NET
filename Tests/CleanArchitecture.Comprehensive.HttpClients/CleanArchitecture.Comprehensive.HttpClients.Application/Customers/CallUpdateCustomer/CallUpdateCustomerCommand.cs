using CleanArchitecture.Comprehensive.HttpClients.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.HttpClients.Application.Customers.CallUpdateCustomer
{
    public class CallUpdateCustomerCommand : IRequest, ICommand
    {
        public CallUpdateCustomerCommand(Guid id,
            string name,
            string surname,
            string email,
            string addressLine1,
            string addressLine2,
            string addressCity,
            string addressPostal,
            Guid addressId)
        {
            Id = id;
            Name = name;
            Surname = surname;
            Email = email;
            AddressLine1 = addressLine1;
            AddressLine2 = addressLine2;
            AddressCity = addressCity;
            AddressPostal = addressPostal;
            AddressId = addressId;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressCity { get; set; }
        public string AddressPostal { get; set; }
        public Guid AddressId { get; set; }
    }
}