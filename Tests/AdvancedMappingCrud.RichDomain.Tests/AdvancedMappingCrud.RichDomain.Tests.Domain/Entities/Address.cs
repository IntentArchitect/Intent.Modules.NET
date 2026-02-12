using System;
using System.Collections.Generic;
using AdvancedMappingCrud.RichDomain.Tests.Domain.Common;
using AdvancedMappingCrud.RichDomain.Tests.Domain.Events;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Domain.Entities
{
    public class Address : IHasDomainEvent
    {
        public Address(string line1, string line2, string city, int postal)
        {
            Line1 = line1;
            Line2 = line2;
            City = city;
            Postal = postal;
            DomainEvents.Add(new OnAddressCreated(
                address: this));
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected Address()
        {
            Line1 = null!;
            Line2 = null!;
            City = null!;
        }
        public Guid Id { get; private set; }

        public Guid UserId { get; private set; }

        public string Line1 { get; private set; }

        public string Line2 { get; private set; }

        public string City { get; private set; }

        public int Postal { get; private set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}