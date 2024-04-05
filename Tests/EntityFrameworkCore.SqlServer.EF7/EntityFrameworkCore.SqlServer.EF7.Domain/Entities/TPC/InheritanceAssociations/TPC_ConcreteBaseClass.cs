using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Domain.Entities.TPC.InheritanceAssociations
{
    public class TPC_ConcreteBaseClass
    {
        public Guid Id { get; set; }

        public string BaseAttribute { get; set; }
    }
}