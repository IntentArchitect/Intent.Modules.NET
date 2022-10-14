using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.TPT.IntentGenerated.Entities.InheritanceAssociations
{

    public partial interface IFkBaseClassAssociated
    {

        /// <summary>
        /// Get the persistent object's identifier
        /// </summary>
        Guid Id { get; }
        string AssociatedField { get; set; }
        Guid FkBaseClassCompositeKeyA { get; }
        Guid FkBaseClassCompositeKeyB { get; }
        FkBaseClass FkBaseClass { get; set; }

    }
}
