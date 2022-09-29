using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities.Associations
{

    public partial interface IL_SelfReferenceMultiple
    {

        /// <summary>
        /// Get the persistent object's identifier
        /// </summary>
        Guid Id { get; }
        string SelfRefMultipleAttr { get; set; }

        ICollection<L_SelfReferenceMultiple> L_SelfReferenceMultiplesDst { get; set; }

    }
}
