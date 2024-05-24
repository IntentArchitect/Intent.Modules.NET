using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Entities.MappingTests
{
    public class NestingChild
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public Guid NestingParentId { get; set; }

        public virtual NestingChildChild NestingChildChild { get; set; }
    }
}