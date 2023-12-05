using System;
using System.Collections.Generic;
using Finbuckle.SharedDatabase.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Finbuckle.SharedDatabase.TestApplication.Domain.Entities
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class User : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public string Email { get; set; }

        public string Username { get; set; }

        public virtual ICollection<Role> Roles { get; set; } = new List<Role>();

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}