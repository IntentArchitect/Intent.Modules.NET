using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Finbuckle.SeparateDatabase.TestApplication.Domain.Entities
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class User
    {
        public Guid Id { get; set; }

        public string Email { get; set; }

        public string Username { get; set; }

        public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
    }
}