using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities.Associations
{

    public partial interface IH_MultipleDependent
    {

        /// <summary>
        /// Get the persistent object's identifier
        /// </summary>
        Guid Id { get; }
        string MultipleDepAttr { get; set; }

        Guid? H_OptionalAggregateNavId { get; }
        H_OptionalAggregateNav H_OptionalAggregateNav { get; set; }

    }
}
