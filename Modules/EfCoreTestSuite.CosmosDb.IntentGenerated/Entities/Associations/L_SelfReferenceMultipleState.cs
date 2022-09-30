using System;
using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Associations
{

    public partial class L_SelfReferenceMultiple : IL_SelfReferenceMultiple
    {

        public Guid Id { get; set; }

        public string PartitionKey { get; set; }

        public string SelfRefMulAttr { get; set; }

        public virtual ICollection<L_SelfReferenceMultiple> L_SelfReferenceMultiplesDst { get; set; } = new List<L_SelfReferenceMultiple>();

        ICollection<IL_SelfReferenceMultiple> IL_SelfReferenceMultiple.L_SelfReferenceMultiplesDst
        {
            get => L_SelfReferenceMultiplesDst.CreateWrapper<IL_SelfReferenceMultiple, L_SelfReferenceMultiple>();
            set => L_SelfReferenceMultiplesDst = value.Cast<L_SelfReferenceMultiple>().ToList();
        }

        private ICollection<L_SelfReferenceMultiple> L_SelfReferenceMultiplesSrc { get; set; }


    }
}
