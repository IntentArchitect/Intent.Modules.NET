using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace MongoDb.TestApplication.Domain.Entities.Associations
{
    public class I_MultipleAggregate
    {
        public I_MultipleAggregate()
        {
            Id = null!;
            Attribute = null!;
            JRequireddependentId = null!;
        }

        public string Id { get; set; }

        public string Attribute { get; set; }

        public string JRequireddependentId { get; set; }
    }
}