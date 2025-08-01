using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace MongoDb.TestApplication.Domain.Entities.Associations
{
    public class B_OptionalAggregate
    {
        public B_OptionalAggregate()
        {
            Id = null!;
            Attribute = null!;
        }

        public string Id { get; set; }

        public string Attribute { get; set; }

        public string? BOptionaldependentId { get; set; }
    }
}