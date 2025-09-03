using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.PkNoneProvider.Domain.Entities.TPH.InheritanceAssociations
{
    public abstract class TPH_AbstractBaseClass
    {
        public TPH_AbstractBaseClass()
        {
            BaseAttribute = null!;
        }

        public Guid Id { get; set; }

        public string BaseAttribute { get; set; }
    }
}