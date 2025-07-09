using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Domain.Entities.Nullability
{
    public class TestNullablityChild
    {
        public Guid Id { get; set; }

        public Guid TestNullablityId { get; set; }
    }
}