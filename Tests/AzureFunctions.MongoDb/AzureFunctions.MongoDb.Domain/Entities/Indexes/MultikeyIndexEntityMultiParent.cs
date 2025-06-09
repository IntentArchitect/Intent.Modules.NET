using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AzureFunctions.MongoDb.Domain.Entities.Indexes
{
    public class MultikeyIndexEntityMultiParent
    {
        public MultikeyIndexEntityMultiParent()
        {
            Id = null!;
            SomeField = null!;
        }

        public string Id { get; set; }

        public string SomeField { get; set; }

        public ICollection<MultikeyIndexEntityMultiChild> MultikeyIndexEntityMultiChild { get; set; } = [];
    }
}