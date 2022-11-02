using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Domain.Entities
{

    public partial interface IA_Aggregation_Single
    {

        Guid Id { get; set; }

        string AggregationAttr { get; set; }

        AA3_Composite_Single Composite { get; set; }

        ICollection<AA3_Composite_Many> Composites { get; set; }

        AA3_Aggregation_Single Aggregation { get; set; }

        ICollection<AA3_Aggregation_Many> Aggregations { get; set; }

    }
}
