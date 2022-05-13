using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.TPH.IntentGenerated.Entities
{

    public partial interface IConcreteBaseClass
    {

        /// <summary>
        /// Get the persistent object's identifier
        /// </summary>
        Guid Id { get; }
        string BaseAttribute { get; set; }

    }
}
