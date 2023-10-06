using System;
using Intent.RoslynWeaver.Attributes;

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Inheritance
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class Composite
    {
        public Guid Id { get; set; }

        public string CompositeField1 { get; set; }

        public string PartitionKey { get; set; }

        public Guid DerivedId { get; set; }
    }
}