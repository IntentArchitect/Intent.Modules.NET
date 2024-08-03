using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.Postgres.Domain.Entities.NestedAssociations
{
    public class Sun
    {
        public Guid Id { get; set; }

        public float Temp { get; set; }
    }
}