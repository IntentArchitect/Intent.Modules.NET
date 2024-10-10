using System;
using System.Collections.Generic;
using CleanArchitecture.Comprehensive.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Domain.Entities
{
    /// <summary>
    /// This is to test we properly disambiguate the enum type with the same name for the property.
    /// </summary>
    public class TypeName : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public Domain.TypeName Attribute { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}