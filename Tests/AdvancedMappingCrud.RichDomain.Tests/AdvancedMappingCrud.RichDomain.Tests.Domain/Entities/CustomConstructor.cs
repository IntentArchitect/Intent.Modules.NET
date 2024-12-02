using System;
using System.Collections.Generic;
using AdvancedMappingCrud.RichDomain.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Domain.Entities
{
    public class CustomConstructor : IHasDomainEvent
    {
        public CustomConstructor()
        {
            Col1 = null!;
            // This should NOT be overwritten by the SF
            Col2 = string.Empty!;
        }

        public Guid Id { get; private set; }

        public string Col1 { get; private set; }

        public string Col2 { get; private set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}