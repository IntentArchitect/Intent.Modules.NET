using AdvancedMappingCrud.Cosmos.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Customers.CreateCustomer
{
    public class CreateCustomerCommand : IRequest<string>, ICommand
    {
        public CreateCustomerCommand(string name,
            string surname,
            bool isActive,
            string line1,
            string line2,
            string city,
            string postalCode)
        {
            Name = name;
            Surname = surname;
            IsActive = isActive;
            Line1 = line1;
            Line2 = line2;
            City = city;
            PostalCode = postalCode;
        }

        public string Name { get; set; }
        public string Surname { get; set; }
        public bool IsActive { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
    }
}