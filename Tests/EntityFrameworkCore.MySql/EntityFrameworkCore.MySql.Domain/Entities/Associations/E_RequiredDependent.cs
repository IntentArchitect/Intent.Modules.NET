using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.MySql.Domain.Entities.Associations
{
    public class E_RequiredDependent
    {
        public Guid Id { get; set; }

        public string RequiredDepAttr { get; set; }

        public virtual E_RequiredCompositeNav E_RequiredCompositeNav { get; set; }
    }
}