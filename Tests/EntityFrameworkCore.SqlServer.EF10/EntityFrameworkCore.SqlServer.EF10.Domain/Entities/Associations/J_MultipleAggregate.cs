using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF10.Domain.Entities.Associations
{
    public class J_MultipleAggregate
    {
        public J_MultipleAggregate()
        {
            MultipleAggrAttr = null!;
            J_RequiredDependent = null!;
        }
        public Guid Id { get; set; }

        public string MultipleAggrAttr { get; set; }

        public Guid J_RequiredDependentId { get; set; }

        public virtual J_RequiredDependent J_RequiredDependent { get; set; }
    }
}