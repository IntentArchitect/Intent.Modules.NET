using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Associations
{
    public class C_MultipleDependent
    {
        public C_MultipleDependent()
        {
            MultipleDependentAttr = null!;
        }

        public Guid Id { get; set; }

        public string MultipleDependentAttr { get; set; }

        public Guid CRequiredcompositeId { get; set; }
    }
}