using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Domain.Entities
{
    public class ParentDetails
    {
        public ParentDetails()
        {
            DetailsLine1 = null!;
            DetailsLine2 = null!;
        }

        public string DetailsLine1 { get; set; }

        public string DetailsLine2 { get; set; }

        public ParentSubDetails? ParentSubDetails { get; set; }

        public ICollection<ParentDetailsTags>? ParentDetailsTags { get; set; } = [];
    }
}