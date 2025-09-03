using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.PkNoneProvider.Domain.Entities.Associations
{
    public class F_OptionalDependent
    {
        public F_OptionalDependent()
        {
            OptionalDepAttr = null!;
        }

        public Guid Id { get; set; }

        public string OptionalDepAttr { get; set; }

        public virtual F_OptionalAggregateNav? F_OptionalAggregateNav { get; set; }
    }
}