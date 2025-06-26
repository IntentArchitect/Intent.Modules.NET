using System;
using System.Collections.Generic;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Entities
{
    public class Country : IHasDomainEvent
    {
        public Country()
        {
            MaE = null!;
        }

        public Guid Id { get; set; }

        public string MaE { get; set; }

        public virtual ICollection<State> States { get; set; } = [];

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}