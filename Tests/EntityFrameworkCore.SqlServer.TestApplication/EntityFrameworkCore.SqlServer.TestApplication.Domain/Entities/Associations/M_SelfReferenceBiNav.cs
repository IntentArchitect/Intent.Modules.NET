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
    public class M_SelfReferenceBiNav : IHasDomainEvent
    {
        [IntentManaged(Mode.Fully)]
        public M_SelfReferenceBiNav()
        {
            SelfRefBiNavAttr = null!;
        }
        public Guid Id { get; set; }

        public string SelfRefBiNavAttr { get; set; }

        public Guid? M_SelfReferenceBiNavDstId { get; set; }

        public virtual M_SelfReferenceBiNav? M_SelfReferenceBiNavDst { get; set; }

        public virtual ICollection<M_SelfReferenceBiNav> M_SelfReferenceBiNavs { get; set; } = new List<M_SelfReferenceBiNav>();

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}