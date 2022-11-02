using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Domain.Entities
{

    public partial interface ICompositeManyAA
    {

        Guid Id { get; set; }

        Guid AAggregaterootId { get; set; }

        string CompositeAttr { get; set; }

        CompositeSingleAAA2 Composite { get; set; }

        ICollection<CompositeManyAAA2> Composites { get; set; }
        Guid A_AggregateRootId { get; }
    }
}
