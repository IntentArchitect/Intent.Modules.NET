using System;
using System.Collections.Generic;
using IntegrationTesting.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace IntegrationTesting.Tests.Domain.Entities
{
    public class UniqueConVal : IHasDomainEvent
    {
        public UniqueConVal()
        {
            Att1 = null!;
            Att2 = null!;
            AttInclude = null!;
        }
        public Guid Id { get; set; }

        public string Att1 { get; set; }

        public string Att2 { get; set; }

        public string AttInclude { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}