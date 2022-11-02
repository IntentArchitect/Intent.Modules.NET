using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Domain.Entities
{

    public partial interface IA_Aggregation_Many
    {

        Guid Id { get; set; }

        string AggregationAttr { get; set; }

        Guid? AAggregaterootId { get; set; }

        AA4_Composite_Single Composite { get; set; }

        ICollection<AA4_Composite_Many> Composites { get; set; }

        AA4_Aggregation_Single Aggregation { get; set; }

        ICollection<AA4_Aggregation_Many> Aggregations { get; set; }
        Guid? A_AggregateRootId { get; }
    }
}
