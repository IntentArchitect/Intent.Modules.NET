using System;
using System.Collections.Generic;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Associations
{
    public class M_SelfReferenceBiNav : IHasDomainEvent
    {
        public M_SelfReferenceBiNav()
        {
            PartitionKey = null!;
            SelfRefBiNavAttr = null!;
        }

        public Guid Id { get; set; }

        public string PartitionKey { get; set; }

        public string SelfRefBiNavAttr { get; set; }

        public Guid? MSelfreferencebinavassocationId { get; set; }

        public virtual M_SelfReferenceBiNav? M_SelfReferenceBiNavAssocation { get; set; }

        public virtual ICollection<M_SelfReferenceBiNav> M_SelfReferenceBiNavs { get; set; } = [];

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}