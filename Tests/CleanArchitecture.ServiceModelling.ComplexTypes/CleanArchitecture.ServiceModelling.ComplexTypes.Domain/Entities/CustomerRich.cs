using System;
using System.Collections.Generic;
using CleanArchitecture.ServiceModelling.ComplexTypes.Domain.Common;
using CleanArchitecture.ServiceModelling.ComplexTypes.Domain.Contracts;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Domain.Entities
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods | Targets.Constructors, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class CustomerRich : IHasDomainEvent
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public CustomerRich(Address address)
        {
            Address = address;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        [IntentManaged(Mode.Fully)]
        protected CustomerRich()
        {
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