using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Application.CustomerAnemics
{
    public class CreateCustomerAnemicAddressDto
    {
        public CreateCustomerAnemicAddressDto()
        {
            Line1 = null!;
            Line2 = null!;
            City = null!;
        }

        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }

        public static CreateCustomerAnemicAddressDto Create(string line1, string line2, string city)
        {
            return new CreateCustomerAnemicAddressDto
            {
                Line1 = line1,
                Line2 = line2,
                City = city
            };
        }
    }
}