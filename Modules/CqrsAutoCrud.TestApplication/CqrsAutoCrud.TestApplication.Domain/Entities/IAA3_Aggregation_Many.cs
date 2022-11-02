using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Domain.Entities
{

    public partial interface IAA3_Aggregation_Many
    {

        Guid Id { get; set; }

        string AggregationAttr { get; set; }

        Guid? AAggregationSingleId { get; set; }
        Guid? A_Aggregation_SingleId { get; }
    }
}
