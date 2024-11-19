using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Domain.Entities.Associations
{
    public class N_ComplexRoot
    {
        public N_ComplexRoot()
        {
            ComplexAttr = null!;
            N_CompositeOne = null!;
            N_CompositeTwo = null!;
        }
        public Guid Id { get; set; }

        public string ComplexAttr { get; set; }

        public virtual N_CompositeOne N_CompositeOne { get; set; }

        public virtual N_CompositeTwo N_CompositeTwo { get; set; }

        public virtual ICollection<N_CompositeMany> N_CompositeManies { get; set; } = [];
    }
}