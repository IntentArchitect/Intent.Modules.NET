using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Entities.MappingTests
{
    public class NestingChildChild
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}