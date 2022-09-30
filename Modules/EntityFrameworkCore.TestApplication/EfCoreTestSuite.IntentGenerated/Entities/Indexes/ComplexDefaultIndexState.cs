using System;
using System.Collections.Generic;
using EfCoreTestSuite.IntentGenerated.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities.Indexes
{

    public partial class ComplexDefaultIndex : IComplexDefaultIndex
    {

        public Guid Id { get; set; }

        public Guid FieldA { get; set; }

        public Guid FieldB { get; set; }

    }
}
