using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.MySql.Domain.Entities
{
    public class SchemaInLineChild
    {
        public SchemaInLineChild()
        {
            Name = null!;
        }
        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}