using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Domain.Entities
{
    public class ParentSubDetails
    {
        public ParentSubDetails()
        {
            SubDetailsLine1 = null!;
            SubDetailsLine2 = null!;
        }

        public string SubDetailsLine1 { get; set; }

        public string SubDetailsLine2 { get; set; }
    }
}