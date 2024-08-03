using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.MySql.Domain.Entities.TPH.InheritanceAssociations
{
    public abstract class TPH_AbstractBaseClass
    {
        public Guid Id { get; set; }

        public string BaseAttribute { get; set; }
    }
}