using System;
using System.Collections.Generic;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Common;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.BasicAudit
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods | Targets.Constructors, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public abstract class Audit_BaseClass : IHasDomainEvent, IAuditable
    {
        public Guid Id { get; set; }

        public string PartitionKey { get; set; }

        public string? CreatedBy { get; set; }

        public DateTimeOffset CreatedDate { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTimeOffset? UpdatedDate { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}