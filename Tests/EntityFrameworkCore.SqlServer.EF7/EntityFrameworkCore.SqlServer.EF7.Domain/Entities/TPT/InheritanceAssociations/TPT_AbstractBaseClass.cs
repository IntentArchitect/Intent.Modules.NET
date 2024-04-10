using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Domain.Entities.TPT.InheritanceAssociations
{
    public abstract class TPT_AbstractBaseClass
    {
        public Guid Id { get; set; }

        public string BaseAttribute { get; set; }
    }
}