using System;
using Intent.RoslynWeaver.Attributes;
using TrainingModel.Tests.Domain;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace TrainingModel.Tests.Application.Customers
{
    public class UpdateCustomerCommandAddressDto
    {
        public UpdateCustomerCommandAddressDto()
        {
            Line1 = null!;
            Line2 = null!;
            City = null!;
            Postal = null!;
        }

        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }
        public string Postal { get; set; }
        public AddressType AddressType { get; set; }
        public Guid Id { get; set; }

        public static UpdateCustomerCommandAddressDto Create(
            string line1,
            string line2,
            string city,
            string postal,
            AddressType addressType,
            Guid id)
        {
            return new UpdateCustomerCommandAddressDto
            {
                Line1 = line1,
                Line2 = line2,
                City = city,
                Postal = postal,
                AddressType = addressType,
                Id = id
            };
        }
    }
}