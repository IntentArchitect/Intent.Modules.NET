using System;
using System.Collections.Generic;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Entities
{
    public class CompanyContactSecond : IHasDomainEvent
    {
        public CompanyContactSecond()
        {
            ContactSecond = null!;
        }

        public Guid Id { get; set; }

        public Guid ContactSecondId { get; set; }

        public virtual ContactSecond ContactSecond { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}