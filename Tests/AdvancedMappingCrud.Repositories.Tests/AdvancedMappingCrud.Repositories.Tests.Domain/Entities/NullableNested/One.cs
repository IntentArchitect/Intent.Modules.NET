using System;
using System.Collections.Generic;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Entities.NullableNested
{
    public class One : IHasDomainEvent
    {
        public One()
        {
            OneName = null!;
            Two = null!;
        }

        public Guid Id { get; set; }

        public string OneName { get; set; }

        public int OneAge { get; set; }

        public virtual Two Two { get; set; }

        public virtual Four? Four { get; set; }

        public virtual ICollection<Five> Fives { get; set; } = [];

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}