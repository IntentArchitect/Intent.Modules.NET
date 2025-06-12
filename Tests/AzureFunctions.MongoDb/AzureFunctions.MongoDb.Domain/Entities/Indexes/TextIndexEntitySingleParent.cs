using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AzureFunctions.MongoDb.Domain.Entities.Indexes
{
    public class TextIndexEntitySingleParent
    {
        public TextIndexEntitySingleParent()
        {
            Id = null!;
            SomeField = null!;
            TextIndexEntitySingleChild = null!;
        }

        public string Id { get; set; }

        public string SomeField { get; set; }

        public TextIndexEntitySingleChild TextIndexEntitySingleChild { get; set; }
    }
}