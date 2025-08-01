using System;
using System.Collections.Generic;
using Finbuckle.SharedDatabase.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Finbuckle.SharedDatabase.TestApplication.Domain.Entities
{
    public class User : IHasDomainEvent
    {
        public User()
        {
            Email = null!;
            Username = null!;
        }

        public Guid Id { get; set; }

        public string Email { get; set; }

        public string Username { get; set; }

        public virtual ICollection<Role> Roles { get; set; } = [];

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}