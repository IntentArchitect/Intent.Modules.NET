using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.Postgres.Domain.Entities.NestedAssociations
{
    public class Worm
    {
        public Guid Id { get; set; }

        public string Color { get; set; }

        public Guid? LeafId { get; set; }
    }
}