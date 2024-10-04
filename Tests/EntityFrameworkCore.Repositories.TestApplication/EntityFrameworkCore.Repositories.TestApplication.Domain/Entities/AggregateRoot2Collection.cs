using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Domain.Entities
{
    public class AggregateRoot2Collection
    {
        public Guid Id { get; set; }

        public Guid AggregateRoot2CompositionId { get; set; }
    }
}