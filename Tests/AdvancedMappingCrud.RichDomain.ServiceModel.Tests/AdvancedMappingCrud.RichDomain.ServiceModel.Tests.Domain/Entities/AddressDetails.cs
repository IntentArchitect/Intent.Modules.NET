using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Entities
{
    public class AddressDetails
    {
        public AddressDetails()
        {
            Name = null!;
        }

        public Guid Id { get; private set; }

        public AddressType AddressType { get; private set; }

        public AddressType? AddressTypeNullable { get; private set; }

        public string Name { get; private set; }

        public int Number { get; private set; }
    }
}