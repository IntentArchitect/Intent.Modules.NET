using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Associations
{
    public class A_OptionalDependent
    {
        public A_OptionalDependent()
        {
            OptionalDependentAttr = null!;
        }

        public Guid Id { get; set; }

        public string OptionalDependentAttr { get; set; }
    }
}