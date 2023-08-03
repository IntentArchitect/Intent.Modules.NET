using System;
using System.Collections.Generic;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.Associations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class H_MultipleDependent : IHasDomainEvent
    {
        [IntentManaged(Mode.Fully)]
        public H_MultipleDependent()
        {
            MultipleDepAttr = null!;
        }
        public Guid Id { get; set; }

        public string MultipleDepAttr { get; set; }

        public Guid? H_OptionalAggregateNavId { get; set; }

        public virtual H_OptionalAggregateNav? H_OptionalAggregateNav { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}