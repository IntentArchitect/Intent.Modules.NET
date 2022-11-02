using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Domain.Entities
{

    public partial interface IA_Composite_Many
    {

        Guid Id { get; set; }

        Guid AAggregaterootId { get; set; }

        string CompositeAttr { get; set; }

        AA2_Composite_Single Composite { get; set; }

        ICollection<AA2_Composite_Many> Composites { get; set; }

        AA2_Aggregation_Single Aggregation { get; set; }
        Guid A_AggregateRootId { get; }
    }
}
