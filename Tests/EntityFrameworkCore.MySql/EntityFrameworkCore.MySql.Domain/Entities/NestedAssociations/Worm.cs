using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.MySql.Domain.Entities.NestedAssociations
{
    public class Worm
    {
        public Worm()
        {
            Color = null!;
        }
        public Guid Id { get; set; }

        public string Color { get; set; }

        public Guid? LeafId { get; set; }
    }
}