using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Domain.Entities
{

    public partial interface IAA4_Composite_Many
    {

        Guid Id { get; set; }

        string CompositeAttr { get; set; }

        Guid AAggregationManyId { get; set; }
        Guid A_Aggregation_ManyId { get; }
    }
}
