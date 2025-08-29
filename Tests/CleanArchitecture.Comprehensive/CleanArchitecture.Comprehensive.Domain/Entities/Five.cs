using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Domain.Entities
{
    public class Five
    {
        public Guid Id { get; set; }

        public Guid? ParentId { get; set; }
    }
}