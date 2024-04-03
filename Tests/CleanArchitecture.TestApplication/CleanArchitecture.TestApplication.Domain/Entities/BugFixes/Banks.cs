using System;
using System.Collections.Generic;
using CleanArchitecture.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CleanArchitecture.TestApplication.Domain.Entities.BugFixes
{
    public class Banks : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}