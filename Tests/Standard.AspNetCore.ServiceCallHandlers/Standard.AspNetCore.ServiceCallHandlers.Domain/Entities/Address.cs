using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Standard.AspNetCore.ServiceCallHandlers.Domain.Entities
{
    public class Address
    {
        public Address()
        {
            Line1 = null!;
            Line2 = null!;
            PostalCode = null!;
        }

        public Guid Id { get; set; }

        public string Line1 { get; set; }

        public string Line2 { get; set; }

        public string PostalCode { get; set; }
    }
}