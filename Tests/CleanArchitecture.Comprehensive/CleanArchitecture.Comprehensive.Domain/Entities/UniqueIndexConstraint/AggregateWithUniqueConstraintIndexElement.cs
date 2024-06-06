using System;
using System.Collections.Generic;
using CleanArchitecture.Comprehensive.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Domain.Entities.UniqueIndexConstraint
{
    public class AggregateWithUniqueConstraintIndexElement : IHasDomainEvent
    {
        public AggregateWithUniqueConstraintIndexElement(string singleUniqueField,
            string compUniqueFieldA,
            string compUniqueFieldB)
        {
            SingleUniqueField = singleUniqueField;
            CompUniqueFieldA = compUniqueFieldA;
            CompUniqueFieldB = compUniqueFieldB;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        [IntentManaged(Mode.Ignore)]
        public AggregateWithUniqueConstraintIndexElement()
        {
            SingleUniqueField = null!;
            CompUniqueFieldA = null!;
            CompUniqueFieldB = null!;
        }
        public Guid Id { get; set; }

        public string SingleUniqueField { get; set; }

        public string CompUniqueFieldA { get; set; }

        public string CompUniqueFieldB { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}