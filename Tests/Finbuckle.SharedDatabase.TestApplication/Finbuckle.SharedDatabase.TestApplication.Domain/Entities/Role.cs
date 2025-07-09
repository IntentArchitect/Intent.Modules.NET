using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Finbuckle.SharedDatabase.TestApplication.Domain.Entities
{
    public class Role
    {
        public Role()
        {
            Name = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid UserId { get; set; }
    }
}