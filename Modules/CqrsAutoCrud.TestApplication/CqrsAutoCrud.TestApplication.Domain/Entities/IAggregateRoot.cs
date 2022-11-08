using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Domain.Entities
{

    public partial interface IAggregateRoot
    {

        Guid Id { get; set; }

        string AggregateAttr { get; set; }

        CompositeSingleA? Composite { get; set; }

        ICollection<CompositeManyB> Composites { get; set; }

        AggregateSingleC Aggregate { get; set; }

    }
}
