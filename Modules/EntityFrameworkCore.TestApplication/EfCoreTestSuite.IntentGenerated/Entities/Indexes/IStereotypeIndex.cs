using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities.Indexes
{

    public interface IStereotypeIndex
    {
        Guid DefaultIndexField { get; set; }

        Guid CustomIndexField { get; set; }

        Guid GroupedIndexFieldA { get; set; }

        Guid GroupedIndexFieldB { get; set; }

    }
}
