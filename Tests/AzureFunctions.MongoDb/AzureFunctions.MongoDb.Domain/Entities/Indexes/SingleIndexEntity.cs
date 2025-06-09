using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AzureFunctions.MongoDb.Domain.Entities.Indexes
{
    public class SingleIndexEntity
    {
        public SingleIndexEntity()
        {
            Id = null!;
            SomeField = null!;
            SingleIndex = null!;
        }

        public string Id { get; set; }

        public string SomeField { get; set; }

        public string SingleIndex { get; set; }
    }
}