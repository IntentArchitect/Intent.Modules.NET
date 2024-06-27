using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace OpenApiImporterTest.Application.Customers
{
    public class CustomerDto
    {
        public CustomerDto()
        {
            Address = null!;
        }

        public Guid Id { get; set; }
        public string? Name { get; set; }
        public bool Isac { get; set; }
        public CustomerAddressDto Address { get; set; }

        public static CustomerDto Create(Guid id, string? name, bool isac, CustomerAddressDto address)
        {
            return new CustomerDto
            {
                Id = id,
                Name = name,
                Isac = isac,
                Address = address
            };
        }
    }
}