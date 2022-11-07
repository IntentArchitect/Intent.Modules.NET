using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Domain.Entities
{

    public partial interface ICompositeManyAAA1
    {

        Guid Id { get; set; }

        string CompositeAttr { get; set; }

        Guid ACompositeSingleId { get; set; }
        Guid A_Composite_SingleId { get; }
    }
}
