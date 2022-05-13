using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.TPT.IntentGenerated.Entities
{

    public partial interface IFkAssociatedClass
    {

        /// <summary>
        /// Get the persistent object's identifier
        /// </summary>
        Guid Id { get; }
        Guid FkDerivedClassCompositeKeyA { get; }
        Guid FkDerivedClassCompositeKeyB { get; }
        FkDerivedClass FkDerivedClass { get; set; }

    }
}
