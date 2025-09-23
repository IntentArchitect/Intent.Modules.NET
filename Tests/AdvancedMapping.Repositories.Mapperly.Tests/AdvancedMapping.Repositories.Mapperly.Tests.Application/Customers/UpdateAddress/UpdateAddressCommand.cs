using AdvancedMapping.Repositories.Mapperly.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Customers.UpdateAddress
{
    public class UpdateAddressCommand : IRequest, ICommand
    {
        public UpdateAddressCommand(Guid customerId,
            Guid id,
            string line1,
            string line2,
            string city,
            string province,
            string postalCode,
            string country)
        {
            CustomerId = customerId;
            Id = id;
            Line1 = line1;
            Line2 = line2;
            City = city;
            Province = province;
            PostalCode = postalCode;
            Country = country;
        }

        public Guid CustomerId { get; set; }
        public Guid Id { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
    }
}