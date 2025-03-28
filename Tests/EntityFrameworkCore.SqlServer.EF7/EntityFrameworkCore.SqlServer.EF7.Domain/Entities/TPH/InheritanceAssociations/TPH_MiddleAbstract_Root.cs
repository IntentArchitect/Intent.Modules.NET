using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Domain.Entities.TPH.InheritanceAssociations
{
    public class TPH_MiddleAbstract_Root
    {
        public TPH_MiddleAbstract_Root()
        {
            RootAttribute = null!;
        }
        public Guid Id { get; set; }

        public string RootAttribute { get; set; }
    }
}