using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace MongoDb.TestApplication.Domain.Entities.Indexes
{
    public class TextIndexEntity
    {
        public TextIndexEntity()
        {
            Id = null!;
            FullText = null!;
            SomeField = null!;
        }

        public string Id { get; set; }

        public string FullText { get; set; }

        public string SomeField { get; set; }
    }
}