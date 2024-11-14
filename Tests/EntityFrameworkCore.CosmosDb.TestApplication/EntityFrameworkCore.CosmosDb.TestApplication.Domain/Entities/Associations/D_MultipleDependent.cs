using System;
using System.Collections.Generic;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Associations
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class D_MultipleDependent : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public string MultipleDependentAttr { get; set; }

        public string PartitionKey { get; set; }

        public Guid? DOptionalaggregateId { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}