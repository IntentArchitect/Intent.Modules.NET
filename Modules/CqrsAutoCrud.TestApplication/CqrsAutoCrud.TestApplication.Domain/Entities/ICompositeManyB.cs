using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Domain.Entities
{

    public partial interface ICompositeManyB
    {

        Guid Id { get; set; }

        Guid AAggregaterootId { get; set; }

        DateTime? SomeDate { get; set; }

        string CompositeAttr { get; set; }

        CompositeSingleBB Composite { get; set; }

        ICollection<CompositeManyBB> Composites { get; set; }
        Guid A_AggregateRootId { get; }
    }
}
