using System;
using System.Collections.Generic;
using CleanArchitecture.Comprehensive.Domain.Common;
using CleanArchitecture.Comprehensive.Domain.ComplexTypes;
using CleanArchitecture.Comprehensive.Domain.Contracts.ComplexTypes;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Domain.Entities.ComplexTypes
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods | Targets.Constructors, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class Customer2CT : IHasDomainEvent
    {
        public Customer2CT()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public Customer2CT(string name, AddressDC address)
        {
            Name = name;
            Address2CT = new Address2CT(address.Line1, address.Line2, address.Line2);
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public Address2CT Address2CT { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();

        public AddressDC GetAddress()
        {
            return new AddressDC(Address2CT.Line1, Address2CT.Line2, Address2CT.City);
        }

        public void UpdateAddress(AddressDC address)
        {
            Address2CT = new Address2CT(address.Line1, address.Line2, address.Line2);
        }
    }
}