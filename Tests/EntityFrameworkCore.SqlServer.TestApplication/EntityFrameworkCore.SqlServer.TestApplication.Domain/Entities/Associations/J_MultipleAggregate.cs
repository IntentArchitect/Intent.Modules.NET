using System;
using System.Collections.Generic;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.Associations
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class J_MultipleAggregate : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public string MultipleAggrAttr { get; set; }

        public Guid J_RequiredDependentId { get; set; }

        public virtual J_RequiredDependent J_RequiredDependent { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}