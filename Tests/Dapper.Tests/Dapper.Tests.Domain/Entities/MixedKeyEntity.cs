using System;
using System.Collections.Generic;
using Dapper.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Dapper.Tests.Domain.Entities
{
    public class MixedKeyEntity : IHasDomainEvent
    {
        public MixedKeyEntity()
        {
            Name = null!;
        }

        public Guid TenantId { get; set; }

        public Guid RowId { get; set; }

        public string Name { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}