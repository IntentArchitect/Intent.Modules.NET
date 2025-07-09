using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Associations
{
    public class E_RequiredDependent
    {
        public E_RequiredDependent()
        {
            RequiredDependentAttr = null!;
            E_RequiredCompositeNav = null!;
        }

        public Guid Id { get; set; }

        public string RequiredDependentAttr { get; set; }

        public virtual E_RequiredCompositeNav E_RequiredCompositeNav { get; set; }
    }
}