using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Domain.Entities
{

    public partial interface IA_Composite_Single
    {

        Guid Id { get; set; }

        string CompositeAttr { get; set; }

        AA1_Composite_Single Composite { get; set; }

        ICollection<AA1_Composite_Many> Composites { get; set; }

        AA1_Aggregation_Single Aggregation { get; set; }

    }
}
