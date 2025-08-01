using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Finbuckle.SeparateDatabase.TestApplication.Domain.Entities
{
    public class User
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
    }
}