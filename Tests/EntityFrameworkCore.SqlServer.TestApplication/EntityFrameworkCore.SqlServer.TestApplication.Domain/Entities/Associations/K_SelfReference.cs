using System;
using System.Collections.Generic;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

namespace EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.Associations
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class K_SelfReference : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public string SelfRefAttr { get; set; }

        public Guid? K_SelfReferenceAssociationId { get; set; }

        public virtual K_SelfReference? K_SelfReferenceAssociation { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}