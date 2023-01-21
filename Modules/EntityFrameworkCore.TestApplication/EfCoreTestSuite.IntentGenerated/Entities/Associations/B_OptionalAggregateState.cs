using System;
using System.Collections.Generic;
using EfCoreTestSuite.IntentGenerated.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities.Associations
{

    public partial class B_OptionalAggregate : IB_OptionalAggregate
    {

        public Guid Id { get; set; }

        public string OptionalAggrAttr { get; set; }

        public Guid? BOptionalDependentId { get; set; }

        public virtual B_OptionalDependent BOptionalDependent { get; set; }

        IB_OptionalDependent IB_OptionalAggregate.BOptionalDependent
        {
            get => BOptionalDependent;
            set => BOptionalDependent = (B_OptionalDependent)value;
        }
    }
}
