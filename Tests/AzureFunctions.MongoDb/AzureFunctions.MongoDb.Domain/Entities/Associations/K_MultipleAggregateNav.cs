using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AzureFunctions.MongoDb.Domain.Entities.Associations
{
    public class K_MultipleAggregateNav
    {
        public K_MultipleAggregateNav()
        {
            Id = null!;
            Attribute = null!;
        }

        public string Id { get; set; }

        public string Attribute { get; set; }

        public IList<string> JMultipleDependentsIds { get; set; } = [];
    }
}