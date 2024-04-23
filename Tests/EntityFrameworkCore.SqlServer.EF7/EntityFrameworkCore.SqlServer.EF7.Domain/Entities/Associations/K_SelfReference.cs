using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Domain.Entities.Associations
{
    public class K_SelfReference
    {
        public Guid Id { get; set; }

        public string SelfRefAttr { get; set; }

        public Guid? K_SelfReferenceAssociationId { get; set; }

        public virtual K_SelfReference? K_SelfReferenceAssociation { get; set; }
    }
}