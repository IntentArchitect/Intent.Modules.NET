using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.Postgres.Domain.Entities.Associations
{
    public class Root
    {
        public Root()
        {
            Name = null!;
            ChildNoPK = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public virtual ChildNoPK ChildNoPK { get; set; }
    }
}