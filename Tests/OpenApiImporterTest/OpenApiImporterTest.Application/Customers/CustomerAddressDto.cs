using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace OpenApiImporterTest.Application.Customers
{
    public class CustomerAddressDto
    {
        public CustomerAddressDto()
        {
        }

        public string? Line1 { get; set; }
        public Guid Id { get; set; }

        public static CustomerAddressDto Create(string? line1, Guid id)
        {
            return new CustomerAddressDto
            {
                Line1 = line1,
                Id = id
            };
        }
    }
}