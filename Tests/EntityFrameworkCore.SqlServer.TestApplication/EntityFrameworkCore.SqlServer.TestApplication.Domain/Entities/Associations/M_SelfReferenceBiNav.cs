using System;
using System.Collections.Generic;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.Associations
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class M_SelfReferenceBiNav : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public string SelfRefBiNavAttr { get; set; }

        public Guid? M_SelfReferenceBiNavDstId { get; set; }

        public virtual M_SelfReferenceBiNav? M_SelfReferenceBiNavDst { get; set; }

        public virtual ICollection<M_SelfReferenceBiNav> M_SelfReferenceBiNavs { get; set; } = new List<M_SelfReferenceBiNav>();

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}