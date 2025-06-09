using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AzureFunctions.MongoDb.Domain.Entities.Indexes
{
    public class MultikeyIndexEntitySingleChild
    {
        public IList<string> MultiKey { get; set; } = [];
    }
}