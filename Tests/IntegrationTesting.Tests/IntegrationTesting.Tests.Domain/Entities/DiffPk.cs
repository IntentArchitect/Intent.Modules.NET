using System.Collections.Generic;
using IntegrationTesting.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace IntegrationTesting.Tests.Domain.Entities
{
    public class DiffPk : IHasDomainEvent
    {
        public string Id { get; set; }

        public string NAme { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}