using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.Postgres.Domain.Entities.TPC.InheritanceAssociations
{
    public class TPC_DerivedClassForAbstract : TPC_AbstractBaseClass
    {
        public Guid Id { get; set; }

        public string DerivedAttribute { get; set; }
    }
}