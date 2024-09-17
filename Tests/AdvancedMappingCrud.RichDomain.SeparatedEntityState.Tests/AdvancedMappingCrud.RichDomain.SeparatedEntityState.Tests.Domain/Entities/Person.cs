using System;
using System.Collections.Generic;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Entities
{
    public class Person : IHasDomainEvent
    {
        public Person(PersonDetails details)
        {
            Details = details;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected Person()
        {
            Details = null!;
        }

        public Guid Id { get; set; }

        public PersonDetails Details { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();

        public void Update(PersonDetails details)
        {
            Details = details;
        }
    }
}