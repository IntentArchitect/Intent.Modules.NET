using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Redis.Om.Repositories.Domain.Entities
{
    public class DerivedType : BaseType
    {
        public string DerivedName { get; set; }
        public DerivedTypeAggregate DerivedTypeAggregate { get; set; }
    }
}