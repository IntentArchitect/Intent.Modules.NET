using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Associations
{
    public class G_MultipleDependent
    {
        public G_MultipleDependent()
        {
            MultipleDepAttr = null!;
            G_RequiredCompositeNav = null!;
        }

        public Guid Id { get; set; }

        public string MultipleDepAttr { get; set; }

        public Guid GRequiredcompositenavId { get; set; }

        public virtual G_RequiredCompositeNav G_RequiredCompositeNav { get; set; }
    }
}