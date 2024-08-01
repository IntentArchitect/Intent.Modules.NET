using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.MySql.Domain.Entities.Associations
{
    public class D_MultipleDependent
    {
        public Guid Id { get; set; }

        public string MultipleDepAttr { get; set; }

        public Guid? D_OptionalAggregateId { get; set; }
    }
}