using System;
using System.Collections.Generic;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Associations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class M_SelfReferenceBiNav : IHasDomainEvent
    {
        [IntentManaged(Mode.Fully)]
        public M_SelfReferenceBiNav()
        {
            PartitionKey = null!;
            SelfRefBiNavAttr = null!;
        }
        public Guid Id { get; set; }

        public string PartitionKey { get; set; }

        public string SelfRefBiNavAttr { get; set; }

        public Guid? M_SelfReferenceBiNavAssocationId { get; set; }

        public virtual M_SelfReferenceBiNav? M_SelfReferenceBiNavAssocation { get; set; }

        public virtual ICollection<M_SelfReferenceBiNav> M_SelfReferenceBiNavs { get; set; } = new List<M_SelfReferenceBiNav>();

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}