using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities.NestedAssociations
{
    public partial class Inhabitant : IInhabitant
    {
        public Guid Id { get; set; }

        public string InhabitantAttribute { get; set; }

        protected virtual ICollection<Branch> Branches { get; set; }
    }
}