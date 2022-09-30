using System;
using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Associations
{

    public partial class G_RequiredCompositeNav : IG_RequiredCompositeNav
    {

        public Guid Id { get; set; }

        public string PartitionKey { get; set; }

        public string RequiredCompNavAttr { get; set; }

        public virtual ICollection<G_MultipleDependent> G_MultipleDependents { get; set; } = new List<G_MultipleDependent>();

        ICollection<IG_MultipleDependent> IG_RequiredCompositeNav.G_MultipleDependents
        {
            get => G_MultipleDependents.CreateWrapper<IG_MultipleDependent, G_MultipleDependent>();
            set => G_MultipleDependents = value.Cast<G_MultipleDependent>().ToList();
        }


    }
}
