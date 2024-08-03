using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.MySql.Domain.Entities.NestedAssociations
{
    public class Internode
    {
        public Guid Id { get; set; }

        public string InternodeAttribute { get; set; }
    }
}