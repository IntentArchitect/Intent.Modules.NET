using System;
using System.Collections.Generic;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Entities
{
    public class Contact : IHasDomainEvent
    {
        public Contact()
        {
            FirstName = null!;
        }

        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}