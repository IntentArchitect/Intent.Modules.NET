using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Domain.Entities.Associations
{
    public class M_SelfReferenceBiNav
    {
        public M_SelfReferenceBiNav()
        {
            SelfRefBiNavAttr = null!;
        }
        public Guid Id { get; set; }

        public string SelfRefBiNavAttr { get; set; }

        public Guid? M_SelfReferenceBiNavDstId { get; set; }

        public virtual M_SelfReferenceBiNav? M_SelfReferenceBiNavDst { get; set; }

        public virtual ICollection<M_SelfReferenceBiNav> M_SelfReferenceBiNavs { get; set; } = [];
    }
}