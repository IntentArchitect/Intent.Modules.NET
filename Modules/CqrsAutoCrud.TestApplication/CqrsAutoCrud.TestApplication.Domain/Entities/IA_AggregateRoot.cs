using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Domain.Entities
{

    public partial interface IA_AggregateRoot
    {

        Guid Id { get; set; }

        string AggregateAttr { get; set; }

        A_Composite_Single Composite { get; set; }

        ICollection<A_Composite_Many> Composites { get; set; }

        A_Aggregation_Single Aggregation { get; set; }

        ICollection<A_Aggregation_Many> Aggregations { get; set; }

    }
}
