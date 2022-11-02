using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Domain.Entities
{

    public partial interface IAA4_Aggregation_Many
    {

        Guid Id { get; set; }

        string AggregationAttr { get; set; }

        Guid? AAggregationManyId { get; set; }
        Guid? A_Aggregation_ManyId { get; }
    }
}
