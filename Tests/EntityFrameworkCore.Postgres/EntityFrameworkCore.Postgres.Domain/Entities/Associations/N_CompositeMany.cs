using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.Postgres.Domain.Entities.Associations
{
    public class N_CompositeMany
    {
        public N_CompositeMany()
        {
            ManyAttr = null!;
        }
        public Guid Id { get; set; }

        public string ManyAttr { get; set; }

        public Guid NComplexrootId { get; set; }
    }
}