using System;
using System.Collections.Generic;
using CleanArchitecture.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CleanArchitecture.TestApplication.Domain.Entities.ConventionBasedEventPublishing
{
    public class IntegrationTriggering : IHasDomainEvent
    {
        public IntegrationTriggering()
        {
        }

        public IntegrationTriggering(string value)
        {
            Value = value;
        }

        public Guid Id { get; set; }

        public string Value { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();

        public void Update(string value)
        {
            Value = value;
        }
    }
}