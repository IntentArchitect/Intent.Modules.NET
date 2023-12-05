using System;
using System.Collections.Generic;
using GraphQL.MongoDb.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace GraphQL.MongoDb.TestApplication.Domain.Entities
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class User : IHasDomainEvent
    {
        public User(string name, string surname, string email, IEnumerable<AssignedPrivilege> assignedPrivileges)
        {
            Name = name;
            Surname = surname;
            Email = email;
            AssignedPrivileges = new List<AssignedPrivilege>(assignedPrivileges);
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Email { get; set; }

        public ICollection<AssignedPrivilege> AssignedPrivileges { get; set; } = new List<AssignedPrivilege>();

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}