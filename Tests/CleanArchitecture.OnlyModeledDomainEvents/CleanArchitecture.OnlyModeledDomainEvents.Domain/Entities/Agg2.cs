using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CleanArchitecture.OnlyModeledDomainEvents.Domain.Entities
{
    public class Agg2
    {
        public Guid Id { get; set; }
    }
}