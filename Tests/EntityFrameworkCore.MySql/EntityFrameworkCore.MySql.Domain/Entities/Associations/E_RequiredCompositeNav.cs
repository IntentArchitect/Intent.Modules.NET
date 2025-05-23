using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.MySql.Domain.Entities.Associations
{
    public class E_RequiredCompositeNav
    {
        public E_RequiredCompositeNav()
        {
            RequiredCompNavAttr = null!;
            E_RequiredDependent = null!;
        }
        public Guid Id { get; set; }

        public string RequiredCompNavAttr { get; set; }

        public virtual E_RequiredDependent E_RequiredDependent { get; set; }
    }
}