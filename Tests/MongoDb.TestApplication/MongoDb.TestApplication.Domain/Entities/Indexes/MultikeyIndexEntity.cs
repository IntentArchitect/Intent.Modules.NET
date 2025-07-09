using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace MongoDb.TestApplication.Domain.Entities.Indexes
{
    public class MultikeyIndexEntity
    {
        public MultikeyIndexEntity()
        {
            Id = null!;
            SomeField = null!;
        }

        public string Id { get; set; }

        public IList<string> MultiKey { get; set; } = [];

        public string SomeField { get; set; }
    }
}