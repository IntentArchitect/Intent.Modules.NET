using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Domain.Entities.UniqueIndexConstraint
{
    public class UniqueConstraintIndexCompositeEntityForElement
    {
        public UniqueConstraintIndexCompositeEntityForElement()
        {
            Field = null!;
        }

        public Guid Id { get; set; }

        public string Field { get; set; }

        public Guid AggregateWithUniqueConstraintIndexElementId { get; set; }
    }
}