using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Domain.Entities
{

    public partial interface ICompositeSingleA
    {

        Guid Id { get; set; }

        string CompositeAttr { get; set; }

        CompositeSingleAA Composite { get; set; }

        ICollection<CompositeManyAA> Composites { get; set; }

    }
}
