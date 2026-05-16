using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Entities
{
    public partial class AddressDetails
    {
        public Guid Id { get; set; }

        public AddressType AddressType { get; set; }

        public AddressType? AddressTypeNullable { get; set; }

        public string Name { get; set; }

        public int Number { get; set; }
    }
}