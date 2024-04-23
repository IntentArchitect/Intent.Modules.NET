using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Domain.Entities.Associations
{
    public class H_MultipleDependent
    {
        public Guid Id { get; set; }

        public string MultipleDepAttr { get; set; }

        public Guid? H_OptionalAggregateNavId { get; set; }

        public virtual H_OptionalAggregateNav? H_OptionalAggregateNav { get; set; }
    }
}