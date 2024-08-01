using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.Postgres.Domain.Entities.Associations
{
    public class E_RequiredCompositeNav
    {
        public Guid Id { get; set; }

        public string RequiredCompNavAttr { get; set; }

        public virtual E_RequiredDependent E_RequiredDependent { get; set; }
    }
}