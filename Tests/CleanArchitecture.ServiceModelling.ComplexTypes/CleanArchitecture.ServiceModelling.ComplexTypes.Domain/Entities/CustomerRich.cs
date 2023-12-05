using System;
using System.Collections.Generic;
using CleanArchitecture.ServiceModelling.ComplexTypes.Domain.Common;
using CleanArchitecture.ServiceModelling.ComplexTypes.Domain.Contracts;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Domain.Entities
{
    public class CustomerRich : IHasDomainEvent
    {
        public CustomerRich(Address address)
        {
            Address = address;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected CustomerRich()
        {
            Address = null!;
        }

        public Guid Id { get; set; }

        public Address Address { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();

        public AddressDC GetAddress()
        {
            return new AddressDC(Address.Line1, Address.Line2, Address.City);
        }

        public void UpdateAddress(AddressDC address)
        {
            Address = new Address(address.Line1, address.Line2, address.City);
        }
    }
}