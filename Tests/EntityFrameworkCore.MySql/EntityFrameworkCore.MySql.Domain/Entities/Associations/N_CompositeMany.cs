using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.MySql.Domain.Entities.Associations
{
    public class N_CompositeMany
    {
        public Guid Id { get; set; }

        public string ManyAttr { get; set; }

        public Guid NComplexrootId { get; set; }
    }
}