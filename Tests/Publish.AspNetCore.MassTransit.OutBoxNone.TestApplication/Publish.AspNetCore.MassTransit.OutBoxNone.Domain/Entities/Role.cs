using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Publish.AspNetCore.MassTransit.OutBoxNone.Domain.Entities
{
    public class Role
    {
        public Role()
        {
            Name = null!;
        }
        public Guid Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<Priviledge> Priviledges { get; set; } = [];
    }
}